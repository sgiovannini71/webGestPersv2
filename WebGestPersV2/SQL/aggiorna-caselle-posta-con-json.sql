SET NOCOUNT ON;

/* ===================== PARAMETRI ===================== */
DECLARE @Path     nvarchar(500) = N'C:\temp\risultati.json'; -- percorso VISTO DAL SERVER SQL
DECLARE @Tipo     varchar(50)   = 'funzionale';
DECLARE @Inserisci bit          = 1;   -- 1 = crea la cassetta se la persona non ne ha una funzionale
DECLARE @Applica   bit          = 1;   -- 1 = scrive davvero, 0 = simulazione
/* ===================================================== */

-- 1) Lettura del file JSON (il percorso in OPENROWSET deve essere un letterale: SQL dinamico)
DECLARE @json nvarchar(max), @sql nvarchar(max);
SET @sql = N'SELECT @j = BulkColumn FROM OPENROWSET(BULK ''' + REPLACE(@Path, '''', '''''')
         + N''', CODEPAGE = ''65001'', SINGLE_CLOB) AS f;';
EXEC sp_executesql @sql, N'@j nvarchar(max) OUTPUT', @j = @json OUTPUT;

IF @json IS NULL OR ISJSON(@json) = 0
BEGIN RAISERROR('File vuoto o JSON non valido.', 16, 1); RETURN; END

-- il file creato con -cf contiene un oggetto singolo: lo trasformo in array
IF LEFT(LTRIM(@json), 1) = '{' SET @json = N'[' + @json + N']';

-- 2) Dati sorgente
IF OBJECT_ID('tempdb..#src') IS NOT NULL DROP TABLE #src;
CREATE TABLE #src (
    cf          varchar(16)  COLLATE DATABASE_DEFAULT,
    trovato     bit,
    mail        varchar(255) COLLATE DATABASE_DEFAULT,
    upn         varchar(100) COLLATE DATABASE_DEFAULT,
    IDPersonale int,
    ID_CP       int,
    esito       varchar(200) COLLATE DATABASE_DEFAULT
);

-- OPENJSON eseguito nel contesto di tempdb (compatibilita' >= 130)
EXEC sp_executesql N'
USE tempdb;
INSERT INTO #src (cf, trovato, mail, upn)
SELECT UPPER(LTRIM(RTRIM(codiceFiscale))), trovato, LTRIM(RTRIM(mail)), LTRIM(RTRIM(userPrincipalName))
FROM OPENJSON(@j) WITH (
        codiceFiscale     varchar(16)  ''$.codiceFiscale'',
        trovato           bit          ''$.trovato'',
        mail              varchar(255) ''$.mail'',
        userPrincipalName varchar(100) ''$.userPrincipalName''
);', N'@j nvarchar(max)', @j = @json;

-- 3) Controlli preliminari
UPDATE #src SET esito = 'saltato: non trovato in AD'            WHERE esito IS NULL AND ISNULL(trovato,0) = 0;
UPDATE #src SET esito = 'saltato: mail vuota'                   WHERE esito IS NULL AND ISNULL(mail,'') = '';
UPDATE #src SET esito = 'saltato: UPN piu'' lungo di 100 car.'  WHERE esito IS NULL AND LEN(upn) > 100;
UPDATE s SET esito = 'saltato: CF presente piu'' volte nel JSON (account AD multipli)'
FROM #src s WHERE esito IS NULL AND cf IN (SELECT cf FROM #src GROUP BY cf HAVING COUNT(*) > 1);

-- 4) IDPersonale da ElencoPersonale
UPDATE s SET IDPersonale = e.IDPersonale
FROM #src s
JOIN dbo.ElencoPersonale e
  ON UPPER(LTRIM(RTRIM(e.CodiceFiscale))) COLLATE DATABASE_DEFAULT = s.cf
WHERE s.esito IS NULL
  AND (SELECT COUNT(*) FROM dbo.ElencoPersonale x
       WHERE UPPER(LTRIM(RTRIM(x.CodiceFiscale))) COLLATE DATABASE_DEFAULT = s.cf) = 1;

UPDATE #src SET esito = 'saltato: CF assente o duplicato in ElencoPersonale'
WHERE esito IS NULL AND IDPersonale IS NULL;

-- 5) Cassetta funzionale da aggiornare
--    una sola cassetta -> quella; piu' cassette -> solo se una ha gia' stessa mail o UPN
UPDATE s SET ID_CP = c.ID_CP
FROM #src s
CROSS APPLY (
    SELECT TOP (1) cp.ID_CP
    FROM dbo.Cassette_Postali cp
    WHERE cp.IDPersonale = s.IDPersonale
      AND cp.tipo COLLATE DATABASE_DEFAULT = @Tipo
      AND ( (SELECT COUNT(*) FROM dbo.Cassette_Postali z
             WHERE z.IDPersonale = s.IDPersonale
               AND z.tipo COLLATE DATABASE_DEFAULT = @Tipo) = 1
            OR cp.Denominazione COLLATE DATABASE_DEFAULT = s.mail
            OR cp.nomeistituzionale COLLATE DATABASE_DEFAULT = s.upn )
    ORDER BY cp.ID_CP
) c
WHERE s.esito IS NULL
  AND ( (SELECT COUNT(*) FROM dbo.Cassette_Postali z
         WHERE z.IDPersonale = s.IDPersonale
           AND z.tipo COLLATE DATABASE_DEFAULT = @Tipo) = 1
     OR (SELECT COUNT(*) FROM dbo.Cassette_Postali z
         WHERE z.IDPersonale = s.IDPersonale
           AND z.tipo COLLATE DATABASE_DEFAULT = @Tipo
           AND (z.Denominazione COLLATE DATABASE_DEFAULT = s.mail
             OR z.nomeistituzionale COLLATE DATABASE_DEFAULT = s.upn)) = 1 );

UPDATE s SET esito = 'saltato: piu'' cassette ''' + @Tipo + ''', impossibile scegliere'
FROM #src s
WHERE esito IS NULL AND ID_CP IS NULL
  AND EXISTS (SELECT 1 FROM dbo.Cassette_Postali z
              WHERE z.IDPersonale = s.IDPersonale
                AND z.tipo COLLATE DATABASE_DEFAULT = @Tipo);
-- 6) Scrittura (transazione unica)
BEGIN TRY
    BEGIN TRAN;

    UPDATE cp SET Denominazione = s.mail, nomeistituzionale = s.upn
    FROM dbo.Cassette_Postali cp
    JOIN #src s ON s.ID_CP = cp.ID_CP
    WHERE s.esito IS NULL;

    UPDATE #src SET esito = 'AGGIORNATA' WHERE esito IS NULL AND ID_CP IS NOT NULL;

    IF @Inserisci = 1
    BEGIN
        INSERT INTO dbo.Cassette_Postali (IDPersonale, Denominazione, tipo, nomeistituzionale)
        SELECT IDPersonale, mail, @Tipo, upn FROM #src WHERE esito IS NULL AND ID_CP IS NULL;
        UPDATE #src SET esito = 'INSERITA' WHERE esito IS NULL AND ID_CP IS NULL;
    END
    ELSE
        UPDATE #src SET esito = 'saltato: nessuna cassetta ''' + @Tipo + ''' (usa @Inserisci = 1)'
        WHERE esito IS NULL AND ID_CP IS NULL;

    -- Report
    SELECT cf, IDPersonale, ID_CP, mail, upn, esito FROM #src ORDER BY esito, cf;
    SELECT esito, COUNT(*) AS n FROM #src GROUP BY esito ORDER BY esito;

    IF @Applica = 1 BEGIN COMMIT; PRINT 'COMMIT eseguito.'; END
    ELSE            BEGIN ROLLBACK; PRINT 'SIMULAZIONE: nessuna modifica scritta (imposta @Applica = 1).'; END
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    PRINT 'ERRORE, rollback eseguito: ' + ERROR_MESSAGE();
END CATCH;

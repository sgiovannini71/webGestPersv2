USE DipendentiDB;
GO

SET XACT_ABORT ON;
GO

DECLARE @DataChiusuraIncarichi date = '2026-05-03';

DECLARE @ID_Uff1_NA int = 22;
DECLARE @ID_Uff2_NA int = 119;
DECLARE @ID_Uff3_NA int = 195;

DECLARE @IdTipoIncaricoNA int;

SELECT TOP (1)
    @IdTipoIncaricoNA = id_tipo_incarico
FROM dbo.Tipo_incarichi
WHERE LOWER(LTRIM(RTRIM(Sigla_incarico))) = 'n/a';

IF @IdTipoIncaricoNA IS NULL
BEGIN
    THROW 50001, 'Tipo incarico n/a non trovato nella tabella Tipo_incarichi.', 1;
END;

------------------------------------------------------------
-- ANTEPRIMA INCARICHI CHE VERRANNO CHIUSI
------------------------------------------------------------
SELECT
    EP.IDPersonale,
    EP.Cognome,
    EP.Nome,
    EP.Stato_Servizio,
    I.id_incarico,
    TI.Sigla_incarico,
    TI.Descr_incarico,
    I.principale,
    I.Data_inizio,
    I.ID_Uff1,
    I.ID_Uff2,
    I.ID_Uff3
FROM dbo.Incarichi I
INNER JOIN dbo.ElencoPersonale EP
    ON EP.IDPersonale = I.IDPersonale
INNER JOIN dbo.Tipo_incarichi TI
    ON TI.id_tipo_incarico = I.id_tipo_incarico
WHERE EP.Stato_Servizio = 'Attivo'
  AND LOWER(LTRIM(RTRIM(TI.Sigla_incarico))) <> 'n/a'
ORDER BY EP.Cognome, EP.Nome;

------------------------------------------------------------
-- ESECUZIONE
------------------------------------------------------------
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('tempdb..#IncarichiDaChiudere') IS NOT NULL
        DROP TABLE #IncarichiDaChiudere;

    SELECT
        I.id_incarico,
        I.IDPersonale,
        I.principale,
        I.Data_inizio,
        I.id_tipo_incarico,
        I.ID_Uff1,
        I.ID_Uff2,
        I.ID_Uff3
    INTO #IncarichiDaChiudere
    FROM dbo.Incarichi I
    INNER JOIN dbo.ElencoPersonale EP
        ON EP.IDPersonale = I.IDPersonale
    INNER JOIN dbo.Tipo_incarichi TI
        ON TI.id_tipo_incarico = I.id_tipo_incarico
    WHERE EP.Stato_Servizio = 'Attivo'
      AND LOWER(LTRIM(RTRIM(TI.Sigla_incarico))) <> 'n/a';

    --------------------------------------------------------
    -- Inserimento nello storico incarichi
    --------------------------------------------------------
    INSERT INTO dbo.StoricoIncarichi
    (
        IDpersonale,
        Incarico,
        principale,
        data_inizio,
        date_fine
    )
    SELECT
        IDC.IDPersonale,
        CONCAT(
            ISNULL(TI.Descr_incarico, ''),
            ' - ',
            ISNULL(L1.DescUff1, ''),
            ' - ',
            ISNULL(L2.DescUff2, ''),
            ' - ',
            ISNULL(L3.DescUff3, '')
        ) AS Incarico,
        IDC.principale,
        IDC.Data_inizio,
        @DataChiusuraIncarichi
    FROM #IncarichiDaChiudere IDC
    LEFT JOIN dbo.Tipo_incarichi TI
        ON TI.id_tipo_incarico = IDC.id_tipo_incarico
    LEFT JOIN dbo.Liv1Uff L1
        ON L1.ID_Uff1 = IDC.ID_Uff1
    LEFT JOIN dbo.Liv2Uff L2
        ON L2.ID_Uff1 = IDC.ID_Uff1
       AND L2.ID_Uff2 = IDC.ID_Uff2
    LEFT JOIN dbo.Liv3Uff L3
        ON L3.ID_Uff1 = IDC.ID_Uff1
       AND L3.ID_Uff2 = IDC.ID_Uff2
       AND L3.ID_Uff3 = IDC.ID_Uff3;

    --------------------------------------------------------
    -- Eliminazione incarichi attivi chiusi
    --------------------------------------------------------
    DELETE I
    FROM dbo.Incarichi I
    INNER JOIN #IncarichiDaChiudere IDC
        ON IDC.id_incarico = I.id_incarico;

    --------------------------------------------------------
    -- Inserimento incarico n/a per ogni persona interessata
    --------------------------------------------------------
    INSERT INTO dbo.Incarichi
    (
        IDPersonale,
        id_tipo_incarico,
        principale,
        Data_inizio,
        ID_Uff1,
        ID_Uff2,
        ID_Uff3
    )
    SELECT DISTINCT
        IDC.IDPersonale,
        @IdTipoIncaricoNA,
        1,
        CAST(@DataChiusuraIncarichi AS datetime),
        @ID_Uff1_NA,
        @ID_Uff2_NA,
        @ID_Uff3_NA
    FROM #IncarichiDaChiudere IDC
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Incarichi I
        WHERE I.IDPersonale = IDC.IDPersonale
          AND I.id_tipo_incarico = @IdTipoIncaricoNA
    );

    --------------------------------------------------------
    -- RIEPILOGO
    --------------------------------------------------------
    SELECT
        COUNT(*) AS IncarichiChiusi,
        COUNT(DISTINCT IDPersonale) AS PersoneInteressate
    FROM #IncarichiDaChiudere;

    --------------------------------------------------------
    -- SICUREZZA:
    -- lasciare ROLLBACK per prova;
    -- sostituire con COMMIT solo dopo verifica.
    --------------------------------------------------------
    -- ROLLBACK TRANSACTION;
    COMMIT TRANSACTION;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    SELECT
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_MESSAGE() AS ErrorMessage,
        ERROR_LINE() AS ErrorLine;

    THROW;
END CATCH;
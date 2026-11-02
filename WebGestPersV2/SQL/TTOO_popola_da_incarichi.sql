USE [DipendentiDB];
GO

/*
    Popolamento automatico dbo.TTOO_Posizioni da incarichi principali effettivi.

    Logica:
    - considera solo dbo.Incarichi.principale = 1;
    - considera solo personale in uno degli stati servizio configurati;
    - prende l'ultima versione del profilo militare/civile;
    - raggruppa per ufficio, tipo incarico, tipo personale, grado/sottoarea;
    - aggiorna le righe TTOO gia' presenti;
    - inserisce le righe TTOO mancanti;
    - opzionalmente disattiva le righe TTOO non piu' presenti nella sorgente.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRAN;

/* =========================
   CONFIGURAZIONE
   ========================= */
DECLARE @DisattivaNonPresenti bit = 0;

DECLARE @StatiServizioAttivi TABLE
(
    Stato_Servizio varchar(20) NOT NULL PRIMARY KEY
);

/* Adeguare l'elenco agli stati usati nel database. */
INSERT INTO @StatiServizioAttivi (Stato_Servizio)
VALUES
    ('In Servizio'),
    ('IN SERVIZIO'),
    ('Servizio'),
    ('ATTIVO'),
    ('Attivo');

CREATE TABLE #SorgenteTTOO
(
    ID_Uff1 int NOT NULL,
    ID_Uff2 int NULL,
    ID_Uff3 int NULL,
    id_tipo_incarico int NOT NULL,
    TipoPersonale char(1) NOT NULL,
    ID_Grado_Previsto int NULL,
    id_sottoarea_prevista int NULL,
    NumeroPrevisto int NOT NULL
);

/* =========================
   SORGENTE NORMALIZZATA
   ========================= */
;WITH UltimoProfiloMilitare AS
(
    SELECT
        PM.IDPersonale,
        PM.ID_Grado,
        ROW_NUMBER() OVER
        (
            PARTITION BY PM.IDPersonale
            ORDER BY PM.Data_Versione_Profilo DESC, PM.ID_Militare DESC
        ) AS rn
    FROM dbo.Profilo_militare PM
),
UltimoProfiloCivile AS
(
    SELECT
        PC.IDPersonale,
        PC.ID_sottoarea,
        ROW_NUMBER() OVER
        (
            PARTITION BY PC.IDPersonale
            ORDER BY PC.Data_Versione_Profilo DESC, PC.ID_Civile DESC
        ) AS rn
    FROM dbo.PersCivile PC
)
INSERT INTO #SorgenteTTOO
(
    ID_Uff1,
    ID_Uff2,
    ID_Uff3,
    id_tipo_incarico,
    TipoPersonale,
    ID_Grado_Previsto,
    id_sottoarea_prevista,
    NumeroPrevisto
)
SELECT
    I.ID_Uff1,
    I.ID_Uff2,
    I.ID_Uff3,
    I.id_tipo_incarico,
    CAST(CASE WHEN EP.Militare = 1 THEN 'M' ELSE 'C' END AS char(1)) AS TipoPersonale,
    CASE WHEN EP.Militare = 1 THEN PM.ID_Grado ELSE NULL END AS ID_Grado_Previsto,
    CASE WHEN EP.Militare = 0 THEN PC.ID_sottoarea ELSE NULL END AS id_sottoarea_prevista,
    COUNT(DISTINCT I.IDPersonale) AS NumeroPrevisto
FROM dbo.Incarichi I
INNER JOIN dbo.ElencoPersonale EP
    ON EP.IDPersonale = I.IDPersonale
INNER JOIN @StatiServizioAttivi SSA
    ON SSA.Stato_Servizio = EP.Stato_Servizio
LEFT JOIN UltimoProfiloMilitare PM
    ON PM.IDPersonale = I.IDPersonale
   AND PM.rn = 1
LEFT JOIN UltimoProfiloCivile PC
    ON PC.IDPersonale = I.IDPersonale
   AND PC.rn = 1
WHERE I.ID_Uff1 IS NOT NULL
  AND I.id_tipo_incarico IS NOT NULL
  AND ISNULL(I.principale, 0) = 1
  AND (EP.Data_Usc_Armaereo IS NULL OR EP.Data_Usc_Armaereo > GETDATE())
  AND
  (
      (EP.Militare = 1 AND PM.ID_Grado IS NOT NULL)
   OR (EP.Militare = 0 AND PC.ID_sottoarea IS NOT NULL)
  )
GROUP BY
    I.ID_Uff1,
    I.ID_Uff2,
    I.ID_Uff3,
    I.id_tipo_incarico,
    CASE WHEN EP.Militare = 1 THEN 'M' ELSE 'C' END,
    CASE WHEN EP.Militare = 1 THEN PM.ID_Grado ELSE NULL END,
    CASE WHEN EP.Militare = 0 THEN PC.ID_sottoarea ELSE NULL END;

/* =========================
   UPDATE ESISTENTI
   ========================= */
UPDATE T
SET
    T.NumeroPrevisto = S.NumeroPrevisto,
    T.Attivo = 'S',
    T.Note = COALESCE(T.Note, 'Popolata automaticamente da incarichi principali')
FROM dbo.TTOO_Posizioni T
INNER JOIN #SorgenteTTOO S
    ON T.ID_Uff1 = S.ID_Uff1
   AND ISNULL(T.ID_Uff2, -1) = ISNULL(S.ID_Uff2, -1)
   AND ISNULL(T.ID_Uff3, -1) = ISNULL(S.ID_Uff3, -1)
   AND T.id_tipo_incarico = S.id_tipo_incarico
   AND T.TipoPersonale = S.TipoPersonale
   AND ISNULL(T.ID_Grado_Previsto, -1) = ISNULL(S.ID_Grado_Previsto, -1)
   AND ISNULL(T.id_sottoarea_prevista, -1) = ISNULL(S.id_sottoarea_prevista, -1);

/* =========================
   INSERT MANCANTI
   ========================= */
INSERT INTO dbo.TTOO_Posizioni
(
    ID_Uff1,
    ID_Uff2,
    ID_Uff3,
    id_tipo_incarico,
    TipoPersonale,
    ID_Grado_Previsto,
    id_sottoarea_prevista,
    NumeroPrevisto,
    Attivo,
    Note
)
SELECT
    S.ID_Uff1,
    S.ID_Uff2,
    S.ID_Uff3,
    S.id_tipo_incarico,
    S.TipoPersonale,
    S.ID_Grado_Previsto,
    S.id_sottoarea_prevista,
    S.NumeroPrevisto,
    'S',
    'Popolata automaticamente da incarichi principali'
FROM #SorgenteTTOO S
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TTOO_Posizioni T
    WHERE T.ID_Uff1 = S.ID_Uff1
      AND ISNULL(T.ID_Uff2, -1) = ISNULL(S.ID_Uff2, -1)
      AND ISNULL(T.ID_Uff3, -1) = ISNULL(S.ID_Uff3, -1)
      AND T.id_tipo_incarico = S.id_tipo_incarico
      AND T.TipoPersonale = S.TipoPersonale
      AND ISNULL(T.ID_Grado_Previsto, -1) = ISNULL(S.ID_Grado_Previsto, -1)
      AND ISNULL(T.id_sottoarea_prevista, -1) = ISNULL(S.id_sottoarea_prevista, -1)
);

/* =========================
   DISATTIVAZIONE OPZIONALE
   ========================= */
IF @DisattivaNonPresenti = 1
BEGIN
    UPDATE T
    SET T.Attivo = 'N'
    FROM dbo.TTOO_Posizioni T
    WHERE T.Attivo = 'S'
      AND NOT EXISTS
      (
          SELECT 1
          FROM #SorgenteTTOO S
          WHERE T.ID_Uff1 = S.ID_Uff1
            AND ISNULL(T.ID_Uff2, -1) = ISNULL(S.ID_Uff2, -1)
            AND ISNULL(T.ID_Uff3, -1) = ISNULL(S.ID_Uff3, -1)
            AND T.id_tipo_incarico = S.id_tipo_incarico
            AND T.TipoPersonale = S.TipoPersonale
            AND ISNULL(T.ID_Grado_Previsto, -1) = ISNULL(S.ID_Grado_Previsto, -1)
            AND ISNULL(T.id_sottoarea_prevista, -1) = ISNULL(S.id_sottoarea_prevista, -1)
      );
END;

COMMIT TRAN;
GO

/* Controllo risultato */
SELECT
    T.IDPosizioneTTOO,
    L1.SgUff1 AS Uff1,
    L2.SgUff2 AS Uff2,
    L3.SgUff3 AS Uff3,
    TI.Sigla_incarico,
    T.TipoPersonale,
    T.ID_Grado_Previsto,
    T.id_sottoarea_prevista,
    T.NumeroPrevisto,
    T.Attivo,
    T.Note
FROM dbo.TTOO_Posizioni T
INNER JOIN dbo.Liv1Uff L1 ON L1.ID_Uff1 = T.ID_Uff1
LEFT JOIN dbo.Liv2Uff L2 ON L2.ID_Uff2 = T.ID_Uff2
LEFT JOIN dbo.Liv3Uff L3 ON L3.ID_Uff3 = T.ID_Uff3
INNER JOIN dbo.Tipo_incarichi TI ON TI.id_tipo_incarico = T.id_tipo_incarico
ORDER BY L1.livello, L2.livello, L3.livello, TI.livello, T.TipoPersonale;
GO

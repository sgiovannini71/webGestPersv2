USE [DipendentiDB];
GO

/*
    Import TTOO da elenco organico manuale.

    Questo script serve per trasformare l'elenco:
        1 LIV | 2 LIV | 3 LIV | INCARICO
    in righe risolte con ID_Uff1, ID_Uff2, ID_Uff3 e id_tipo_incarico.

    Nota importante:
    dbo.TTOO_Posizioni richiede anche:
        - TipoPersonale = 'M' e ID_Grado_Previsto valorizzato
        oppure
        - TipoPersonale = 'C' e id_sottoarea_prevista valorizzata

    L'elenco fornito non contiene questi dati, quindi la prima fase valida uffici/incarichi.
    L'INSERT finale va eseguito solo dopo aver compilato TipoPersonale e grado/sottoarea.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRAN;

CREATE TABLE #TTOO_Input
(
    RowNo int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SgUff1 varchar(50) NOT NULL,
    SgUff2 varchar(50) NULL,
    SgUff3 varchar(50) NULL,
    Descr_incarico varchar(50) NOT NULL,
    NumeroPrevisto int NOT NULL DEFAULT 1,

    -- Da compilare prima dell'INSERT finale in dbo.TTOO_Posizioni.
    TipoPersonale char(1) NULL,
    ID_Grado_Previsto int NULL,
    id_sottoarea_prevista int NULL
);

/*
    Inserire qui l'elenco TTOO.
    Ogni riga vale una posizione prevista per una persona: NumeroPrevisto = 1.

    Esempi gia' impostati:
*/
INSERT INTO #TTOO_Input (SgUff1, SgUff2, SgUff3, Descr_incarico)
VALUES
    ('D', NULL, NULL, 'Direttore'),
    ('SPD', NULL, NULL, 'Capo Segreteria'),
    ('SPD', NULL, NULL, 'Addetto'),
    ('SPD', NULL, NULL, 'Addetto'),
    ('SPD', NULL, NULL, 'Addetto'),
    ('SPD', NULL, NULL, 'Addetto'),
    ('SPP', NULL, NULL, 'Capo Servizio'),
    ('SPP', NULL, NULL, 'Addetto'),
    ('SPP', NULL, NULL, 'Addetto'),
    ('SVRP', NULL, NULL, 'Capo Servizio'),
    ('SVRP', NULL, NULL, 'Addetto'),
    ('SVRP', NULL, NULL, 'Addetto'),
    ('SPMP', NULL, NULL, 'Capo Servizio'),
    ('SPMP', 'S1', NULL, 'Capo Sezione'),
    ('SPMP', 'S1', NULL, 'Addetto'),
    ('SPMP', 'S1', NULL, 'Addetto'),
    ('SPMP', 'S2', NULL, 'Capo Sezione'),
    ('SPMP', 'S2', NULL, 'Addetto'),
    ('SPMP', 'S2', NULL, 'Addetto'),
    ('SPMP', 'S3', NULL, 'Capo Sezione'),
    ('SPMP', 'S3', NULL, 'Addetto'),
    ('SPMP', 'S3', NULL, 'Addetto');

/*
    TODO: completare qui il resto dell'elenco con lo stesso formato:
        ('SSD', NULL, NULL, 'Capo Servizio'),
        ('SSD', 'S1', NULL, 'Capo Sezione'),
        ('SSD', 'S1', 'NIPM', 'Capo Nucleo'),
        ...
*/

/* Normalizzazione spazi vuoti */
UPDATE #TTOO_Input
SET
    SgUff1 = NULLIF(LTRIM(RTRIM(SgUff1)), ''),
    SgUff2 = NULLIF(LTRIM(RTRIM(SgUff2)), ''),
    SgUff3 = NULLIF(LTRIM(RTRIM(SgUff3)), ''),
    Descr_incarico = NULLIF(LTRIM(RTRIM(Descr_incarico)), '');

CREATE TABLE #TTOO_Risolte
(
    RowNo int NOT NULL PRIMARY KEY,
    ID_Uff1 int NULL,
    ID_Uff2 int NULL,
    ID_Uff3 int NULL,
    id_tipo_incarico int NULL,
    TipoPersonale char(1) NULL,
    ID_Grado_Previsto int NULL,
    id_sottoarea_prevista int NULL,
    NumeroPrevisto int NOT NULL,
    SgUff1 varchar(50) NOT NULL,
    SgUff2 varchar(50) NULL,
    SgUff3 varchar(50) NULL,
    Descr_incarico varchar(50) NOT NULL
);

INSERT INTO #TTOO_Risolte
(
    RowNo,
    ID_Uff1,
    ID_Uff2,
    ID_Uff3,
    id_tipo_incarico,
    TipoPersonale,
    ID_Grado_Previsto,
    id_sottoarea_prevista,
    NumeroPrevisto,
    SgUff1,
    SgUff2,
    SgUff3,
    Descr_incarico
)
SELECT
    I.RowNo,
    L1.ID_Uff1,
    L2.ID_Uff2,
    L3.ID_Uff3,
    TI.id_tipo_incarico,
    I.TipoPersonale,
    I.ID_Grado_Previsto,
    I.id_sottoarea_prevista,
    I.NumeroPrevisto,
    I.SgUff1,
    I.SgUff2,
    I.SgUff3,
    I.Descr_incarico
FROM #TTOO_Input I
LEFT JOIN dbo.Liv1Uff L1
    ON L1.SgUff1 = I.SgUff1
   AND L1.Attivo = 'S'
LEFT JOIN dbo.Liv2Uff L2
    ON L2.ID_Uff1 = L1.ID_Uff1
   AND L2.SgUff2 = I.SgUff2
LEFT JOIN dbo.Liv3Uff L3
    ON L3.ID_Uff1 = L1.ID_Uff1
   AND L3.ID_Uff2 = L2.ID_Uff2
   AND L3.SgUff3 = I.SgUff3
LEFT JOIN dbo.Tipo_incarichi TI
    ON TI.Descr_incarico = I.Descr_incarico;

/* =========================
   VALIDAZIONE
   ========================= */
SELECT
    RowNo,
    SgUff1,
    SgUff2,
    SgUff3,
    Descr_incarico,
    CASE WHEN ID_Uff1 IS NULL THEN 'SgUff1 non trovato o non attivo' END AS ErroreUff1,
    CASE WHEN SgUff2 IS NOT NULL AND ID_Uff2 IS NULL THEN 'SgUff2 non trovato sotto SgUff1' END AS ErroreUff2,
    CASE WHEN SgUff3 IS NOT NULL AND ID_Uff3 IS NULL THEN 'SgUff3 non trovato sotto SgUff1/SgUff2' END AS ErroreUff3,
    CASE WHEN id_tipo_incarico IS NULL THEN 'Tipo incarico non trovato' END AS ErroreIncarico,
    CASE
        WHEN TipoPersonale IS NULL THEN 'TipoPersonale mancante'
        WHEN TipoPersonale = 'M' AND ID_Grado_Previsto IS NULL THEN 'Grado previsto mancante'
        WHEN TipoPersonale = 'C' AND id_sottoarea_prevista IS NULL THEN 'Sottoarea prevista mancante'
    END AS ErroreProfiloTTOO
FROM #TTOO_Risolte
WHERE ID_Uff1 IS NULL
   OR (SgUff2 IS NOT NULL AND ID_Uff2 IS NULL)
   OR (SgUff3 IS NOT NULL AND ID_Uff3 IS NULL)
   OR id_tipo_incarico IS NULL
   OR TipoPersonale IS NULL
   OR (TipoPersonale = 'M' AND ID_Grado_Previsto IS NULL)
   OR (TipoPersonale = 'C' AND id_sottoarea_prevista IS NULL);

/*
    INSERT finale.
    Lasciato commentato finche' non sono compilati TipoPersonale e grado/sottoarea.

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
    R.ID_Uff1,
    R.ID_Uff2,
    R.ID_Uff3,
    R.id_tipo_incarico,
    R.TipoPersonale,
    R.ID_Grado_Previsto,
    R.id_sottoarea_prevista,
    R.NumeroPrevisto,
    'S',
    'Import da elenco organico TTOO'
FROM #TTOO_Risolte R
WHERE R.ID_Uff1 IS NOT NULL
  AND (R.SgUff2 IS NULL OR R.ID_Uff2 IS NOT NULL)
  AND (R.SgUff3 IS NULL OR R.ID_Uff3 IS NOT NULL)
  AND R.id_tipo_incarico IS NOT NULL
  AND
  (
      (R.TipoPersonale = 'M' AND R.ID_Grado_Previsto IS NOT NULL AND R.id_sottoarea_prevista IS NULL)
   OR (R.TipoPersonale = 'C' AND R.id_sottoarea_prevista IS NOT NULL AND R.ID_Grado_Previsto IS NULL)
  )
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.TTOO_Posizioni T
      WHERE T.ID_Uff1 = R.ID_Uff1
        AND ISNULL(T.ID_Uff2, -1) = ISNULL(R.ID_Uff2, -1)
        AND ISNULL(T.ID_Uff3, -1) = ISNULL(R.ID_Uff3, -1)
        AND T.id_tipo_incarico = R.id_tipo_incarico
        AND T.TipoPersonale = R.TipoPersonale
        AND ISNULL(T.ID_Grado_Previsto, -1) = ISNULL(R.ID_Grado_Previsto, -1)
        AND ISNULL(T.id_sottoarea_prevista, -1) = ISNULL(R.id_sottoarea_prevista, -1)
  );
*/

ROLLBACK TRAN;
GO

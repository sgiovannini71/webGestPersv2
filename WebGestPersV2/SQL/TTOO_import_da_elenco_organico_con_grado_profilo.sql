USE [DipendentiDB];
GO

/*
    Import TTOO da elenco organico con GRADO/PROFILO.

    Lo script e' volutamente in staging:
    1. carica le righe testuali in #TTOO_Input;
    2. determina TipoPersonale:
       - C per Dirigente/Funzionario/Assistente;
       - M per gli altri gradi militari;
       - X per EXTRA-ORGANICO o righe non inseribili in dbo.TTOO_Posizioni;
    3. risolve uffici e incarichi;
    4. usa mapping espliciti #MapGradi e #MapAree per tradurre GRADO/PROFILO in ID;
    5. mostra gli errori residui;
    6. inserisce solo le righe completamente risolte.

    Nota: dbo.TTOO_Posizioni consente una sola qualifica per riga:
      - militare: un solo ID_Grado_Previsto
      - civile: una sola id_sottoarea_prevista
    Le righe con valori multipli tipo Ten.Col./Magg. o profili multipli devono essere mappate
    manualmente al grado/sottoarea prevalente, oppure duplicate a monte se servono piu' posizioni.
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
    Descr_incarico varchar(50) NULL,
    GradoRaw varchar(255) NULL,
    ProfiloRaw varchar(500) NULL,
    NumeroPrevisto int NOT NULL DEFAULT 1
);

/*
    PASTE DATI QUI.
    Formato:
      ('1LIV','2LIV','3LIV','INCARICO','GRADO','PROFILO'),

    Ho lasciato qui le prime righe come esempio operativo. Per evitare errori di copiatura su
    un elenco cosi' lungo, conviene incollare/esportare il tuo elenco completo da Excel in CSV/TSV
    e trasformarlo in VALUES con un editor o con uno script dedicato.
*/
INSERT INTO #TTOO_Input (SgUff1, SgUff2, SgUff3, Descr_incarico, GradoRaw, ProfiloRaw)
VALUES
    ('D', NULL, NULL, 'Direttore', 'Gen.Isp.Capo/Gen.Isp.', 'G.A.r.n.'),
    ('SPD', NULL, NULL, 'Capo Segreteria', 'Ten.Col.', 'A.A.r.a.n.'),
    ('SPD', NULL, NULL, 'Addetto', 'Magg./Cap.', 'A.A.r.a.n./s.'),
    ('SPD', NULL, NULL, 'Addetto', 'SU.', 'Supporto Servizi di Amministrazione'),
    ('SPD', NULL, NULL, 'Addetto', 'SU.', 'Supporto Servizi di Amministrazione'),
    ('SPD', NULL, NULL, 'Addetto', 'SU.', 'Supporto Servizi di Amministrazione'),
    ('SPP', NULL, NULL, 'Capo Servizio', 'Funzionario', 'Funzionario Specialista Tecnico'),
    ('SPP', NULL, NULL, 'Addetto', 'Assistente', 'Assistente amministrativo'),
    ('SPP', NULL, NULL, 'Addetto', 'Assistente', 'Assistente amministrativo'),
    ('SVRP', NULL, NULL, 'Capo Servizio', 'Funzionario', 'Funzionario Amministrativo'),
    ('SVRP', NULL, NULL, 'Addetto', 'SU.', 'Supporto Serv. Amm./Manut. Aeromobili'),
    ('SVRP', NULL, NULL, 'Addetto', 'Graduato', 'Ass. di branca/SAAL Fr./Supporto Serv. Amm. - Manut. Aeromobili'),
    ('SPMP', NULL, NULL, 'Capo Servizio', 'Ten.Col.', 'G.A.r.n.'),
    ('SPMP', 'S1', NULL, 'Capo Sezione', 'Ten.Col.', 'C.C.r.n.'),
    ('SPMP', 'S1', NULL, 'Addetto', 'SU.', 'Ass. di branca/SAAL Fr./Supporto Serv. Amm. - Manut. Aeromobili'),
    ('SPMP', 'S1', NULL, 'Addetto', 'SU.', 'Supporto Serv. Amm./Manut. Aeromobili'),
    ('SPMP', 'S2', NULL, 'Capo Sezione', 'Ten.Col.', 'G.A.r.n.'),
    ('SPMP', 'S2', NULL, 'Addetto', 'SU.', 'Ass. di branca/SAAL Fr./Supporto Serv. Amm. - Manut. Aeromobili'),
    ('SPMP', 'S2', NULL, 'Addetto', 'SU.', 'Supporto Servizi di Amministrazione/Supp.Inf.Cib.'),
    ('SPMP', 'S3', NULL, 'Capo Sezione', 'Ten.Col.', 'G.A.r.n.'),
    ('SPMP', 'S3', NULL, 'Addetto', 'SU.', 'Supporto Serv. Amm./Manut. Aeromobili'),
    ('SPMP', 'S3', NULL, 'Addetto', 'Assistente', 'Assistente Amministrativo');

/*
    MAPPING GRADI.
    Compila/aggiusta questi mapping in base ai valori reali della tabella dbo.Gradi.
    GradoRaw e' il testo dell'elenco; ID_Grado va preso da dbo.Gradi.
*/
CREATE TABLE #MapGradi
(
    GradoRaw varchar(255) NOT NULL PRIMARY KEY,
    ID_Grado int NOT NULL
);

/* Esempio: decommentare e sostituire gli ID reali.
INSERT INTO #MapGradi (GradoRaw, ID_Grado)
VALUES
    ('Gen.Isp.Capo/Gen.Isp.', 0),
    ('Brig.Gen.', 0),
    ('Col.', 0),
    ('Ten.Col.', 0),
    ('Ten.Col./Magg.', 0),
    ('Magg./Cap.', 0),
    ('SU.', 0),
    ('Graduato', 0);
*/

/*
    MAPPING AREE/SOTTOAREE CIVILI.
    ProfiloRaw e' il testo dell'elenco; id_sottoarea va preso da dbo.Aree.
*/
CREATE TABLE #MapAree
(
    ProfiloRaw varchar(500) NOT NULL PRIMARY KEY,
    id_sottoarea int NOT NULL
);

/* Esempio: decommentare e sostituire gli ID reali.
INSERT INTO #MapAree (ProfiloRaw, id_sottoarea)
VALUES
    ('Dirigente Amministrativo', 0),
    ('Funzionario Amministrativo', 0),
    ('Funzionario Specialista Tecnico', 0),
    ('Funzionario tecnico per l''informatica', 0),
    ('Assistente Amministrativo', 0),
    ('Assistente amministrativo', 0),
    ('Assistente tecnico per l''informatica', 0);
*/

UPDATE #TTOO_Input
SET
    SgUff1 = NULLIF(LTRIM(RTRIM(SgUff1)), ''),
    SgUff2 = NULLIF(LTRIM(RTRIM(SgUff2)), ''),
    SgUff3 = NULLIF(LTRIM(RTRIM(SgUff3)), ''),
    Descr_incarico = NULLIF(LTRIM(RTRIM(Descr_incarico)), ''),
    GradoRaw = NULLIF(LTRIM(RTRIM(GradoRaw)), ''),
    ProfiloRaw = NULLIF(LTRIM(RTRIM(ProfiloRaw)), '');

;WITH Risolte AS
(
    SELECT
        I.RowNo,
        L1.ID_Uff1,
        L2.ID_Uff2,
        L3.ID_Uff3,
        TI.id_tipo_incarico,
        CASE
            WHEN I.ProfiloRaw LIKE '%EXTRA-ORGANICO%' THEN 'X'
            WHEN I.GradoRaw IN ('Dirigente', 'Funzionario', 'Assistente') THEN 'C'
            WHEN I.GradoRaw IS NULL OR I.GradoRaw = '/' THEN 'X'
            ELSE 'M'
        END AS TipoPersonale,
        MG.ID_Grado AS ID_Grado_Previsto,
        MA.id_sottoarea AS id_sottoarea_prevista,
        I.NumeroPrevisto,
        I.SgUff1,
        I.SgUff2,
        I.SgUff3,
        I.Descr_incarico,
        I.GradoRaw,
        I.ProfiloRaw
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
        ON TI.Descr_incarico = I.Descr_incarico
    LEFT JOIN #MapGradi MG
        ON MG.GradoRaw = I.GradoRaw
    LEFT JOIN #MapAree MA
        ON MA.ProfiloRaw = I.ProfiloRaw
)
SELECT
    RowNo,
    SgUff1,
    SgUff2,
    SgUff3,
    Descr_incarico,
    GradoRaw,
    ProfiloRaw,
    TipoPersonale,
    CASE WHEN ID_Uff1 IS NULL THEN 'SgUff1 non trovato o non attivo' END AS ErroreUff1,
    CASE WHEN SgUff2 IS NOT NULL AND ID_Uff2 IS NULL THEN 'SgUff2 non trovato sotto SgUff1' END AS ErroreUff2,
    CASE WHEN SgUff3 IS NOT NULL AND ID_Uff3 IS NULL THEN 'SgUff3 non trovato sotto SgUff1/SgUff2' END AS ErroreUff3,
    CASE WHEN Descr_incarico IS NOT NULL AND id_tipo_incarico IS NULL THEN 'Tipo incarico non trovato' END AS ErroreIncarico,
    CASE WHEN TipoPersonale = 'M' AND ID_Grado_Previsto IS NULL THEN 'Mapping grado mancante' END AS ErroreGrado,
    CASE WHEN TipoPersonale = 'C' AND id_sottoarea_prevista IS NULL THEN 'Mapping profilo/sottoarea mancante' END AS ErroreArea,
    CASE WHEN TipoPersonale = 'X' THEN 'Riga extra-organico o non inseribile nella struttura attuale' END AS ErroreExtraOrganico
FROM Risolte
WHERE ID_Uff1 IS NULL
   OR (SgUff2 IS NOT NULL AND ID_Uff2 IS NULL)
   OR (SgUff3 IS NOT NULL AND ID_Uff3 IS NULL)
   OR (Descr_incarico IS NOT NULL AND id_tipo_incarico IS NULL)
   OR (TipoPersonale = 'M' AND ID_Grado_Previsto IS NULL)
   OR (TipoPersonale = 'C' AND id_sottoarea_prevista IS NULL)
   OR TipoPersonale = 'X'
ORDER BY RowNo;

/*
    INSERT finale: decommentare solo quando la SELECT di validazione non restituisce errori
    per le righe che vuoi inserire.

;WITH Risolte AS
(
    SELECT
        I.RowNo,
        L1.ID_Uff1,
        L2.ID_Uff2,
        L3.ID_Uff3,
        TI.id_tipo_incarico,
        CASE
            WHEN I.ProfiloRaw LIKE '%EXTRA-ORGANICO%' THEN 'X'
            WHEN I.GradoRaw IN ('Dirigente', 'Funzionario', 'Assistente') THEN 'C'
            WHEN I.GradoRaw IS NULL OR I.GradoRaw = '/' THEN 'X'
            ELSE 'M'
        END AS TipoPersonale,
        MG.ID_Grado AS ID_Grado_Previsto,
        MA.id_sottoarea AS id_sottoarea_prevista,
        I.NumeroPrevisto
    FROM #TTOO_Input I
    LEFT JOIN dbo.Liv1Uff L1 ON L1.SgUff1 = I.SgUff1 AND L1.Attivo = 'S'
    LEFT JOIN dbo.Liv2Uff L2 ON L2.ID_Uff1 = L1.ID_Uff1 AND L2.SgUff2 = I.SgUff2
    LEFT JOIN dbo.Liv3Uff L3 ON L3.ID_Uff1 = L1.ID_Uff1 AND L3.ID_Uff2 = L2.ID_Uff2 AND L3.SgUff3 = I.SgUff3
    LEFT JOIN dbo.Tipo_incarichi TI ON TI.Descr_incarico = I.Descr_incarico
    LEFT JOIN #MapGradi MG ON MG.GradoRaw = I.GradoRaw
    LEFT JOIN #MapAree MA ON MA.ProfiloRaw = I.ProfiloRaw
)
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
    CASE WHEN R.TipoPersonale = 'M' THEN R.ID_Grado_Previsto ELSE NULL END,
    CASE WHEN R.TipoPersonale = 'C' THEN R.id_sottoarea_prevista ELSE NULL END,
    R.NumeroPrevisto,
    'S',
    'Import da elenco organico TTOO con grado/profilo'
FROM Risolte R
WHERE R.ID_Uff1 IS NOT NULL
  AND R.id_tipo_incarico IS NOT NULL
  AND
  (
      (R.TipoPersonale = 'M' AND R.ID_Grado_Previsto IS NOT NULL)
   OR (R.TipoPersonale = 'C' AND R.id_sottoarea_prevista IS NOT NULL)
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

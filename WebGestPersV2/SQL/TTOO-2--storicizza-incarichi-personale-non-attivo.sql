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
-- ANTEPRIMA INCARICHI INTERESSATI
------------------------------------------------------------
SELECT
    EP.IDPersonale,
    EP.Cognome,
    EP.Nome,
    EP.Stato_Servizio,
    I.id_incarico,
    I.id_tipo_incarico,
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
LEFT JOIN dbo.Tipo_incarichi TI
    ON TI.id_tipo_incarico = I.id_tipo_incarico
WHERE ISNULL(EP.Stato_Servizio, '') <> 'Attivo'
ORDER BY
    EP.Stato_Servizio,
    EP.Cognome,
    EP.Nome;

------------------------------------------------------------
-- ESECUZIONE
------------------------------------------------------------
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('tempdb..#IncarichiDaPulire') IS NOT NULL
        DROP TABLE #IncarichiDaPulire;

    SELECT
        I.id_incarico,
        I.IDPersonale,
        I.principale,
        I.Data_inizio,
        I.id_tipo_incarico,
        I.ID_Uff1,
        I.ID_Uff2,
        I.ID_Uff3,
        EP.Stato_Servizio
    INTO #IncarichiDaPulire
    FROM dbo.Incarichi I
    INNER JOIN dbo.ElencoPersonale EP
        ON EP.IDPersonale = I.IDPersonale
    WHERE ISNULL(EP.Stato_Servizio, '') <> 'Attivo';

    --------------------------------------------------------
    -- Storico incarichi
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
        IDP.IDPersonale,
        CONCAT(
            ISNULL(TI.Descr_incarico, ''),
            ' - ',
            ISNULL(L1.DescUff1, ''),
            ' - ',
            ISNULL(L2.DescUff2, ''),
            ' - ',
            ISNULL(L3.DescUff3, ''),
            ' - Stato servizio: ',
            ISNULL(IDP.Stato_Servizio, '')
        ) AS Incarico,
        IDP.principale,
        IDP.Data_inizio,
        @DataChiusuraIncarichi
    FROM #IncarichiDaPulire IDP
    LEFT JOIN dbo.Tipo_incarichi TI
        ON TI.id_tipo_incarico = IDP.id_tipo_incarico
    LEFT JOIN dbo.Liv1Uff L1
        ON L1.ID_Uff1 = IDP.ID_Uff1
    LEFT JOIN dbo.Liv2Uff L2
        ON L2.ID_Uff1 = IDP.ID_Uff1
       AND L2.ID_Uff2 = IDP.ID_Uff2
    LEFT JOIN dbo.Liv3Uff L3
        ON L3.ID_Uff1 = IDP.ID_Uff1
       AND L3.ID_Uff2 = IDP.ID_Uff2
       AND L3.ID_Uff3 = IDP.ID_Uff3;

    --------------------------------------------------------
    -- Eliminazione incarichi esistenti
    --------------------------------------------------------
    DELETE I
    FROM dbo.Incarichi I
    INNER JOIN #IncarichiDaPulire IDP
        ON IDP.id_incarico = I.id_incarico;

    --------------------------------------------------------
    -- Inserimento di un solo incarico default per persona
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
        IDP.IDPersonale,
        @IdTipoIncaricoNA,
        1,
        CAST(@DataChiusuraIncarichi AS datetime),
        @ID_Uff1_NA,
        @ID_Uff2_NA,
        @ID_Uff3_NA
    FROM #IncarichiDaPulire IDP;

    --------------------------------------------------------
    -- RIEPILOGO
    --------------------------------------------------------
    SELECT
        COUNT(*) AS IncarichiStoricizzatiERimossi,
        COUNT(DISTINCT IDPersonale) AS PersoneInteressate
    FROM #IncarichiDaPulire;

    --------------------------------------------------------
    -- VERIFICA FINALE
    --------------------------------------------------------
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
    LEFT JOIN dbo.Tipo_incarichi TI
        ON TI.id_tipo_incarico = I.id_tipo_incarico
    WHERE ISNULL(EP.Stato_Servizio, '') <> 'Attivo'
    ORDER BY
        EP.Stato_Servizio,
        EP.Cognome,
        EP.Nome;

    --------------------------------------------------------
    -- SICUREZZA: per test lasciare ROLLBACK.
    -- Per rendere definitive le modifiche, commentare ROLLBACK
    -- e decommentare COMMIT.
    --------------------------------------------------------
    ROLLBACK TRANSACTION;
    -- COMMIT TRANSACTION;

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
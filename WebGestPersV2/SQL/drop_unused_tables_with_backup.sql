/*
    Backup e rimozione tabelle non usate dal codice webGestPers.

    Database sorgente: DipendentiDB
    Include anche dbo.Tipo_Statini su richiesta esplicita.

    ATTENZIONE
    - Eseguire prima in ambiente di test.
    - Lo script copia struttura base e dati in un database di backup tramite SELECT INTO.
      Non replica indici, PK, FK, trigger, default constraint o permessi.
    - Se vuoi un backup completo fisico del database, eseguire anche un BACKUP DATABASE
      prima di questo script.
    - La rimozione delle tabelle e' distruttiva. Lo script usa una transazione e verifica
      i conteggi, ma dopo il COMMIT le tabelle dbo originali non esisteranno piu.
*/

USE [master];
GO

IF DB_ID(N'DipendentiDB') IS NULL
BEGIN
    THROW 50000, 'Database sorgente DipendentiDB non trovato.', 1;
END;
GO

DECLARE @BackupDb sysname = N'DipendentiDB_Backup_TabelleNonUsate_' + CONVERT(char(8), GETDATE(), 112);
DECLARE @Sql nvarchar(max);

IF DB_ID(@BackupDb) IS NOT NULL
BEGIN
    THROW 50001, 'Il database di backup giornaliero esiste gia. Rinominarlo o rieseguire domani.', 1;
END;

SET @Sql = N'CREATE DATABASE ' + QUOTENAME(@BackupDb) + N';';
EXEC sys.sp_executesql @Sql;
GO

USE [DipendentiDB];
GO

SET XACT_ABORT ON;
GO

DECLARE @BackupDb sysname = N'DipendentiDB_Backup_TabelleNonUsate_' + CONVERT(char(8), GETDATE(), 112);
DECLARE @BackupSchema sysname = N'dbo';
DECLARE @Sql nvarchar(max);
DECLARE @TableName sysname;
DECLARE @SourceObject nvarchar(300);
DECLARE @BackupObject nvarchar(500);
DECLARE @SourceCount bigint;
DECLARE @BackupCount bigint;

DECLARE @Tables TABLE
(
    TableName sysname NOT NULL PRIMARY KEY
);

INSERT INTO @Tables (TableName)
VALUES
    (N'Allegati'),
    (N'Corsi_inf'),
    (N'CorsiQualificazione'),
    (N'CorsiSuperiori'),
    (N'DateFestivita'),
    (N'Deleghe'),
    (N'LingueEstere'),
    (N'Modifiche_Presenze'),
    (N'NucleoFamigliare'),
    (N'OrariFascia'),
    (N'OrariFascia-old'),
    (N'OrarioC4'),
    (N'PianoFerie'),
    (N'Presenze'),
    (N'Profilo_inf'),
    (N'RicompenseOnoreficenze'),
    (N'SediServizio'),
    (N'Test_ElencoPersonale'),
    (N'Tipo_corsi'),
    (N'Tipo_Statini'),
    (N'TipoRilevamento'),
    (N'Utente'),
    (N'VisibilitaStatini');

IF EXISTS
(
    SELECT 1
    FROM @Tables t
    WHERE OBJECT_ID(QUOTENAME(N'dbo') + N'.' + QUOTENAME(t.TableName), N'U') IS NULL
)
BEGIN
    SELECT t.TableName AS TabellaNonTrovata
    FROM @Tables t
    WHERE OBJECT_ID(QUOTENAME(N'dbo') + N'.' + QUOTENAME(t.TableName), N'U') IS NULL;

    THROW 50002, 'Una o piu tabelle da trattare non esistono nel database sorgente. Controllare elenco.', 1;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'tempdb..#BackupCheck') IS NOT NULL
    BEGIN
        DROP TABLE #BackupCheck;
    END;

    CREATE TABLE #BackupCheck
    (
        TableName sysname NOT NULL,
        SourceRows bigint NOT NULL,
        BackupRows bigint NOT NULL
    );

    DECLARE table_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT TableName
        FROM @Tables
        ORDER BY TableName;

    OPEN table_cursor;
    FETCH NEXT FROM table_cursor INTO @TableName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SourceObject = QUOTENAME(N'dbo') + N'.' + QUOTENAME(@TableName);
        SET @BackupObject = QUOTENAME(@BackupDb) + N'.' + QUOTENAME(@BackupSchema) + N'.' + QUOTENAME(@TableName);

        SET @Sql = N'SELECT @CountOut = COUNT_BIG(*) FROM ' + @SourceObject + N';';
        EXEC sys.sp_executesql @Sql, N'@CountOut bigint OUTPUT', @SourceCount OUTPUT;

        SET @Sql = N'SELECT * INTO ' + @BackupObject + N' FROM ' + @SourceObject + N';';
        EXEC sys.sp_executesql @Sql;

        SET @Sql = N'SELECT @CountOut = COUNT_BIG(*) FROM ' + @BackupObject + N';';
        EXEC sys.sp_executesql @Sql, N'@CountOut bigint OUTPUT', @BackupCount OUTPUT;

        INSERT INTO #BackupCheck (TableName, SourceRows, BackupRows)
        VALUES (@TableName, @SourceCount, @BackupCount);

        IF @SourceCount <> @BackupCount
        BEGIN
            DECLARE @Msg nvarchar(4000) = N'Conteggio non coincidente per tabella ' + @TableName + N'. Operazione annullata.';
            THROW 50003, @Msg, 1;
        END;

        FETCH NEXT FROM table_cursor INTO @TableName;
    END;

    CLOSE table_cursor;
    DEALLOCATE table_cursor;

    /*
        Rimuove le foreign key che puntano alle tabelle da eliminare o partono da esse.
        Necessario per consentire il DROP TABLE. Attenzione: modifica anche tabelle non eliminate
        se contengono FK verso una delle tabelle in elenco.
    */
    DECLARE @DropFkSql nvarchar(max) = N'';

    SELECT @DropFkSql = @DropFkSql +
        N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id)) + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) +
        N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(13) + CHAR(10)
    FROM sys.foreign_keys fk
    WHERE fk.parent_object_id IN
    (
        SELECT OBJECT_ID(QUOTENAME(N'dbo') + N'.' + QUOTENAME(TableName)) FROM @Tables
    )
    OR fk.referenced_object_id IN
    (
        SELECT OBJECT_ID(QUOTENAME(N'dbo') + N'.' + QUOTENAME(TableName)) FROM @Tables
    );

    IF LEN(@DropFkSql) > 0
    BEGIN
        EXEC sys.sp_executesql @DropFkSql;
    END;

    DECLARE drop_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT TableName
        FROM @Tables
        ORDER BY TableName DESC;

    OPEN drop_cursor;
    FETCH NEXT FROM drop_cursor INTO @TableName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SourceObject = QUOTENAME(N'dbo') + N'.' + QUOTENAME(@TableName);
        SET @Sql = N'DROP TABLE ' + @SourceObject + N';';
        EXEC sys.sp_executesql @Sql;

        FETCH NEXT FROM drop_cursor INTO @TableName;
    END;

    CLOSE drop_cursor;
    DEALLOCATE drop_cursor;

    SELECT
        TableName,
        SourceRows,
        BackupRows,
        CASE WHEN SourceRows = BackupRows THEN 'OK' ELSE 'KO' END AS EsitoBackup
    FROM #BackupCheck
    ORDER BY TableName;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF CURSOR_STATUS('local', 'table_cursor') > -1
    BEGIN
        CLOSE table_cursor;
    END;

    IF CURSOR_STATUS('local', 'table_cursor') > -3
    BEGIN
        DEALLOCATE table_cursor;
    END;

    IF CURSOR_STATUS('local', 'drop_cursor') > -1
    BEGIN
        CLOSE drop_cursor;
    END;

    IF CURSOR_STATUS('local', 'drop_cursor') > -3
    BEGIN
        DEALLOCATE drop_cursor;
    END;

    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity int = ERROR_SEVERITY();
    DECLARE @ErrorState int = ERROR_STATE();

    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO

/*
    Verifica post-esecuzione consigliata:

    SELECT name FROM sys.databases WHERE name LIKE 'DipendentiDB_Backup_TabelleNonUsate_%';

    -- Sostituire AAAAMMGG con la data usata nel nome DB:
    SELECT TABLE_SCHEMA, TABLE_NAME
    FROM [DipendentiDB_Backup_TabelleNonUsate_AAAAMMGG].INFORMATION_SCHEMA.TABLES
    ORDER BY TABLE_SCHEMA, TABLE_NAME;
*/
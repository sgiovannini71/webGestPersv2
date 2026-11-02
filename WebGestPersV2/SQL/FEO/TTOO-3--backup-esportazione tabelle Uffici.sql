------------------------------------------------------------
-- BACKUP TABELLE UFFICI PRIMA DELLA RIORGANIZZAZIONE
------------------------------------------------------------

DECLARE @Suffix varchar(20) = CONVERT(varchar(8), GETDATE(), 112);

DECLARE @sql nvarchar(max);

------------------------------------------------------------
-- Liv1Uff
------------------------------------------------------------
SET @sql = '
SELECT *
INTO dbo.Liv1Uff_BKP_' + @Suffix + '
FROM dbo.Liv1Uff;
';

EXEC sp_executesql @sql;

------------------------------------------------------------
-- Liv2Uff
------------------------------------------------------------
SET @sql = '
SELECT *
INTO dbo.Liv2Uff_BKP_' + @Suffix + '
FROM dbo.Liv2Uff;
';

EXEC sp_executesql @sql;

------------------------------------------------------------
-- Liv3Uff
------------------------------------------------------------
SET @sql = '
SELECT *
INTO dbo.Liv3Uff_BKP_' + @Suffix + '
FROM dbo.Liv3Uff;
';

EXEC sp_executesql @sql;

------------------------------------------------------------
-- VERIFICA BACKUP
------------------------------------------------------------
SELECT 'Liv1Uff' AS Tabella, COUNT(*) AS Righe FROM dbo.Liv1Uff
UNION ALL
SELECT 'Liv2Uff', COUNT(*) FROM dbo.Liv2Uff
UNION ALL
SELECT 'Liv3Uff', COUNT(*) FROM dbo.Liv3Uff;
USE [DipendentiDB];
GO

IF OBJECT_ID(N'dbo.TTOO_Posizioni_FFAA', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.TTOO_Posizioni_FFAA;
END;
GO

CREATE TABLE dbo.TTOO_Posizioni_FFAA
(
    IDPosizioneTTOO int NOT NULL,
    id_arma int NOT NULL,

    CONSTRAINT PK_TTOO_Posizioni_FFAA
        PRIMARY KEY CLUSTERED
        (
            IDPosizioneTTOO,
            id_arma
        ),

    CONSTRAINT FK_TTOO_Posizioni_FFAA_Posizione
        FOREIGN KEY (IDPosizioneTTOO)
        REFERENCES dbo.TTOO_Posizioni(IDPosizioneTTOO)
        ON DELETE CASCADE,

    CONSTRAINT FK_TTOO_Posizioni_FFAA_FFAA
        FOREIGN KEY (id_arma)
        REFERENCES dbo.FFAA(id_arma)
);
GO

CREATE NONCLUSTERED INDEX IX_TTOO_Posizioni_FFAA_id_arma
ON dbo.TTOO_Posizioni_FFAA(id_arma);
GO
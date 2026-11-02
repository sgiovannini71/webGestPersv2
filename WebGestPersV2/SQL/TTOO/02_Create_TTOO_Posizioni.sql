/*
===============================================================================
 Script.......: 02_Create_TTOO_Posizioni.sql
 Database.....: DipendentiDB
 Scopo........: Crea la tabella principale delle posizioni previste TTOO.

 La tabella distingue le posizioni militari da quelle civili:
 - militari: categoria valorizzata e titolo civile nullo;
 - civili: titolo valorizzato e categoria militare nulla.
===============================================================================
*/
USE [DipendentiDB];
GO

IF OBJECT_ID(N'dbo.TTOO_Posizioni', N'U') IS NOT NULL
    DROP TABLE dbo.TTOO_Posizioni;
GO

CREATE TABLE dbo.TTOO_Posizioni
(
    IDPosizioneTTOO int IDENTITY(1,1) NOT NULL,
    ID_Uff1 int NOT NULL,
    ID_Uff2 int NULL,
    ID_Uff3 int NULL,
    id_tipo_incarico int NOT NULL,
    TipoPersonale char(1) NOT NULL,
    ID_CategoriaMilitare int NULL,
    ID_Titolo_Previsto int NULL,
    NumeroPrevisto int NOT NULL
        CONSTRAINT DF_TTOO_Posizioni_NumeroPrevisto DEFAULT (1),
    Attivo char(1) NOT NULL
        CONSTRAINT DF_TTOO_Posizioni_Attivo DEFAULT ('S'),
    Note varchar(255) NULL,

    CONSTRAINT PK_TTOO_Posizioni
        PRIMARY KEY CLUSTERED (IDPosizioneTTOO),

    CONSTRAINT FK_TTOO_Posizioni_CategoriaMilitare
        FOREIGN KEY (ID_CategoriaMilitare)
        REFERENCES dbo.TTOO_CategorieMilitari(ID_CategoriaMilitare),

    CONSTRAINT FK_TTOO_Posizioni_Liv1Uff
        FOREIGN KEY (ID_Uff1)
        REFERENCES dbo.Liv1Uff(ID_Uff1),

    CONSTRAINT FK_TTOO_Posizioni_Liv2Uff
        FOREIGN KEY (ID_Uff2)
        REFERENCES dbo.Liv2Uff(ID_Uff2),

    CONSTRAINT FK_TTOO_Posizioni_Liv3Uff
        FOREIGN KEY (ID_Uff3)
        REFERENCES dbo.Liv3Uff(ID_Uff3),

    CONSTRAINT FK_TTOO_Posizioni_TipoIncarico
        FOREIGN KEY (id_tipo_incarico)
        REFERENCES dbo.Tipo_incarichi(id_tipo_incarico),

    CONSTRAINT FK_TTOO_Posizioni_Titoli
        FOREIGN KEY (ID_Titolo_Previsto)
        REFERENCES dbo.Titoli(ID_Titolo),

    CONSTRAINT CK_TTOO_Posizioni_Attivo
        CHECK (Attivo IN ('S','N')),

    CONSTRAINT CK_TTOO_Posizioni_TipoPersonale
        CHECK (TipoPersonale IN ('M','C')),

    CONSTRAINT CK_TTOO_Posizioni_NumeroPrevisto
        CHECK (NumeroPrevisto > 0),

    CONSTRAINT CK_TTOO_Posizioni_Gerarchia
        CHECK (ID_Uff3 IS NULL OR ID_Uff2 IS NOT NULL),

    CONSTRAINT CK_TTOO_Posizioni_CategoriaTitolo
        CHECK
        (
            (TipoPersonale = 'M'
             AND ID_CategoriaMilitare IS NOT NULL
             AND ID_Titolo_Previsto IS NULL)
            OR
            (TipoPersonale = 'C'
             AND ID_CategoriaMilitare IS NULL
             AND ID_Titolo_Previsto IS NOT NULL)
        )
);
GO

CREATE NONCLUSTERED INDEX IX_TTOO_Posizioni_Uffici
ON dbo.TTOO_Posizioni(ID_Uff1, ID_Uff2, ID_Uff3);
GO

CREATE NONCLUSTERED INDEX IX_TTOO_Posizioni_Incarico
ON dbo.TTOO_Posizioni(id_tipo_incarico);
GO

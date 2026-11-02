/*
===============================================================================
 Script.......: 01_Create_TTOO_CategorieMilitari.sql
 Database.....: DipendentiDB
 Scopo........: Ricrea la tabella delle categorie militari TTOO.

 ATTENZIONE
 ------------------------------------------------------------------------------
 L'eventuale tabella esistente viene eliminata. Eseguire lo script solo durante
 l'installazione o il riallineamento completo del modulo TTOO.
===============================================================================
*/
USE [DipendentiDB];
GO

IF OBJECT_ID(N'dbo.TTOO_Posizioni_FFAA', N'U') IS NOT NULL
    DROP TABLE dbo.TTOO_Posizioni_FFAA;
GO
IF OBJECT_ID(N'dbo.TTOO_Posizioni', N'U') IS NOT NULL
    DROP TABLE dbo.TTOO_Posizioni;
GO
IF OBJECT_ID(N'dbo.TTOO_CategorieMilitari', N'U') IS NOT NULL
    DROP TABLE dbo.TTOO_CategorieMilitari;
GO

CREATE TABLE dbo.TTOO_CategorieMilitari
(
    ID_CategoriaMilitare int IDENTITY(1,1) NOT NULL,
    Sigla varchar(3) NOT NULL,
    Descrizione varchar(100) NOT NULL,
    Ordine int NOT NULL
        CONSTRAINT DF_TTOO_CategorieMilitari_Ordine DEFAULT (0),
    Attivo char(1) NOT NULL
        CONSTRAINT DF_TTOO_CategorieMilitari_Attivo DEFAULT ('S'),

    CONSTRAINT PK_TTOO_CategorieMilitari
        PRIMARY KEY CLUSTERED (ID_CategoriaMilitare),

    CONSTRAINT UQ_TTOO_CategorieMilitari_Sigla
        UNIQUE NONCLUSTERED (Sigla),

    CONSTRAINT CK_TTOO_CategorieMilitari_Attivo
        CHECK (Attivo IN ('S','N'))
);
GO

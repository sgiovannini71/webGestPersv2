USE [DipendentiDB];
GO

CREATE TABLE dbo.TTOO_Posizioni
(
    IDPosizioneTTOO int IDENTITY(1,1) NOT NULL PRIMARY KEY,

    ID_Uff1 int NOT NULL,
    ID_Uff2 int NULL,
    ID_Uff3 int NULL,

    id_tipo_incarico int NOT NULL,

    TipoPersonale char(1) NOT NULL, -- M = militare, C = civile

    ID_Grado_Previsto int NULL,     -- per militari
    id_sottoarea_prevista int NULL, -- per civili

    NumeroPrevisto int NOT NULL DEFAULT 1,

    Attivo char(1) NOT NULL DEFAULT 'S',
    Note varchar(255) NULL
);
GO
ALTER TABLE dbo.TTOO_Posizioni
ADD CONSTRAINT CK_TTOO_TipoPersonale
CHECK (TipoPersonale IN ('M','C'));
GO

ALTER TABLE dbo.TTOO_Posizioni
ADD CONSTRAINT CK_TTOO_Grado_Area
CHECK
(
    (TipoPersonale = 'M' AND ID_Grado_Previsto IS NOT NULL AND id_sottoarea_prevista IS NULL)
 OR (TipoPersonale = 'C' AND id_sottoarea_prevista IS NOT NULL AND ID_Grado_Previsto IS NULL)
);
GO
ALTER TABLE dbo.TTOO_Posizioni
ADD CONSTRAINT FK_TTOO_Liv1Uff
FOREIGN KEY (ID_Uff1) REFERENCES dbo.Liv1Uff(ID_Uff1);
GO

ALTER TABLE dbo.TTOO_Posizioni
ADD CONSTRAINT FK_TTOO_Liv2Uff
FOREIGN KEY (ID_Uff2) REFERENCES dbo.Liv2Uff(ID_Uff2);
GO

ALTER TABLE dbo.TTOO_Posizioni
ADD CONSTRAINT FK_TTOO_Liv3Uff
FOREIGN KEY (ID_Uff3) REFERENCES dbo.Liv3Uff(ID_Uff3);
GO

ALTER TABLE dbo.TTOO_Posizioni
ADD CONSTRAINT FK_TTOO_TipoIncarico
FOREIGN KEY (id_tipo_incarico) REFERENCES dbo.Tipo_incarichi(id_tipo_incarico);
GO

ALTER TABLE dbo.TTOO_Posizioni
ADD CONSTRAINT FK_TTOO_Gradi
FOREIGN KEY (ID_Grado_Previsto) REFERENCES dbo.Gradi(ID_Grado);
GO

ALTER TABLE dbo.TTOO_Posizioni
ADD CONSTRAINT FK_TTOO_Aree
FOREIGN KEY (id_sottoarea_prevista) REFERENCES dbo.Aree(id_sottoarea);
GO
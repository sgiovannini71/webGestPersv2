USE [DipendentiDB]
GO

/*
  Script generato da OdS-civili.txt
  Modalità test attiva di default: @TestMode = 1 esegue ROLLBACK.
  Per confermare gli inserimenti impostare @TestMode = 0.
*/

DECLARE @TestMode bit = 1;
DECLARE @DataInizio datetime = '2026-05-04';

BEGIN TRY
    BEGIN TRAN;

    PRINT 'Inserimento incarichi civili - avvio';

    -- Record 1 - sorgente riga 2
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 413
          AND id_tipo_incarico = 71
          AND principale = 1
          AND ID_Uff1 = 11
          AND ISNULL(ID_Uff2, -1) = ISNULL(77, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            413,
            71,
            @DataInizio,
            1,
            11,
            77,
            NULL
        );
    END

    -- Record 2 - sorgente riga 3
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 576
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 11
          AND ISNULL(ID_Uff2, -1) = ISNULL(77, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            576,
            38,
            @DataInizio,
            1,
            11,
            77,
            NULL
        );
    END

    -- Record 3 - sorgente riga 4
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 1
          AND id_tipo_incarico = 71
          AND principale = 0
          AND ID_Uff1 = 11
          AND ISNULL(ID_Uff2, -1) = ISNULL(108, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            1,
            71,
            @DataInizio,
            0,
            11,
            108,
            NULL
        );
    END

    -- Record 4 - sorgente riga 5
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 774
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 11
          AND ISNULL(ID_Uff2, -1) = ISNULL(107, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(165, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            774,
            38,
            @DataInizio,
            1,
            11,
            107,
            165
        );
    END

    -- Record 5 - sorgente riga 6
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 395
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(52, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            395,
            4,
            @DataInizio,
            1,
            12,
            52,
            NULL
        );
    END

    -- Record 6 - sorgente riga 7
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 594
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(52, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            594,
            38,
            @DataInizio,
            1,
            12,
            52,
            NULL
        );
    END

    -- Record 7 - sorgente riga 8
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 814
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(52, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            814,
            38,
            @DataInizio,
            1,
            12,
            52,
            NULL
        );
    END

    -- Record 8 - sorgente riga 9
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 766
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(53, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            766,
            4,
            @DataInizio,
            1,
            12,
            53,
            NULL
        );
    END

    -- Record 9 - sorgente riga 10
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 654
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(117, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            654,
            38,
            @DataInizio,
            1,
            12,
            117,
            NULL
        );
    END

    -- Record 10 - sorgente riga 11
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 644
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(117, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            644,
            38,
            @DataInizio,
            1,
            12,
            117,
            NULL
        );
    END

    -- Record 11 - sorgente riga 12
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 573
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(117, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            573,
            38,
            @DataInizio,
            1,
            12,
            117,
            NULL
        );
    END

    -- Record 12 - sorgente riga 13
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 638
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(117, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            638,
            38,
            @DataInizio,
            1,
            12,
            117,
            NULL
        );
    END

    -- Record 13 - sorgente riga 14
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 333
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(109, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            333,
            4,
            @DataInizio,
            1,
            12,
            109,
            NULL
        );
    END

    -- Record 14 - sorgente riga 15
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 806
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(109, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            806,
            38,
            @DataInizio,
            1,
            12,
            109,
            NULL
        );
    END

    -- Record 15 - sorgente riga 16
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 798
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(109, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            798,
            38,
            @DataInizio,
            1,
            12,
            109,
            NULL
        );
    END

    -- Record 16 - sorgente riga 17
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 328
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 16
          AND ISNULL(ID_Uff2, -1) = ISNULL(118, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(193, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            328,
            4,
            @DataInizio,
            1,
            16,
            118,
            193
        );
    END

    -- Record 17 - sorgente riga 18
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 760
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 16
          AND ISNULL(ID_Uff2, -1) = ISNULL(118, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(193, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            760,
            38,
            @DataInizio,
            1,
            16,
            118,
            193
        );
    END

    -- Record 18 - sorgente riga 19
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 771
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 16
          AND ISNULL(ID_Uff2, -1) = ISNULL(76, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(117, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            771,
            38,
            @DataInizio,
            1,
            16,
            76,
            117
        );
    END

    -- Record 19 - sorgente riga 20
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 818
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 16
          AND ISNULL(ID_Uff2, -1) = ISNULL(76, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(119, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            818,
            38,
            @DataInizio,
            1,
            16,
            76,
            119
        );
    END

    -- Record 20 - sorgente riga 21
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 495
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 16
          AND ISNULL(ID_Uff2, -1) = ISNULL(76, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(186, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            495,
            4,
            @DataInizio,
            1,
            16,
            76,
            186
        );
    END

    -- Record 21 - sorgente riga 22
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 256
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 16
          AND ISNULL(ID_Uff2, -1) = ISNULL(76, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(186, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            256,
            38,
            @DataInizio,
            1,
            16,
            76,
            186
        );
    END

    -- Record 22 - sorgente riga 23
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 819
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 16
          AND ISNULL(ID_Uff2, -1) = ISNULL(76, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(186, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            819,
            38,
            @DataInizio,
            1,
            16,
            76,
            186
        );
    END

    -- Record 23 - sorgente riga 24
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 581
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 12
          AND ISNULL(ID_Uff2, -1) = ISNULL(117, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            581,
            4,
            @DataInizio,
            1,
            12,
            117,
            NULL
        );
    END

    -- Record 24 - sorgente riga 25
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 788
          AND id_tipo_incarico = 3
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            788,
            3,
            @DataInizio,
            1,
            13,
            70,
            NULL
        );
    END

    -- Record 25 - sorgente riga 26
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 744
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(102, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            744,
            4,
            @DataInizio,
            1,
            13,
            70,
            102
        );
    END

    -- Record 26 - sorgente riga 27
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 779
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(102, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            779,
            38,
            @DataInizio,
            1,
            13,
            70,
            102
        );
    END

    -- Record 27 - sorgente riga 28
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 249
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(102, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            249,
            38,
            @DataInizio,
            1,
            13,
            70,
            102
        );
    END

    -- Record 28 - sorgente riga 29
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 764
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(103, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            764,
            4,
            @DataInizio,
            1,
            13,
            70,
            103
        );
    END

    -- Record 29 - sorgente riga 30
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 831
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(103, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            831,
            38,
            @DataInizio,
            1,
            13,
            70,
            103
        );
    END

    -- Record 30 - sorgente riga 31
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 822
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(103, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            822,
            38,
            @DataInizio,
            1,
            13,
            70,
            103
        );
    END

    -- Record 31 - sorgente riga 32
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 547
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(104, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            547,
            4,
            @DataInizio,
            1,
            13,
            70,
            104
        );
    END

    -- Record 32 - sorgente riga 33
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 588
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(104, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            588,
            38,
            @DataInizio,
            1,
            13,
            70,
            104
        );
    END

    -- Record 33 - sorgente riga 34
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 738
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(105, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            738,
            4,
            @DataInizio,
            1,
            13,
            70,
            105
        );
    END

    -- Record 34 - sorgente riga 35
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 582
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(105, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            582,
            38,
            @DataInizio,
            1,
            13,
            70,
            105
        );
    END

    -- Record 35 - sorgente riga 36
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 317
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(106, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            317,
            4,
            @DataInizio,
            1,
            13,
            70,
            106
        );
    END

    -- Record 36 - sorgente riga 37
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 759
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(106, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            759,
            38,
            @DataInizio,
            1,
            13,
            70,
            106
        );
    END

    -- Record 37 - sorgente riga 38
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 776
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(106, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            776,
            38,
            @DataInizio,
            1,
            13,
            70,
            106
        );
    END

    -- Record 38 - sorgente riga 39
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 657
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(107, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            657,
            4,
            @DataInizio,
            1,
            13,
            70,
            107
        );
    END

    -- Record 39 - sorgente riga 40
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 761
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(107, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            761,
            38,
            @DataInizio,
            1,
            13,
            70,
            107
        );
    END

    -- Record 40 - sorgente riga 41
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 608
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(70, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(107, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            608,
            38,
            @DataInizio,
            1,
            13,
            70,
            107
        );
    END

    -- Record 41 - sorgente riga 42
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 778
          AND id_tipo_incarico = 3
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(71, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            778,
            3,
            @DataInizio,
            1,
            13,
            71,
            NULL
        );
    END

    -- Record 42 - sorgente riga 43
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 7
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(71, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(109, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            7,
            4,
            @DataInizio,
            1,
            13,
            71,
            109
        );
    END

    -- Record 43 - sorgente riga 44
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 739
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(71, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(110, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            739,
            4,
            @DataInizio,
            1,
            13,
            71,
            110
        );
    END

    -- Record 44 - sorgente riga 45
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 763
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(71, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(111, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            763,
            4,
            @DataInizio,
            1,
            13,
            71,
            111
        );
    END

    -- Record 45 - sorgente riga 46
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 803
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(71, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(111, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            803,
            38,
            @DataInizio,
            1,
            13,
            71,
            111
        );
    END

    -- Record 46 - sorgente riga 47
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 812
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(71, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(111, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            812,
            38,
            @DataInizio,
            1,
            13,
            71,
            111
        );
    END

    -- Record 47 - sorgente riga 48
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 780
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(71, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(168, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            780,
            4,
            @DataInizio,
            1,
            13,
            71,
            168
        );
    END

    -- Record 48 - sorgente riga 49
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 817
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(71, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(168, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            817,
            38,
            @DataInizio,
            1,
            13,
            71,
            168
        );
    END

    -- Record 49 - sorgente riga 50
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 284
          AND id_tipo_incarico = 71
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(111, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            284,
            71,
            @DataInizio,
            1,
            13,
            111,
            NULL
        );
    END

    -- Record 50 - sorgente riga 51
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 775
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(111, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            775,
            38,
            @DataInizio,
            1,
            13,
            111,
            NULL
        );
    END

    -- Record 51 - sorgente riga 52
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 572
          AND id_tipo_incarico = 71
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(111, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            572,
            71,
            @DataInizio,
            1,
            13,
            111,
            NULL
        );
    END

    -- Record 52 - sorgente riga 53
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 1
          AND id_tipo_incarico = 71
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(113, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            1,
            71,
            @DataInizio,
            1,
            13,
            113,
            NULL
        );
    END

    -- Record 53 - sorgente riga 54
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 606
          AND id_tipo_incarico = 71
          AND principale = 1
          AND ID_Uff1 = 13
          AND ISNULL(ID_Uff2, -1) = ISNULL(73, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            606,
            71,
            @DataInizio,
            1,
            13,
            73,
            NULL
        );
    END

    -- Record 54 - sorgente riga 55
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 672
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 17
          AND ISNULL(ID_Uff2, -1) = ISNULL(85, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(133, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            672,
            38,
            @DataInizio,
            1,
            17,
            85,
            133
        );
    END

    -- Record 55 - sorgente riga 56
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 478
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 17
          AND ISNULL(ID_Uff2, -1) = ISNULL(86, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(135, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            478,
            4,
            @DataInizio,
            1,
            17,
            86,
            135
        );
    END

    -- Record 56 - sorgente riga 57
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 807
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 17
          AND ISNULL(ID_Uff2, -1) = ISNULL(86, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(136, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            807,
            38,
            @DataInizio,
            1,
            17,
            86,
            136
        );
    END

    -- Record 57 - sorgente riga 58
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 231
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 18
          AND ISNULL(ID_Uff2, -1) = ISNULL(NULL, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(NULL, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            231,
            38,
            @DataInizio,
            1,
            18,
            NULL,
            NULL
        );
    END

    -- Record 58 - sorgente riga 59
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 423
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 18
          AND ISNULL(ID_Uff2, -1) = ISNULL(89, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(146, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            423,
            38,
            @DataInizio,
            1,
            18,
            89,
            146
        );
    END

    -- Record 59 - sorgente riga 60
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 808
          AND id_tipo_incarico = 38
          AND principale = 1
          AND ID_Uff1 = 17
          AND ISNULL(ID_Uff2, -1) = ISNULL(85, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(133, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            808,
            38,
            @DataInizio,
            1,
            17,
            85,
            133
        );
    END

    -- Record 60 - sorgente riga 61
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 501
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 19
          AND ISNULL(ID_Uff2, -1) = ISNULL(92, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(154, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            501,
            4,
            @DataInizio,
            1,
            19,
            92,
            154
        );
    END

    -- Record 61 - sorgente riga 62
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 460
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 19
          AND ISNULL(ID_Uff2, -1) = ISNULL(92, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(188, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            460,
            4,
            @DataInizio,
            1,
            19,
            92,
            188
        );
    END

    -- Record 62 - sorgente riga 63
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Incarichi
        WHERE IDPersonale = 223
          AND id_tipo_incarico = 4
          AND principale = 1
          AND ID_Uff1 = 19
          AND ISNULL(ID_Uff2, -1) = ISNULL(94, -1)
          AND ISNULL(ID_Uff3, -1) = ISNULL(183, -1)
    )
    BEGIN
        INSERT INTO [dbo].[Incarichi]
        (
            [IDPersonale],
            [id_tipo_incarico],
            [Data_inizio],
            [principale],
            [ID_Uff1],
            [ID_Uff2],
            [ID_Uff3]
        )
        VALUES
        (
            223,
            4,
            @DataInizio,
            1,
            19,
            94,
            183
        );
    END

    PRINT CONCAT('Righe inserite nella transazione: ', @@ROWCOUNT);

    SELECT
        COUNT(*) AS RecordSorgente
    FROM (VALUES (1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1),(1)) AS V(x);

    IF @TestMode = 1
    BEGIN
        ROLLBACK TRAN;
        PRINT 'TEST MODE: modifiche annullate con ROLLBACK.';
    END
    ELSE
    BEGIN
        COMMIT TRAN;
        PRINT 'MODIFICA CONFERMATA con COMMIT.';
    END
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRAN;

    PRINT 'ERRORE: modifiche annullate.';
    THROW;
END CATCH;
GO

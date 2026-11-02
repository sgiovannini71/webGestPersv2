USE [DipendentiDB];
GO

DECLARE @DataInizio datetime = '2026-05-04';

BEGIN TRAN;

------------------------------------------------------------
-- 1. RIMOZIONE EVENTUALI INCARICHI FITTIZI
--    id_tipo_incarico = -1
------------------------------------------------------------

DELETE FROM dbo.Incarichi
WHERE id_tipo_incarico = -1
  AND IDPersonale IN (
      413,576,1,774,395,594,814,766,654,644,573,638,
      333,806,798,328,760,771,818,495,256,819,581,
      788,744,779,249,764,831,822,547,588,738,582,
      317,759,776,657,761,608,778,7,739,763,803,
      812,780,817,284,775,572,606,672,478,807,231,
      423,808,501,460,223
  );

------------------------------------------------------------
-- 2. INSERIMENTO NUOVI INCARICHI
------------------------------------------------------------

INSERT INTO dbo.Incarichi
(
    IDPersonale,
    id_tipo_incarico,
    Data_inizio,
    principale,
    ID_Uff1,
    ID_Uff2,
    ID_Uff3
)
VALUES
    (413, 71, @DataInizio, 1, 11, 77, NULL),
    (576, 38, @DataInizio, 1, 11, 77, NULL),
    (1,   71, @DataInizio, 0, 11, 108, NULL),
    (774, 38, @DataInizio, 1, 11, 107, 165),

    (395, 4,  @DataInizio, 1, 12, 52, NULL),
    (594, 38, @DataInizio, 1, 12, 52, NULL),
    (814, 38, @DataInizio, 1, 12, 52, NULL),
    (766, 4,  @DataInizio, 1, 12, 53, NULL),
    (654, 38, @DataInizio, 1, 12, 117, NULL),
    (644, 38, @DataInizio, 1, 12, 117, NULL),
    (573, 38, @DataInizio, 1, 12, 117, NULL),
    (638, 38, @DataInizio, 1, 12, 117, NULL),
    (333, 4,  @DataInizio, 1, 12, 109, NULL),
    (806, 38, @DataInizio, 1, 12, 109, NULL),
    (798, 38, @DataInizio, 1, 12, 109, NULL),

    (328, 4,  @DataInizio, 1, 16, 118, 193),
    (760, 38, @DataInizio, 1, NULL, NULL, NULL),
    (771, 38, @DataInizio, 1, 16, 76, 117),
    (818, 38, @DataInizio, 1, 16, 76, 119),
    (495, 4,  @DataInizio, 1, 16, 76, 186),
    (256, 38, @DataInizio, 1, 16, 76, 186),
    (819, 38, @DataInizio, 1, 16, 76, 186),
    (581, 4,  @DataInizio, 1, 12, 117, NULL),

    (788, 3,  @DataInizio, 1, 13, 70, NULL),
    (744, 4,  @DataInizio, 1, 13, 70, 102),
    (779, 38, @DataInizio, 1, 13, 70, 102),
    (249, 38, @DataInizio, 1, 13, 70, 102),
    (764, 4,  @DataInizio, 1, 13, 70, 103),
    (831, 38, @DataInizio, 1, 13, 70, 103),
    (822, 38, @DataInizio, 1, 13, 70, 103),
    (547, 4,  @DataInizio, 1, 13, 70, 104),
    (588, 38, @DataInizio, 1, 13, 70, 104),
    (738, 4,  @DataInizio, 1, 13, 70, 105),
    (582, 38, @DataInizio, 1, 13, 70, 105),
    (317, 4,  @DataInizio, 1, 13, 70, 106),
    (759, 38, @DataInizio, 1, 13, 70, 106),
    (776, 38, @DataInizio, 1, 13, 70, 106),
    (657, 4,  @DataInizio, 1, 13, 70, 107),
    (761, 38, @DataInizio, 1, 13, 70, 107),
    (608, 38, @DataInizio, 1, 13, 70, 107),

    (778, 3,  @DataInizio, 1, 13, 71, NULL),
    (7,   4,  @DataInizio, 1, 13, 71, 109),
    (739, 4,  @DataInizio, 1, 13, 71, 110),
    (763, 4,  @DataInizio, 1, 13, 71, 111),
    (803, 38, @DataInizio, 1, 13, 71, 111),
    (812, 38, @DataInizio, 1, 13, 71, 111),
    (780, 4,  @DataInizio, 1, 13, 71, 168),
    (817, 38, @DataInizio, 1, 13, 71, 168),

    (284, 71, @DataInizio, 1, 13, 111, NULL),
    (775, 38, @DataInizio, 1, 13, 111, NULL),
    (572, 71, @DataInizio, 1, 13, 111, NULL),
    (1,   71, @DataInizio, 1, 13, 113, NULL),
    (606, 71, @DataInizio, 1, 13, 73, NULL),

    (672, 38, @DataInizio, 1, 17, 85, 133),
    (478, 4,  @DataInizio, 1, 17, 86, 135),
    (807, 38, @DataInizio, 1, 17, 86, 136),
    (231, 38, @DataInizio, 1, 18, NULL, NULL),
    (423, 38, @DataInizio, 1, 18, 89, 146),
    (808, 38, @DataInizio, 1, 17, 85, 133),

    (501, 4,  @DataInizio, 1, 19, 92, 154),
    (460, 4,  @DataInizio, 1, 19, 92, 189),
    (223, 4,  @DataInizio, 1, 19, 94, 183);

------------------------------------------------------------
-- 3. VERIFICA
------------------------------------------------------------

SELECT *
FROM dbo.Incarichi
WHERE IDPersonale IN (
      413,576,1,774,395,594,814,766,654,644,573,638,
      333,806,798,328,760,771,818,495,256,819,581,
      788,744,779,249,764,831,822,547,588,738,582,
      317,759,776,657,761,608,778,7,739,763,803,
      812,780,817,284,775,572,606,672,478,807,231,
      423,808,501,460,223
)
ORDER BY IDPersonale, principale DESC, id_tipo_incarico;

COMMIT TRAN;

-- Per test:
-- ROLLBACK TRAN;
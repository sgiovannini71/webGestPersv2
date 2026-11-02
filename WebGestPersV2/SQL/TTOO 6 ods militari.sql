USE [DipendentiDB];
GO

DECLARE @DataInizio datetime = '2026-05-04';

BEGIN TRAN;

DELETE FROM dbo.Incarichi
WHERE id_tipo_incarico = -1
  AND IDPersonale IN (
        584,741,199,603,686,811,792,692,668,794,
        13,773,590,746,162,184,542,655,791,402,
        689,614,681,463,160,685,204,790,801,514,
        830,734,787,136,714,647,677,600,634,619,
        663,525,676,196,810,461,555,826
  );

/* ============================================================
   2. Inserimento incarichi
   (usare @DataInizio al posto di GETDATE())
   ============================================================ */
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
    (584, 71, @DataInizio, 1, 11, 107, NULL),
    (584,  4, @DataInizio, 0, 11, 107, 167),
    (741,  4, @DataInizio, 1, 11, 107, 166),
    (199, 38, @DataInizio, 1, 11, 107, 167),
    (603, 38, @DataInizio, 1, 11, 107, 167),
    (603, 16, @DataInizio, 1, 12, NULL, NULL),
    (686, 21, @DataInizio, 0, 16, 105, NULL),
    (686, 71, @DataInizio, 1, 12, NULL, NULL),
    (811,  4, @DataInizio, 1, 12,  51, NULL),
    (792, 38, @DataInizio, 1, 12,  51, NULL),
    (692, 70, @DataInizio, 1, 12,  51, 170),
    (668, 38, @DataInizio, 1, 12,  51, 170),
    (794, 38, @DataInizio, 1, 12,  51, 170),
    (794, 38, @DataInizio, 0, 12, 117, NULL),
    (13,  70, @DataInizio, 1, 12,  51, 169),
    (773, 38, @DataInizio, 1, 12,  51, 169),
    (590, 38, @DataInizio, 1, 12,  51, 169),
    (746, 38, @DataInizio, 1, 12,  53, NULL),
    (162, 38, @DataInizio, 1, 12,  53, NULL),
    (184, 38, @DataInizio, 1, 12,  53, NULL),
    (542,  4, @DataInizio, 1, 12,  54, NULL),
    (542,  4, @DataInizio, 0, 12, 116, NULL),
    (655, 38, @DataInizio, 1, 12,  54, NULL),
    (791, 38, @DataInizio, 1, 12,  54, NULL),
    (402, 38, @DataInizio, 1, 12, 116, NULL),
    (689, 38, @DataInizio, 1, 12, 116, NULL),
    (530, 38, @DataInizio, 1, 12, 116, NULL),
    (614, 38, @DataInizio, 1, 12, 116, NULL),
    (681, 38, @DataInizio, 1, 12, 116, NULL),
    (463, 38, @DataInizio, 1, 12, 109, NULL),
    (160, 38, @DataInizio, 1, 12, 109, NULL),
    (685, 44, @DataInizio, 1, 16, NULL, NULL),
    (685,  2, @DataInizio, 0, 17, NULL, NULL),
    (204, 22, @DataInizio, 1, 16,  74, NULL),
    (204, 38, @DataInizio, 1, 16,  74, NULL),
    (790, 38, @DataInizio, 1, 16,  74, NULL),
    (801, 38, @DataInizio, 1, 16,  74, NULL),
    (514, 71, @DataInizio, 1, 16, 118, NULL),
    (830,  4, @DataInizio, 1, 16, 118, 192),
    (734, 21, @DataInizio, 1, 16,  75, NULL),
    (787,  4, @DataInizio, 1, 16,  75, 112),
    (136, 38, @DataInizio, 1, 16,  75, 112),
    (714,  4, @DataInizio, 1, 16,  75, 113),
    (647, 38, @DataInizio, 1, 16,  75, 113),
    (677,  4, @DataInizio, 1, 16,  75, 114),
    (677, 38, @DataInizio, 1, 16,  75, 114),
    (600, 38, @DataInizio, 1, 16,  75, 114),
    (634,  4, @DataInizio, 1, 16,  75, 115),
    (619, 38, @DataInizio, 1, 16,  75, 115),
    (663, 21, @DataInizio, 1, 16,  76, NULL),
    (525, 41, @DataInizio, 1, 16,  76, NULL),
    (676,  4, @DataInizio, 1, 16,  76, 117),
    (196, 38, @DataInizio, 1, 16,  76, 117),
    (810,  4, @DataInizio, 1, 16,  76, 118),
    (461, 38, @DataInizio, 1, 16,  76, 118),
    (555,  4, @DataInizio, 1, 16,  76, 119),
    (826, 38, @DataInizio, 1, 16,  76, 120);

/* ============================================================
   3. Verifica
   ============================================================ */
SELECT *
FROM dbo.Incarichi
WHERE IDPersonale IN (
    584,741,199,603,686,811,792,692,668,794,
    13,773,590,746,162,184,542,655,791,402,
    689,614,681,463,160,685,204,790,801,514,
    830,734,787,136,714,647,677,600,634,619,
    663,525,676,196,810,461,555,826
)
ORDER BY IDPersonale, principale DESC, id_tipo_incarico;

/* ============================================================
   4. Conferma
   ============================================================ */
--COMMIT TRAN;
-- Per eseguire una prova senza salvare:
 ROLLBACK TRAN;
GO
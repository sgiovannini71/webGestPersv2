/*
===============================================================================
 Script.......: 10_Insert_TTOO_CategorieMilitari.sql
 Scopo........: Inserisce o riallinea le categorie U, SU e T.
===============================================================================
*/
USE [DipendentiDB];
GO
SET NOCOUNT ON;
GO

MERGE dbo.TTOO_CategorieMilitari AS T
USING
(
    VALUES
        ('U',  'Ufficiali',      1, 'S'),
        ('SU', 'Sottufficiali',  2, 'S'),
        ('T',  'Graduati',       3, 'S')
) AS S(Sigla, Descrizione, Ordine, Attivo)
ON T.Sigla = S.Sigla
WHEN MATCHED THEN
    UPDATE SET
        T.Descrizione = S.Descrizione,
        T.Ordine = S.Ordine,
        T.Attivo = S.Attivo
WHEN NOT MATCHED THEN
    INSERT (Sigla, Descrizione, Ordine, Attivo)
    VALUES (S.Sigla, S.Descrizione, S.Ordine, S.Attivo);
GO

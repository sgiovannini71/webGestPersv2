PACCHETTO MODULO TTOO
=====================

Database di destinazione
------------------------
DipendentiDB

Contenuto
---------
00_INSTALL_TTOO.sql
    Script master. Richiede la Modalità SQLCMD di SQL Server Management Studio.

00_INSTALL_TTOO_CONSOLIDATO.sql
    Versione unica, eseguibile direttamente senza Modalità SQLCMD.

01_Create_TTOO_CategorieMilitari.sql
    Ricrea la tabella delle categorie militari.

02_Create_TTOO_Posizioni.sql
    Crea la tabella principale delle posizioni previste.

03_Create_TTOO_Posizioni_FFAA.sql
    Crea la relazione molti-a-molti tra posizioni e Forze Armate.

10_Insert_TTOO_CategorieMilitari.sql
    Inserisce o riallinea U, SU e T.

20_Load_TTOO_Posizioni.sql
    Carica tutte le posizioni previste incluse nel prospetto TTOO di riferimento.
    Per sostituire dati già presenti impostare:
        DECLARE @SostituisciTutto bit = 1;

30_Verifica_TTOO.sql
    Controllo diagnostico completo, di sola lettura.

Prerequisiti
------------
Devono esistere e contenere i valori richiamati:
- dbo.Liv1Uff
- dbo.Liv2Uff
- dbo.Liv3Uff
- dbo.Tipo_incarichi
- dbo.Titoli
- dbo.FFAA

Ordine consigliato
------------------
1. Eseguire 00_INSTALL_TTOO_CONSOLIDATO.sql
2. Eseguire 30_Verifica_TTOO.sql
3. Verificare che l'esito finale sia CONTROLLO SUPERATO

Avvertenza
----------
Gli script di installazione eliminano e ricreano le tabelle TTOO.
Eseguire prima un backup del database o delle tabelle interessate.

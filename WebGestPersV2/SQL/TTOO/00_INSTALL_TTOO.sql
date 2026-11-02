/*
===============================================================================
 Script.......: 00_INSTALL_TTOO.sql
 Uso..........: Eseguire da SSMS con Modalità SQLCMD attiva.
 Attenzione...: Ricrea le tabelle TTOO e ricarica i dati.
===============================================================================
*/
:r .\01_Create_TTOO_CategorieMilitari.sql
:r .\02_Create_TTOO_Posizioni.sql
:r .\03_Create_TTOO_Posizioni_FFAA.sql
:r .\10_Insert_TTOO_CategorieMilitari.sql
:r .\20_Load_TTOO_Posizioni.sql

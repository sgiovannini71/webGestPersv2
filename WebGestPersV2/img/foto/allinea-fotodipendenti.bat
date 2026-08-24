@echo off
setlocal EnableExtensions

REM ============================================================
REM CONFIGURAZIONE
REM ============================================================

set "SOURCE=G:\shared\fotodipendenti"
set "DEST='C:\Program Files (x86)\nginx-1.23.3\html\img'"

set "LOG=%SOURCE%\copia_fotodipendenti.log"

REM ============================================================
REM INIZIO LOG
REM Il file viene sovrascritto ad ogni esecuzione
REM ============================================================

(
    echo ============================================================
    echo COPIA FOTO DIPENDENTI
    echo Data/Ora: %date% %time%
    echo ============================================================
    echo.
    echo Origine:
    echo %SOURCE%
    echo.
    echo Destinazione:
    echo %DEST%
    echo.
) > "%LOG%"

REM ============================================================
REM VERIFICA CARTELLA ORIGINE
REM ============================================================

if not exist "%SOURCE%\" (
    echo ERRORE: la cartella origine non esiste. >> "%LOG%"
    echo ERRORE: la cartella origine non esiste.
    exit /b 1
)

REM ============================================================
REM COPIA FILE JPG
REM
REM /E       copia anche eventuali sottocartelle
REM /R:2     massimo 2 tentativi in caso di errore
REM /W:2     attesa di 2 secondi tra i tentativi
REM /NP      non mostra la percentuale di avanzamento
REM /NFL     non elenca i singoli file
REM /NDL     non elenca le directory
REM
REM NON viene usato /XO:
REM i file vengono quindi aggiornati/sovrascritti
REM anche se quello della sorgente non è più recente.
REM ============================================================

echo ------------------------------------------------------------ >> "%LOG%"
echo INIZIO COPIA >> "%LOG%"
echo ------------------------------------------------------------ >> "%LOG%"

robocopy "%SOURCE%" "%DEST%" "*.jpg" /E /R:2 /W:2 /NP /NFL /NDL >> "%LOG%" 2>&1

set "RC=%ERRORLEVEL%"

REM ============================================================
REM VALUTAZIONE ESITO
REM Robocopy considera 0-7 come esiti non critici.
REM 8 o superiore = errore.
REM ============================================================

echo. >> "%LOG%"
echo Codice restituito da Robocopy: %RC% >> "%LOG%"

if %RC% LEQ 7 (
    echo ESITO: COPIA COMPLETATA CORRETTAMENTE >> "%LOG%"
    echo.
    echo ==========================================
    echo COPIA COMPLETATA CORRETTAMENTE
    echo Codice Robocopy: %RC%
    echo ==========================================
    echo.
    exit /b 0
) else (
    echo ESITO: ERRORE DURANTE LA COPIA >> "%LOG%"
    echo.
    echo ==========================================
    echo ERRORE DURANTE LA COPIA
    echo Codice Robocopy: %RC%
    echo Consultare il file di log:
    echo %LOG%
    echo ==========================================
    echo.
    exit /b 1
)
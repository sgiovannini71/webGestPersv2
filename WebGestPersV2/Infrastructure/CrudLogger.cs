using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using WebGestPersV2.Configuration;

namespace WebGestPersV2.Infrastructure
{
    public static class CrudLogger
    {
        private static readonly object Sync = new object();

        public static string UtenteCorrente
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context == null || context.User == null || context.User.Identity == null || string.IsNullOrWhiteSpace(context.User.Identity.Name)) return "utente-non-disponibile";
                return context.User.Identity.Name;
            }
        }

        public static void Info(string operazione, string entita, string dettaglio) { Scrivi("INFO", operazione, entita, dettaglio, null); }
        public static void Errore(string operazione, string entita, string dettaglio, Exception errore) { Scrivi("ERROR", operazione, entita, dettaglio, errore); }

        private static void Scrivi(string livello, string operazione, string entita, string dettaglio, Exception errore)
        {
            try
            {
                var riga = new StringBuilder();
                riga.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append(" [").Append(Thread.CurrentThread.ManagedThreadId).Append("] ")
                    .Append(livello).Append(" ErrorLog [utente=").Append(Pulisci(UtenteCorrente)).Append("] [operazione=").Append(Pulisci(operazione))
                    .Append("] [entita=").Append(Pulisci(entita)).Append("] - ").Append(Pulisci(dettaglio));
                if (errore != null) riga.AppendLine().Append(errore);
                riga.AppendLine();
                lock (Sync)
                {
                    try { Accoda(AppConfig.LogFilePath, riga.ToString()); }
                    catch
                    {
                        string baseApplicazione = HttpRuntime.AppDomainAppPath ?? AppDomain.CurrentDomain.BaseDirectory;
                        Accoda(Path.Combine(baseApplicazione, "App_Data", "Logs"), riga.ToString());
                    }
                }
            }
            catch { /* Il logging non deve interrompere l'operazione applicativa. */ }
        }

        private static void Accoda(string cartella, string contenuto)
        {
            Directory.CreateDirectory(cartella);
            File.AppendAllText(Path.Combine(cartella, DateTime.Now.ToString("yyyy-MM-dd") + ".log"), contenuto, new UTF8Encoding(false));
        }

        private static string Pulisci(string valore) { return (valore ?? string.Empty).Replace("\r", " ").Replace("\n", " "); }
    }
}

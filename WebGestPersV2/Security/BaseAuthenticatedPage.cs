using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using WebGestPersV2.Data;

namespace WebGestPersV2.Security
{
    public abstract class BaseAuthenticatedPage : Page
    {
        protected virtual int[] LivelliConsentiti { get { return new int[0]; } }

        protected UtenteAutorizzato UtenteCorrente
        {
            get { return Context.Items["UtenteAutorizzato"] as UtenteAutorizzato; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Context.User == null || Context.User.Identity == null || !Context.User.Identity.IsAuthenticated)
            {
                NegaAccesso();
                return;
            }

            UtenteAutorizzato utente;
            try { utente = CaricaUtente(Context.User.Identity.Name); }
            catch (SqlException) { NegaAccesso(); return; }
            catch (InvalidOperationException) { NegaAccesso(); return; }

            if (utente == null || (LivelliConsentiti.Length > 0 && !LivelliConsentiti.Contains(utente.Livello)))
            {
                NegaAccesso();
                return;
            }

            Context.Items["UtenteAutorizzato"] = utente;
            Session["UserId"] = utente.Id;
            Session["Username"] = utente.Username;
            Session["Livello"] = utente.Livello;
            Session["WindowsUser"] = Context.User.Identity.Name;
        }

        private static UtenteAutorizzato CaricaUtente(string identitaWindows)
        {
            if (string.IsNullOrWhiteSpace(identitaWindows)) return null;
            string username = identitaWindows;
            int separatore = identitaWindows.IndexOf('\\');
            if (separatore >= 0 && separatore < identitaWindows.Length - 1)
                username = identitaWindows.Substring(separatore + 1);

            const string sql = @"SELECT TOP (1) id, username, livello
                                 FROM dbo.Autorizzazioni
                                 WHERE username = @UsernameCompleto OR username = @Username
                                 ORDER BY CASE WHEN username = @UsernameCompleto THEN 0 ELSE 1 END, id";
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@UsernameCompleto", SqlDbType.NVarChar, 50).Value = identitaWindows;
                command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read()) return null;
                    return new UtenteAutorizzato {
                        Id = reader.GetInt32(0), Username = reader.GetString(1), Livello = reader.GetInt32(2)
                    };
                }
            }
        }

        private void NegaAccesso()
        {
            Context.Items["UtenteAutorizzato"] = null;
            Response.Redirect("~/Account/AccessoNegato.aspx", true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebGestPersV2.Data;
using WebGestPersV2.Models;

namespace WebGestPersV2.PersonaleEsterno
{
    public partial class Modifica : Security.BaseAuthenticatedPage
    {
        protected override int[] LivelliConsentiti { get { return new[] { 150, 160, 170, 200 }; } }

        private int Id { get { int x; return int.TryParse(Request.QueryString["id"], out x) ? x : 0; } }
        private int IdCasellaInModifica { get { return ViewState["IdCasella"] == null ? 0 : (int)ViewState["IdCasella"]; } set { ViewState["IdCasella"] = value; } }
        private string EmailVerificata { get { return Convert.ToString(ViewState["EmailVerificata"]); } set { ViewState["EmailVerificata"] = value; } }
        private PersonaleEsternoRepository Repository { get { return new PersonaleEsternoRepository(); } }
        private PersonaleEsternoCaselleRepository MailboxRepository { get { return new PersonaleEsternoCaselleRepository(); } }

        protected void Page_Load(object s, EventArgs e)
        {
            if (Id <= 0) { Response.Redirect("Elenco.aspx", true); return; }
            if (!IsPostBack) Carica();
        }

        private void Carica()
        {
            var p = Repository.Trova(Id);
            if (p == null) { Response.Redirect("Elenco.aspx", true); return; }
            Heading.Text = Server.HtmlEncode(p.Cognome + " " + p.Nome);
            Nuovo.Bind(Company, Repository.Ditte(), "-- seleziona --");
            Nuovo.Bind(Office1, Repository.Uffici1(), "-- ufficio predefinito --");
            Seleziona(Company, p.IdDitta);
            Seleziona(Office1, p.IdUfficio1);
            CaricaUffici2(p.IdUfficio2);
            CaricaUffici3(p.IdUfficio3);
            LastName.Text = p.Cognome;
            FirstName.Text = p.Nome;
            TaxCode.Text = p.CodiceFiscale;
            Phone.Text = p.Telefono;
            Room.Text = p.Stanza;
            Active.Checked = p.Attivo;
            DeactivateButton.Visible = p.Attivo;
            CaricaStorico();
            CaricaCaselle();
        }

        protected void Office1_Changed(object s, EventArgs e) { CaricaUffici2(null); Nuovo.Bind(Office3, new List<LookupItem>(), "-- ufficio predefinito --"); }
        protected void Office2_Changed(object s, EventArgs e) { CaricaUffici3(null); }
        private void CaricaUffici2(int? selezione) { int id; Nuovo.Bind(Office2, int.TryParse(Office1.SelectedValue, out id) ? Repository.Uffici2(id) : new List<LookupItem>(), "-- ufficio predefinito --"); Seleziona(Office2, selezione); }
        private void CaricaUffici3(int? selezione) { int id; Nuovo.Bind(Office3, int.TryParse(Office2.SelectedValue, out id) ? Repository.Uffici3(id) : new List<LookupItem>(), "-- ufficio predefinito --"); Seleziona(Office3, selezione); }

        protected void SaveButton_Click(object s, EventArgs e)
        {
            Page.Validate("external"); if (!Page.IsValid) return;
            try { Repository.Aggiorna(Leggi(), Context.User.Identity.Name); Response.Redirect("Modifica.aspx?id=" + Id, false); Context.ApplicationInstance.CompleteRequest(); }
            catch (Exception ex) { Message.Text = Server.HtmlEncode(ex.Message); Message.Visible = true; }
        }

        protected void DeactivateButton_Click(object s, EventArgs e)
        {
            try { Repository.Disattiva(Id, Context.User.Identity.Name); Response.Redirect("Elenco.aspx", false); Context.ApplicationInstance.CompleteRequest(); }
            catch (Exception ex) { Message.Text = Server.HtmlEncode(ex.Message); Message.Visible = true; }
        }

        private PersonaleEsternoRequest Leggi()
        {
            int d, u1, u2, u3;
            return new PersonaleEsternoRequest
            {
                Id = Id,
                Cognome = LastName.Text.Trim(),
                Nome = FirstName.Text.Trim(),
                CodiceFiscale = TaxCode.Text.Trim().ToUpperInvariant(),
                Telefono = Phone.Text.Trim(),
                Stanza = Room.Text.Trim(),
                IdDitta = int.TryParse(Company.SelectedValue, out d) ? d : 0,
                IdUfficio1 = int.TryParse(Office1.SelectedValue, out u1) ? (int?)u1 : null,
                IdUfficio2 = int.TryParse(Office2.SelectedValue, out u2) ? (int?)u2 : null,
                IdUfficio3 = int.TryParse(Office3.SelectedValue, out u3) ? (int?)u3 : null,
                Attivo = Active.Checked
            };
        }

        private void CaricaStorico()
        {
            const string sql = "SELECT Campo_Variato AS Campo,Data_Modifica AS Data,Utente_Modificatore AS Utente,Valore_Vecchio AS Prima,Valore_Nuovo AS Dopo FROM dbo.PE_StoricoModifiche WHERE ID_Pe=@Id ORDER BY Data_Modifica DESC,ID_Storico DESC";
            var table = new DataTable();
            using (var c = new SqlConnection(Db.ConnectionString)) using (var q = new SqlCommand(sql, c))
            { q.Parameters.Add("@Id", SqlDbType.Int).Value = Id; using (var a = new SqlDataAdapter(q)) a.Fill(table); }
            HistoryGrid.DataSource = table;
            HistoryGrid.DataBind();
        }

        protected void MailboxesGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id) || id <= 0) return;
            try
            {
                if (e.CommandName == "ModificaCasella")
                {
                    CasellaPostaleItem x = MailboxRepository.Trova(id, Id);
                    if (x == null) throw new InvalidOperationException("Casella postale non trovata.");
                    IdCasellaInModifica = id;
                    MailboxEmail.Text = x.Denominazione;
                    SelezionaMailboxTipo(x.Tipo);
                    MailboxFormTitle.Text = "Modifica casella postale";
                    MailboxSaveButton.Text = "Salva modifica";
                    MailboxCancelButton.Visible = true;
                    MailboxVerifyButton.Visible = false;
                    MailboxDetectedType.Visible = false;
                    MailboxMessage.Visible = false;
                    NascondiMailboxVerifica();
                }
                else if (e.CommandName == "EliminaCasella")
                {
                    MailboxRepository.Elimina(id, Id, Context.User.Identity.Name);
                    AzzeraMailboxForm();
                    CaricaCaselle();
                    CaricaStorico();
                }
            }
            catch (Exception ex) { MostraMailboxErrore(ex.Message); }
        }

        protected void MailboxSaveButton_Click(object sender, EventArgs e)
        {
            Page.Validate("mailbox"); if (!Page.IsValid) return;
            try
            {
                if (IdCasellaInModifica > 0) MailboxRepository.Aggiorna(IdCasellaInModifica, Id, MailboxEmail.Text, MailboxType.SelectedValue, Context.User.Identity.Name);
                else MailboxRepository.Inserisci(Id, MailboxEmail.Text, MailboxType.SelectedValue, Context.User.Identity.Name, false);
                AzzeraMailboxForm();
                CaricaCaselle();
                CaricaStorico();
            }
            catch (Exception ex) { MostraMailboxErrore(ex.Message); }
        }

        protected void MailboxVerifyButton_Click(object sender, EventArgs e)
        {
            Page.Validate("mailbox"); if (!Page.IsValid) return;
            try
            {
                if (MailboxRepository.DeterminaTipoCasella(MailboxEmail.Text, MailboxType.SelectedValue) == "condivisa")
                {
                    NascondiMailboxVerifica();
                    MailboxAvailability.Text = "La casella condivisa può essere associata a più persone. Non è necessaria la verifica di disponibilità.";
                    MailboxAvailability.Visible = true;
                    MailboxMessage.Visible = false;
                    return;
                }
                var assegnazioni = MailboxRepository.CercaAssegnazioni(MailboxEmail.Text, Id);
                EmailVerificata = MailboxEmail.Text.Trim().ToLowerInvariant();
                MailboxAssignedPeopleGrid.DataSource = assegnazioni;
                MailboxAssignedPeopleGrid.DataBind();
                MailboxAssignmentAlert.Visible = assegnazioni.Count > 0;
                MailboxAvailability.Visible = assegnazioni.Count == 0;
                MailboxAvailability.Text = assegnazioni.Count == 0 ? "La casella non risulta assegnata ad altre persone esterne." : "";
                MailboxMessage.Visible = false;
                if (assegnazioni.Count > 0) ClientScript.RegisterStartupScript(GetType(), "mailboxAssigned", "alert('La casella è già assegnata. Verificare i nominativi mostrati nella pagina.');", true);
            }
            catch (Exception ex) { NascondiMailboxVerifica(); MostraMailboxErrore(ex.Message); }
        }

        protected void MailboxEmail_Changed(object sender, EventArgs e)
        {
            NascondiMailboxVerifica();
            if (string.IsNullOrWhiteSpace(MailboxEmail.Text)) { MailboxDetectedType.Visible = false; return; }
            try
            {
                string tipo = MailboxRepository.DeterminaTipoCasella(MailboxEmail.Text, MailboxType.SelectedValue);
                SelezionaMailboxTipo(tipo);
                MailboxDetectedType.Text = "Tipo applicato all'indirizzo: " + Server.HtmlEncode(tipo) + ".";
                MailboxDetectedType.Visible = true;
                MailboxMessage.Visible = false;
            }
            catch (Exception ex) { MailboxDetectedType.Visible = false; MostraMailboxErrore(ex.Message); }
        }

        protected void MailboxTransferButton_Click(object sender, EventArgs e)
        {
            Page.Validate("mailbox"); if (!Page.IsValid) return;
            try
            {
                if (!string.Equals(EmailVerificata, MailboxEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("L'indirizzo è cambiato dopo la verifica. Verificare nuovamente la disponibilità.");
                MailboxRepository.Inserisci(Id, MailboxEmail.Text, MailboxType.SelectedValue, Context.User.Identity.Name, true);
                AzzeraMailboxForm();
                CaricaCaselle();
                CaricaStorico();
            }
            catch (Exception ex) { MostraMailboxErrore(ex.Message); }
        }

        protected void MailboxCancelButton_Click(object sender, EventArgs e) { AzzeraMailboxForm(); }

        private void CaricaCaselle()
        {
            MailboxesGrid.DataSource = MailboxRepository.Elenca(Id);
            MailboxesGrid.DataBind();
            MailboxMessage.Visible = false;
        }

        private void AzzeraMailboxForm()
        {
            IdCasellaInModifica = 0;
            MailboxEmail.Text = "";
            MailboxType.SelectedValue = "personale";
            MailboxFormTitle.Text = "Aggiungi casella postale";
            MailboxSaveButton.Text = "Aggiungi";
            MailboxCancelButton.Visible = false;
            MailboxVerifyButton.Visible = true;
            MailboxDetectedType.Visible = false;
            NascondiMailboxVerifica();
            MailboxMessage.Visible = false;
        }

        private void NascondiMailboxVerifica()
        {
            EmailVerificata = "";
            MailboxAssignmentAlert.Visible = false;
            MailboxAvailability.Visible = false;
            MailboxAssignedPeopleGrid.DataSource = null;
            MailboxAssignedPeopleGrid.DataBind();
        }

        private void SelezionaMailboxTipo(string tipo)
        {
            if (!string.IsNullOrEmpty(tipo) && MailboxType.Items.FindByValue(tipo) != null) MailboxType.SelectedValue = tipo;
        }

        private void MostraMailboxErrore(string x)
        {
            MailboxMessage.Text = Server.HtmlEncode(x);
            MailboxMessage.Visible = true;
        }

        private static void Seleziona(System.Web.UI.WebControls.DropDownList c, int? id) { if (id.HasValue && c.Items.FindByValue(id.Value.ToString()) != null) c.SelectedValue = id.Value.ToString(); }
    }
}
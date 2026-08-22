using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using WebGestPersV2.Data;
using WebGestPersV2.Models;

namespace WebGestPersV2.Personale
{
    public partial class Incarichi : Security.BaseAuthenticatedPage
    {
        protected override int[] LivelliConsentiti { get { return new[] { 150, 160, 170, 200 }; } }
        private int Id { get { int id; return int.TryParse(Request.QueryString["id"], out id) ? id : 0; } }
        private int? IdIncarico { get { int id; return int.TryParse(Request.QueryString["incarico"], out id) && id > 0 ? (int?)id : null; } }
        private int? IdIncaricoDaChiudere { get { int id; return int.TryParse(Request.QueryString["chiudi"], out id) && id > 0 ? (int?)id : null; } }
        private bool FormRichiesto { get { return Request.QueryString["incarico"] != null; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Id <= 0) { Response.Redirect("Elenco.aspx", true); return; }
            BackLink.NavigateUrl = "Dettaglio.aspx?id=" + Id;
            CancelLink.NavigateUrl = "Incarichi.aspx?id=" + Id;
            CloseCancelLink.NavigateUrl = "Incarichi.aspx?id=" + Id;
            NewLink.NavigateUrl = "Incarichi.aspx?id=" + Id + "&incarico=nuovo";
            if (!IsPostBack) Carica();
        }

        private void Carica()
        {
            PersonaDettaglio persona = new PersonaleRepository().TrovaAttivo(Id);
            if (persona == null) throw new InvalidOperationException("Persona attiva non trovata.");
            Verifica(persona.Militare);
            FullName.Text = Server.HtmlEncode(persona.Cognome + " " + persona.Nome);
            BindLista();
            HistoryGrid.DataSource = new PersonaleRepository().CercaStoricoIncarichi(Id); HistoryGrid.DataBind();
            if (Request.QueryString["salvato"] == "1") { Message.Text = "Incarico salvato correttamente."; Message.CssClass = "message success"; Message.Visible = true; }
            EditPanel.Visible = FormRichiesto;
            ClosePanel.Visible = IdIncaricoDaChiudere.HasValue;
            if (IdIncaricoDaChiudere.HasValue)
            {
                IncaricoListaItem daChiudere = new IncarichiWriteRepository().Trova(Id, IdIncaricoDaChiudere.Value);
                if (daChiudere == null || daChiudere.Predefinito) throw new InvalidOperationException("Incarico chiudibile non trovato.");
                EndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
            if (!FormRichiesto) return;

            var repository = new IncarichiWriteRepository();
            Bind(AssignmentType, repository.TipiIncarico(), "-- seleziona --");
            Bind(Office1, repository.UfficiPrimoLivello(), "-- nessuno --");
            FormTitle.Text = IdIncarico.HasValue ? "Modifica incarico" : "Nuovo incarico";
            if (!IdIncarico.HasValue) { StartDate.Text = DateTime.Today.ToString("yyyy-MM-dd"); Bind(Office2, new List<LookupItem>(), "-- nessuno --"); Bind(Office3, new List<LookupItem>(), "-- nessuno --"); return; }
            IncaricoListaItem incarico = repository.Trova(Id, IdIncarico.Value);
            if (incarico == null) throw new InvalidOperationException("Incarico non trovato.");
            Seleziona(AssignmentType, incarico.IdTipoIncarico); Seleziona(Office1, incarico.IdUfficio1);
            CaricaSecondoLivello(repository, incarico.IdUfficio2); CaricaTerzoLivello(repository, incarico.IdUfficio3);
            StartDate.Text = incarico.DataInizio.HasValue ? incarico.DataInizio.Value.ToString("yyyy-MM-dd") : "";
            IsPrimary.Checked = incarico.Principale;
        }

        protected void Office1_Changed(object sender, EventArgs e) { var r = new IncarichiWriteRepository(); CaricaSecondoLivello(r, null); Bind(Office3, new List<LookupItem>(), "-- nessuno --"); }
        protected void Office2_Changed(object sender, EventArgs e) { CaricaTerzoLivello(new IncarichiWriteRepository(), null); }
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Page.Validate("assignment"); if (!Page.IsValid) return;
            try
            {
                PersonaDettaglio persona = new PersonaleRepository().TrovaAttivo(Id);
                if (persona == null) throw new InvalidOperationException("Persona attiva non trovata.");
                Verifica(persona.Militare);
                int tipo; if (!int.TryParse(AssignmentType.SelectedValue, out tipo)) throw new InvalidOperationException("Selezionare il tipo di incarico.");
                var dati = new ModificaIncaricoRequest { IdPersonale = Id, IdIncarico = IdIncarico, IdTipoIncarico = tipo, DataInizio = ParseData(StartDate.Text), Principale = IsPrimary.Checked, IdUfficio1 = Selezionato(Office1), IdUfficio2 = Selezionato(Office2), IdUfficio3 = Selezionato(Office3) };
                new IncarichiWriteRepository().Salva(dati, Context.User.Identity.Name);
                Response.Redirect("Incarichi.aspx?id=" + Id + "&salvato=1", false); Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex) { Message.Text = Server.HtmlEncode(ex.Message); Message.CssClass = "message error"; Message.Visible = true; }
        }

        protected void CloseButton_Click(object sender, EventArgs e)
        {
            Page.Validate("closeAssignment"); if (!Page.IsValid) return;
            try
            {
                if (!IdIncaricoDaChiudere.HasValue) throw new InvalidOperationException("Incarico da chiudere non valido.");
                PersonaDettaglio persona = new PersonaleRepository().TrovaAttivo(Id);
                if (persona == null) throw new InvalidOperationException("Persona attiva non trovata.");
                Verifica(persona.Militare);
                DateTime? dataFine = ParseData(EndDate.Text, "La data di fine incarico non è valida.");
                if (!dataFine.HasValue) throw new InvalidOperationException("Indicare la data di fine incarico.");
                new IncarichiWriteRepository().Chiudi(Id, IdIncaricoDaChiudere.Value, dataFine.Value, Context.User.Identity.Name);
                Response.Redirect("Incarichi.aspx?id=" + Id + "&salvato=1", false); Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex) { Message.Text = Server.HtmlEncode(ex.Message); Message.CssClass = "message error"; Message.Visible = true; }
        }

        protected string UrlModifica(object idIncarico) { return "Incarichi.aspx?id=" + Id + "&incarico=" + Convert.ToString(idIncarico); }
        protected string UrlChiusura(object idIncarico) { return "Incarichi.aspx?id=" + Id + "&chiudi=" + Convert.ToString(idIncarico); }
        private void BindLista() { AssignmentsGrid.DataSource = new PersonaleRepository().CercaIncarichi(Id); AssignmentsGrid.DataBind(); }
        private void CaricaSecondoLivello(IncarichiWriteRepository r, int? selezione) { int id; Bind(Office2, int.TryParse(Office1.SelectedValue, out id) ? r.UfficiSecondoLivello(id) : new List<LookupItem>(), "-- nessuno --"); Seleziona(Office2, selezione); }
        private void CaricaTerzoLivello(IncarichiWriteRepository r, int? selezione) { int id1, id2; Bind(Office3, int.TryParse(Office1.SelectedValue, out id1) && int.TryParse(Office2.SelectedValue, out id2) ? r.UfficiTerzoLivello(id1, id2) : new List<LookupItem>(), "-- nessuno --"); Seleziona(Office3, selezione); }
        private void Verifica(bool militare) { int livello = UtenteCorrente.Livello; if ((militare && livello != 150 && livello != 170 && livello != 200) || (!militare && livello != 160 && livello != 170 && livello != 200)) Response.Redirect("~/Account/AccessoNegato.aspx", true); }
        private static DateTime? ParseData(string testo, string messaggioErrore = "La data di inizio non è valida.") { if (string.IsNullOrWhiteSpace(testo)) return null; DateTime data; if (DateTime.TryParseExact(testo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data)) return data; throw new InvalidOperationException(messaggioErrore); }
        private static int? Selezionato(DropDownList c) { int id; return int.TryParse(c.SelectedValue, out id) ? (int?)id : null; }
        private static void Bind(DropDownList c, IList<LookupItem> valori, string vuoto) { c.Items.Clear(); c.Items.Add(new ListItem(vuoto, "")); foreach (LookupItem x in valori) c.Items.Add(new ListItem(x.Text, x.Value)); }
        private static void Seleziona(DropDownList c, int? id) { if (id.HasValue && c.Items.FindByValue(id.Value.ToString()) != null) c.SelectedValue = id.Value.ToString(); }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using WebGestPersV2.Configuration;
using WebGestPersV2.Data;
using WebGestPersV2.Models;

namespace WebGestPersV2.Personale
{
    public partial class Modifica : Security.BaseAuthenticatedPage
    {
        protected override int[] LivelliConsentiti { get { return new[] { 150, 160, 170, 200 }; } }

        private int Id
        {
            get { int id; return int.TryParse(Request.QueryString["id"], out id) ? id : 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Id <= 0) { Response.Redirect("Elenco.aspx", true); return; }
            AssignmentsLink.NavigateUrl = "Incarichi.aspx?id=" + Id;
            TopBackLink.NavigateUrl = "Dettaglio.aspx?id=" + Id;
            if (!IsPostBack) { CancelLink.NavigateUrl = "Dettaglio.aspx?id=" + Id; Carica(); }
        }

        private void Carica()
        {
            PersonaDettaglio persona = new PersonaleRepository().Trova(Id);
            if (persona == null) throw new InvalidOperationException("Persona non trovata.");
            Verifica(persona.Militare);

            var repository = new PersonaleWriteRepository();
            Bind(EducationTitle, repository.TitoliStudio(), "-- nessuno --");
            Bind(TimeBand, repository.FasceOrarie(), "-- nessuna --");
            Bind(ServiceStatus, repository.StatiServizio(), "-- seleziona --");
            Seleziona(EducationTitle, persona.IdTitoloStudio);
            Seleziona(TimeBand, persona.IdFasciaOraria);
            SelezionaTesto(ServiceStatus, persona.StatoServizio, "attivo");
            LastName.Text = persona.Cognome;
            FirstName.Text = persona.Nome;
            TaxCode.Text = persona.CodiceFiscale;
            SourceOrganization.Text = persona.EnteProvenienza;
            OfficePhone.Text = persona.TelefonoUfficio;
            Room.Text = persona.NumeroStanza;
            AssignmentDate.Text = FormattaData(persona.DataAssegnazione);
            ExitDate.Text = FormattaData(persona.DataUscita);
            AssignmentsClosingDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            bool attivo = string.Equals(persona.StatoServizio, "attivo", StringComparison.OrdinalIgnoreCase);
            InactivePhotoPanel.Visible = !attivo;
            EditingPersonName.Text = !attivo ? Server.HtmlEncode(persona.Cognome + " " + persona.Nome) : string.Empty;
            if (!attivo) InactiveProfilePhoto.ImageUrl = ResolveUrl(PercorsoFoto(persona));
            ReactivationMessage.Visible = !attivo;
            AssignmentsLink.Visible = attivo;
            CancelLink.NavigateUrl = attivo ? "Dettaglio.aspx?id=" + Id : "NonAttivi.aspx";
            TopBackLink.NavigateUrl = attivo ? "Dettaglio.aspx?id=" + Id : "NonAttivi.aspx";
            TopBackLink.Text = attivo ? "← Torna al dettaglio" : "← Torna al personale non attivo";

            MilitaryPanel.Visible = persona.Militare;
            if (persona.Militare) CaricaProfiloMilitare(repository, persona);
            CivilianPanel.Visible = !persona.Militare;
            if (!persona.Militare) CaricaProfiloCivile(repository, persona);
        }

        private void CaricaProfiloMilitare(PersonaleWriteRepository repository, PersonaDettaglio persona)
        {
            Bind(MilitaryPosition, repository.PosizioniMilitari(), "-- seleziona --");
            Seleziona(MilitaryPosition, persona.IdPosizioneMilitare);
            Bind(ArmedForce, repository.ForzeArmate(), "-- seleziona --");
            Seleziona(ArmedForce, persona.IdForzaArmata);
            string categorico = (persona.Categorico ?? "").Trim();
            if (Category.Items.FindByValue(categorico) != null)
                Category.SelectedValue = categorico;
            CaricaInquadramentoMilitare(repository, persona.IdGrado, persona.IdCategoriaMilitare,
                persona.IdRuoloMilitare, persona.IdSpecialitaMilitare);
            RegistrationNumber.Text = persona.Matricola;
            DecreeNumber.Text = persona.NumeroDecreto;
            TrainingSchool.Text = persona.ScuolaFormazione;
            MilitaryLegalDate.Text = FormattaData(persona.DataGiuridicaMilitare);
            MilitaryEconomicDate.Text = FormattaData(persona.DataEconomicaMilitare);
            EnlistmentDate.Text = FormattaData(persona.DataArruolamento);
            PromotionDate.Text = FormattaData(persona.DataPromozione);
        }

        private void CaricaProfiloCivile(PersonaleWriteRepository repository, PersonaDettaglio persona)
        {
            Bind(HealthAuthority, repository.Asl(), "-- seleziona --");
            Bind(HiringTitle, repository.TitoliCivili(), "-- seleziona --");
            Bind(CurrentTitle, repository.TitoliCivili(), "-- seleziona --");
            Bind(CivilianArea, repository.AreeCivili(), "-- seleziona --");
            Seleziona(HealthAuthority, persona.IdAsl);
            Seleziona(HiringTitle, persona.IdTitoloAssunzione);
            Seleziona(CurrentTitle, persona.IdTitoloAttuale);
            Seleziona(CivilianArea, persona.IdSottoarea);
            HiringDecree.Text = persona.DecretoAssunzione;
            HiringLegalDate.Text = FormattaData(persona.DataGiuridicaAssunzione);
            HiringEconomicDate.Text = FormattaData(persona.DataEconomicaAssunzione);
            CurrentDecree.Text = persona.DecretoAttuale;
            CurrentLegalDate.Text = FormattaData(persona.DataGiuridicaAttuale);
            CurrentEconomicDate.Text = FormattaData(persona.DataEconomicaAttuale);
            SelezionaTesto(ManagementBand, persona.FasciaDirigenziale, "n/a");
            SelezionaTesto(PayBand, persona.FasciaRetributiva, "n/a");
        }

        protected void MilitaryFilter_Changed(object sender, EventArgs e)
        {
            CaricaInquadramentoMilitare(new PersonaleWriteRepository(), null, null, null, null);
        }

        protected void ServiceStatus_Changed(object sender,EventArgs e)
        {
            if(string.Equals(ServiceStatus.SelectedValue,"attivo",StringComparison.OrdinalIgnoreCase))
            {
                ExitDate.Text=string.Empty;
            }
            else
            {
                if(string.IsNullOrWhiteSpace(ExitDate.Text))ExitDate.Text=DateTime.Today.ToString("yyyy-MM-dd");
                if(string.IsNullOrWhiteSpace(AssignmentsClosingDate.Text))AssignmentsClosingDate.Text=DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        private void CaricaInquadramentoMilitare(PersonaleWriteRepository repository, int? gradoDaSelezionare,
            int? categoriaDaSelezionare, int? ruoloDaSelezionare, int? specialitaDaSelezionare)
        {
            int idArma;
            if (int.TryParse(ArmedForce.SelectedValue, out idArma) && !string.IsNullOrWhiteSpace(Category.SelectedValue))
            {
                Bind(Rank, repository.Gradi(idArma, Category.SelectedValue), "-- seleziona --");
                Bind(MilitaryCategory, repository.CategorieMilitari(idArma, Category.SelectedValue), "-- seleziona --");
                Bind(MilitaryRole, repository.RuoliMilitari(idArma, Category.SelectedValue), "-- seleziona --");
                Bind(MilitarySpecialty, repository.SpecialitaMilitari(idArma, Category.SelectedValue), "-- seleziona --");
            }
            else
            {
                Bind(Rank, new List<LookupItem>(), "-- seleziona --");
                Bind(MilitaryCategory, new List<LookupItem>(), "-- seleziona --");
                Bind(MilitaryRole, new List<LookupItem>(), "-- seleziona --");
                Bind(MilitarySpecialty, new List<LookupItem>(), "-- seleziona --");
            }
            Seleziona(Rank, gradoDaSelezionare);
            Seleziona(MilitaryCategory, categoriaDaSelezionare);
            Seleziona(MilitaryRole, ruoloDaSelezionare);
            Seleziona(MilitarySpecialty, specialitaDaSelezionare);
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Page.Validate("edit");
            if (!Page.IsValid) return;
            try
            {
                PersonaDettaglio persona = new PersonaleRepository().Trova(Id);
                if (persona == null) throw new InvalidOperationException("Persona non trovata.");
                Verifica(persona.Militare);
                int titolo, fascia, forzaArmata, grado, posizione, categoriaMilitare, ruoloMilitare, specialitaMilitare;
                var dati = new ModificaPersonaleRequest
                {
                    IdPersonale = Id,
                    Militare = persona.Militare,
                    Cognome = LastName.Text.Trim(), Nome = FirstName.Text.Trim(),
                    CodiceFiscale = TaxCode.Text.Trim().ToUpperInvariant(),
                    EnteProvenienza = SourceOrganization.Text.Trim(),
                    DataAssegnazione = ParseData(AssignmentDate.Text, "data di assegnazione"),
                    TelefonoUfficio = OfficePhone.Text.Trim(), NumeroStanza = Room.Text.Trim(),
                    IdTitoloStudio = int.TryParse(EducationTitle.SelectedValue, out titolo) ? (int?)titolo : null,
                    IdFasciaOraria = int.TryParse(TimeBand.SelectedValue, out fascia) ? (int?)fascia : null,
                    StatoServizio = ServiceStatus.SelectedValue,
                    DataUscita = ParseData(ExitDate.Text, "data di uscita"),
                    DataChiusuraIncarichi = ParseData(AssignmentsClosingDate.Text, "data di chiusura degli incarichi")
                };
                if (persona.Militare)
                {
                    dati.IdForzaArmata = int.TryParse(ArmedForce.SelectedValue, out forzaArmata) ? (int?)forzaArmata : null;
                    dati.IdGrado = int.TryParse(Rank.SelectedValue, out grado) ? (int?)grado : null;
                    dati.IdPosizioneMilitare = int.TryParse(MilitaryPosition.SelectedValue, out posizione) ? (int?)posizione : null;
                    dati.IdCategoriaMilitare = int.TryParse(MilitaryCategory.SelectedValue, out categoriaMilitare) ? (int?)categoriaMilitare : null;
                    dati.IdRuoloMilitare = int.TryParse(MilitaryRole.SelectedValue, out ruoloMilitare) ? (int?)ruoloMilitare : null;
                    dati.IdSpecialitaMilitare = int.TryParse(MilitarySpecialty.SelectedValue, out specialitaMilitare) ? (int?)specialitaMilitare : null;
                    dati.Categorico = Category.SelectedValue;
                    dati.Matricola = RegistrationNumber.Text.Trim();
                    dati.NumeroDecreto = DecreeNumber.Text.Trim();
                    dati.ScuolaFormazione = TrainingSchool.Text.Trim();
                    dati.DataGiuridicaMilitare = ParseData(MilitaryLegalDate.Text, "data giuridica");
                    dati.DataEconomicaMilitare = ParseData(MilitaryEconomicDate.Text, "data economica");
                    dati.DataArruolamento = ParseData(EnlistmentDate.Text, "data di arruolamento");
                    dati.DataPromozione = ParseData(PromotionDate.Text, "data di promozione");
                }
                else
                {
                    dati.IdAsl = int.TryParse(HealthAuthority.SelectedValue, out forzaArmata) ? (int?)forzaArmata : null;
                    dati.IdTitoloAssunzione = int.TryParse(HiringTitle.SelectedValue, out grado) ? (int?)grado : null;
                    dati.IdTitoloAttuale = int.TryParse(CurrentTitle.SelectedValue, out titolo) ? (int?)titolo : null;
                    dati.IdSottoarea = int.TryParse(CivilianArea.SelectedValue, out fascia) ? (int?)fascia : null;
                    dati.DecretoAssunzione = HiringDecree.Text.Trim();
                    dati.DataGiuridicaAssunzione = ParseData(HiringLegalDate.Text, "data giuridica di assunzione");
                    dati.DataEconomicaAssunzione = ParseData(HiringEconomicDate.Text, "data economica di assunzione");
                    dati.DecretoAttuale = CurrentDecree.Text.Trim();
                    dati.DataGiuridicaAttuale = ParseData(CurrentLegalDate.Text, "data giuridica attuale");
                    dati.DataEconomicaAttuale = ParseData(CurrentEconomicDate.Text, "data economica attuale");
                    dati.FasciaDirigenziale = ManagementBand.SelectedValue;
                    dati.FasciaRetributiva = PayBand.SelectedValue;
                }
                new PersonaleWriteRepository().AggiornaDatiGenerali(dati, Context.User.Identity.Name);
                string destinazione=string.Equals(dati.StatoServizio,"attivo",StringComparison.OrdinalIgnoreCase)?"Dettaglio.aspx?id="+Id:"NonAttivi.aspx";
                Response.Redirect(destinazione, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Message.Text = Server.HtmlEncode(ex.Message);
                Message.Visible = true;
            }
        }

        private void Verifica(bool militare)
        {
            int livello = UtenteCorrente.Livello;
            if ((militare && livello != 150 && livello != 170 && livello != 200) ||
                (!militare && livello != 160 && livello != 170 && livello != 200))
                Response.Redirect("~/Account/AccessoNegato.aspx", true);
        }

        private static DateTime? ParseData(string valore, string nomeCampo)
        {
            if (string.IsNullOrWhiteSpace(valore)) return null;
            DateTime data;
            if (DateTime.TryParseExact(valore.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data)) return data;
            throw new InvalidOperationException("Il valore indicato per " + nomeCampo + " non è una data valida.");
        }

        private static string FormattaData(DateTime? data) { return data.HasValue ? data.Value.ToString("yyyy-MM-dd") : ""; }

        private static string PercorsoFoto(PersonaDettaglio persona)
        {
            string percorso = (persona.ImgPath ?? string.Empty).Trim();
            if (percorso.StartsWith("~/", StringComparison.Ordinal)) return percorso;
            if (!string.IsNullOrWhiteSpace(percorso))
            {
                string nomeFile = Path.GetFileName(percorso);
                if (!string.IsNullOrWhiteSpace(nomeFile)) return AppConfig.PhotoFileServerPath.TrimEnd('/') + "/" + nomeFile;
            }
            return AppConfig.GetFotoPredefinita(persona.Militare,
                string.Equals(persona.Sesso, "Maschile", StringComparison.OrdinalIgnoreCase));
        }

        private static void Bind(DropDownList controllo, IList<LookupItem> valori, string voceVuota)
        {
            controllo.Items.Clear();
            controllo.Items.Add(new ListItem(voceVuota, ""));
            foreach (LookupItem valore in valori) controllo.Items.Add(new ListItem(valore.Text, valore.Value));
        }

        private static void Seleziona(DropDownList controllo, int? id)
        {
            if (id.HasValue && controllo.Items.FindByValue(id.Value.ToString()) != null)
                controllo.SelectedValue = id.Value.ToString();
        }

        private static void SelezionaTesto(DropDownList controllo, string valore, string predefinito)
        {
            valore = string.IsNullOrWhiteSpace(valore) ? predefinito : valore.Trim();
            foreach(ListItem item in controllo.Items)
                if(string.Equals(item.Value,valore,StringComparison.OrdinalIgnoreCase)){controllo.SelectedValue=item.Value;return;}
            foreach(ListItem item in controllo.Items)
                if(string.Equals(item.Value,predefinito,StringComparison.OrdinalIgnoreCase)){controllo.SelectedValue=item.Value;return;}
        }
    }
}

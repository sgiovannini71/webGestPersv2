using System;
using System.Configuration;
using System.IO;
using System.Web;
using WebGestPersV2.Configuration;
using WebGestPersV2.Data;
using WebGestPersV2.Infrastructure;
using WebGestPersV2.Models;

namespace WebGestPersV2.Personale
{
    public partial class Dettaglio : Security.BaseAuthenticatedPage
    {
        private const int DimensioneMassimaFoto = 5 * 1024 * 1024;

        protected string OrganizationChartPersonUrl { get; private set; }

        protected override int[] LivelliConsentiti
        {
            get
            {
                return new[]
                {
                      (int)Security.LivelloUtente.VisualizzatorePrivacy,
                      (int)Security.LivelloUtente.AdminMilitare,
                      (int)Security.LivelloUtente.AdminCivile,
                      (int)Security.LivelloUtente.AdminCivMil,
                      (int)Security.LivelloUtente.Amministratore
                  };
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CaricaDettaglio();
        }

        private void CaricaDettaglio()
        {
            int idPersonale;

            if (!int.TryParse(Request.QueryString["id"], out idPersonale)
                || idPersonale <= 0)
            {
                MostraErrore("Identificativo del personale non valido.");
                return;
            }

            try
            {
                PersonaDettaglio persona =
                    new PersonaleRepository().TrovaAttivo(idPersonale);

                if (persona == null)
                {
                    MostraErrore("Persona attiva non trovata.");
                    return;
                }

                PopolaControlli(persona);

                MailboxesGrid.DataSource =
                    new CasellePostaliRepository().Elenca(idPersonale);
                MailboxesGrid.DataBind();

                AssignmentsGrid.DataSource =
                    new PersonaleRepository().CercaIncarichi(idPersonale);
                AssignmentsGrid.DataBind();

                AssignmentHistoryGrid.DataSource =
                    new PersonaleRepository()
                        .CercaStoricoIncarichi(idPersonale);
                AssignmentHistoryGrid.DataBind();

                ModificationHistoryGrid.DataSource =
                    new PersonaleRepository()
                        .CercaStoricoModifiche(idPersonale);
                ModificationHistoryGrid.DataBind();
            }
            catch (Exception ex)
            {
                CrudLogger.Errore(
                    "READ",
                    "Dettaglio personale",
                    "IDPersonale=" + idPersonale,
                    ex);

                MostraErrore(
                    "Impossibile caricare il dettaglio. " +
                    "Verificare la connessione a DipendentiDB.");
            }
        }

        protected void UploadPhotoButton_Click(
            object sender,
            EventArgs e)
        {
            int idPersonale;

            if (!int.TryParse(Request.QueryString["id"], out idPersonale)
                || idPersonale <= 0)
            {
                MostraAlertUpload(
                    "Identificativo del personale non valido.");

                return;
            }

            try
            {
                PersonaDettaglio persona =
                    new PersonaleRepository().TrovaAttivo(idPersonale);

                if (persona == null)
                    throw new InvalidOperationException(
                        "Persona attiva non trovata.");

                // Ripristina i dati dinamici della pagina anche durante
                // il postback generato dal caricamento della foto.
                PopolaControlli(persona);

                if (!PuoModificare(persona))
                {
                    throw new UnauthorizedAccessException(
                        "Non si dispone dell'autorizzazione " +
                        "per cambiare questa foto.");
                }

                if (!PhotoUpload.HasFile)
                {
                    throw new InvalidOperationException(
                        "Selezionare un file da caricare.");
                }

                string nomeAtteso = idPersonale + ".jpg";
                string nomeRicevuto =
                    Path.GetFileName(PhotoUpload.FileName);

                if (!string.Equals(
                    Path.GetExtension(nomeRicevuto),
                    ".jpg",
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "È consentita esclusivamente " +
                        "l'estensione .jpg.");
                }

                if (!string.Equals(
                    nomeRicevuto,
                    nomeAtteso,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Il file deve chiamarsi " +
                        nomeAtteso + ".");
                }

                if (PhotoUpload.PostedFile.ContentLength <= 0)
                {
                    throw new InvalidOperationException(
                        "Il file selezionato è vuoto.");
                }

                if (PhotoUpload.PostedFile.ContentLength
                    > DimensioneMassimaFoto)
                {
                    throw new InvalidOperationException(
                        "La foto deve avere una dimensione " +
                        "massima di 5 MB.");
                }

                if (!ContieneFirmaJpeg(
                    PhotoUpload.PostedFile.InputStream))
                {
                    throw new InvalidOperationException(
                        "Il contenuto del file non è " +
                        "un'immagine JPEG valida.");
                }

                string cartella = PercorsoFisicoFoto();

                Directory.CreateDirectory(cartella);

                string destinazione =
                    Path.Combine(cartella, nomeAtteso);

                PhotoUpload.PostedFile.SaveAs(destinazione);

                new PersonaleWriteRepository().AggiornaImmagine(
                    idPersonale,
                    nomeAtteso,
                    Context.User.Identity.Name);

                // Il parametro variabile impedisce al browser di
                // mostrare la precedente immagine dalla cache.
                ProfilePhoto.ImageUrl =
                    ResolveUrl(
                        AppConfig.PhotoFileServerPath.TrimEnd('/')
                        + "/"
                        + nomeAtteso)
                    + "?v="
                    + DateTime.UtcNow.Ticks;

                MostraMessaggioUpload(
                    "Foto aggiornata correttamente.",
                    false);
            }
            catch (Exception ex)
            {
                CrudLogger.Errore(
                    "UPLOAD",
                    "Foto personale",
                    "IDPersonale=" + idPersonale,
                    ex);

                MostraAlertUpload(ex.Message);
            }
        }

        private void PopolaControlli(PersonaDettaglio p)
        {
            DetailsPanel.Visible = true;

            PersonnelType.Text = p.TipoPersonale;

            FullName.Text =
                Server.HtmlEncode(p.Cognome + " " + p.Nome);

            string organizationChartBaseUrl =
                ConfigurationManager.AppSettings[
                    "URLOrganizationChart"]
                ?? ConfigurationManager.AppSettings[
                    "URLOrgChart"]
                ?? string.Empty;

            OrganizationChartPersonUrl =
                string.IsNullOrWhiteSpace(
                    organizationChartBaseUrl)
                    ? string.Empty
                    : organizationChartBaseUrl
                        .Trim()
                        .TrimEnd('/')
                        + "/#/?employee="
                        + p.IdPersonale;

            ProfilePhoto.ImageUrl =
                ResolveUrl(PercorsoFoto(p));

            int livello = UtenteCorrente.Livello;

            EditLink.Visible =
                livello == 200
                || livello == 170
                || (p.Militare && livello == 150)
                || (!p.Militare && livello == 160);

            EditLink.NavigateUrl =
                "Modifica.aspx?id=" + p.IdPersonale;

            PhotoUploadPanel.Visible = EditLink.Visible;

            ExpectedPhotoFileName.Text =
                p.IdPersonale + ".jpg";

            AssignmentsEditLink.Visible =
                EditLink.Visible;

            AssignmentsEditLink.NavigateUrl =
                "Incarichi.aspx?id=" + p.IdPersonale;

            MailboxesEditLink.Visible =
                EditLink.Visible;

            MailboxesEditLink.NavigateUrl =
                "CasellePostali.aspx?id=" + p.IdPersonale;

            PersonnelId.Text =
                p.IdPersonale.ToString();

            TaxCode.Text =
                Valore(p.CodiceFiscale);

            Gender.Text =
                Valore(p.Sesso);

            ServiceStatus.Text =
                Valore(p.StatoServizio);

            OfficePhone.Text =
                Valore(p.TelefonoUfficio);

            Room.Text =
                Valore(p.NumeroStanza);

            SourceOrganization.Text =
                Valore(p.EnteProvenienza);

            AssignmentDate.Text =
                Data(p.DataAssegnazione);

            ExitDate.Text =
                Data(p.DataUscita);

            Notes.Text =
                Valore(p.Note);

            BirthDate.Text =
                Data(p.DataNascita);

            BirthTown.Text =
                Valore(p.ComuneNascita);

            MaritalStatus.Text =
                Valore(p.StatoCivile);

            MobilePhone.Text =
                Valore(p.Cellulare);

            ResidenceAddress.Text =
                Valore(p.IndirizzoResidenza);

            ResidenceTown.Text =
                Valore(p.ComuneResidenza);

            ResidencePhone.Text =
                Valore(p.TelefonoResidenza);

            DomicileAddress.Text =
                Valore(p.IndirizzoDomicilio);

            DomicileTown.Text =
                Valore(p.ComuneDomicilio);

            DomicilePhone.Text =
                Valore(p.TelefonoDomicilio);

            ServicePassport.Text =
                Valore(p.PassaportoServizio);

            ServicePassportDate.Text =
                Data(p.DataPassaportoServizio);

            CmdValue.Text =
                Valore(p.Cmd);

            CmdDate.Text =
                Data(p.DataCmd);

            AtModel.Text =
                Valore(p.ModelloAt);

            AtDate.Text =
                Data(p.DataAt);

            SensitiveDataUpdatedAt.Text =
                Data(p.AggiornamentoDatiSensibili);

            MilitaryPanel.Visible =
                p.Militare;

            CivilianPanel.Visible =
                !p.Militare;

            Rank.Text =
                Valore(p.Grado);

            RankDescription.Text =
                Valore(p.DescrizioneGrado);

            ArmedForce.Text =
                Valore(p.ForzaArmata);

            Category.Text =
                Valore(p.Categorico);

            RegistrationNumber.Text =
                Valore(p.Matricola);

            MilitaryStatus.Text =
                Valore(p.PosizioneStato);

            MilitaryCategory.Text =
                Valore(p.CategoriaMilitare);

            Role.Text =
                Valore(p.Ruolo);

            Specialty.Text =
                Valore(p.Specialita);

            MilitaryLegalDate.Text =
                Data(p.DataGiuridicaMilitare);

            MilitaryEconomicDate.Text =
                Data(p.DataEconomicaMilitare);

            DecreeNumber.Text =
                Valore(p.NumeroDecreto);

            TrainingSchool.Text =
                Valore(p.ScuolaFormazione);

            EnlistmentDate.Text =
                Data(p.DataArruolamento);

            PromotionDate.Text =
                Data(p.DataPromozione);

            MilitaryUpdatedAt.Text =
                Data(p.AggiornamentoProfiloMilitare);

            TitleCode.Text =
                Valore(p.Titolo);

            TitleDescription.Text =
                Valore(p.DescrizioneTitolo);

            Area.Text =
                Valore(p.Area);

            Subarea.Text =
                Valore(p.Sottoarea);

            ManagementBand.Text =
                Valore(p.FasciaDirigenziale);

            PayBand.Text =
                Valore(p.FasciaRetributiva);

            HealthAuthority.Text =
                Valore(p.Asl);

            HiringTitle.Text =
                Valore(p.TitoloAssunzione);

            HiringTitleDescription.Text =
                Valore(p.DescrizioneTitoloAssunzione);

            HiringDecree.Text =
                Valore(p.DecretoAssunzione);

            HiringLegalDate.Text =
                Data(p.DataGiuridicaAssunzione);

            HiringEconomicDate.Text =
                Data(p.DataEconomicaAssunzione);

            CurrentDecree.Text =
                Valore(p.DecretoAttuale);

            CurrentLegalDate.Text =
                Data(p.DataGiuridicaAttuale);

            CurrentEconomicDate.Text =
                Data(p.DataEconomicaAttuale);

            CivilianUpdatedAt.Text =
                Data(p.AggiornamentoProfiloCivile);
        }

        private bool PuoModificare(
            PersonaDettaglio persona)
        {
            int livello = UtenteCorrente.Livello;

            return livello == 200
                || livello == 170
                || (persona.Militare && livello == 150)
                || (!persona.Militare && livello == 160);
        }

        private string PercorsoFisicoFoto()
        {
            string percorso =
                AppConfig.PhotoFileServerPath;

            if (Path.IsPathRooted(percorso))
                return percorso;

            return Server.MapPath(percorso);
        }

        private static bool ContieneFirmaJpeg(
            Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long posizione =
                stream.CanSeek ? stream.Position : 0;

            try
            {
                int primoByte = stream.ReadByte();
                int secondoByte = stream.ReadByte();
                int terzoByte = stream.ReadByte();

                return primoByte == 0xFF
                    && secondoByte == 0xD8
                    && terzoByte == 0xFF;
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = posizione;
            }
        }

        private void MostraAlertUpload(
            string messaggio)
        {
            MostraMessaggioUpload(
                messaggio,
                true);

            ClientScript.RegisterStartupScript(
                GetType(),
                "photo-upload-alert",
                "alert('"
                    + HttpUtility.JavaScriptStringEncode(
                        messaggio)
                    + "');",
                true);
        }

        private void MostraMessaggioUpload(
            string messaggio,
            bool errore)
        {
            PhotoUploadMessage.Text =
                Server.HtmlEncode(messaggio);

            PhotoUploadMessagePanel.CssClass =
                errore
                    ? "message error"
                    : "message success";

            PhotoUploadMessagePanel.Visible = true;
        }

        private string Valore(string valore)
        {
            return string.IsNullOrWhiteSpace(valore)
                ? "—"
                : Server.HtmlEncode(valore);
        }

        private static string Data(
            DateTime? valore)
        {
            return valore.HasValue
                ? valore.Value.ToString("dd/MM/yyyy")
                : "—";
        }

        private static string PercorsoFoto(
            PersonaDettaglio persona)
        {
            string percorso =
                (persona.ImgPath ?? string.Empty).Trim();

            if (percorso.StartsWith(
                "~/",
                StringComparison.Ordinal))
            {
                return percorso;
            }

            if (!string.IsNullOrWhiteSpace(percorso))
            {
                string nomeFile =
                    Path.GetFileName(percorso);

                if (!string.IsNullOrWhiteSpace(nomeFile))
                {
                    return AppConfig.PhotoFileServerPath
                        .TrimEnd('/')
                        + "/"
                        + nomeFile;
                }
            }

            return AppConfig.GetFotoPredefinita(
                persona.Militare,
                persona.Sesso == "Maschile");
        }

        private void MostraErrore(string messaggio)
        {
            ErrorMessage.Text =
                Server.HtmlEncode(messaggio);

            ErrorPanel.Visible = true;
            DetailsPanel.Visible = false;
        }
    }
}

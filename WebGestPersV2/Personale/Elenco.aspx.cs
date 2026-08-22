using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using System.Xml;
using WebGestPersV2.Data;
using WebGestPersV2.Models;

namespace WebGestPersV2.Personale
{
    public partial class Elenco : Security.BaseAuthenticatedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { CaricaListe(); AggiornaVisibilita(); CaricaDati(); }
        }

        protected void SearchButton_Click(object sender, EventArgs e) { CaricaDati(); }
        protected void ExportExcelButton_Click(object sender, EventArgs e)
        {
            try
            {
                IList<PersonaListaItem> persone = OttieniPersone();
                byte[] file = CreaExcel(persone);
                Response.Clear(); Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment; filename=ElencoPersonale_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx");
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.BinaryWrite(file); Response.Flush(); Response.End();
            }
            catch (ThreadAbortException) { }
            catch (Exception)
            {
                ErrorMessage.Text = "Impossibile generare il file Excel."; ErrorMessage.Visible = true;
            }
        }
        protected void PersonnelType_Changed(object sender, EventArgs e)
        {
            if (PersonnelType.SelectedValue == "M") CivilianProfile.SelectedIndex = 0;
            if (PersonnelType.SelectedValue == "C") { ArmedForce.SelectedIndex = 0; CategoryCode.SelectedIndex = 0; CaricaGradi(); }
            AggiornaVisibilita(); CaricaDati();
        }
        protected void MilitaryFilter_Changed(object sender, EventArgs e) { PersonnelType.SelectedValue = "M"; CivilianProfile.SelectedIndex = 0; CaricaGradi(); AggiornaVisibilita(); CaricaDati(); }
        protected void Filter_Changed(object sender, EventArgs e)
        {
            if (object.ReferenceEquals(sender, MilitaryGrade)) { PersonnelType.SelectedValue = "M"; CivilianProfile.SelectedIndex = 0; }
            if (object.ReferenceEquals(sender, CivilianProfile)) { PersonnelType.SelectedValue = "C"; ArmedForce.SelectedIndex = 0; CategoryCode.SelectedIndex = 0; CaricaGradi(); }
            AggiornaVisibilita(); CaricaDati();
        }
        protected void ResetFilters_Click(object sender, EventArgs e)
        {
            SearchTextBox.Text = string.Empty; PersonnelType.SelectedIndex = 0; ArmedForce.SelectedIndex = 0;
            CategoryCode.SelectedIndex = 0; CivilianProfile.SelectedIndex = 0; CaricaGradi(); AggiornaVisibilita(); CaricaDati();
        }

        private void CaricaListe()
        {
            Bind(ArmedForce, new PersonaleWriteRepository().ForzeArmate(), "Tutte");
            Bind(CivilianProfile, new PersonaleRepository().ProfiliCiviliPerFiltro(), "Tutti");
            CaricaGradi();
        }

        private void CaricaGradi()
        {
            string precedente = MilitaryGrade.Items.Count > 0 ? MilitaryGrade.SelectedValue : string.Empty;
            Bind(MilitaryGrade, new PersonaleRepository().GradiPerFiltro(SelectedInt(ArmedForce), CategoryCode.SelectedValue), "Tutti");
            ListItem item = MilitaryGrade.Items.FindByValue(precedente); if (item != null) { MilitaryGrade.ClearSelection(); item.Selected = true; }
        }

        private static void Bind(DropDownList list, IList<LookupItem> dati, string primo)
        {
            list.Items.Clear(); list.Items.Add(new ListItem(primo, string.Empty));
            foreach (LookupItem item in dati) list.Items.Add(new ListItem(item.Text, item.Value));
        }

        private void AggiornaVisibilita()
        {
            MilitaryFilters.Visible = PersonnelType.SelectedValue != "C";
            CivilianFilters.Visible = PersonnelType.SelectedValue != "M";
        }

        private static int? SelectedInt(ListControl list) { int n; return int.TryParse(list.SelectedValue, out n) ? (int?)n : null; }

        private void CaricaDati()
        {
            try
            {
                ErrorMessage.Visible = false;
                IList<PersonaListaItem> persone = OttieniPersone();
                PeopleGrid.DataSource = persone;
                PeopleGrid.DataBind();
                ResultCount.Text = persone.Count + " nominativi visualizzati" + (persone.Count == 500 ? " (limite massimo raggiunto)" : string.Empty);
            }
            catch (Exception)
            {
                ErrorMessage.Text = "Impossibile caricare i dati. Verificare la connessione a DipendentiDB.";
                ErrorMessage.Visible = true;
                PeopleGrid.DataSource = null;
                PeopleGrid.DataBind();
                ResultCount.Text = string.Empty;
            }
        }

        private IList<PersonaListaItem> OttieniPersone()
        {
            return new PersonaleRepository().Cerca(SearchTextBox.Text, 500, PersonnelType.SelectedValue,
                SelectedInt(MilitaryGrade), SelectedInt(ArmedForce), CategoryCode.SelectedValue, SelectedInt(CivilianProfile));
        }

        private static byte[] CreaExcel(IList<PersonaListaItem> persone)
        {
            using (var memoria = new MemoryStream())
            {
                using (var archivio = new ZipArchive(memoria, ZipArchiveMode.Create, true))
                {
                    ScriviTesto(archivio, "[Content_Types].xml", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>");
                    ScriviTesto(archivio, "_rels/.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");
                    ScriviTesto(archivio, "xl/workbook.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Elenco personale\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");
                    ScriviTesto(archivio, "xl/_rels/workbook.xml.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>");
                    ScriviFoglio(archivio, persone);
                }
                return memoria.ToArray();
            }
        }

        private static void ScriviTesto(ZipArchive archivio, string nome, string contenuto)
        {
            ZipArchiveEntry voce = archivio.CreateEntry(nome, CompressionLevel.Fastest);
            using (Stream stream = voce.Open()) using (var writer = new StreamWriter(stream, new UTF8Encoding(false))) writer.Write(contenuto);
        }

        private static void ScriviFoglio(ZipArchive archivio, IList<PersonaListaItem> persone)
        {
            ZipArchiveEntry voce = archivio.CreateEntry("xl/worksheets/sheet1.xml", CompressionLevel.Fastest);
            var impostazioni = new XmlWriterSettings { Encoding = new UTF8Encoding(false), CloseOutput = false };
            using (Stream stream = voce.Open()) using (XmlWriter xml = XmlWriter.Create(stream, impostazioni))
            {
                const string ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                xml.WriteStartDocument(true); xml.WriteStartElement("worksheet", ns);
                xml.WriteStartElement("sheetData", ns);
                ScriviRiga(xml, new[] { "ID", "Grado/Profilo", "Cognome", "Nome", "Codice fiscale", "Tipo personale", "Incarico principale", "Ufficio 1° livello", "Ufficio 2° livello", "Ufficio 3° livello" });
                foreach (PersonaListaItem p in persone)
                    ScriviRiga(xml, new[] { p.IdPersonale.ToString(), p.GradoProfilo, p.Cognome, p.Nome, p.CodiceFiscale, p.Militare ? "Militare" : "Civile", p.IncaricoPrincipale, p.UfficioLivello1, p.UfficioLivello2, p.UfficioLivello3 });
                xml.WriteEndElement(); xml.WriteEndElement(); xml.WriteEndDocument();
            }
        }

        private static void ScriviRiga(XmlWriter xml, string[] valori)
        {
            const string ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            xml.WriteStartElement("row", ns);
            foreach (string valore in valori)
            {
                xml.WriteStartElement("c", ns); xml.WriteAttributeString("t", "inlineStr"); xml.WriteStartElement("is", ns);
                xml.WriteStartElement("t", ns); xml.WriteAttributeString("xml", "space", null, "preserve"); xml.WriteString(valore ?? string.Empty);
                xml.WriteEndElement(); xml.WriteEndElement(); xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }
    }
}

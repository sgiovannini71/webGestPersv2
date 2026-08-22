using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using WebGestPersV2.Data;

namespace WebGestPersV2
{
    public partial class Organigramma : Security.BaseAuthenticatedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) CaricaOrganigramma();
        }

        protected void ExportExcelButton_Click(object sender, EventArgs e)
        {
            try
            {
                string ricerca = (ExportSearch.Value ?? string.Empty).Trim();
                string tipo = (ExportType.Value ?? string.Empty).Trim().ToLowerInvariant();
                IList<OrganigrammaPersona> persone = new OrganigrammaRepository().CercaPersonaleAttivo()
                    .Where(p => UfficioDefinito(p.Ufficio1))
                    .Where(p => (tipo != "militare" && tipo != "civile") || (tipo == "militare" ? p.Militare : !p.Militare))
                    .Where(p => Corrisponde(p, ricerca)).ToList();
                byte[] file = CreaExcel(persone);
                Response.Clear(); Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment; filename=Personale_per_unita_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx");
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.BinaryWrite(file); Response.Flush(); Response.End();
            }
            catch (ThreadAbortException) { }
            catch (Exception)
            {
                ErrorMessage.Text = "Impossibile generare il file Excel.";
                ErrorPanel.Visible = true;
            }
        }

        private void CaricaOrganigramma()
        {
            try
            {
                IList<OrganigrammaPersona> persone = new OrganigrammaRepository().CercaPersonaleAttivo()
                    .Where(p => UfficioDefinito(p.Ufficio1)).ToList();
                PersonnelCount.Text = persone.Count.ToString();
                EmptyPanel.Visible = persone.Count == 0;
                OrganizationTree.Text = persone.Count == 0 ? string.Empty : CreaAlbero(persone);
            }
            catch (Exception)
            {
                ErrorMessage.Text = "Impossibile caricare il personale per unità organizzativa.";
                ErrorPanel.Visible = true;
            }
        }

        private static string CreaAlbero(IEnumerable<OrganigrammaPersona> persone)
        {
            var html = new StringBuilder();
            html.Append("<div class=\"org-tree\">");
            foreach (var livello1 in persone.GroupBy(p => new { p.IdUfficio1, p.Ufficio1, p.Ordinamento1 }).OrderBy(g => g.Key.Ordinamento1).ThenBy(g => g.Key.Ufficio1))
            {
                ApriRamo(html, livello1.Key.Ufficio1, livello1.Count(), "livello 1");
                ScriviPersoneSenzaLivelloSuccessivo(html, livello1, 2);
                foreach (var livello2 in livello1.Where(p => UfficioDefinito(p.Ufficio2)).GroupBy(p => new { p.IdUfficio2, p.Ufficio2, p.Ordinamento2 }).OrderBy(g => g.Key.Ordinamento2).ThenBy(g => g.Key.Ufficio2))
                {
                    ApriRamo(html, livello2.Key.Ufficio2, livello2.Count(), "livello 2");
                    ScriviPersoneSenzaLivelloSuccessivo(html, livello2, 3);
                    foreach (var livello3 in livello2.Where(p => UfficioDefinito(p.Ufficio3)).GroupBy(p => new { p.IdUfficio3, p.Ufficio3, p.Ordinamento3 }).OrderBy(g => g.Key.Ordinamento3).ThenBy(g => g.Key.Ufficio3))
                    {
                        ApriRamo(html, livello3.Key.Ufficio3, livello3.Count(), "livello 3");
                        ScriviPersone(html, livello3);
                        ChiudiRamo(html);
                    }
                    ChiudiRamo(html);
                }
                ChiudiRamo(html);
            }
            html.Append("</div>");
            return html.ToString();
        }

        private static void ScriviPersoneSenzaLivelloSuccessivo(StringBuilder html, IEnumerable<OrganigrammaPersona> persone, int livelloSuccessivo)
        {
            ScriviPersone(html, persone.Where(p => livelloSuccessivo == 2 ? !UfficioDefinito(p.Ufficio2) : !UfficioDefinito(p.Ufficio3)));
        }

        private static void ScriviPersone(StringBuilder html, IEnumerable<OrganigrammaPersona> persone)
        {
            var elenco = persone.OrderBy(p => p.Cognome).ThenBy(p => p.Nome).ToList();
            if (elenco.Count == 0) return;
            html.Append("<ul class=\"org-people\">");
            foreach (OrganigrammaPersona p in elenco)
            {
                string tipo = p.Militare ? "militare" : "civile";
                html.Append("<li class=\"org-person\" data-type=\"").Append(tipo).Append("\">");
                html.Append("<a href=\"Personale/Dettaglio.aspx?id=").Append(p.IdPersonale).Append("\">");
                if (!string.IsNullOrWhiteSpace(p.GradoProfilo)) html.Append("<span class=\"org-profile\">").Append(H(p.GradoProfilo)).Append("</span> ");
                html.Append("<strong>").Append(H(p.Cognome)).Append(" ").Append(H(p.Nome)).Append("</strong></a>");
                html.Append("<span class=\"org-person-type ").Append(tipo).Append("\">").Append(p.Militare ? "Militare" : "Civile").Append("</span>");
                if (!string.IsNullOrWhiteSpace(p.Incarico)) html.Append("<small>").Append(H(p.Incarico)).Append("</small>");
                html.Append("</li>");
            }
            html.Append("</ul>");
        }

        private static void ApriRamo(StringBuilder html, string nome, int consistenza, string livello)
        {
            html.Append("<section class=\"org-branch\"><div class=\"org-unit\"><button type=\"button\" class=\"org-toggle\" aria-expanded=\"true\" aria-label=\"Espandi o comprimi\">−</button><div><span class=\"org-level\">").Append(H(livello)).Append("</span><h2>").Append(H(nome)).Append("</h2></div><span class=\"org-count\">").Append(consistenza).Append("</span></div><div class=\"org-branch-body\">");
        }

        private static void ChiudiRamo(StringBuilder html) { html.Append("</div></section>"); }
        private static bool UfficioDefinito(string valore)
        {
            if (string.IsNullOrWhiteSpace(valore)) return false;
            string testo = valore.Trim();
            return !testo.Equals("N/A", StringComparison.OrdinalIgnoreCase)
                && !testo.StartsWith("N/A -", StringComparison.OrdinalIgnoreCase);
        }

        private static bool Corrisponde(OrganigrammaPersona persona, string ricerca)
        {
            if (string.IsNullOrWhiteSpace(ricerca)) return true;
            return Contiene(persona.Cognome, ricerca) || Contiene(persona.Nome, ricerca)
                || Contiene(persona.GradoProfilo, ricerca) || Contiene(persona.Incarico, ricerca)
                || Contiene(persona.Ufficio1, ricerca) || Contiene(persona.Ufficio2, ricerca)
                || Contiene(persona.Ufficio3, ricerca);
        }

        private static bool Contiene(string valore, string ricerca)
        { return (valore ?? string.Empty).IndexOf(ricerca, StringComparison.OrdinalIgnoreCase) >= 0; }

        private static byte[] CreaExcel(IList<OrganigrammaPersona> persone)
        {
            using (var memoria = new MemoryStream())
            {
                using (var archivio = new ZipArchive(memoria, ZipArchiveMode.Create, true))
                {
                    ScriviTesto(archivio, "[Content_Types].xml", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>");
                    ScriviTesto(archivio, "_rels/.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");
                    ScriviTesto(archivio, "xl/workbook.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Personale per unita\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");
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

        private static void ScriviFoglio(ZipArchive archivio, IList<OrganigrammaPersona> persone)
        {
            ZipArchiveEntry voce = archivio.CreateEntry("xl/worksheets/sheet1.xml", CompressionLevel.Fastest);
            var impostazioni = new XmlWriterSettings { Encoding = new UTF8Encoding(false), CloseOutput = false };
            using (Stream stream = voce.Open()) using (XmlWriter xml = XmlWriter.Create(stream, impostazioni))
            {
                const string ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                xml.WriteStartDocument(true); xml.WriteStartElement("worksheet", ns); xml.WriteStartElement("sheetData", ns);
                ScriviRiga(xml, new[] { "ID", "Grado/Profilo", "Cognome", "Nome", "Tipo personale", "Incarico principale", "Ufficio 1° livello", "Ufficio 2° livello", "Ufficio 3° livello" });
                foreach (OrganigrammaPersona p in persone)
                    ScriviRiga(xml, new[] { p.IdPersonale.ToString(), p.GradoProfilo, p.Cognome, p.Nome, p.Militare ? "Militare" : "Civile", p.Incarico, p.Ufficio1, UfficioDefinito(p.Ufficio2) ? p.Ufficio2 : string.Empty, UfficioDefinito(p.Ufficio3) ? p.Ufficio3 : string.Empty });
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
        private static string H(string valore) { return HttpUtility.HtmlEncode(valore ?? string.Empty); }
    }
}

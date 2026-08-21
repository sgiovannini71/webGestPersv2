using System; using System.Collections.Generic; using System.Text; using WebGestPersV2.Data; using WebGestPersV2.Models;
namespace WebGestPersV2.Report
{
 public partial class InOut:Security.BaseAuthenticatedPage
 {
  protected void Page_Load(object sender,EventArgs e){if(!IsPostBack)Carica();} protected void Period_Changed(object sender,EventArgs e){Carica();}
  protected void ExportIncoming_Click(object sender,EventArgs e){Esporta(new ReportRepository().PersonaleEntrato(Mesi(IncomingPeriod.SelectedValue)),"Personale_entrato.csv","Data ingresso");}
  protected void ExportOutgoing_Click(object sender,EventArgs e){Esporta(new ReportRepository().PersonaleUscito(Mesi(OutgoingPeriod.SelectedValue)),"Personale_uscito.csv","Data uscita");}
  private void Carica(){try{var repo=new ReportRepository();IList<MovimentoPersonaleItem> a=repo.PersonaleEntrato(Mesi(IncomingPeriod.SelectedValue)),u=repo.PersonaleUscito(Mesi(OutgoingPeriod.SelectedValue));IncomingGrid.DataSource=a;IncomingGrid.DataBind();IncomingCount.Text=a.Count+" risultati";OutgoingGrid.DataSource=u;OutgoingGrid.DataBind();OutgoingCount.Text=u.Count+" risultati";}catch(Exception ex){Message.Text=Server.HtmlEncode(ex.Message);Message.Visible=true;}}
  private void Esporta(IList<MovimentoPersonaleItem> dati,string file,string dataHeader){var s=new StringBuilder();s.AppendLine("ID;Cognome;Nome;Grado/Profilo;Tipo;Stato servizio;"+dataHeader);foreach(var x in dati)s.AppendLine(string.Join(";",x.IdPersonale,Csv(x.Cognome),Csv(x.Nome),Csv(x.GradoProfilo),Csv(x.TipoPersonale),Csv(x.StatoServizio),x.DataEvento.ToString("dd/MM/yyyy")));Response.Clear();Response.ContentType="text/csv";Response.ContentEncoding=Encoding.UTF8;Response.AddHeader("Content-Disposition","attachment; filename="+file);Response.BinaryWrite(Encoding.UTF8.GetPreamble());Response.Write(s.ToString());Response.Flush();Context.ApplicationInstance.CompleteRequest();}
  private static string Csv(string x){return "\""+(x??"").Replace("\"","\"\"")+"\"";} private static int Mesi(string x){int n;if(int.TryParse(x,out n))return n;throw new InvalidOperationException("Selezionare un periodo valido.");}
 }
}

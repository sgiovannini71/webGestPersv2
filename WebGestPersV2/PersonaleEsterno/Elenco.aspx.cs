using System;
using WebGestPersV2.Data;

namespace WebGestPersV2.PersonaleEsterno
{
 public partial class Elenco : Security.BaseAuthenticatedPage
 {
  protected override int[] LivelliConsentiti { get { return new[]{150,160,170,200}; } }
  protected void Page_Load(object sender,EventArgs e){if(!IsPostBack)Carica();}
  protected void SearchButton_Click(object sender,EventArgs e){Carica();}
  protected void Filter_Changed(object sender,EventArgs e){Carica();}
  private void Carica(){try{var dati=new PersonaleEsternoRepository().Elenca(SearchText.Text,IncludeInactive.Checked);ExternalGrid.DataSource=dati;ExternalGrid.DataBind();ResultCount.Text=dati.Count+" nominativi";Message.Visible=false;}catch(Exception ex){Message.Text=Server.HtmlEncode(ex.Message);Message.Visible=true;}}
 }
}

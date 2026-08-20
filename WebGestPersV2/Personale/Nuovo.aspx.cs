using System;
using System.Globalization;
using System.Web.UI.WebControls;
using WebGestPersV2.Data;
using WebGestPersV2.Models;

namespace WebGestPersV2.Personale
{
 public partial class Nuovo : Security.BaseAuthenticatedPage
 {
  protected override int[] LivelliConsentiti { get { return new[]{150,160,170,200}; } }
  private PersonaleWriteRepository Repository { get { return new PersonaleWriteRepository(); } }
  protected void Page_Load(object sender,EventArgs e){if(!IsPostBack){CaricaListe();ApplicaPermessi();}}
  private void CaricaListe(){Bind(EducationTitle,Repository.TitoliStudio(),"-- seleziona --");Bind(TimeBand,Repository.FasceOrarie(),"-- seleziona --");Bind(CivilianTitle,Repository.TitoliCivili(),"-- seleziona --");Bind(ArmedForce,Repository.ForzeArmate(),"-- seleziona --");}
  private void ApplicaPermessi(){int l=UtenteCorrente.Livello;if(l==150)PersonnelType.Items.Remove(PersonnelType.Items.FindByValue("C"));if(l==160)PersonnelType.Items.Remove(PersonnelType.Items.FindByValue("M"));}
  protected void PersonnelType_Changed(object s,EventArgs e){CivilianFields.Visible=PersonnelType.SelectedValue=="C";MilitaryFields.Visible=PersonnelType.SelectedValue=="M";}
  protected void MilitaryFilter_Changed(object s,EventArgs e){Rank.Items.Clear();int a;if(int.TryParse(ArmedForce.SelectedValue,out a)&&Category.SelectedValue!="")Bind(Rank,Repository.Gradi(a,Category.SelectedValue),"-- seleziona --");else Rank.Items.Add(new ListItem("-- seleziona --",""));}
  protected void CreateButton_Click(object s,EventArgs e)
  {
   PersonnelType_Changed(null,EventArgs.Empty); Page.Validate("insert"); if(!Page.IsValid)return;
   try{VerificaPermessoTipo();DateTime data;if(!DateTime.TryParseExact(AssignmentDate.Text,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out data))throw new ArgumentException("Data assegnazione non valida.");
    var d=new NuovoPersonaleRequest{Militare=PersonnelType.SelectedValue=="M",Cognome=LastName.Text.Trim(),Nome=FirstName.Text.Trim(),CodiceFiscale=TaxCode.Text.Trim().ToUpperInvariant(),EnteProvenienza=SourceOrganization.Text.Trim(),DataAssegnazione=data,IdTitoloStudio=int.Parse(EducationTitle.SelectedValue),IdFasciaOraria=int.Parse(TimeBand.SelectedValue)};
    if(d.Militare){int a,g;if(!int.TryParse(ArmedForce.SelectedValue,out a)||!int.TryParse(Rank.SelectedValue,out g)||Category.SelectedValue=="")throw new ArgumentException("Completare il profilo militare.");d.IdForzaArmata=a;d.IdGrado=g;d.Categorico=Category.SelectedValue;}else{int t;if(!int.TryParse(CivilianTitle.SelectedValue,out t))throw new ArgumentException("Selezionare la famiglia professionale.");d.IdTitoloCivile=t;}
    int id=Repository.Inserisci(d);Response.Redirect("Dettaglio.aspx?id="+id,false);Context.ApplicationInstance.CompleteRequest();
   }catch(Exception ex){Message.Text=Server.HtmlEncode(ex.Message);Message.Visible=true;}
  }
  private void VerificaPermessoTipo(){int l=UtenteCorrente.Livello;if(PersonnelType.SelectedValue=="M"&&l!=150&&l!=170&&l!=200)throw new UnauthorizedAccessException("Non autorizzato all'inserimento di personale militare.");if(PersonnelType.SelectedValue=="C"&&l!=160&&l!=170&&l!=200)throw new UnauthorizedAccessException("Non autorizzato all'inserimento di personale civile.");if(PersonnelType.SelectedValue!="M"&&PersonnelType.SelectedValue!="C")throw new ArgumentException("Selezionare il tipo di personale.");}
  private static void Bind(DropDownList d,System.Collections.Generic.IList<LookupItem> x,string prompt){d.Items.Clear();d.Items.Add(new ListItem(prompt,""));foreach(var i in x)d.Items.Add(new ListItem(i.Text,i.Value));}
 }
}

using System;
using WebGestPersV2.Data;
using WebGestPersV2.Models;

namespace WebGestPersV2.Personale
{
 public partial class CasellePostali : Security.BaseAuthenticatedPage
 {
  protected override int[] LivelliConsentiti { get { return new[]{150,160,170,200}; } }
  private int IdPersonale { get { int x;return int.TryParse(Request.QueryString["id"],out x)?x:0; } }
  private int IdCasellaInModifica { get { return ViewState["IdCasella"]==null?0:(int)ViewState["IdCasella"]; } set { ViewState["IdCasella"]=value; } }
  private string EmailVerificata { get { return Convert.ToString(ViewState["EmailVerificata"]); } set { ViewState["EmailVerificata"]=value; } }
  private CasellePostaliRepository Repository { get { return new CasellePostaliRepository(); } }

  protected void Page_Load(object sender,EventArgs e)
  { if(IdPersonale<=0){Response.Redirect("Elenco.aspx",true);return;}if(!IsPostBack){PersonaDettaglio p=CaricaPersonaEVerifica();BackLink.NavigateUrl="Dettaglio.aspx?id="+IdPersonale;FullName.Text=Server.HtmlEncode(p.Cognome+" "+p.Nome);PersonnelType.Text=p.TipoPersonale;CaricaGriglia();} }

  protected void MailboxesGrid_RowCommand(object sender,System.Web.UI.WebControls.GridViewCommandEventArgs e)
  { int id;if(!int.TryParse(Convert.ToString(e.CommandArgument),out id)||id<=0)return;try{CaricaPersonaEVerifica();if(e.CommandName=="ModificaCasella"){CasellaPostaleItem x=Repository.Trova(id,IdPersonale);if(x==null)throw new InvalidOperationException("Casella postale non trovata.");IdCasellaInModifica=id;EmailAddress.Text=x.Denominazione;SelezionaTipo(x.Tipo);FormTitle.Text="Modifica casella postale";SaveButton.Text="Salva modifica";CancelEditButton.Visible=true;VerifyButton.Visible=false;NascondiVerifica();}else if(e.CommandName=="EliminaCasella"){Repository.Elimina(id,IdPersonale,Context.User.Identity.Name);AzzeraForm();CaricaGriglia();}}catch(Exception ex){MostraErrore(ex.Message);} }

  protected void SaveButton_Click(object sender,EventArgs e)
  { Page.Validate("mailbox");if(!Page.IsValid)return;try{CaricaPersonaEVerifica();if(IdCasellaInModifica>0)Repository.Aggiorna(IdCasellaInModifica,IdPersonale,EmailAddress.Text,MailboxType.SelectedValue,Context.User.Identity.Name);else Repository.Inserisci(IdPersonale,EmailAddress.Text,MailboxType.SelectedValue,Context.User.Identity.Name,false);AzzeraForm();CaricaGriglia();}catch(Exception ex){MostraErrore(ex.Message);} }
  protected void VerifyButton_Click(object sender,EventArgs e){Page.Validate("mailbox");if(!Page.IsValid)return;try{CaricaPersonaEVerifica();var assegnazioni=Repository.CercaAssegnazioni(EmailAddress.Text,IdPersonale);EmailVerificata=EmailAddress.Text.Trim().ToLowerInvariant();AssignedPeopleGrid.DataSource=assegnazioni;AssignedPeopleGrid.DataBind();AssignmentAlert.Visible=assegnazioni.Count>0;AvailabilityMessage.Visible=assegnazioni.Count==0;AvailabilityMessage.Text=assegnazioni.Count==0?"La casella non risulta assegnata ad altre persone.":"";Message.Visible=false;if(assegnazioni.Count>0)ClientScript.RegisterStartupScript(GetType(),"mailboxAssigned","alert('La casella è già assegnata. Verificare i nominativi mostrati nella pagina.');",true);}catch(Exception ex){NascondiVerifica();MostraErrore(ex.Message);}}
  protected void EmailAddress_Changed(object sender,EventArgs e){NascondiVerifica();if(string.IsNullOrWhiteSpace(EmailAddress.Text)){DetectedTypeMessage.Visible=false;return;}try{string tipo=Repository.DeterminaTipoCasella(EmailAddress.Text,MailboxType.SelectedValue);SelezionaTipo(tipo);DetectedTypeMessage.Text="Tipo applicato all'indirizzo: "+Server.HtmlEncode(tipo)+".";DetectedTypeMessage.Visible=true;Message.Visible=false;}catch(Exception ex){DetectedTypeMessage.Visible=false;MostraErrore(ex.Message);}}
  protected void TransferButton_Click(object sender,EventArgs e){Page.Validate("mailbox");if(!Page.IsValid)return;try{CaricaPersonaEVerifica();if(!string.Equals(EmailVerificata,EmailAddress.Text.Trim(),StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("L'indirizzo è cambiato dopo la verifica. Verificare nuovamente la disponibilità.");Repository.Inserisci(IdPersonale,EmailAddress.Text,MailboxType.SelectedValue,Context.User.Identity.Name,true);AzzeraForm();CaricaGriglia();}catch(Exception ex){MostraErrore(ex.Message);}}
  protected void CancelEditButton_Click(object sender,EventArgs e){AzzeraForm();}
  private PersonaDettaglio CaricaPersonaEVerifica(){PersonaDettaglio p=new PersonaleRepository().Trova(IdPersonale);if(p==null)throw new InvalidOperationException("Persona non trovata.");int l=UtenteCorrente.Livello;bool ammesso=l==200||l==170||(p.Militare&&l==150)||(!p.Militare&&l==160);if(!ammesso){Response.Redirect("~/Account/AccessoNegato.aspx",true);return null;}return p;}
  private void CaricaGriglia(){MailboxesGrid.DataSource=Repository.Elenca(IdPersonale);MailboxesGrid.DataBind();Message.Visible=false;}
  private void AzzeraForm(){IdCasellaInModifica=0;EmailAddress.Text="";MailboxType.SelectedValue="personale";FormTitle.Text="Aggiungi casella postale";SaveButton.Text="Aggiungi";CancelEditButton.Visible=false;VerifyButton.Visible=true;DetectedTypeMessage.Visible=false;NascondiVerifica();Message.Visible=false;}
  private void NascondiVerifica(){EmailVerificata="";AssignmentAlert.Visible=false;AvailabilityMessage.Visible=false;AssignedPeopleGrid.DataSource=null;AssignedPeopleGrid.DataBind();}
  private void SelezionaTipo(string tipo){if(MailboxType.Items.FindByValue(tipo)!=null)MailboxType.SelectedValue=tipo;}
  private void MostraErrore(string x){Message.Text=Server.HtmlEncode(x);Message.Visible=true;}
 }
}

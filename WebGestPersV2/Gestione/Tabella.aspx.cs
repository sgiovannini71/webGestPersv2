using System;
using System.Data;
using System.Web.UI.WebControls;
using WebGestPersV2.Data;
using WebGestPersV2.Models;

namespace WebGestPersV2.Gestione
{
    public partial class Tabella : Security.BaseAuthenticatedPage
    {
        protected override int[] LivelliConsentiti { get { return new[] { 150, 160, 170, 200 }; } }

        private TabellaGestioneDefinizione Definizione
        {
            get { return new TabelleGestioneRepository().Trova(Request.QueryString["tipo"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabellaGestioneDefinizione definizione = Definizione;
            if (definizione == null) { Response.Redirect("Indice.aspx", true); return; }
            VerificaAmbito(definizione.Ambito);
            PageTitle.Text = Server.HtmlEncode(definizione.Titolo);
            Heading.Text = Server.HtmlEncode(definizione.Titolo);
            MilitaryFilter.Visible = definizione.FiltroForzaArmata;
            ConfiguraModifica(definizione.Codice);
            if (!IsPostBack)
            {
                if (definizione.FiltroForzaArmata) CaricaForzeArmate();
                try { CaricaModifica(definizione.Codice); }
                catch(Exception ex) { EditPanel.Visible=false; Message.Text=Server.HtmlEncode(ex.Message); Message.Visible=true; }
                try { CaricaModificaUfficio(definizione.Codice); }
                catch(Exception ex) { OfficePanel.Visible=false; Message.Text=Server.HtmlEncode(ex.Message); Message.Visible=true; }
                try { CaricaModificaCivile(definizione.Codice); }
                catch(Exception ex) { CivilEditPanel.Visible=false; Message.Text=Server.HtmlEncode(ex.Message); Message.Visible=true; }
                try { CaricaModificaMilitare(definizione.Codice); }
                catch(Exception ex) { MilitaryEditPanel.Visible=false; Message.Text=Server.HtmlEncode(ex.Message); Message.Visible=true; }
                CaricaTabella();
            }
        }

        private void CaricaForzeArmate()
        {
            ArmedForce.Items.Clear();
            ArmedForce.Items.Add(new ListItem("Tutte le forze armate", ""));
            foreach (LookupItem item in new TabelleGestioneRepository().ForzeArmate())
                ArmedForce.Items.Add(new ListItem(item.Text, item.Value));
        }

        protected void FilterChanged(object sender, EventArgs e) { CaricaTabella(); }

        private void CaricaTabella()
        {
            try
            {
                int id;
                int? idArma = int.TryParse(ArmedForce.SelectedValue, out id) ? (int?)id : null;
                Items.DataSource = new TabelleGestioneRepository().Elenco(Definizione, idArma);
                Items.DataBind();
            }
            catch (Exception ex)
            {
                Message.Text = Server.HtmlEncode(ex.Message);
                Message.Visible = true;
            }
        }

        private void ConfiguraModifica(string codice)
        {
            var repository=new TabelleGestioneRepository();
            bool supportata = repository.ModificaSupportata(codice);
            NewButton.Visible = supportata;
            NewButton.NavigateUrl = "Tabella.aspx?tipo=" + Server.UrlEncode(codice) + "&edit=new";
            string edit = Request.QueryString["edit"];
            bool semplice=repository.ModificaSempliceSupportata(codice);
            EditPanel.Visible = semplice && !string.IsNullOrWhiteSpace(edit);
            OfficePanel.Visible = repository.ModificaUfficioSupportata(codice) && !string.IsNullOrWhiteSpace(edit);
            CivilEditPanel.Visible = repository.ModificaCivileSupportata(codice) && !string.IsNullOrWhiteSpace(edit);
            MilitaryEditPanel.Visible = repository.ModificaMilitareSupportata(codice) && !string.IsNullOrWhiteSpace(edit);
            if (!semplice) return;
            Field3.Visible = Field3Label.Visible = codice == "incarichi";
            Field2.Visible = Field2Label.Visible = codice != "toponomastica";
            if (codice == "titoli-studio") { Field1Label.Text="Descrizione*"; Field2Label.Text="Ordinamento"; Field1.MaxLength=100; }
            else if (codice == "fasce-orarie") { Field1Label.Text="Descrizione*"; Field2Label.Text="Note"; Field1.MaxLength=50; Field2.MaxLength=255; }
            else if (codice == "toponomastica") { Field1Label.Text="Particella*"; Field1.MaxLength=50; }
            else if(codice=="qualifiche") { Field1Label.Text="Sigla*";Field2Label.Text="Descrizione";Field3Label.Text="Ordinamento";Field3.Visible=Field3Label.Visible=true;Field1.MaxLength=20;Field2.MaxLength=200; }
            else { Field1Label.Text="Sigla*"; Field2Label.Text="Descrizione*"; Field3Label.Text="Ordinamento"; Field1.MaxLength=15; Field2.MaxLength=50; }
            EditTitle.Text = edit == "new" ? "Nuovo elemento" : "Modifica elemento";
        }

        private void CaricaModificaUfficio(string codice)
        {
            var repository=new TabelleGestioneRepository();
            if(!repository.ModificaUfficioSupportata(codice))return;
            string edit=Request.QueryString["edit"];
            if(string.IsNullOrWhiteSpace(edit))return;
            bool primo=codice=="uffici-1";bool terzo=codice=="uffici-3";
            Parent1Label.Visible=ParentOffice1.Visible=!primo;
            Parent2Label.Visible=ParentOffice2.Visible=terzo;
            ActiveLabel.Visible=OfficeActive.Visible=primo;
            OfficeEditTitle.Text=edit=="new"?"Nuovo ufficio":"Modifica ufficio";
            if(!primo)Bind(ParentOffice1,repository.UfficiPrimoLivello(),"-- seleziona --");
            if(edit=="new")return;
            string[] v=repository.LeggiUfficio(codice,edit);if(v==null)throw new InvalidOperationException("Ufficio non trovato.");
            OfficeOriginalKey.Value=edit;
            if(!primo)Seleziona(ParentOffice1,v[0]);
            if(terzo){int u1;if(int.TryParse(v[0],out u1))Bind(ParentOffice2,repository.UfficiSecondoLivello(u1),"-- seleziona --");Seleziona(ParentOffice2,v[1]);}
            OfficeCode.Text=v[2];OfficeDescription.Text=v[3];OfficeOrder.Text=v[4];OfficeCompetences.Text=v[5];
            if(primo&&OfficeActive.Items.FindByValue(v[6])!=null)OfficeActive.SelectedValue=v[6];
        }

        protected void ParentOffice1_Changed(object sender,EventArgs e)
        {
            int id;if(int.TryParse(ParentOffice1.SelectedValue,out id))Bind(ParentOffice2,new TabelleGestioneRepository().UfficiSecondoLivello(id),"-- seleziona --");
            else Bind(ParentOffice2,new System.Collections.Generic.List<LookupItem>(),"-- seleziona --");
        }

        protected void OfficeSaveButton_Click(object sender,EventArgs e)
        {
            try
            {
                int u1,u2;
                new TabelleGestioneRepository().SalvaUfficio(Definizione.Codice,OfficeOriginalKey.Value,
                    int.TryParse(ParentOffice1.SelectedValue,out u1)?(int?)u1:null,int.TryParse(ParentOffice2.SelectedValue,out u2)?(int?)u2:null,
                    OfficeCode.Text,OfficeDescription.Text,OfficeOrder.Text,OfficeCompetences.Text,OfficeActive.SelectedValue);
                Response.Redirect("Tabella.aspx?tipo="+Server.UrlEncode(Definizione.Codice),false);Context.ApplicationInstance.CompleteRequest();
            }
            catch(Exception ex){Message.Text=Server.HtmlEncode(ex.Message);Message.Visible=true;}
        }

        private void CaricaModificaCivile(string codice)
        {
            var repository=new TabelleGestioneRepository();if(!repository.ModificaCivileSupportata(codice))return;
            string edit=Request.QueryString["edit"];if(string.IsNullOrWhiteSpace(edit))return;
            bool asl=codice=="asl";CivilEditTitle.Text=edit=="new"?"Nuovo elemento":"Modifica elemento";
            string[] etichette=asl?new[]{"Nome ASL*","Indirizzo*","Telefono","Prefisso","Fax","CAP","Comune*","Provincia*"}:new[]{"Area*","Posizione economica*","Descrizione","Ordinamento","","","",""};
            Label[] labels={CivilLabel1,CivilLabel2,CivilLabel3,CivilLabel4,CivilLabel5,CivilLabel6,CivilLabel7,CivilLabel8};
            TextBox[] fields={CivilField1,CivilField2,CivilField3,CivilField4,CivilField5,CivilField6,CivilField7,CivilField8};
            int visibili=asl?8:4;for(int i=0;i<fields.Length;i++){labels[i].Visible=fields[i].Visible=i<visibili;labels[i].Text=etichette[i];}
            CivilField1.MaxLength=asl?20:2;CivilField2.MaxLength=asl?100:15;CivilField3.MaxLength=asl?15:50;
            CivilField4.MaxLength=asl?10:10;CivilField5.MaxLength=15;CivilField6.MaxLength=5;CivilField7.MaxLength=20;CivilField8.MaxLength=20;
            if(edit=="new")return;string[] v=repository.LeggiCivile(codice,edit);if(v==null)throw new InvalidOperationException("Elemento non trovato.");
            CivilOriginalKey.Value=edit;for(int i=0;i<fields.Length;i++)fields[i].Text=v[i];
        }

        protected void CivilSaveButton_Click(object sender,EventArgs e)
        {
            try
            {
                string[] v={CivilField1.Text,CivilField2.Text,CivilField3.Text,CivilField4.Text,CivilField5.Text,CivilField6.Text,CivilField7.Text,CivilField8.Text,""};
                new TabelleGestioneRepository().SalvaCivile(Definizione.Codice,CivilOriginalKey.Value,v);
                Response.Redirect("Tabella.aspx?tipo="+Server.UrlEncode(Definizione.Codice),false);Context.ApplicationInstance.CompleteRequest();
            }
            catch(Exception ex){Message.Text=Server.HtmlEncode(ex.Message);Message.Visible=true;}
        }

        private void CaricaModificaMilitare(string codice)
        {
            var repository=new TabelleGestioneRepository();if(!repository.ModificaMilitareSupportata(codice))return;
            string edit=Request.QueryString["edit"];if(string.IsNullOrWhiteSpace(edit))return;
            bool grado=codice=="gradi";MilitaryEditTitle.Text=edit=="new"?"Nuovo elemento":"Modifica elemento";
            GeneralLevelLabel.Visible=MilitaryGeneralLevel.Visible=grado;HierarchyLevelLabel.Visible=MilitaryHierarchyLevel.Visible=grado;
            MilitaryCode.MaxLength=grado?20:10;Bind(MilitaryArmedForce,repository.ForzeArmate(),"-- seleziona --");
            if(edit=="new")return;string[] v=repository.LeggiMilitare(codice,edit);if(v==null)throw new InvalidOperationException("Elemento non trovato.");
            MilitaryOriginalKey.Value=edit;Seleziona(MilitaryArmedForce,v[0]);Seleziona(MilitaryGrouping,v[1]);
            MilitaryCode.Text=v[2];MilitaryDescription.Text=v[3];MilitaryOrder.Text=v[4];MilitaryGeneralLevel.Text=v[5];MilitaryHierarchyLevel.Text=v[6];
        }

        protected void MilitarySaveButton_Click(object sender,EventArgs e)
        {
            try
            {
                int arma;new TabelleGestioneRepository().SalvaMilitare(Definizione.Codice,MilitaryOriginalKey.Value,
                    int.TryParse(MilitaryArmedForce.SelectedValue,out arma)?(int?)arma:null,MilitaryGrouping.SelectedValue,
                    MilitaryCode.Text,MilitaryDescription.Text,MilitaryOrder.Text,MilitaryGeneralLevel.Text,MilitaryHierarchyLevel.Text);
                Response.Redirect("Tabella.aspx?tipo="+Server.UrlEncode(Definizione.Codice),false);Context.ApplicationInstance.CompleteRequest();
            }
            catch(Exception ex){Message.Text=Server.HtmlEncode(ex.Message);Message.Visible=true;}
        }

        private void CaricaModifica(string codice)
        {
            if(!new TabelleGestioneRepository().ModificaSempliceSupportata(codice))return;
            string edit=Request.QueryString["edit"];
            if(string.IsNullOrWhiteSpace(edit)||edit=="new")return;
            string[] valori=new TabelleGestioneRepository().LeggiSemplice(codice,edit);
            if(valori==null)throw new InvalidOperationException("Elemento non trovato.");
            OriginalKey.Value=edit; Field1.Text=valori[0]; Field2.Text=valori[1]; Field3.Text=valori[2];
        }

        protected void SaveButton_Click(object sender,EventArgs e)
        {
            try
            {
                new TabelleGestioneRepository().SalvaSemplice(Definizione.Codice,OriginalKey.Value,Field1.Text,Field2.Text,Field3.Text);
                Response.Redirect("Tabella.aspx?tipo="+Server.UrlEncode(Definizione.Codice),false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch(Exception ex){Message.Text=Server.HtmlEncode(ex.Message);Message.Visible=true;}
        }

        protected void Items_RowDataBound(object sender,GridViewRowEventArgs e)
        {
            if(!new TabelleGestioneRepository().ModificaSupportata(Definizione.Codice))return;
            if(e.Row.RowType==DataControlRowType.Header)e.Row.Cells.Add(new TableHeaderCell{Text="Azioni"});
            else if(e.Row.RowType==DataControlRowType.DataRow)
            {
                var r=e.Row.DataItem as DataRowView; if(r==null)return;
                var cell=new TableCell();
                cell.Controls.Add(new HyperLink{Text="Modifica",NavigateUrl="Tabella.aspx?tipo="+Server.UrlEncode(Definizione.Codice)+"&edit="+Server.UrlEncode(Convert.ToString(r.Row[0]))});
                e.Row.Cells.Add(cell);
            }
        }

        private static void Bind(DropDownList controllo,System.Collections.Generic.IList<LookupItem> valori,string vuoto)
        {controllo.Items.Clear();controllo.Items.Add(new ListItem(vuoto,""));foreach(LookupItem x in valori)controllo.Items.Add(new ListItem(x.Text,x.Value));}
        private static void Seleziona(DropDownList controllo,string valore)
        {if(!string.IsNullOrWhiteSpace(valore)&&controllo.Items.FindByValue(valore)!=null)controllo.SelectedValue=valore;}

        private void VerificaAmbito(string ambito)
        {
            int livello = UtenteCorrente.Livello;
            if ((ambito == "civile" && livello != 160 && livello != 170 && livello != 200) ||
                (ambito == "militare" && livello != 150 && livello != 170 && livello != 200))
                Response.Redirect("~/Account/AccessoNegato.aspx", true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using WebGestPersV2.Data;
using WebGestPersV2.Models;

namespace WebGestPersV2.Personale
{
    public partial class NonAttivi : Security.BaseAuthenticatedPage
    {
        protected override int[] LivelliConsentiti { get { return new[] { 150, 160, 170, 200 }; } }
        protected void Page_Load(object sender,EventArgs e){if(!IsPostBack){CaricaStati();CaricaDati();}}
        protected void SearchButton_Click(object sender,EventArgs e){CaricaDati();}
        protected void ResetButton_Click(object sender,EventArgs e){SearchText.Text="";ServiceStatus.SelectedIndex=0;CaricaDati();}
        private void CaricaStati()
        {
            ServiceStatus.Items.Clear();ServiceStatus.Items.Add(new ListItem("Tutti gli stati",string.Empty));
            foreach(LookupItem x in new PersonaleWriteRepository().StatiServizio())
                if(!string.Equals(x.Value,"attivo",StringComparison.OrdinalIgnoreCase))ServiceStatus.Items.Add(new ListItem(x.Text,x.Value));
        }
        private void CaricaDati()
        {
            try{Message.Visible=false;IList<PersonaListaItem> persone=new PersonaleRepository().CercaNonAttivi(SearchText.Text,ServiceStatus.SelectedValue);if(UtenteCorrente.Livello==150)persone=persone.Where(p=>p.Militare).ToList();else if(UtenteCorrente.Livello==160)persone=persone.Where(p=>!p.Militare).ToList();PeopleGrid.DataSource=persone;PeopleGrid.DataBind();ResultCount.Text=persone.Count+" nominativi trovati";}
            catch(Exception){Message.Text="Impossibile caricare il personale non attivo.";Message.Visible=true;PeopleGrid.DataSource=null;PeopleGrid.DataBind();ResultCount.Text="";}
        }
    }
}

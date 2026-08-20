using System;
using WebGestPersV2.Data;

namespace WebGestPersV2.Personale
{
    public partial class Elenco : Security.BaseAuthenticatedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) CaricaDati();
        }

        protected void SearchButton_Click(object sender, EventArgs e) { CaricaDati(); }

        private void CaricaDati()
        {
            try
            {
                ErrorMessage.Visible = false;
                PeopleGrid.DataSource = new PersonaleRepository().Cerca(SearchTextBox.Text, 200);
                PeopleGrid.DataBind();
            }
            catch (Exception)
            {
                ErrorMessage.Text = "Impossibile caricare i dati. Verificare la connessione a DipendentiDB.";
                ErrorMessage.Visible = true;
                PeopleGrid.DataSource = null;
                PeopleGrid.DataBind();
            }
        }
    }
}

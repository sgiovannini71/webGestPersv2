using System;

namespace WebGestPersV2.Gestione
{
    public partial class Indice : Security.BaseAuthenticatedPage
    {
        protected override int[] LivelliConsentiti { get { return new[] { 150, 160, 170, 200 }; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            CivilianTables.Visible = UtenteCorrente.Livello == 160 || UtenteCorrente.Livello == 170 || UtenteCorrente.Livello == 200;
            MilitaryTables.Visible = UtenteCorrente.Livello == 150 || UtenteCorrente.Livello == 170 || UtenteCorrente.Livello == 200;
        }
    }
}

using System;
using System.Configuration;

namespace WebGestPersV2
{
    public partial class HomePage : Security.BaseAuthenticatedPage
    {
        protected string OrganizationChartUrl
        {
            get
            {
                string url = ConfigurationManager.AppSettings["URLOrganizationChart"]
                    ?? ConfigurationManager.AppSettings["URLOrgChart"]
                    ?? string.Empty;
                return ResolveUrl(url.Trim());
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (NonActivePersonnelLink != null)
                NonActivePersonnelLink.Visible = UtenteCorrente != null &&
                    (UtenteCorrente.Livello == 150 || UtenteCorrente.Livello == 160 || UtenteCorrente.Livello == 170 || UtenteCorrente.Livello == 200);
        }
    }
}

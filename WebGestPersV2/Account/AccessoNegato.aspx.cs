using System;

namespace WebGestPersV2.Account
{
    public partial class AccessoNegato : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.StatusCode = 403;
            Response.TrySkipIisCustomErrors = true;
        }
    }
}

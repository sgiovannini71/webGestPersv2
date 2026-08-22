using System;
using System.Web;
using WebGestPersV2.Infrastructure;

namespace WebGestPersV2
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e) { }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception errore = Server.GetLastError();
            if (errore != null) CrudLogger.Errore("UNHANDLED", "Applicazione", Request == null ? string.Empty : Request.RawUrl, errore);
        }
    }
}

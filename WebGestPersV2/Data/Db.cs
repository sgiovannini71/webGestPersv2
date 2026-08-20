using System.Configuration;

namespace WebGestPersV2.Data
{
    internal static class Db
    {
        internal static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DipendentiDB"].ConnectionString; }
        }
    }
}

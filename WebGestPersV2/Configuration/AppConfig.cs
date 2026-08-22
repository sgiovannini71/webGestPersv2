using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace WebGestPersV2.Configuration
{
    public static class AppConfig
    {
        private static readonly IDictionary<string, int> MappatureMilitari = CaricaMappatureMilitari();

        public static readonly int AnniScadenzaCns = LeggiIntero("anni_scad_CNS");
        public static readonly int AnniScadenzaPassaportoServizio = LeggiIntero("anni_scad_PASS_SVZ");
        public static readonly int UfficioLivello1Vuoto = LeggiIntero("UffLiv1_empty");
        public static readonly int UfficioLivello2Vuoto = LeggiIntero("UffLiv2_empty");
        public static readonly int UfficioLivello3Vuoto = LeggiIntero("UffLiv3_empty");

        public static readonly string FileServerPath = LeggiTesto("myFileServerPath");
        public static readonly string PhotoFileServerPath = LeggiTesto("PhotoFileServerPath");
        public static readonly string LogFilePath = LeggiTesto("logfilepath");
        public static readonly string DominioPostaFunzionale = NormalizzaDominio(LeggiTesto("DominioPostaFunzionale"));
        public static readonly string DominioPostaPersonale = NormalizzaDominio(LeggiTesto("DominioPostaPersonale"));
        public static readonly string LivelloDirigente = LeggiTesto("livelloDir");
        public static readonly string UrlOrganigrammaDipendente = LeggiTesto("URLOrgChart");
        public static readonly string UrlOrganigramma = LeggiTesto("URLOrganizationChart");
        public static readonly string BaseUrlMilitare = LeggiTesto("BaseUrlMilitare");
        public static readonly string BaseUrlCivile = LeggiTesto("BaseUrlCivile");
        public static readonly string BaseUrlCivileEsterno = LeggiTesto("BaseUrlCivileExt");
        public static readonly string LdapPath = LeggiTesto("sLDAP");

        public static readonly string FotoCivileUomo = LeggiTesto("Civ_uomo");
        public static readonly string FotoCivileDonna = LeggiTesto("Civ_donna");
        public static readonly string FotoMilitareUomo = LeggiTesto("Mil_uomo");
        public static readonly string FotoMilitareDonna = LeggiTesto("Mil_donna");

        public static int GetSpecialita(string forzaArmata, string categorico)
        {
            return LeggiMappatura("Spec", forzaArmata, categorico);
        }

        public static int GetRuolo(string forzaArmata, string categorico)
        {
            return LeggiMappatura("Ruolo", forzaArmata, categorico);
        }

        public static int GetCategoria(string forzaArmata, string categorico)
        {
            return LeggiMappatura("Categoria", forzaArmata, categorico);
        }

        public static string GetFotoPredefinita(bool militare, bool uomo)
        {
            if (militare) return uomo ? FotoMilitareUomo : FotoMilitareDonna;
            return uomo ? FotoCivileUomo : FotoCivileDonna;
        }

        private static int LeggiMappatura(string tipo, string forzaArmata, string categorico)
        {
            string chiave = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_Cat{2}",
                tipo, Normalizza(forzaArmata), Normalizza(categorico));
            int valore;
            if (!MappatureMilitari.TryGetValue(chiave, out valore))
                throw new ConfigurationErrorsException("Mappatura appSettings non configurata: " + chiave);
            return valore;
        }

        private static IDictionary<string, int> CaricaMappatureMilitari()
        {
            var valori = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (string chiave in ConfigurationManager.AppSettings.AllKeys)
            {
                if (chiave.StartsWith("Spec_", StringComparison.OrdinalIgnoreCase) ||
                    chiave.StartsWith("Ruolo_", StringComparison.OrdinalIgnoreCase) ||
                    chiave.StartsWith("Categoria_", StringComparison.OrdinalIgnoreCase))
                {
                    valori[chiave] = LeggiIntero(chiave);
                }
            }
            return valori;
        }

        private static int LeggiIntero(string chiave)
        {
            string valore = LeggiTesto(chiave);
            int risultato;
            if (!int.TryParse(valore, NumberStyles.Integer, CultureInfo.InvariantCulture, out risultato))
                throw new ConfigurationErrorsException("La chiave appSettings '" + chiave + "' deve contenere un numero intero.");
            return risultato;
        }

        private static string LeggiTesto(string chiave)
        {
            string valore = ConfigurationManager.AppSettings[chiave];
            if (string.IsNullOrWhiteSpace(valore))
                throw new ConfigurationErrorsException("Chiave appSettings mancante o vuota: " + chiave);
            return valore.Trim();
        }

        private static string Normalizza(string valore)
        {
            return (valore ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static string NormalizzaDominio(string valore)
        {
            valore = (valore ?? string.Empty).Trim().ToLowerInvariant();
            while (valore.StartsWith("@", StringComparison.Ordinal)) valore = valore.Substring(1);
            return valore.TrimEnd('.');
        }
    }
}

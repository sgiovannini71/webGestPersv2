using System;

namespace WebGestPersV2.Security
{
    public sealed class CodiceFiscaleInfo
    {
        public DateTime DataNascita { get; set; }
        public bool Uomo { get; set; }
        public string CodiceComune { get; set; }
    }

    public static class CodiceFiscaleParser
    {
        public static CodiceFiscaleInfo Parse(string valore)
        {
            string cf = (valore ?? string.Empty).Trim().ToUpperInvariant();
            if (cf.Length != 16) throw new ArgumentException("Il codice fiscale deve contenere 16 caratteri.");
            int anno, giorno;
            if (!int.TryParse(cf.Substring(6, 2), out anno) || !int.TryParse(cf.Substring(9, 2), out giorno))
                throw new ArgumentException("Anno o giorno non valido nel codice fiscale.");
            const string codiciMese = "ABCDEHLMPRST";
            int mese = codiciMese.IndexOf(cf[8]) + 1;
            if (mese == 0) throw new ArgumentException("Mese non valido nel codice fiscale.");
            bool uomo = giorno <= 40;
            if (!uomo) giorno -= 40;
            int annoCompleto = anno <= DateTime.Today.Year % 100 ? 2000 + anno : 1900 + anno;
            DateTime nascita;
            try { nascita = new DateTime(annoCompleto, mese, giorno); }
            catch (ArgumentOutOfRangeException) { throw new ArgumentException("Data di nascita non valida nel codice fiscale."); }
            return new CodiceFiscaleInfo { DataNascita = nascita, Uomo = uomo, CodiceComune = cf.Substring(11, 4) };
        }
    }
}

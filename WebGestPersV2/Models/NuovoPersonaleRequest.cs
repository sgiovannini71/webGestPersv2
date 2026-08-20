using System;

namespace WebGestPersV2.Models
{
    public sealed class NuovoPersonaleRequest
    {
        public bool Militare { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string CodiceFiscale { get; set; }
        public string EnteProvenienza { get; set; }
        public DateTime DataAssegnazione { get; set; }
        public int IdTitoloStudio { get; set; }
        public int IdFasciaOraria { get; set; }
        public int? IdTitoloCivile { get; set; }
        public int? IdForzaArmata { get; set; }
        public int? IdGrado { get; set; }
        public string Categorico { get; set; }
    }
}

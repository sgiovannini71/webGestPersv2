namespace WebGestPersV2.Models
{
    public sealed class TabellaGestioneDefinizione
    {
        public string Codice { get; set; }
        public string Titolo { get; set; }
        public string Ambito { get; set; }
        public string Sql { get; set; }
        public bool FiltroForzaArmata { get; set; }
    }
}

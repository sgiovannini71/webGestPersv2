namespace WebGestPersV2.Models
{
    public sealed class CasellaPostaleItem
    {
        public int Id { get; set; }
        public int IdPersonale { get; set; }
        public string Denominazione { get; set; }
        public string Tipo { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Assegnatario { get { return (Cognome + " " + Nome).Trim(); } }
    }
}

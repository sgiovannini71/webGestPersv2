namespace WebGestPersV2.Models
{
    public sealed class PersonaleEsternoRequest
    {
        public int Id { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string CodiceFiscale { get; set; }
        public string Telefono { get; set; }
        public string Stanza { get; set; }
        public int IdDitta { get; set; }
        public int? IdUfficio1 { get; set; }
        public int? IdUfficio2 { get; set; }
        public int? IdUfficio3 { get; set; }
        public bool Attivo { get; set; }
    }
}

namespace WebGestPersV2.Models
{
    public sealed class PersonaleEsternoItem
    {
        public int Id { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string CodiceFiscale { get; set; }
        public string Ditta { get; set; }
        public string Telefono { get; set; }
        public string Stanza { get; set; }
        public string Email { get; set; }
        public bool Attivo { get; set; }
        public string Stato { get { return Attivo ? "Attivo" : "Non attivo"; } }
        public int? IdDitta { get; set; }
        public int? IdUfficio1 { get; set; }
        public int? IdUfficio2 { get; set; }
        public int? IdUfficio3 { get; set; }
        public string Ufficio1 { get; set; }
        public string Ufficio2 { get; set; }
        public string Ufficio3 { get; set; }
    }
}

namespace WebGestPersV2.Models
{
    public sealed class PersonaListaItem
    {
        public int IdPersonale { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string CodiceFiscale { get; set; }
        public string StatoServizio { get; set; }
        public bool Militare { get; set; }
        public string GradoProfilo { get; set; }
        public string IncaricoPrincipale { get; set; }
        public string UfficioLivello1 { get; set; }
        public string UfficioLivello2 { get; set; }
        public string UfficioLivello3 { get; set; }
    }
}

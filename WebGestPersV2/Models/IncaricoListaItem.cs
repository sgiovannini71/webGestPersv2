using System;

namespace WebGestPersV2.Models
{
    public sealed class IncaricoListaItem
    {
        public int IdIncarico { get; set; }
        public int IdTipoIncarico { get; set; }
        public int? IdUfficio1 { get; set; }
        public int? IdUfficio2 { get; set; }
        public int? IdUfficio3 { get; set; }
        public string Descrizione { get; set; }
        public bool Principale { get; set; }
        public DateTime? DataInizio { get; set; }
        public string DataInizioTesto { get { return DataInizio.HasValue ? DataInizio.Value.ToString("dd/MM/yyyy") : string.Empty; } }
        public string UfficioLivello1 { get; set; }
        public string UfficioLivello2 { get; set; }
        public string UfficioLivello3 { get; set; }
    }
}

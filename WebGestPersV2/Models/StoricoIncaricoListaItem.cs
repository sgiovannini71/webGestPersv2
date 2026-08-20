using System;

namespace WebGestPersV2.Models
{
    public sealed class StoricoIncaricoListaItem
    {
        public int IdIncarico { get; set; }
        public string Incarico { get; set; }
        public bool Principale { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        public string DataInizioTesto { get { return DataInizio.HasValue ? DataInizio.Value.ToString("dd/MM/yyyy") : string.Empty; } }
        public string DataFineTesto { get { return DataFine.HasValue ? DataFine.Value.ToString("dd/MM/yyyy") : string.Empty; } }
    }
}

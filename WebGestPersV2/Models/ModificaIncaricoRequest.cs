using System;

namespace WebGestPersV2.Models
{
    public sealed class ModificaIncaricoRequest
    {
        public int IdPersonale { get; set; }
        public int? IdIncarico { get; set; }
        public int IdTipoIncarico { get; set; }
        public DateTime? DataInizio { get; set; }
        public bool Principale { get; set; }
        public int? IdUfficio1 { get; set; }
        public int? IdUfficio2 { get; set; }
        public int? IdUfficio3 { get; set; }
    }
}

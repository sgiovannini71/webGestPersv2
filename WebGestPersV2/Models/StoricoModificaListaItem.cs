using System;

namespace WebGestPersV2.Models
{
    public sealed class StoricoModificaListaItem
    {
        public int IdStorico { get; set; }
        public string CampoVariato { get; set; }
        public DateTime? DataModifica { get; set; }
        public string UtenteModificatore { get; set; }
        public string ValoreVecchio { get; set; }
        public string ValoreNuovo { get; set; }

        public string DataModificaTesto
        {
            get { return DataModifica.HasValue ? DataModifica.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }
    }
}

using System;

namespace WebGestPersV2.Models
{
    public sealed class ModificaPersonaleRequest
    {
        public bool SessoMaschile { get; set; }
        public string Note { get; set; }
        public DateTime? DataNascita { get; set; }
        public int? IdComuneNascita { get; set; }
        public int? IdStatoCivile { get; set; }
        public string Cellulare { get; set; }
        public string IndirizzoResidenza { get; set; }
        public int? IdComuneResidenza { get; set; }
        public string TelefonoResidenza { get; set; }
        public string IndirizzoDomicilio { get; set; }
        public int? IdComuneDomicilio { get; set; }
        public string TelefonoDomicilio { get; set; }
        public string PassaportoServizio { get; set; }
        public DateTime? DataPassaportoServizio { get; set; }
        public string Cmd { get; set; }
        public DateTime? DataCmd { get; set; }
        public string ModelloAt { get; set; }
        public DateTime? DataAt { get; set; }
        public int IdPersonale { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string CodiceFiscale { get; set; }
        public string EnteProvenienza { get; set; }
        public DateTime? DataAssegnazione { get; set; }
        public string TelefonoUfficio { get; set; }
        public string NumeroStanza { get; set; }
        public int? IdTitoloStudio { get; set; }
        public int? IdFasciaOraria { get; set; }
        public string StatoServizio { get; set; }
        public DateTime? DataUscita { get; set; }
        public DateTime? DataChiusuraIncarichi { get; set; }
        public bool Militare { get; set; }
        public int? IdForzaArmata { get; set; }
        public int? IdGrado { get; set; }
        public int? IdPosizioneMilitare { get; set; }
        public int? IdCategoriaMilitare { get; set; }
        public int? IdRuoloMilitare { get; set; }
        public int? IdSpecialitaMilitare { get; set; }
        public string Categorico { get; set; }
        public string Matricola { get; set; }
        public string NumeroDecreto { get; set; }
        public string ScuolaFormazione { get; set; }
        public DateTime? DataGiuridicaMilitare { get; set; }
        public DateTime? DataEconomicaMilitare { get; set; }
        public DateTime? DataArruolamento { get; set; }
        public DateTime? DataPromozione { get; set; }
        public int? IdAsl { get; set; }
        public int? IdTitoloAssunzione { get; set; }
        public int? IdTitoloAttuale { get; set; }
        public int? IdSottoarea { get; set; }
        public string DecretoAssunzione { get; set; }
        public DateTime? DataGiuridicaAssunzione { get; set; }
        public DateTime? DataEconomicaAssunzione { get; set; }
        public string DecretoAttuale { get; set; }
        public DateTime? DataGiuridicaAttuale { get; set; }
        public DateTime? DataEconomicaAttuale { get; set; }
        public string FasciaDirigenziale { get; set; }
        public string FasciaRetributiva { get; set; }
    }
}

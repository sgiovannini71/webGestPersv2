using System;

namespace WebGestPersV2.Models
{
    public sealed class PersonaDettaglio
    {
        public int IdPersonale { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string CodiceFiscale { get; set; }
        public string Sesso { get; set; }
        public bool Militare { get; set; }
        public string TipoPersonale { get { return Militare ? "Militare" : "Civile"; } }
        public string StatoServizio { get; set; }
        public string TelefonoUfficio { get; set; }
        public string NumeroStanza { get; set; }
        public string UserName { get; set; }
        public string EnteProvenienza { get; set; }
        public DateTime? DataAssegnazione { get; set; }
        public DateTime? DataUscita { get; set; }
        public string CodiceMensa { get; set; }
        public string Statino { get; set; }
        public string Note { get; set; }
        public string Grado { get; set; }
        public string DescrizioneGrado { get; set; }
        public string ForzaArmata { get; set; }
        public string Categorico { get; set; }
        public string Matricola { get; set; }
        public string PosizioneStato { get; set; }
        public string CategoriaMilitare { get; set; }
        public string Ruolo { get; set; }
        public string Specialita { get; set; }
        public DateTime? DataGiuridicaMilitare { get; set; }
        public DateTime? DataEconomicaMilitare { get; set; }
        public string NumeroDecreto { get; set; }
        public string ScuolaFormazione { get; set; }
        public DateTime? DataArruolamento { get; set; }
        public DateTime? DataPromozione { get; set; }
        public DateTime? AggiornamentoProfiloMilitare { get; set; }
        public string Titolo { get; set; }
        public string DescrizioneTitolo { get; set; }
        public string Area { get; set; }
        public string Sottoarea { get; set; }
        public string Qualifica { get; set; }
        public string FasciaDirigenziale { get; set; }
        public string FasciaRetributiva { get; set; }
        public string Asl { get; set; }
        public string TitoloAssunzione { get; set; }
        public string DescrizioneTitoloAssunzione { get; set; }
        public string DecretoAssunzione { get; set; }
        public DateTime? DataGiuridicaAssunzione { get; set; }
        public DateTime? DataEconomicaAssunzione { get; set; }
        public string DecretoAttuale { get; set; }
        public DateTime? DataGiuridicaAttuale { get; set; }
        public DateTime? DataEconomicaAttuale { get; set; }
        public DateTime? AggiornamentoProfiloCivile { get; set; }
        public DateTime? DataNascita { get; set; }
        public string StatoCivile { get; set; }
        public string ComuneNascita { get; set; }
        public string IndirizzoResidenza { get; set; }
        public string ComuneResidenza { get; set; }
        public string TelefonoResidenza { get; set; }
        public string IndirizzoDomicilio { get; set; }
        public string ComuneDomicilio { get; set; }
        public string TelefonoDomicilio { get; set; }
        public string Cellulare { get; set; }
        public string PassaportoServizio { get; set; }
        public DateTime? DataPassaportoServizio { get; set; }
        public string Cmd { get; set; }
        public DateTime? DataCmd { get; set; }
        public string ModelloAt { get; set; }
        public DateTime? DataAt { get; set; }
        public DateTime? AggiornamentoDatiSensibili { get; set; }
        public string ImgPath { get; set; }
        public int? IdTitoloStudio { get; set; }
        public int? IdFasciaOraria { get; set; }
        public int? IdForzaArmata { get; set; }
        public int? IdGrado { get; set; }
        public int? IdPosizioneMilitare { get; set; }
        public int? IdCategoriaMilitare { get; set; }
        public int? IdRuoloMilitare { get; set; }
        public int? IdSpecialitaMilitare { get; set; }
        public int? IdAsl { get; set; }
        public int? IdTitoloAssunzione { get; set; }
        public int? IdTitoloAttuale { get; set; }
        public int? IdSottoarea { get; set; }
    }
}

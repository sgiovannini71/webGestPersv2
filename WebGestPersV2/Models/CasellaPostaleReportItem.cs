namespace WebGestPersV2.Models
{
 public sealed class CasellaPostaleReportItem
 {
  public int IdCasella{get;set;} public int IdPersonale{get;set;} public string Indirizzo{get;set;} public string Tipo{get;set;}
  public string Cognome{get;set;} public string Nome{get;set;} public bool Militare{get;set;} public string TipoPersonale{get{return Militare?"Militare":"Civile";}}
  public string GradoProfilo{get;set;} public string StatoServizio{get;set;} public string ForzaArmata{get;set;} public string Categorico{get;set;}
  public string Incarico{get;set;} public string Ufficio1{get;set;} public string Ufficio2{get;set;} public string Ufficio3{get;set;}
  public int NumeroAssegnatari{get;set;} public bool CoerenteDominio{get;set;} public string AnomaliaDominio{get;set;}
  public string Coerenza{get{return CoerenteDominio?"Coerente":AnomaliaDominio;}
  }
 }
 public sealed class PersonaSenzaCasellaItem
 { public int IdPersonale{get;set;} public string Cognome{get;set;} public string Nome{get;set;} public bool Militare{get;set;} public string TipoPersonale{get{return Militare?"Militare":"Civile";}} public string GradoProfilo{get;set;} public string Incarico{get;set;} public string Ufficio1{get;set;} public string Ufficio2{get;set;} public string Ufficio3{get;set;} }
}

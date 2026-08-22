using System;using System.Collections.Generic;using System.Data;using System.Data.SqlClient;using System.Net.Mail;using WebGestPersV2.Configuration;using WebGestPersV2.Models;
namespace WebGestPersV2.Data
{
 public sealed class CasellePostaliReportRepository
 {
  public IList<CasellaPostaleReportItem> Elenca()
  {
   const string sql=@"SELECT c.id_cp,ep.IDPersonale,c.Denominazione,c.tipo,ep.Cognome,ep.Nome,ep.Militare,ep.Stato_Servizio,
CASE WHEN ep.Militare=1 THEN g.SiglaGrado ELSE t.Sigla_titolo END,fa.SiglaArma,pm.Categorico,
inc.Incarico,inc.Uff1,inc.Uff2,inc.Uff3,
(SELECT COUNT(DISTINCT c2.IDPersonale) FROM dbo.Cassette_Postali c2 WHERE LOWER(LTRIM(RTRIM(c2.Denominazione)))=LOWER(LTRIM(RTRIM(c.Denominazione))))
FROM dbo.Cassette_Postali c INNER JOIN dbo.ElencoPersonale ep ON ep.IDPersonale=c.IDPersonale
LEFT JOIN dbo.Profilo_militare pm ON pm.IDPersonale=ep.IDPersonale AND ep.Militare=1 LEFT JOIN dbo.Gradi g ON g.ID_Grado=pm.ID_Grado LEFT JOIN dbo.FFAA fa ON fa.ID_Arma=pm.ID_Arma
LEFT JOIN dbo.PersCivile pc ON pc.IDPersonale=ep.IDPersonale AND ep.Militare=0 LEFT JOIN dbo.Titoli t ON t.ID_Titolo=pc.ID_TitoloAtt
OUTER APPLY(SELECT TOP(1) ti.Descr_incarico Incarico,l1.SgUff1 Uff1,l2.SgUff2 Uff2,l3.SgUff3 Uff3 FROM dbo.Incarichi i LEFT JOIN dbo.Tipo_incarichi ti ON ti.id_tipo_incarico=i.id_tipo_incarico LEFT JOIN dbo.Liv1Uff l1 ON l1.ID_Uff1=i.ID_Uff1 LEFT JOIN dbo.Liv2Uff l2 ON l2.ID_Uff2=i.ID_Uff2 LEFT JOIN dbo.Liv3Uff l3 ON l3.ID_Uff3=i.ID_Uff3 WHERE i.IDPersonale=ep.IDPersonale ORDER BY CASE WHEN i.principale=1 THEN 0 ELSE 1 END,i.Data_inizio DESC)inc
ORDER BY c.Denominazione,ep.Cognome,ep.Nome";
   var x=new List<CasellaPostaleReportItem>();using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c)){c.Open();using(var r=q.ExecuteReader())while(r.Read()){var i=new CasellaPostaleReportItem{IdCasella=r.GetInt32(0),IdPersonale=r.GetInt32(1),Indirizzo=S(r,2),Tipo=S(r,3),Cognome=S(r,4),Nome=S(r,5),Militare=r.GetBoolean(6),StatoServizio=S(r,7),GradoProfilo=S(r,8),ForzaArmata=S(r,9),Categorico=S(r,10),Incarico=S(r,11),Ufficio1=S(r,12),Ufficio2=S(r,13),Ufficio3=S(r,14),NumeroAssegnatari=Convert.ToInt32(r.GetValue(15))};Valuta(i);x.Add(i);}}return x;
  }
  public IList<PersonaSenzaCasellaItem> SenzaCasella()
  {
   const string sql=@"SELECT ep.IDPersonale,ep.Cognome,ep.Nome,ep.Militare,CASE WHEN ep.Militare=1 THEN g.SiglaGrado ELSE t.Sigla_titolo END,inc.Incarico,inc.Uff1,inc.Uff2,inc.Uff3 FROM dbo.ElencoPersonale ep LEFT JOIN dbo.Profilo_militare pm ON pm.IDPersonale=ep.IDPersonale AND ep.Militare=1 LEFT JOIN dbo.Gradi g ON g.ID_Grado=pm.ID_Grado LEFT JOIN dbo.PersCivile pc ON pc.IDPersonale=ep.IDPersonale AND ep.Militare=0 LEFT JOIN dbo.Titoli t ON t.ID_Titolo=pc.ID_TitoloAtt OUTER APPLY(SELECT TOP(1)ti.Descr_incarico Incarico,l1.SgUff1 Uff1,l2.SgUff2 Uff2,l3.SgUff3 Uff3 FROM dbo.Incarichi i LEFT JOIN dbo.Tipo_incarichi ti ON ti.id_tipo_incarico=i.id_tipo_incarico LEFT JOIN dbo.Liv1Uff l1 ON l1.ID_Uff1=i.ID_Uff1 LEFT JOIN dbo.Liv2Uff l2 ON l2.ID_Uff2=i.ID_Uff2 LEFT JOIN dbo.Liv3Uff l3 ON l3.ID_Uff3=i.ID_Uff3 WHERE i.IDPersonale=ep.IDPersonale ORDER BY CASE WHEN i.principale=1 THEN 0 ELSE 1 END,i.Data_inizio DESC)inc WHERE LOWER(LTRIM(RTRIM(ep.Stato_Servizio)))='attivo' AND NOT EXISTS(SELECT 1 FROM dbo.Cassette_Postali c WHERE c.IDPersonale=ep.IDPersonale) ORDER BY ep.Cognome,ep.Nome";
   var x=new List<PersonaSenzaCasellaItem>();using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c)){c.Open();using(var r=q.ExecuteReader())while(r.Read())x.Add(new PersonaSenzaCasellaItem{IdPersonale=r.GetInt32(0),Cognome=S(r,1),Nome=S(r,2),Militare=r.GetBoolean(3),GradoProfilo=S(r,4),Incarico=S(r,5),Ufficio1=S(r,6),Ufficio2=S(r,7),Ufficio3=S(r,8)});}return x;
  }
  private static void Valuta(CasellaPostaleReportItem x){try{string d=new MailAddress((x.Indirizzo??"").Trim()).Host.ToLowerInvariant();if(d==AppConfig.DominioPostaFunzionale&&x.Tipo.Trim().ToLowerInvariant()!="funzionale"){x.CoerenteDominio=false;x.AnomaliaDominio="Atteso: funzionale";}else if(d==AppConfig.DominioPostaPersonale&&x.Tipo.Trim().ToLowerInvariant()!="personale"){x.CoerenteDominio=false;x.AnomaliaDominio="Atteso: personale";}else{x.CoerenteDominio=true;x.AnomaliaDominio="";}}catch{x.CoerenteDominio=false;x.AnomaliaDominio="Indirizzo non valido";}}
  private static string S(SqlDataReader r,int i){return r.IsDBNull(i)?"":Convert.ToString(r.GetValue(i));}
 }
}

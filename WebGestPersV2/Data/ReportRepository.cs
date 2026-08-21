using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebGestPersV2.Models;
namespace WebGestPersV2.Data
{
 public sealed class ReportRepository
 {
  public IList<MovimentoPersonaleItem> PersonaleEntrato(int mesi) { return Leggi(@"SELECT ep.IDPersonale,ep.Cognome,ep.Nome,ep.Militare,ep.Stato_Servizio,CASE WHEN ep.Militare=1 THEN g.SiglaGrado ELSE t.Sigla_titolo END,ep.Data_Ass_Armaereo FROM dbo.ElencoPersonale ep LEFT JOIN dbo.Profilo_militare pm ON pm.IDPersonale=ep.IDPersonale AND ep.Militare=1 LEFT JOIN dbo.Gradi g ON g.ID_Grado=pm.ID_Grado LEFT JOIN dbo.PersCivile pc ON pc.IDPersonale=ep.IDPersonale AND ep.Militare=0 LEFT JOIN dbo.Titoli t ON t.ID_Titolo=pc.ID_TitoloAtt WHERE ep.Data_Ass_Armaereo>=DATEADD(MONTH,-@mesi,GETDATE()) ORDER BY ep.Data_Ass_Armaereo DESC,ep.Cognome,ep.Nome",mesi); }
  public IList<MovimentoPersonaleItem> PersonaleUscito(int mesi) { return Leggi(@"SELECT ep.IDPersonale,ep.Cognome,ep.Nome,ep.Militare,ep.Stato_Servizio,CASE WHEN ep.Militare=1 THEN g.SiglaGrado ELSE t.Sigla_titolo END,ep.Data_Usc_Armaereo FROM dbo.ElencoPersonale ep LEFT JOIN dbo.Profilo_militare pm ON pm.IDPersonale=ep.IDPersonale AND ep.Militare=1 LEFT JOIN dbo.Gradi g ON g.ID_Grado=pm.ID_Grado LEFT JOIN dbo.PersCivile pc ON pc.IDPersonale=ep.IDPersonale AND ep.Militare=0 LEFT JOIN dbo.Titoli t ON t.ID_Titolo=pc.ID_TitoloAtt WHERE ep.Data_Usc_Armaereo>=DATEADD(MONTH,-@mesi,GETDATE()) ORDER BY ep.Data_Usc_Armaereo DESC,ep.Cognome,ep.Nome",mesi); }
  private static IList<MovimentoPersonaleItem> Leggi(string sql,int mesi)
  {
   if(mesi!=3&&mesi!=6&&mesi!=12&&mesi!=24&&mesi!=36)throw new InvalidOperationException("Intervallo temporale non valido.");
   var x=new List<MovimentoPersonaleItem>(); using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c)){q.Parameters.Add("@mesi",SqlDbType.Int).Value=mesi;c.Open();using(var r=q.ExecuteReader())while(r.Read())x.Add(new MovimentoPersonaleItem{IdPersonale=r.GetInt32(0),Cognome=T(r,1),Nome=T(r,2),TipoPersonale=r.GetBoolean(3)?"Militare":"Civile",StatoServizio=T(r,4),GradoProfilo=T(r,5),DataEvento=r.GetDateTime(6)});}return x;
  }
  private static string T(SqlDataReader r,int i){return r.IsDBNull(i)?"":Convert.ToString(r.GetValue(i)).Trim();}
 }
}

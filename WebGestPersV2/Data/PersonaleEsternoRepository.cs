using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebGestPersV2.Configuration;
using WebGestPersV2.Infrastructure;
using WebGestPersV2.Models;

namespace WebGestPersV2.Data
{
    public sealed class PersonaleEsternoRepository
    {
        public IList<PersonaleEsternoItem> Elenca(string ricerca, bool includiNonAttivi)
        {
            const string sql = @"SELECT pe.id_pe, pe.Cognome, pe.Nome, pe.CodiceFiscale,
 d.nomeditta, pe.telefono, pe.stanza, ISNULL(pe.attivo,1),
 STUFF((SELECT '; ' + cp.Denominazione FROM dbo.PE_Cassette_Postali cp
        WHERE cp.id_pe=pe.id_pe ORDER BY cp.id_cp FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,2,'')
FROM dbo.PE_ElencoPersonaleEsterno pe
LEFT JOIN dbo.PE_dittaPersonaleEsterno d ON d.idpeditta=pe.id_dittaesterna
WHERE (@Tutti=1 OR ISNULL(pe.attivo,1)=1)
AND (@Ricerca='' OR pe.Cognome LIKE @Filtro OR pe.Nome LIKE @Filtro OR pe.CodiceFiscale LIKE @Filtro OR d.nomeditta LIKE @Filtro)
ORDER BY pe.Cognome, pe.Nome";
            var elenco = new List<PersonaleEsternoItem>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                string valore = (ricerca ?? string.Empty).Trim();
                command.Parameters.Add("@Tutti", SqlDbType.Bit).Value = includiNonAttivi;
                command.Parameters.Add("@Ricerca", SqlDbType.NVarChar, 100).Value = valore;
                command.Parameters.Add("@Filtro", SqlDbType.NVarChar, 104).Value = "%" + valore + "%";
                connection.Open();
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) elenco.Add(new PersonaleEsternoItem {
                        Id=reader.GetInt32(0), Cognome=Testo(reader,1), Nome=Testo(reader,2), CodiceFiscale=Testo(reader,3),
                        Ditta=Testo(reader,4), Telefono=Testo(reader,5), Stanza=Testo(reader,6), Attivo=reader.GetBoolean(7), Email=Testo(reader,8)
                    });
            }
            return elenco;
        }

        public PersonaleEsternoItem Trova(int id)
        {
            const string sql = @"SELECT pe.id_pe,pe.Cognome,pe.Nome,pe.CodiceFiscale,pe.telefono,pe.stanza,ISNULL(pe.attivo,1),pe.id_dittaesterna,d.nomeditta,
i.id_uff1liv,l1.SgUff1,i.id_uff2liv,l2.SgUff2,i.id_uff3liv,l3.SgUff3
FROM dbo.PE_ElencoPersonaleEsterno pe
LEFT JOIN dbo.PE_dittaPersonaleEsterno d ON d.idpeditta=pe.id_dittaesterna
LEFT JOIN dbo.PE_Incarico i ON i.id_pe=pe.id_pe
LEFT JOIN dbo.Liv1Uff l1 ON l1.ID_Uff1=i.id_uff1liv
LEFT JOIN dbo.Liv2Uff l2 ON l2.ID_Uff2=i.id_uff2liv
LEFT JOIN dbo.Liv3Uff l3 ON l3.ID_Uff3=i.id_uff3liv WHERE pe.id_pe=@Id";
            using(var connection=new SqlConnection(Db.ConnectionString)) using(var command=new SqlCommand(sql,connection))
            { command.Parameters.Add("@Id",SqlDbType.Int).Value=id; connection.Open(); using(var reader=command.ExecuteReader(CommandBehavior.SingleRow))
              { if(!reader.Read())return null; return new PersonaleEsternoItem { Id=reader.GetInt32(0),Cognome=Testo(reader,1),Nome=Testo(reader,2),CodiceFiscale=Testo(reader,3),Telefono=Testo(reader,4),Stanza=Testo(reader,5),Attivo=reader.GetBoolean(6),IdDitta=Intero(reader,7),Ditta=Testo(reader,8),IdUfficio1=Intero(reader,9),Ufficio1=Testo(reader,10),IdUfficio2=Intero(reader,11),Ufficio2=Testo(reader,12),IdUfficio3=Intero(reader,13),Ufficio3=Testo(reader,14)}; } }
        }

        public IList<LookupItem> Ditte(){return Lookup("SELECT idpeditta,nomeditta FROM dbo.PE_dittaPersonaleEsterno ORDER BY nomeditta",null);}
        public IList<LookupItem> Uffici1(){return Lookup("SELECT ID_Uff1,SgUff1 FROM dbo.Liv1Uff ORDER BY SgUff1",null);}
        public IList<LookupItem> Uffici2(int id){return Lookup("SELECT ID_Uff2,SgUff2 FROM dbo.Liv2Uff WHERE ID_Uff1=@Id ORDER BY SgUff2",id);}
        public IList<LookupItem> Uffici3(int id){return Lookup("SELECT ID_Uff3,SgUff3 FROM dbo.Liv3Uff WHERE ID_Uff2=@Id ORDER BY SgUff3",id);}

        public int Inserisci(PersonaleEsternoRequest dati,string utente)
        {
            Valida(dati); using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { int id; const string sql=@"INSERT dbo.PE_ElencoPersonaleEsterno(Cognome,Nome,CodiceFiscale,id_dittaesterna,telefono,stanza,attivo) VALUES(@Cognome,@Nome,@Cf,@Ditta,@Telefono,@Stanza,1); SELECT CAST(SCOPE_IDENTITY() AS int);";
              using(var command=new SqlCommand(sql,connection,transaction)){ParametriAnagrafica(command,dati);id=(int)command.ExecuteScalar();}
              SalvaIncarico(connection,transaction,id,dati); RegistraCreazione(connection,transaction,id,dati,utente); transaction.Commit(); CrudLogger.Info("CREATE","PE_ElencoPersonaleEsterno","id_pe="+id); return id;
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("CREATE","PE_ElencoPersonaleEsterno",dati.Cognome+" "+dati.Nome,ex);throw;}}
        }

        public void Aggiorna(PersonaleEsternoRequest dati,string utente)
        {
            Valida(dati); PersonaleEsternoItem prima=Trova(dati.Id);if(prima==null)throw new InvalidOperationException("Persona esterna non trovata.");
            using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { const string sql=@"UPDATE dbo.PE_ElencoPersonaleEsterno SET Cognome=@Cognome,Nome=@Nome,CodiceFiscale=@Cf,id_dittaesterna=@Ditta,telefono=@Telefono,stanza=@Stanza,attivo=@Attivo WHERE id_pe=@Id";
              using(var command=new SqlCommand(sql,connection,transaction)){ParametriAnagrafica(command,dati);command.Parameters.Add("@Attivo",SqlDbType.Bit).Value=dati.Attivo;command.Parameters.Add("@Id",SqlDbType.Int).Value=dati.Id;if(command.ExecuteNonQuery()!=1)throw new InvalidOperationException("Aggiornamento non eseguito.");}
              SalvaIncarico(connection,transaction,dati.Id,dati); RegistraDifferenze(connection,transaction,prima,dati,utente); transaction.Commit(); CrudLogger.Info("UPDATE","PE_ElencoPersonaleEsterno","id_pe="+dati.Id);
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("UPDATE","PE_ElencoPersonaleEsterno","id_pe="+dati.Id,ex);throw;}}
        }

        public void Disattiva(int id,string utente)
        {
            using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { using(var command=new SqlCommand("UPDATE dbo.PE_ElencoPersonaleEsterno SET attivo=0 WHERE id_pe=@Id",connection,transaction)){command.Parameters.Add("@Id",SqlDbType.Int).Value=id;if(command.ExecuteNonQuery()!=1)throw new InvalidOperationException("Persona esterna non trovata.");}
              Storico(connection,transaction,id,"attivo","1","0",utente);transaction.Commit();CrudLogger.Info("DELETE","PE_ElencoPersonaleEsterno","disattivazione id_pe="+id);
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("DELETE","PE_ElencoPersonaleEsterno","id_pe="+id,ex);throw;}}
        }

        private static void SalvaIncarico(SqlConnection c,SqlTransaction t,int id,PersonaleEsternoRequest d)
        { const string sql=@"IF EXISTS(SELECT 1 FROM dbo.PE_Incarico WHERE id_pe=@Id) UPDATE dbo.PE_Incarico SET id_uff1liv=@U1,id_uff2liv=@U2,id_uff3liv=@U3 WHERE id_pe=@Id ELSE INSERT dbo.PE_Incarico(id_pe,id_uff1liv,id_uff2liv,id_uff3liv) VALUES(@Id,@U1,@U2,@U3)";
          using(var command=new SqlCommand(sql,c,t)){command.Parameters.Add("@Id",SqlDbType.Int).Value=id;command.Parameters.Add("@U1",SqlDbType.Int).Value=d.IdUfficio1??AppConfig.UfficioLivello1Vuoto;command.Parameters.Add("@U2",SqlDbType.Int).Value=d.IdUfficio2??AppConfig.UfficioLivello2Vuoto;command.Parameters.Add("@U3",SqlDbType.Int).Value=d.IdUfficio3??AppConfig.UfficioLivello3Vuoto;command.ExecuteNonQuery();}}

        private static void RegistraCreazione(SqlConnection c,SqlTransaction t,int id,PersonaleEsternoRequest d,string u)
        { Storico(c,t,id,"cognome",null,d.Cognome,u);Storico(c,t,id,"nome",null,d.Nome,u);Storico(c,t,id,"codicefiscale",null,d.CodiceFiscale,u);Storico(c,t,id,"id_dittaesterna",null,d.IdDitta.ToString(),u);Storico(c,t,id,"attivo",null,"1",u);Storico(c,t,id,"id_uff1liv",null,(d.IdUfficio1??AppConfig.UfficioLivello1Vuoto).ToString(),u);Storico(c,t,id,"id_uff2liv",null,(d.IdUfficio2??AppConfig.UfficioLivello2Vuoto).ToString(),u);Storico(c,t,id,"id_uff3liv",null,(d.IdUfficio3??AppConfig.UfficioLivello3Vuoto).ToString(),u);}
        private static void RegistraDifferenze(SqlConnection c,SqlTransaction t,PersonaleEsternoItem p,PersonaleEsternoRequest d,string u)
        { Differenza(c,t,d.Id,"cognome",p.Cognome,d.Cognome,u);Differenza(c,t,d.Id,"nome",p.Nome,d.Nome,u);Differenza(c,t,d.Id,"codicefiscale",p.CodiceFiscale,d.CodiceFiscale,u);Differenza(c,t,d.Id,"telefono",p.Telefono,d.Telefono,u);Differenza(c,t,d.Id,"stanza",p.Stanza,d.Stanza,u);Differenza(c,t,d.Id,"id_dittaesterna",Val(p.IdDitta),d.IdDitta.ToString(),u);Differenza(c,t,d.Id,"attivo",p.Attivo?"1":"0",d.Attivo?"1":"0",u);Differenza(c,t,d.Id,"id_uff1liv",Val(p.IdUfficio1),(d.IdUfficio1??AppConfig.UfficioLivello1Vuoto).ToString(),u);Differenza(c,t,d.Id,"id_uff2liv",Val(p.IdUfficio2),(d.IdUfficio2??AppConfig.UfficioLivello2Vuoto).ToString(),u);Differenza(c,t,d.Id,"id_uff3liv",Val(p.IdUfficio3),(d.IdUfficio3??AppConfig.UfficioLivello3Vuoto).ToString(),u);}
        private static void Differenza(SqlConnection c,SqlTransaction t,int id,string campo,string prima,string dopo,string u){if(!string.Equals(prima??"",dopo??"",StringComparison.Ordinal))Storico(c,t,id,campo,prima,dopo,u);}
        private static void Storico(SqlConnection c,SqlTransaction t,int id,string campo,string prima,string dopo,string utente)
        { const string sql="INSERT dbo.PE_StoricoModifiche(ID_Pe,Campo_Variato,Data_Modifica,Utente_Modificatore,Valore_Vecchio,Valore_Nuovo) VALUES(@Id,@Campo,GETDATE(),@Utente,@Prima,@Dopo)";using(var command=new SqlCommand(sql,c,t)){command.Parameters.Add("@Id",SqlDbType.Int).Value=id;command.Parameters.Add("@Campo",SqlDbType.NVarChar,100).Value=campo;command.Parameters.Add("@Utente",SqlDbType.NVarChar,100).Value=(object)utente??DBNull.Value;command.Parameters.Add("@Prima",SqlDbType.NVarChar,-1).Value=(object)prima??DBNull.Value;command.Parameters.Add("@Dopo",SqlDbType.NVarChar,-1).Value=(object)dopo??DBNull.Value;command.ExecuteNonQuery();}}
        private static void ParametriAnagrafica(SqlCommand c,PersonaleEsternoRequest d){c.Parameters.Add("@Cognome",SqlDbType.NVarChar,100).Value=d.Cognome;c.Parameters.Add("@Nome",SqlDbType.NVarChar,100).Value=d.Nome;c.Parameters.Add("@Cf",SqlDbType.NVarChar,50).Value=(object)d.CodiceFiscale??"";c.Parameters.Add("@Ditta",SqlDbType.Int).Value=d.IdDitta;c.Parameters.Add("@Telefono",SqlDbType.NVarChar,20).Value=(object)d.Telefono??"";c.Parameters.Add("@Stanza",SqlDbType.NVarChar,20).Value=(object)d.Stanza??"";}
        private IList<LookupItem> Lookup(string sql,int? id){var x=new List<LookupItem>();using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c)){if(id.HasValue)q.Parameters.Add("@Id",SqlDbType.Int).Value=id.Value;c.Open();using(var r=q.ExecuteReader())while(r.Read())x.Add(new LookupItem{Value=r.GetInt32(0).ToString(),Text=Testo(r,1)});}return x;}
        private static void Valida(PersonaleEsternoRequest d){if(d==null)throw new ArgumentNullException("dati");if(string.IsNullOrWhiteSpace(d.Cognome)||string.IsNullOrWhiteSpace(d.Nome))throw new InvalidOperationException("Nome e cognome sono obbligatori.");if(d.IdDitta<=0)throw new InvalidOperationException("Selezionare la ditta.");}
        private static string Testo(SqlDataReader r,int i){return r.IsDBNull(i)?string.Empty:Convert.ToString(r.GetValue(i));}
        private static int? Intero(SqlDataReader r,int i){return r.IsDBNull(i)?(int?)null:Convert.ToInt32(r.GetValue(i));}
        private static string Val(int? x){return x.HasValue?x.Value.ToString():string.Empty;}
    }
}

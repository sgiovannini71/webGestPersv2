using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using WebGestPersV2.Infrastructure;
using WebGestPersV2.Models;
using WebGestPersV2.Configuration;

namespace WebGestPersV2.Data
{
    public sealed class CasellePostaliRepository
    {
        private static readonly string[] TipiConsentiti = { "personale", "funzionale", "funzionale (sec)", "condivisa" };

        public IList<CasellaPostaleItem> Elenca(int idPersonale)
        {
            const string sql = @"SELECT id_cp, IDPersonale, Denominazione, tipo
                                 FROM dbo.Cassette_Postali
                                 WHERE IDPersonale=@IdPersonale
                                 ORDER BY CASE tipo WHEN 'personale' THEN 0 WHEN 'funzionale' THEN 1 WHEN 'funzionale (sec)' THEN 2 ELSE 3 END, Denominazione";
            var risultato = new List<CasellaPostaleItem>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@IdPersonale", SqlDbType.Int).Value = idPersonale;
                connection.Open();
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) risultato.Add(new CasellaPostaleItem {
                        Id = reader.GetInt32(0), IdPersonale = reader.GetInt32(1),
                        Denominazione = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Tipo = reader.IsDBNull(3) ? "" : reader.GetString(3)
                    });
            }
            return risultato;
        }

        public CasellaPostaleItem Trova(int idCasella, int idPersonale)
        {
            const string sql = "SELECT id_cp,IDPersonale,Denominazione,tipo FROM dbo.Cassette_Postali WHERE id_cp=@IdCasella AND IDPersonale=@IdPersonale";
            using(var connection=new SqlConnection(Db.ConnectionString)) using(var command=new SqlCommand(sql,connection))
            { command.Parameters.Add("@IdCasella",SqlDbType.Int).Value=idCasella;command.Parameters.Add("@IdPersonale",SqlDbType.Int).Value=idPersonale;connection.Open();using(var reader=command.ExecuteReader(CommandBehavior.SingleRow))
              { if(!reader.Read())return null;return new CasellaPostaleItem{Id=reader.GetInt32(0),IdPersonale=reader.GetInt32(1),Denominazione=reader.GetString(2),Tipo=reader.GetString(3)}; } }
        }

        public IList<CasellaPostaleItem> CercaAssegnazioni(string denominazione, int idPersonaleDaEscludere)
        {
            denominazione = NormalizzaEmail(denominazione);
            const string sql = @"SELECT c.id_cp,c.IDPersonale,c.Denominazione,c.tipo,p.Cognome,p.Nome
FROM dbo.Cassette_Postali c INNER JOIN dbo.ElencoPersonale p ON p.IDPersonale=c.IDPersonale
WHERE LOWER(LTRIM(RTRIM(c.Denominazione)))=@Email AND c.IDPersonale<>@Escluso
ORDER BY p.Cognome,p.Nome,c.tipo";
            var risultato=new List<CasellaPostaleItem>();
            using(var connection=new SqlConnection(Db.ConnectionString))using(var command=new SqlCommand(sql,connection))
            { command.Parameters.Add("@Email",SqlDbType.VarChar,255).Value=denominazione;command.Parameters.Add("@Escluso",SqlDbType.Int).Value=idPersonaleDaEscludere;connection.Open();using(var reader=command.ExecuteReader())while(reader.Read())risultato.Add(new CasellaPostaleItem{Id=reader.GetInt32(0),IdPersonale=reader.GetInt32(1),Denominazione=reader.GetString(2),Tipo=reader.GetString(3),Cognome=reader.GetString(4),Nome=reader.GetString(5)}); }
            return risultato;
        }

        public int Inserisci(int idPersonale, string denominazione, string tipo, string utente, bool trasferisci)
        {
            denominazione=NormalizzaEmail(denominazione);tipo=DeterminaTipoCasella(denominazione,tipo);VerificaPersona(idPersonale);
            using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { VerificaDuplicato(connection,transaction,idPersonale,denominazione,tipo,null);
              IList<CasellaPostaleItem> assegnazioni=CercaAssegnazioni(connection,transaction,denominazione,idPersonale);
              if(assegnazioni.Count>0&&!trasferisci)throw new InvalidOperationException("La casella è già assegnata. Utilizzare Verifica disponibilità e confermare il trasferimento.");
              if(assegnazioni.Count>0)RimuoviAssegnazioniPrecedenti(connection,transaction,assegnazioni,denominazione,utente);
              int id;using(var command=new SqlCommand("INSERT dbo.Cassette_Postali(IDPersonale,Denominazione,tipo) VALUES(@Persona,@Email,@Tipo);SELECT CAST(SCOPE_IDENTITY() AS int);",connection,transaction)){Parametri(command,idPersonale,denominazione,tipo);id=(int)command.ExecuteScalar();}
              Storico(connection,transaction,idPersonale,trasferisci&&assegnazioni.Count>0?"Casella postale trasferita":"Casella postale inserita","",Descrivi(denominazione,tipo),utente);transaction.Commit();CrudLogger.Info(trasferisci&&assegnazioni.Count>0?"TRANSFER":"CREATE","Cassette_Postali","id_cp="+id+"; IDPersonale="+idPersonale+"; precedenti="+assegnazioni.Count);return id;
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("CREATE","Cassette_Postali","IDPersonale="+idPersonale,ex);throw;}}
        }

        public void Aggiorna(int idCasella,int idPersonale,string denominazione,string tipo,string utente)
        {
            denominazione=NormalizzaEmail(denominazione);tipo=DeterminaTipoCasella(denominazione,tipo);CasellaPostaleItem prima=Trova(idCasella,idPersonale);if(prima==null)throw new InvalidOperationException("Casella postale non trovata.");
            if(string.Equals(prima.Denominazione.Trim(),denominazione,StringComparison.OrdinalIgnoreCase)&&string.Equals(prima.Tipo.Trim(),tipo,StringComparison.OrdinalIgnoreCase))return;
            using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { VerificaDuplicato(connection,transaction,idPersonale,denominazione,tipo,idCasella);using(var command=new SqlCommand("UPDATE dbo.Cassette_Postali SET Denominazione=@Email,tipo=@Tipo WHERE id_cp=@IdCasella AND IDPersonale=@Persona",connection,transaction)){Parametri(command,idPersonale,denominazione,tipo);command.Parameters.Add("@IdCasella",SqlDbType.Int).Value=idCasella;if(command.ExecuteNonQuery()!=1)throw new InvalidOperationException("Aggiornamento non eseguito.");}
              Storico(connection,transaction,idPersonale,"Casella postale modificata",Descrivi(prima.Denominazione,prima.Tipo),Descrivi(denominazione,tipo),utente);transaction.Commit();CrudLogger.Info("UPDATE","Cassette_Postali","id_cp="+idCasella+"; IDPersonale="+idPersonale);
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("UPDATE","Cassette_Postali","id_cp="+idCasella,ex);throw;}}
        }

        public void Elimina(int idCasella,int idPersonale,string utente)
        {
            CasellaPostaleItem prima=Trova(idCasella,idPersonale);if(prima==null)throw new InvalidOperationException("Casella postale non trovata.");
            using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { using(var command=new SqlCommand("DELETE dbo.Cassette_Postali WHERE id_cp=@IdCasella AND IDPersonale=@Persona",connection,transaction)){command.Parameters.Add("@IdCasella",SqlDbType.Int).Value=idCasella;command.Parameters.Add("@Persona",SqlDbType.Int).Value=idPersonale;if(command.ExecuteNonQuery()!=1)throw new InvalidOperationException("Eliminazione non eseguita.");}
              Storico(connection,transaction,idPersonale,"Casella postale eliminata",Descrivi(prima.Denominazione,prima.Tipo),"",utente);transaction.Commit();CrudLogger.Info("DELETE","Cassette_Postali","id_cp="+idCasella+"; IDPersonale="+idPersonale);
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("DELETE","Cassette_Postali","id_cp="+idCasella,ex);throw;}}
        }

        private static string NormalizzaEmail(string valore)
        { valore=(valore??"").Trim().ToLowerInvariant();if(valore.Length==0)throw new InvalidOperationException("L'indirizzo e-mail è obbligatorio.");if(valore.Length>255)throw new InvalidOperationException("L'indirizzo e-mail è troppo lungo.");try{var x=new MailAddress(valore);if(!string.Equals(x.Address,valore,StringComparison.OrdinalIgnoreCase))throw new FormatException();}catch{throw new InvalidOperationException("Il formato dell'indirizzo e-mail non è valido.");}return valore; }
        private static string NormalizzaTipo(string valore){valore=(valore??"").Trim().ToLowerInvariant();if(!TipiConsentiti.Contains(valore,StringComparer.Ordinal))throw new InvalidOperationException("Il tipo di casella postale non è valido.");return valore;}
        public string DeterminaTipoCasella(string denominazione,string tipoSelezionato)
        {
            string email=NormalizzaEmail(denominazione);string dominio=new MailAddress(email).Host.Trim().TrimEnd('.').ToLowerInvariant();
            if(string.Equals(dominio,AppConfig.DominioPostaFunzionale,StringComparison.OrdinalIgnoreCase))return "funzionale";
            if(string.Equals(dominio,AppConfig.DominioPostaPersonale,StringComparison.OrdinalIgnoreCase))return "personale";
            return NormalizzaTipo(tipoSelezionato);
        }
        private static void VerificaPersona(int id){using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand("SELECT COUNT(1) FROM dbo.ElencoPersonale WHERE IDPersonale=@Id",c)){q.Parameters.Add("@Id",SqlDbType.Int).Value=id;c.Open();if((int)q.ExecuteScalar()!=1)throw new InvalidOperationException("Persona non trovata.");}}
        private static void VerificaDuplicato(SqlConnection c,SqlTransaction t,int persona,string email,string tipo,int? esclusa){using(var q=new SqlCommand(@"SELECT COUNT(1) FROM dbo.Cassette_Postali WHERE IDPersonale=@Persona AND LOWER(LTRIM(RTRIM(Denominazione)))=@Email AND LOWER(LTRIM(RTRIM(tipo)))=@Tipo AND (@Esclusa IS NULL OR id_cp<>@Esclusa)",c,t)){q.Parameters.Add("@Persona",SqlDbType.Int).Value=persona;q.Parameters.Add("@Email",SqlDbType.NVarChar,255).Value=email;q.Parameters.Add("@Tipo",SqlDbType.NVarChar,50).Value=tipo;q.Parameters.Add("@Esclusa",SqlDbType.Int).Value=(object)esclusa??DBNull.Value;if((int)q.ExecuteScalar()>0)throw new InvalidOperationException("La stessa casella e lo stesso tipo sono già associati a questa persona.");}}
        private static IList<CasellaPostaleItem> CercaAssegnazioni(SqlConnection c,SqlTransaction t,string email,int escluso){const string sql=@"SELECT c.id_cp,c.IDPersonale,c.Denominazione,c.tipo,p.Cognome,p.Nome FROM dbo.Cassette_Postali c INNER JOIN dbo.ElencoPersonale p ON p.IDPersonale=c.IDPersonale WHERE LOWER(LTRIM(RTRIM(c.Denominazione)))=@Email AND c.IDPersonale<>@Escluso";var x=new List<CasellaPostaleItem>();using(var q=new SqlCommand(sql,c,t)){q.Parameters.Add("@Email",SqlDbType.VarChar,255).Value=email;q.Parameters.Add("@Escluso",SqlDbType.Int).Value=escluso;using(var r=q.ExecuteReader())while(r.Read())x.Add(new CasellaPostaleItem{Id=r.GetInt32(0),IdPersonale=r.GetInt32(1),Denominazione=r.GetString(2),Tipo=r.GetString(3),Cognome=r.GetString(4),Nome=r.GetString(5)});}return x;}
        private static void RimuoviAssegnazioniPrecedenti(SqlConnection c,SqlTransaction t,IList<CasellaPostaleItem> assegnazioni,string email,string utente){foreach(var x in assegnazioni){using(var q=new SqlCommand("DELETE dbo.Cassette_Postali WHERE id_cp=@Id AND IDPersonale=@Persona",c,t)){q.Parameters.Add("@Id",SqlDbType.Int).Value=x.Id;q.Parameters.Add("@Persona",SqlDbType.Int).Value=x.IdPersonale;if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Non è stato possibile rimuovere una precedente associazione della casella.");}Storico(c,t,x.IdPersonale,"Casella postale trasferita ad altro utente",Descrivi(email,x.Tipo),"",utente);}}
        private static void Parametri(SqlCommand q,int persona,string email,string tipo){q.Parameters.Add("@Persona",SqlDbType.Int).Value=persona;q.Parameters.Add("@Email",SqlDbType.NVarChar,255).Value=email;q.Parameters.Add("@Tipo",SqlDbType.NVarChar,50).Value=tipo;}
        private static void Storico(SqlConnection c,SqlTransaction t,int persona,string campo,string prima,string dopo,string utente){const string sql="INSERT dbo.StoricoModifiche(IDPersonale,Campo_Variato,Data_Modifica,Utente_Modificatore,Valore_Vecchio,Valore_Nuovo) VALUES(@Persona,@Campo,GETDATE(),@Utente,@Prima,@Dopo)";using(var q=new SqlCommand(sql,c,t)){q.Parameters.Add("@Persona",SqlDbType.Int).Value=persona;q.Parameters.Add("@Campo",SqlDbType.VarChar,255).Value=campo;q.Parameters.Add("@Utente",SqlDbType.VarChar,255).Value=(object)utente??"";q.Parameters.Add("@Prima",SqlDbType.VarChar,255).Value=Limita(prima);q.Parameters.Add("@Dopo",SqlDbType.VarChar,255).Value=Limita(dopo);q.ExecuteNonQuery();}}
        private static string Descrivi(string email,string tipo){return (email??"")+" ["+(tipo??"")+"]";}private static string Limita(string x){x=x??"";return x.Length<=255?x:x.Substring(0,255);}
    }
}

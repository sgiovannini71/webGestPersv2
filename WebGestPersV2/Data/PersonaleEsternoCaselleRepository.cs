using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using WebGestPersV2.Configuration;
using WebGestPersV2.Infrastructure;
using WebGestPersV2.Models;

namespace WebGestPersV2.Data
{
    /// <summary>
    /// CRUD delle caselle di posta del personale esterno (dbo.PE_Cassette_Postali).
    /// Rispecchia il comportamento di <see cref="CasellePostaliRepository"/> per il personale
    /// interno, ma opera esclusivamente sulle tabelle PE_ e sul relativo storico.
    /// </summary>
    public sealed class PersonaleEsternoCaselleRepository
    {
        private static readonly string[] TipiConsentiti = { "personale", "funzionale", "funzionale (sec)", "condivisa" };

        public IList<CasellaPostaleItem> Elenca(int idPe)
        {
            const string sql = @"SELECT id_cp, id_pe, Denominazione, ISNULL(tipo,'')
                                 FROM dbo.PE_Cassette_Postali
                                 WHERE id_pe=@IdPe
                                 ORDER BY CASE ISNULL(tipo,'') WHEN 'personale' THEN 0 WHEN 'funzionale' THEN 1 WHEN 'funzionale (sec)' THEN 2 ELSE 3 END, Denominazione";
            var risultato = new List<CasellaPostaleItem>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@IdPe", SqlDbType.Int).Value = idPe;
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

        public CasellaPostaleItem Trova(int idCasella, int idPe)
        {
            const string sql = "SELECT id_cp,id_pe,Denominazione,ISNULL(tipo,'') FROM dbo.PE_Cassette_Postali WHERE id_cp=@IdCasella AND id_pe=@IdPe";
            using(var connection=new SqlConnection(Db.ConnectionString)) using(var command=new SqlCommand(sql,connection))
            { command.Parameters.Add("@IdCasella",SqlDbType.Int).Value=idCasella;command.Parameters.Add("@IdPe",SqlDbType.Int).Value=idPe;connection.Open();using(var reader=command.ExecuteReader(CommandBehavior.SingleRow))
              { if(!reader.Read())return null;return new CasellaPostaleItem{Id=reader.GetInt32(0),IdPersonale=reader.GetInt32(1),Denominazione=reader.IsDBNull(2)?"":reader.GetString(2),Tipo=reader.IsDBNull(3)?"":reader.GetString(3)}; } }
        }

        public IList<CasellaPostaleItem> CercaAssegnazioni(string denominazione, int idPeDaEscludere)
        {
            denominazione = NormalizzaEmail(denominazione);
            const string sql = @"SELECT c.id_cp,c.id_pe,c.Denominazione,ISNULL(c.tipo,''),p.Cognome,p.Nome
FROM dbo.PE_Cassette_Postali c INNER JOIN dbo.PE_ElencoPersonaleEsterno p ON p.id_pe=c.id_pe
WHERE LOWER(LTRIM(RTRIM(c.Denominazione)))=@Email AND c.id_pe<>@Escluso
ORDER BY p.Cognome,p.Nome,ISNULL(c.tipo,'')";
            var risultato=new List<CasellaPostaleItem>();
            using(var connection=new SqlConnection(Db.ConnectionString))using(var command=new SqlCommand(sql,connection))
            { command.Parameters.Add("@Email",SqlDbType.VarChar,255).Value=denominazione;command.Parameters.Add("@Escluso",SqlDbType.Int).Value=idPeDaEscludere;connection.Open();using(var reader=command.ExecuteReader())while(reader.Read())risultato.Add(new CasellaPostaleItem{Id=reader.GetInt32(0),IdPersonale=reader.GetInt32(1),Denominazione=reader.GetString(2),Tipo=reader.GetString(3),Cognome=reader.GetString(4),Nome=reader.GetString(5)}); }
            return risultato;
        }

        public int Inserisci(int idPe, string denominazione, string tipo, string utente, bool trasferisci)
        {
            denominazione=NormalizzaEmail(denominazione);tipo=DeterminaTipoCasella(denominazione,tipo);VerificaPersona(idPe);
            using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { VerificaDuplicato(connection,transaction,idPe,denominazione,tipo,null);
              IList<CasellaPostaleItem> assegnazioni=tipo=="condivisa"
                  ? new List<CasellaPostaleItem>()
                  : CercaAssegnazioni(connection,transaction,denominazione,idPe);
              if(assegnazioni.Count>0&&!trasferisci)throw new InvalidOperationException("La casella è già assegnata a un'altra persona esterna. Utilizzare Verifica disponibilità e confermare il trasferimento.");
              if(assegnazioni.Count>0)RimuoviAssegnazioniPrecedenti(connection,transaction,assegnazioni,denominazione,utente);
              int id;using(var command=new SqlCommand("INSERT dbo.PE_Cassette_Postali(id_pe,Denominazione,tipo) VALUES(@Persona,@Email,@Tipo);SELECT CAST(SCOPE_IDENTITY() AS int);",connection,transaction)){Parametri(command,idPe,denominazione,tipo);object nuovo=command.ExecuteScalar();if(nuovo==null||nuovo==DBNull.Value)throw new InvalidOperationException("Identificativo della casella postale non generato: verificare che PE_Cassette_Postali.id_cp sia una colonna IDENTITY.");id=Convert.ToInt32(nuovo);}
              Storico(connection,transaction,idPe,trasferisci&&assegnazioni.Count>0?"Casella postale trasferita":"Casella postale inserita","",Descrivi(denominazione,tipo),utente);transaction.Commit();CrudLogger.Info(trasferisci&&assegnazioni.Count>0?"TRANSFER":"CREATE","PE_Cassette_Postali","id_cp="+id+"; id_pe="+idPe+"; precedenti="+assegnazioni.Count);return id;
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("CREATE","PE_Cassette_Postali","id_pe="+idPe,ex);throw;}}
        }

        public void Aggiorna(int idCasella,int idPe,string denominazione,string tipo,string utente)
        {
            denominazione=NormalizzaEmail(denominazione);tipo=DeterminaTipoCasella(denominazione,tipo);CasellaPostaleItem prima=Trova(idCasella,idPe);if(prima==null)throw new InvalidOperationException("Casella postale non trovata.");
            if(string.Equals((prima.Denominazione??"").Trim(),denominazione,StringComparison.OrdinalIgnoreCase)&&string.Equals((prima.Tipo??"").Trim(),tipo,StringComparison.OrdinalIgnoreCase))return;
            using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { VerificaDuplicato(connection,transaction,idPe,denominazione,tipo,idCasella);using(var command=new SqlCommand("UPDATE dbo.PE_Cassette_Postali SET Denominazione=@Email,tipo=@Tipo WHERE id_cp=@IdCasella AND id_pe=@Persona",connection,transaction)){Parametri(command,idPe,denominazione,tipo);command.Parameters.Add("@IdCasella",SqlDbType.Int).Value=idCasella;if(command.ExecuteNonQuery()!=1)throw new InvalidOperationException("Aggiornamento non eseguito.");}
              Storico(connection,transaction,idPe,"Casella postale modificata",Descrivi(prima.Denominazione,prima.Tipo),Descrivi(denominazione,tipo),utente);transaction.Commit();CrudLogger.Info("UPDATE","PE_Cassette_Postali","id_cp="+idCasella+"; id_pe="+idPe);
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("UPDATE","PE_Cassette_Postali","id_cp="+idCasella,ex);throw;}}
        }

        public void Elimina(int idCasella,int idPe,string utente)
        {
            CasellaPostaleItem prima=Trova(idCasella,idPe);if(prima==null)throw new InvalidOperationException("Casella postale non trovata.");
            using(var connection=new SqlConnection(Db.ConnectionString)){connection.Open();using(var transaction=connection.BeginTransaction())try
            { using(var command=new SqlCommand("DELETE dbo.PE_Cassette_Postali WHERE id_cp=@IdCasella AND id_pe=@Persona",connection,transaction)){command.Parameters.Add("@IdCasella",SqlDbType.Int).Value=idCasella;command.Parameters.Add("@Persona",SqlDbType.Int).Value=idPe;if(command.ExecuteNonQuery()!=1)throw new InvalidOperationException("Eliminazione non eseguita.");}
              Storico(connection,transaction,idPe,"Casella postale eliminata",Descrivi(prima.Denominazione,prima.Tipo),"",utente);transaction.Commit();CrudLogger.Info("DELETE","PE_Cassette_Postali","id_cp="+idCasella+"; id_pe="+idPe);
            }catch(Exception ex){transaction.Rollback();CrudLogger.Errore("DELETE","PE_Cassette_Postali","id_cp="+idCasella,ex);throw;}}
        }

        private static string NormalizzaEmail(string valore)
        { valore=(valore??"").Trim().ToLowerInvariant();if(valore.Length==0)throw new InvalidOperationException("L'indirizzo e-mail è obbligatorio.");if(valore.Length>255)throw new InvalidOperationException("L'indirizzo e-mail è troppo lungo.");try{var x=new MailAddress(valore);if(!string.Equals(x.Address,valore,StringComparison.OrdinalIgnoreCase))throw new FormatException();}catch{throw new InvalidOperationException("Il formato dell'indirizzo e-mail non è valido.");}return valore; }
        private static string NormalizzaTipo(string valore){valore=(valore??"").Trim().ToLowerInvariant();if(!TipiConsentiti.Contains(valore,StringComparer.Ordinal))throw new InvalidOperationException("Il tipo di casella postale non è valido.");return valore;}
        public string DeterminaTipoCasella(string denominazione,string tipoSelezionato)
        {
            string email=NormalizzaEmail(denominazione);string dominio=new MailAddress(email).Host.Trim().TrimEnd('.').ToLowerInvariant();
            if(string.Equals((tipoSelezionato??"").Trim(),"condivisa",StringComparison.OrdinalIgnoreCase))return "condivisa";
            if(string.Equals(dominio,AppConfig.DominioPostaFunzionale,StringComparison.OrdinalIgnoreCase))return "funzionale";
            if(string.Equals(dominio,AppConfig.DominioPostaPersonale,StringComparison.OrdinalIgnoreCase))return "personale";
            return NormalizzaTipo(tipoSelezionato);
        }
        private static void VerificaPersona(int id){using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand("SELECT COUNT(1) FROM dbo.PE_ElencoPersonaleEsterno WHERE id_pe=@Id",c)){q.Parameters.Add("@Id",SqlDbType.Int).Value=id;c.Open();if((int)q.ExecuteScalar()!=1)throw new InvalidOperationException("Persona esterna non trovata.");}}
        private static void VerificaDuplicato(SqlConnection c,SqlTransaction t,int persona,string email,string tipo,int? esclusa){using(var q=new SqlCommand(@"SELECT COUNT(1) FROM dbo.PE_Cassette_Postali WHERE id_pe=@Persona AND LOWER(LTRIM(RTRIM(Denominazione)))=@Email AND LOWER(LTRIM(RTRIM(ISNULL(tipo,''))))=@Tipo AND (@Esclusa IS NULL OR id_cp<>@Esclusa)",c,t)){q.Parameters.Add("@Persona",SqlDbType.Int).Value=persona;q.Parameters.Add("@Email",SqlDbType.NVarChar,255).Value=email;q.Parameters.Add("@Tipo",SqlDbType.NVarChar,50).Value=tipo;q.Parameters.Add("@Esclusa",SqlDbType.Int).Value=(object)esclusa??DBNull.Value;if((int)q.ExecuteScalar()>0)throw new InvalidOperationException("La stessa casella e lo stesso tipo sono già associati a questa persona.");}}
        private static IList<CasellaPostaleItem> CercaAssegnazioni(SqlConnection c,SqlTransaction t,string email,int escluso){const string sql=@"SELECT c.id_cp,c.id_pe,c.Denominazione,ISNULL(c.tipo,''),p.Cognome,p.Nome FROM dbo.PE_Cassette_Postali c INNER JOIN dbo.PE_ElencoPersonaleEsterno p ON p.id_pe=c.id_pe WHERE LOWER(LTRIM(RTRIM(c.Denominazione)))=@Email AND c.id_pe<>@Escluso";var x=new List<CasellaPostaleItem>();using(var q=new SqlCommand(sql,c,t)){q.Parameters.Add("@Email",SqlDbType.VarChar,255).Value=email;q.Parameters.Add("@Escluso",SqlDbType.Int).Value=escluso;using(var r=q.ExecuteReader())while(r.Read())x.Add(new CasellaPostaleItem{Id=r.GetInt32(0),IdPersonale=r.GetInt32(1),Denominazione=r.GetString(2),Tipo=r.GetString(3),Cognome=r.GetString(4),Nome=r.GetString(5)});}return x;}
        private static void RimuoviAssegnazioniPrecedenti(SqlConnection c,SqlTransaction t,IList<CasellaPostaleItem> assegnazioni,string email,string utente){foreach(var x in assegnazioni){using(var q=new SqlCommand("DELETE dbo.PE_Cassette_Postali WHERE id_cp=@Id AND id_pe=@Persona",c,t)){q.Parameters.Add("@Id",SqlDbType.Int).Value=x.Id;q.Parameters.Add("@Persona",SqlDbType.Int).Value=x.IdPersonale;if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Non è stato possibile rimuovere una precedente associazione della casella.");}Storico(c,t,x.IdPersonale,"Casella postale trasferita ad altro utente",Descrivi(email,x.Tipo),"",utente);}}
        private static void Parametri(SqlCommand q,int persona,string email,string tipo){q.Parameters.Add("@Persona",SqlDbType.Int).Value=persona;q.Parameters.Add("@Email",SqlDbType.NVarChar,255).Value=email;q.Parameters.Add("@Tipo",SqlDbType.NVarChar,50).Value=tipo;}
        private static void Storico(SqlConnection c,SqlTransaction t,int persona,string campo,string prima,string dopo,string utente)
        {
            const string sql="INSERT dbo.PE_StoricoModifiche(ID_Pe,Campo_Variato,Data_Modifica,Utente_Modificatore,Valore_Vecchio,Valore_Nuovo) VALUES(@Persona,@Campo,GETDATE(),@Utente,@Prima,@Dopo)";
            using(var q=new SqlCommand(sql,c,t))
            {
                q.Parameters.Add("@Persona",SqlDbType.Int).Value=persona;
                q.Parameters.Add("@Campo",SqlDbType.NVarChar,100).Value=campo;
                q.Parameters.Add("@Utente",SqlDbType.NVarChar,100).Value=(object)utente??DBNull.Value;
                q.Parameters.Add("@Prima",SqlDbType.NVarChar,-1).Value=prima??string.Empty;
                q.Parameters.Add("@Dopo",SqlDbType.NVarChar,-1).Value=dopo??string.Empty;
                q.ExecuteNonQuery();
            }
        }
        private static string Descrivi(string email,string tipo){return (email??"")+" ["+(tipo??"")+"]";}
    }
}

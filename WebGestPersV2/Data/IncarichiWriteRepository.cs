using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebGestPersV2.Configuration;
using WebGestPersV2.Infrastructure;
using WebGestPersV2.Models;

namespace WebGestPersV2.Data
{
    public sealed class IncarichiWriteRepository
    {
        public IList<LookupItem> TipiIncarico() { return Leggi("SELECT id_tipo_incarico, RTRIM(ISNULL(Sigla_incarico,'')) + CASE WHEN ISNULL(Descr_incarico,'')='' THEN '' ELSE ' - '+Descr_incarico END FROM dbo.Tipo_incarichi ORDER BY Descr_incarico"); }
        public IList<LookupItem> UfficiPrimoLivello() { return Leggi("SELECT ID_Uff1, RTRIM(ISNULL(SgUff1,'')) + CASE WHEN ISNULL(DescUff1,'')='' THEN '' ELSE ' - '+DescUff1 END FROM dbo.Liv1Uff ORDER BY SgUff1"); }
        public IList<LookupItem> UfficiSecondoLivello(int idUfficio1) { return Leggi("SELECT ID_Uff2, RTRIM(ISNULL(SgUff2,'')) + CASE WHEN ISNULL(DescUff2,'')='' THEN '' ELSE ' - '+DescUff2 END FROM dbo.Liv2Uff WHERE ID_Uff1=@id ORDER BY SgUff2", Parametro("@id", idUfficio1)); }
        public IList<LookupItem> UfficiTerzoLivello(int idUfficio1, int idUfficio2) { return Leggi("SELECT ID_Uff3, RTRIM(ISNULL(SgUff3,'')) + CASE WHEN ISNULL(DescUff3,'')='' THEN '' ELSE ' - '+DescUff3 END FROM dbo.Liv3Uff WHERE ID_Uff1=@id1 AND ID_Uff2=@id2 ORDER BY SgUff3", Parametro("@id1", idUfficio1), Parametro("@id2", idUfficio2)); }

        public IncaricoListaItem Trova(int idPersonale, int idIncarico)
        {
            foreach (IncaricoListaItem item in new PersonaleRepository().CercaIncarichi(idPersonale))
                if (item.IdIncarico == idIncarico) return item;
            return null;
        }

        public void Salva(ModificaIncaricoRequest dati, string utente)
        {
            using (var c = new SqlConnection(Db.ConnectionString))
            {
                c.Open();
                using (var tx = c.BeginTransaction())
                {
                    try
                    {
                        if (Conta(c, tx, "SELECT COUNT(*) FROM dbo.ElencoPersonale WITH (UPDLOCK) WHERE IDPersonale=@id AND Stato_Servizio='attivo'", Parametro("@id", dati.IdPersonale)) != 1)
                            throw new InvalidOperationException("La persona non è attiva o non esiste.");
                        if (Conta(c, tx, "SELECT COUNT(*) FROM dbo.Tipo_incarichi WHERE id_tipo_incarico=@id", Parametro("@id", dati.IdTipoIncarico)) != 1)
                            throw new InvalidOperationException("Selezionare un tipo di incarico valido.");
                        VerificaUffici(c, tx, dati);

                        string prima = "";
                        if (dati.IdIncarico.HasValue)
                        {
                            prima = Descrizione(c, tx, dati.IdPersonale, dati.IdIncarico.Value);
                            if (string.IsNullOrEmpty(prima)) throw new InvalidOperationException("Incarico non trovato.");
                        }
                        if (dati.Principale)
                            Esegui(c, tx, "UPDATE dbo.Incarichi SET principale=0 WHERE IDPersonale=@persona AND (@incarico IS NULL OR id_incarico<>@incarico)", Parametro("@persona", dati.IdPersonale), new SqlParameter("@incarico", SqlDbType.Int) { Value = dati.IdIncarico.HasValue ? (object)dati.IdIncarico.Value : DBNull.Value });

                        if (dati.IdIncarico.HasValue)
                        {
                            int righe = Esegui(c, tx, @"UPDATE dbo.Incarichi SET id_tipo_incarico=@tipo,Data_inizio=@data,principale=@principale,ID_Uff1=@u1,ID_Uff2=@u2,ID_Uff3=@u3
                                WHERE id_incarico=@incarico AND IDPersonale=@persona", Parametro("@tipo", dati.IdTipoIncarico), Data("@data", dati.DataInizio), Bit("@principale", dati.Principale), NullableInt("@u1", dati.IdUfficio1), NullableInt("@u2", dati.IdUfficio2), NullableInt("@u3", dati.IdUfficio3), Parametro("@incarico", dati.IdIncarico.Value), Parametro("@persona", dati.IdPersonale));
                            if (righe != 1) throw new InvalidOperationException("Incarico non trovato.");
                        }
                        else
                            Esegui(c, tx, "INSERT dbo.Incarichi(IDPersonale,id_tipo_incarico,Data_inizio,principale,ID_Uff1,ID_Uff2,ID_Uff3) VALUES(@persona,@tipo,@data,@principale,@u1,@u2,@u3)", Parametro("@persona", dati.IdPersonale), Parametro("@tipo", dati.IdTipoIncarico), Data("@data", dati.DataInizio), Bit("@principale", dati.Principale), NullableInt("@u1", dati.IdUfficio1), NullableInt("@u2", dati.IdUfficio2), NullableInt("@u3", dati.IdUfficio3));

                        string dopo = dati.IdTipoIncarico + "|" + DataTesto(dati.DataInizio) + "|" + dati.Principale + "|" + Valore(dati.IdUfficio1) + "|" + Valore(dati.IdUfficio2) + "|" + Valore(dati.IdUfficio3);
                        Esegui(c, tx, "INSERT dbo.StoricoModifiche(IDPersonale,Campo_Variato,Data_Modifica,Utente_Modificatore,Valore_Vecchio,Valore_Nuovo) VALUES(@persona,@campo,@ora,@utente,@prima,@dopo)", Parametro("@persona", dati.IdPersonale), Testo("@campo", dati.IdIncarico.HasValue ? "Modifica incarico" : "Inserimento incarico"), new SqlParameter("@ora", SqlDbType.DateTime) { Value = DateTime.Now }, Testo("@utente", utente), Testo("@prima", prima), Testo("@dopo", dopo));
                        tx.Commit();
                        CrudLogger.Info(dati.IdIncarico.HasValue ? "UPDATE" : "CREATE", "Incarico", "IDPersonale="+dati.IdPersonale+(dati.IdIncarico.HasValue?"; id_incarico="+dati.IdIncarico.Value:""));
                    }
                    catch(Exception ex) { tx.Rollback(); CrudLogger.Errore(dati.IdIncarico.HasValue ? "UPDATE" : "CREATE", "Incarico", "IDPersonale="+dati.IdPersonale, ex); throw; }
                }
            }
        }

        public void Chiudi(int idPersonale, int idIncarico, DateTime dataFine, string utente)
        {
            using (var c = new SqlConnection(Db.ConnectionString))
            {
                c.Open();
                using (var tx = c.BeginTransaction())
                {
                    try
                    {
                        string descrizione;
                        DateTime? dataInizio;
                        bool principale;
                        int idTipo;
                        using (var q = new SqlCommand(@"SELECT i.id_tipo_incarico,i.Data_inizio,i.principale,
                            LTRIM(RTRIM(RTRIM(ISNULL(ti.Sigla_incarico,''))+' '+RTRIM(ISNULL(u1.SgUff1,''))+' '+RTRIM(ISNULL(u2.SgUff2,''))+' '+RTRIM(ISNULL(u3.SgUff3,''))))
                            FROM dbo.Incarichi i WITH (UPDLOCK)
                            INNER JOIN dbo.ElencoPersonale ep ON ep.IDPersonale=i.IDPersonale AND ep.Stato_Servizio='attivo'
                            LEFT JOIN dbo.Tipo_incarichi ti ON ti.id_tipo_incarico=i.id_tipo_incarico
                            LEFT JOIN dbo.Liv1Uff u1 ON u1.ID_Uff1=i.ID_Uff1
                            LEFT JOIN dbo.Liv2Uff u2 ON u2.ID_Uff2=i.ID_Uff2
                            LEFT JOIN dbo.Liv3Uff u3 ON u3.ID_Uff3=i.ID_Uff3
                            WHERE i.IDPersonale=@persona AND i.id_incarico=@incarico", c, tx))
                        {
                            q.Parameters.Add(Parametro("@persona", idPersonale)); q.Parameters.Add(Parametro("@incarico", idIncarico));
                            using (var r = q.ExecuteReader(CommandBehavior.SingleRow))
                            {
                                if (!r.Read()) throw new InvalidOperationException("Incarico attivo non trovato.");
                                idTipo = r.GetInt32(0);
                                dataInizio = r.IsDBNull(1) ? (DateTime?)null : r.GetDateTime(1);
                                principale = !r.IsDBNull(2) && r.GetBoolean(2);
                                descrizione = r.IsDBNull(3) ? "" : Convert.ToString(r.GetValue(3));
                            }
                        }
                        if (idTipo == -1) throw new InvalidOperationException("L'incarico predefinito non può essere chiuso.");

                        Esegui(c, tx, @"INSERT dbo.StoricoIncarichi(IDPersonale,Incarico,principale,data_inizio,date_fine)
                            VALUES(@persona,@descrizione,@principale,@inizio,@fine)", Parametro("@persona", idPersonale),
                            new SqlParameter("@descrizione", SqlDbType.VarChar, 306) { Value = descrizione }, Bit("@principale", principale),
                            Data("@inizio", dataInizio), new SqlParameter("@fine", SqlDbType.DateTime) { Value = dataFine });
                        Esegui(c, tx, "DELETE dbo.Incarichi WHERE IDPersonale=@persona AND id_incarico=@incarico", Parametro("@persona", idPersonale), Parametro("@incarico", idIncarico));
                        if (Conta(c, tx, "SELECT COUNT(*) FROM dbo.Incarichi WHERE IDPersonale=@persona", Parametro("@persona", idPersonale)) == 0)
                            Esegui(c, tx, @"INSERT dbo.Incarichi(IDPersonale,id_tipo_incarico,Data_inizio,principale,ID_Uff1,ID_Uff2,ID_Uff3)
                                VALUES(@persona,-1,NULL,1,@u1,@u2,@u3)", Parametro("@persona", idPersonale),
                                Parametro("@u1", AppConfig.UfficioLivello1Vuoto), Parametro("@u2", AppConfig.UfficioLivello2Vuoto), Parametro("@u3", AppConfig.UfficioLivello3Vuoto));
                        Esegui(c, tx, "INSERT dbo.StoricoModifiche(IDPersonale,Campo_Variato,Data_Modifica,Utente_Modificatore,Valore_Vecchio,Valore_Nuovo) VALUES(@persona,'Incarico chiuso',@ora,@utente,@prima,@dopo)",
                            Parametro("@persona", idPersonale), new SqlParameter("@ora", SqlDbType.DateTime) { Value = DateTime.Now }, Testo("@utente", utente), Testo("@prima", descrizione), Testo("@dopo", "—"));
                        tx.Commit();
                        CrudLogger.Info("DELETE", "Incarico", "Chiusura e storicizzazione; IDPersonale="+idPersonale+"; id_incarico="+idIncarico);
                    }
                    catch(Exception ex) { tx.Rollback(); CrudLogger.Errore("DELETE", "Incarico", "IDPersonale="+idPersonale+"; id_incarico="+idIncarico, ex); throw; }
                }
            }
        }

        private static void VerificaUffici(SqlConnection c, SqlTransaction tx, ModificaIncaricoRequest d)
        {
            if (d.IdUfficio2.HasValue && !d.IdUfficio1.HasValue || d.IdUfficio3.HasValue && (!d.IdUfficio1.HasValue || !d.IdUfficio2.HasValue)) throw new InvalidOperationException("La gerarchia degli uffici non è completa.");
            if (d.IdUfficio1.HasValue && Conta(c, tx, "SELECT COUNT(*) FROM dbo.Liv1Uff WHERE ID_Uff1=@u1", Parametro("@u1", d.IdUfficio1.Value)) != 1) throw new InvalidOperationException("Ufficio di primo livello non valido.");
            if (d.IdUfficio2.HasValue && Conta(c, tx, "SELECT COUNT(*) FROM dbo.Liv2Uff WHERE ID_Uff1=@u1 AND ID_Uff2=@u2", Parametro("@u1", d.IdUfficio1.Value), Parametro("@u2", d.IdUfficio2.Value)) != 1) throw new InvalidOperationException("Ufficio di secondo livello non coerente.");
            if (d.IdUfficio3.HasValue && Conta(c, tx, "SELECT COUNT(*) FROM dbo.Liv3Uff WHERE ID_Uff1=@u1 AND ID_Uff2=@u2 AND ID_Uff3=@u3", Parametro("@u1", d.IdUfficio1.Value), Parametro("@u2", d.IdUfficio2.Value), Parametro("@u3", d.IdUfficio3.Value)) != 1) throw new InvalidOperationException("Ufficio di terzo livello non coerente.");
        }
        private static string Descrizione(SqlConnection c, SqlTransaction tx, int persona, int incarico) { using (var q = new SqlCommand("SELECT CONCAT(id_tipo_incarico,'|',CONVERT(varchar(30),Data_inizio,126),'|',principale,'|',ISNULL(ID_Uff1,''),'|',ISNULL(ID_Uff2,''),'|',ISNULL(ID_Uff3,'')) FROM dbo.Incarichi WITH (UPDLOCK) WHERE IDPersonale=@p AND id_incarico=@i", c, tx)) { q.Parameters.Add(Parametro("@p", persona)); q.Parameters.Add(Parametro("@i", incarico)); return Convert.ToString(q.ExecuteScalar()); } }
        private static IList<LookupItem> Leggi(string sql, params SqlParameter[] ps) { var x = new List<LookupItem>(); using (var c = new SqlConnection(Db.ConnectionString)) using (var q = new SqlCommand(sql, c)) { q.Parameters.AddRange(ps); c.Open(); using (var r = q.ExecuteReader()) while (r.Read()) x.Add(new LookupItem { Value = Convert.ToString(r.GetValue(0)), Text = Convert.ToString(r.GetValue(1)) }); } return x; }
        private static int Conta(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] ps) { using (var q = new SqlCommand(sql, c, tx)) { q.Parameters.AddRange(ps); return Convert.ToInt32(q.ExecuteScalar()); } }
        private static int Esegui(SqlConnection c, SqlTransaction tx, string sql, params SqlParameter[] ps) { using (var q = new SqlCommand(sql, c, tx)) { q.Parameters.AddRange(ps); return q.ExecuteNonQuery(); } }
        private static SqlParameter Parametro(string nome, int valore) { return new SqlParameter(nome, SqlDbType.Int) { Value = valore }; }
        private static SqlParameter NullableInt(string nome, int? valore) { return new SqlParameter(nome, SqlDbType.Int) { Value = valore.HasValue ? (object)valore.Value : DBNull.Value }; }
        private static SqlParameter Data(string nome, DateTime? valore) { return new SqlParameter(nome, SqlDbType.DateTime) { Value = valore.HasValue ? (object)valore.Value : DBNull.Value }; }
        private static SqlParameter Bit(string nome, bool valore) { return new SqlParameter(nome, SqlDbType.Bit) { Value = valore }; }
        private static SqlParameter Testo(string nome, string valore) { return new SqlParameter(nome, SqlDbType.VarChar, 255) { Value = (object)(valore ?? "") }; }
        private static string DataTesto(DateTime? valore) { return valore.HasValue ? valore.Value.ToString("s") : ""; }
        private static string Valore(int? valore) { return valore.HasValue ? valore.Value.ToString() : ""; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebGestPersV2.Configuration;
using WebGestPersV2.Models;
using WebGestPersV2.Security;

namespace WebGestPersV2.Data
{
    public sealed class PersonaleWriteRepository
    {
        public void AggiornaDatiGenerali(ModificaPersonaleRequest dati, string modificatoDa)
        {
            using (var connection = new SqlConnection(Db.ConnectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var vecchi = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        using (var q = new SqlCommand("SELECT Cognome,Nome,CodiceFiscale,EnteProvenienza,Data_Ass_Armaereo,TelefonoUfficio,NumeroStanza,id_Tit_Studio,IDFasciaOraria FROM dbo.ElencoPersonale WITH (UPDLOCK) WHERE IDPersonale=@id", connection, tx))
                        {
                            q.Parameters.Add("@id", SqlDbType.Int).Value = dati.IdPersonale;
                            using (var r = q.ExecuteReader(CommandBehavior.SingleRow))
                            {
                                if (!r.Read()) throw new InvalidOperationException("Persona non trovata.");
                                for (int i=0;i<r.FieldCount;i++) vecchi[r.GetName(i)] = r.IsDBNull(i) ? "" : Convert.ToString(r.GetValue(i));
                            }
                        }
                        if (!string.Equals(vecchi["CodiceFiscale"], dati.CodiceFiscale, StringComparison.OrdinalIgnoreCase))
                        {
                            CodiceFiscaleParser.Parse(dati.CodiceFiscale);
                            if (Scalar<int>(connection,tx,"SELECT COUNT(*) FROM dbo.ElencoPersonale WHERE CodiceFiscale=@cf AND IDPersonale<>@id",P("@cf",SqlDbType.VarChar,16,dati.CodiceFiscale),P("@id",SqlDbType.Int,dati.IdPersonale))>0)
                                throw new InvalidOperationException("Codice fiscale già associato a un'altra persona.");
                        }
                        DateTime ora=DateTime.Now;
                        Esegui(connection,tx,@"UPDATE dbo.ElencoPersonale SET Cognome=@c,Nome=@n,CodiceFiscale=@cf,EnteProvenienza=@ente,Data_Ass_Armaereo=@data,TelefonoUfficio=@tel,NumeroStanza=@stanza,id_Tit_Studio=@titolo,IDFasciaOraria=@fascia,Data_Versione_Profilo=@ora WHERE IDPersonale=@id",
                            P("@c",SqlDbType.VarChar,30,dati.Cognome),P("@n",SqlDbType.VarChar,30,dati.Nome),P("@cf",SqlDbType.VarChar,16,dati.CodiceFiscale),P("@ente",SqlDbType.VarChar,50,dati.EnteProvenienza),P("@data",SqlDbType.DateTime,dati.DataAssegnazione.HasValue?(object)dati.DataAssegnazione.Value:DBNull.Value),P("@tel",SqlDbType.VarChar,15,dati.TelefonoUfficio),P("@stanza",SqlDbType.VarChar,5,dati.NumeroStanza),P("@titolo",SqlDbType.Int,dati.IdTitoloStudio.HasValue?(object)dati.IdTitoloStudio.Value:DBNull.Value),P("@fascia",SqlDbType.Int,dati.IdFasciaOraria.HasValue?(object)dati.IdFasciaOraria.Value:DBNull.Value),P("@ora",SqlDbType.DateTime,ora),P("@id",SqlDbType.Int,dati.IdPersonale));
                        var nuovi=new Dictionary<string,string>{{"Cognome",dati.Cognome},{"Nome",dati.Nome},{"CodiceFiscale",dati.CodiceFiscale},{"EnteProvenienza",dati.EnteProvenienza},{"Data_Ass_Armaereo",dati.DataAssegnazione.HasValue?dati.DataAssegnazione.Value.ToString("s"):""},{"TelefonoUfficio",dati.TelefonoUfficio},{"NumeroStanza",dati.NumeroStanza},{"id_Tit_Studio",dati.IdTitoloStudio.HasValue?dati.IdTitoloStudio.Value.ToString():""},{"IDFasciaOraria",dati.IdFasciaOraria.HasValue?dati.IdFasciaOraria.Value.ToString():""}};
                        foreach(var x in nuovi) if(!ValoriUguali(vecchi[x.Key],x.Value)) Esegui(connection,tx,"INSERT dbo.StoricoModifiche(IDPersonale,Campo_Variato,Data_Modifica,Utente_Modificatore,Valore_Vecchio,Valore_Nuovo) VALUES(@id,@campo,@ora,@utente,@prima,@dopo)",P("@id",SqlDbType.Int,dati.IdPersonale),P("@campo",SqlDbType.VarChar,255,x.Key),P("@ora",SqlDbType.DateTime,ora),P("@utente",SqlDbType.VarChar,255,modificatoDa),P("@prima",SqlDbType.VarChar,255,vecchi[x.Key]),P("@dopo",SqlDbType.VarChar,255,x.Value));
                        if (dati.Militare) AggiornaProfiloMilitare(connection, tx, dati, ora, modificatoDa);
                        else AggiornaProfiloCivile(connection, tx, dati, ora, modificatoDa);
                        tx.Commit();
                    }
                    catch { tx.Rollback(); throw; }
                }
            }
        }
        private static bool ValoriUguali(string a,string b){DateTime da,db;if(DateTime.TryParse(a,out da)&&DateTime.TryParse(b,out db))return da==db;return string.Equals((a??"").Trim(),(b??"").Trim(),StringComparison.Ordinal);}

        private static void AggiornaProfiloMilitare(SqlConnection c, SqlTransaction t, ModificaPersonaleRequest d, DateTime ora, string utente)
        {
            if (!d.IdForzaArmata.HasValue || !d.IdGrado.HasValue || !d.IdPosizioneMilitare.HasValue ||
                !d.IdCategoriaMilitare.HasValue || !d.IdRuoloMilitare.HasValue || !d.IdSpecialitaMilitare.HasValue ||
                string.IsNullOrWhiteSpace(d.Categorico))
                throw new InvalidOperationException("Completare posizione, forza armata, categorico, grado, categoria, ruolo e specialità.");
            if (Scalar<int>(c,t,@"SELECT
                (SELECT COUNT(*) FROM dbo.Gradi WHERE ID_Grado=@grado AND ID_Arma=@arma AND Categorico=@cat)+
                (SELECT COUNT(*) FROM dbo.Categorie WHERE ID_Categoria=@categoria AND ID_Arma=@arma AND Categorico=@cat)+
                (SELECT COUNT(*) FROM dbo.Ruoli WHERE ID_Ruolo=@ruolo AND ID_Arma=@arma AND Categorico=@cat)+
                (SELECT COUNT(*) FROM dbo.Specialita WHERE [ID_Specialità]=@spec AND ID_Arma=@arma AND Categorico=@cat)",
                P("@grado",SqlDbType.Int,d.IdGrado.Value),P("@arma",SqlDbType.Int,d.IdForzaArmata.Value),P("@cat",SqlDbType.VarChar,1,d.Categorico),
                P("@categoria",SqlDbType.Int,d.IdCategoriaMilitare.Value),P("@ruolo",SqlDbType.Int,d.IdRuoloMilitare.Value),P("@spec",SqlDbType.Int,d.IdSpecialitaMilitare.Value)) != 4)
                throw new InvalidOperationException("L'inquadramento militare selezionato non è coerente con forza armata e categorico.");
            string prima = Convert.ToString(ScalarObject(c,t,"SELECT CONCAT(ISNULL(Categorico,''),'|',ISNULL(id_posizione,''),'|',ISNULL(ID_Arma,''),'|',ISNULL(ID_Grado,''),'|',ISNULL(ID_Categoria,''),'|',ISNULL(ID_Ruolo,''),'|',ISNULL([ID_Specialità],''),'|',ISNULL(Matricola,''),'|',ISNULL(num_dercreto,'')) FROM dbo.Profilo_militare WITH (UPDLOCK) WHERE IDPersonale=@id",P("@id",SqlDbType.Int,d.IdPersonale)));
            Esegui(c,t,@"UPDATE dbo.Profilo_militare SET Data_Versione_Profilo=@ora,Categorico=@cat,ID_Arma=@arma,ID_Grado=@grado,
                id_posizione=@posizione,ID_Specialità=@spec,ID_Categoria=@categoria,ID_Ruolo=@ruolo,Matricola=@matricola,num_dercreto=@decreto,
                ScuolaFormazione=@scuola,data_decreto=@giur,data_economica=@econ,data_arruolamento=@arr,data_promoz=@prom WHERE IDPersonale=@id",
                P("@ora",SqlDbType.DateTime,ora),P("@cat",SqlDbType.VarChar,1,d.Categorico),P("@arma",SqlDbType.Int,d.IdForzaArmata.Value),P("@grado",SqlDbType.Int,d.IdGrado.Value),P("@posizione",SqlDbType.Int,d.IdPosizioneMilitare.Value),
                P("@spec",SqlDbType.Int,d.IdSpecialitaMilitare.Value),P("@categoria",SqlDbType.Int,d.IdCategoriaMilitare.Value),P("@ruolo",SqlDbType.Int,d.IdRuoloMilitare.Value),P("@matricola",SqlDbType.VarChar,20,d.Matricola),P("@decreto",SqlDbType.VarChar,20,d.NumeroDecreto),P("@scuola",SqlDbType.VarChar,50,d.ScuolaFormazione),
                P("@giur",SqlDbType.DateTime,d.DataGiuridicaMilitare.HasValue?(object)d.DataGiuridicaMilitare.Value:DBNull.Value),P("@econ",SqlDbType.DateTime,d.DataEconomicaMilitare.HasValue?(object)d.DataEconomicaMilitare.Value:DBNull.Value),P("@arr",SqlDbType.DateTime,d.DataArruolamento.HasValue?(object)d.DataArruolamento.Value:DBNull.Value),P("@prom",SqlDbType.DateTime,d.DataPromozione.HasValue?(object)d.DataPromozione.Value:DBNull.Value),P("@id",SqlDbType.Int,d.IdPersonale));
            string dopo=string.Join("|",d.Categorico,d.IdPosizioneMilitare,d.IdForzaArmata,d.IdGrado,d.IdCategoriaMilitare,d.IdRuoloMilitare,d.IdSpecialitaMilitare,d.Matricola,d.NumeroDecreto);
            if(!ValoriUguali(prima,dopo)) Esegui(c,t,"INSERT dbo.StoricoModifiche(IDPersonale,Campo_Variato,Data_Modifica,Utente_Modificatore,Valore_Vecchio,Valore_Nuovo) VALUES(@id,'Profilo_militare',@ora,@utente,@prima,@dopo)",P("@id",SqlDbType.Int,d.IdPersonale),P("@ora",SqlDbType.DateTime,ora),P("@utente",SqlDbType.VarChar,255,utente),P("@prima",SqlDbType.VarChar,255,prima),P("@dopo",SqlDbType.VarChar,255,dopo));
        }

        private static void AggiornaProfiloCivile(SqlConnection c, SqlTransaction t, ModificaPersonaleRequest d, DateTime ora, string utente)
        {
            if (!d.IdAsl.HasValue || !d.IdTitoloAssunzione.HasValue || !d.IdTitoloAttuale.HasValue || !d.IdSottoarea.HasValue)
                throw new InvalidOperationException("Completare ASL, profili professionali e area di inquadramento.");

            var vecchi = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (var q = new SqlCommand(@"SELECT ID_Asl,ID_TitoloAss,DecrAss,DataGiuridAss,DataEconAss,ID_TitoloAtt,DecrAtt,
                DataGiuridAtt,DataEconAtt,ID_sottoarea,FasciaDirig,FasciaRetrib FROM dbo.PersCivile WITH (UPDLOCK) WHERE IDPersonale=@id", c, t))
            {
                q.Parameters.Add("@id", SqlDbType.Int).Value = d.IdPersonale;
                using (var r = q.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!r.Read()) throw new InvalidOperationException("Profilo civile non trovato.");
                    for (int i = 0; i < r.FieldCount; i++) vecchi[r.GetName(i)] = r.IsDBNull(i) ? "" : Convert.ToString(r.GetValue(i));
                }
            }

            string fasciaDirigenziale = string.IsNullOrWhiteSpace(d.FasciaDirigenziale) ? "n/a" : d.FasciaDirigenziale;
            string fasciaRetributiva = string.IsNullOrWhiteSpace(d.FasciaRetributiva) ? "n/a" : d.FasciaRetributiva;
            Esegui(c, t, @"UPDATE dbo.PersCivile SET Data_Versione_Profilo=@ora,ID_Asl=@asl,ID_TitoloAss=@titAss,
                DecrAss=@decrAss,DataGiuridAss=@giurAss,DataEconAss=@econAss,ID_TitoloAtt=@titAtt,DecrAtt=@decrAtt,
                DataGiuridAtt=@giurAtt,DataEconAtt=@econAtt,ID_sottoarea=@area,FasciaDirig=@fasciaDir,
                FasciaRetrib=@fasciaRetr WHERE IDPersonale=@id",
                P("@ora",SqlDbType.DateTime,ora),P("@asl",SqlDbType.Int,d.IdAsl.Value),P("@titAss",SqlDbType.Int,d.IdTitoloAssunzione.Value),
                P("@decrAss",SqlDbType.VarChar,20,d.DecretoAssunzione),P("@giurAss",SqlDbType.DateTime,d.DataGiuridicaAssunzione.HasValue?(object)d.DataGiuridicaAssunzione.Value:DBNull.Value),
                P("@econAss",SqlDbType.DateTime,d.DataEconomicaAssunzione.HasValue?(object)d.DataEconomicaAssunzione.Value:DBNull.Value),P("@titAtt",SqlDbType.Int,d.IdTitoloAttuale.Value),
                P("@decrAtt",SqlDbType.VarChar,20,d.DecretoAttuale),P("@giurAtt",SqlDbType.DateTime,d.DataGiuridicaAttuale.HasValue?(object)d.DataGiuridicaAttuale.Value:DBNull.Value),
                P("@econAtt",SqlDbType.DateTime,d.DataEconomicaAttuale.HasValue?(object)d.DataEconomicaAttuale.Value:DBNull.Value),P("@area",SqlDbType.Int,d.IdSottoarea.Value),
                P("@fasciaDir",SqlDbType.VarChar,3,fasciaDirigenziale),P("@fasciaRetr",SqlDbType.VarChar,3,fasciaRetributiva),P("@id",SqlDbType.Int,d.IdPersonale));

            var nuovi = new Dictionary<string,string>{{"ID_Asl",d.IdAsl.Value.ToString()},{"ID_TitoloAss",d.IdTitoloAssunzione.Value.ToString()},
                {"DecrAss",d.DecretoAssunzione},{"DataGiuridAss",DataStorico(d.DataGiuridicaAssunzione)},{"DataEconAss",DataStorico(d.DataEconomicaAssunzione)},
                {"ID_TitoloAtt",d.IdTitoloAttuale.Value.ToString()},{"DecrAtt",d.DecretoAttuale},{"DataGiuridAtt",DataStorico(d.DataGiuridicaAttuale)},
                {"DataEconAtt",DataStorico(d.DataEconomicaAttuale)},{"ID_sottoarea",d.IdSottoarea.Value.ToString()},
                {"FasciaDirig",fasciaDirigenziale},{"FasciaRetrib",fasciaRetributiva}};
            foreach (var x in nuovi)
                if (!ValoriUguali(vecchi[x.Key], x.Value))
                    Esegui(c,t,"INSERT dbo.StoricoModifiche(IDPersonale,Campo_Variato,Data_Modifica,Utente_Modificatore,Valore_Vecchio,Valore_Nuovo) VALUES(@id,@campo,@ora,@utente,@prima,@dopo)",P("@id",SqlDbType.Int,d.IdPersonale),P("@campo",SqlDbType.VarChar,255,x.Key),P("@ora",SqlDbType.DateTime,ora),P("@utente",SqlDbType.VarChar,255,utente),P("@prima",SqlDbType.VarChar,255,vecchi[x.Key]),P("@dopo",SqlDbType.VarChar,255,x.Value));
        }

        private static string DataStorico(DateTime? data) { return data.HasValue ? data.Value.ToString("s") : ""; }

        public IList<LookupItem> TitoliStudio() { return Leggi("SELECT id_Tit_Studio, Desc_Tit_Studio FROM dbo.Titoli_Studio ORDER BY Desc_Tit_Studio"); }
        public IList<LookupItem> FasceOrarie() { return Leggi("SELECT IDFascia, Descrizione FROM dbo.TipoFascia ORDER BY Descrizione"); }
        public IList<LookupItem> TitoliCivili() { return Leggi("SELECT ID_Titolo, Sigla_titolo + ' - ' + ISNULL(Descr, '') FROM dbo.Titoli ORDER BY Descr"); }
        public IList<LookupItem> ForzeArmate() { return Leggi("SELECT ID_Arma, RTRIM(SiglaArma) + ' - ' + ISNULL(DescArma, '') FROM dbo.FFAA ORDER BY livello"); }
        public IList<LookupItem> Asl() { return Leggi("SELECT id_Asl, Nome_Asl + CASE WHEN ISNULL(Indirizzo_Asl,'')='' THEN '' ELSE ' ('+Indirizzo_Asl+')' END FROM dbo.ASL ORDER BY Nome_Asl"); }
        public IList<LookupItem> AreeCivili() { return Leggi("SELECT id_sottoarea, Area + '/' + sottoarea FROM dbo.Aree ORDER BY Area,sottoarea"); }
        public IList<LookupItem> Gradi(int idArma, string categorico)
        {
            return Leggi("SELECT ID_Grado, SiglaGrado + ' - ' + ISNULL(Descr_grado, '') FROM dbo.Gradi WHERE ID_Arma=@a AND Categorico=@c ORDER BY LivelloGerarchico",
                new SqlParameter("@a", SqlDbType.Int) { Value = idArma }, new SqlParameter("@c", SqlDbType.VarChar, 1) { Value = categorico });
        }
        public IList<LookupItem> PosizioniMilitari() { return Leggi("SELECT id_posizione, Sigla_Pos + CASE WHEN ISNULL(Desc_Pos,'')='' THEN '' ELSE ' ('+Desc_Pos+')' END FROM dbo.PosizioniStato ORDER BY livello"); }
        public IList<LookupItem> CategorieMilitari(int idArma,string categorico) { return Leggi("SELECT ID_Categoria, SiglaCategoria + CASE WHEN ISNULL(DescCategoria,'')='' THEN '' ELSE ' ('+DescCategoria+')' END FROM dbo.Categorie WHERE ID_Arma=@a AND Categorico=@c ORDER BY livello",new SqlParameter("@a",SqlDbType.Int){Value=idArma},new SqlParameter("@c",SqlDbType.VarChar,1){Value=categorico}); }
        public IList<LookupItem> RuoliMilitari(int idArma,string categorico) { return Leggi("SELECT ID_Ruolo, SiglaRuolo + CASE WHEN ISNULL(DescRuolo,'')='' THEN '' ELSE ' ('+DescRuolo+')' END FROM dbo.Ruoli WHERE ID_Arma=@a AND Categorico=@c ORDER BY livello",new SqlParameter("@a",SqlDbType.Int){Value=idArma},new SqlParameter("@c",SqlDbType.VarChar,1){Value=categorico}); }
        public IList<LookupItem> SpecialitaMilitari(int idArma,string categorico) { return Leggi("SELECT [ID_Specialità], SiglaSpecialita + CASE WHEN ISNULL(DescSpecialita,'')='' THEN '' ELSE ' ('+DescSpecialita+')' END FROM dbo.Specialita WHERE ID_Arma=@a AND Categorico=@c ORDER BY livello",new SqlParameter("@a",SqlDbType.Int){Value=idArma},new SqlParameter("@c",SqlDbType.VarChar,1){Value=categorico}); }

        public int Inserisci(NuovoPersonaleRequest dati)
        {
            CodiceFiscaleInfo cf = CodiceFiscaleParser.Parse(dati.CodiceFiscale);
            using (var connection = new SqlConnection(Db.ConnectionString))
            {
                connection.Open();
                using (SqlTransaction tx = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        if (Scalar<int>(connection, tx, "SELECT COUNT(*) FROM dbo.ElencoPersonale WITH (UPDLOCK,HOLDLOCK) WHERE CodiceFiscale=@cf", P("@cf", SqlDbType.VarChar, 16, dati.CodiceFiscale)) > 0)
                            throw new InvalidOperationException("Esiste già una persona con questo codice fiscale.");
                        int id = Scalar<int>(connection, tx, "SELECT ISNULL(MAX(IDPersonale),0)+1 FROM dbo.ElencoPersonale WITH (UPDLOCK,HOLDLOCK)");
                        DateTime ora = DateTime.Now;
                        Esegui(connection, tx, @"INSERT dbo.ElencoPersonale(IDPersonale,id_Tit_Studio,Data_Versione_Profilo,Cognome,Nome,IDFasciaOraria,EnteProvenienza,Data_Ass_Armaereo,SessoM,Militare,CodiceFiscale,Stato_Servizio,ImgPath)
                            VALUES(@id,@ts,@ora,@cognome,@nome,@fascia,@ente,@data,@sesso,@mil,@cf,'attivo',@foto)",
                            P("@id",SqlDbType.Int,id),P("@ts",SqlDbType.Int,dati.IdTitoloStudio),P("@ora",SqlDbType.DateTime,ora),P("@cognome",SqlDbType.VarChar,30,dati.Cognome),P("@nome",SqlDbType.VarChar,30,dati.Nome),P("@fascia",SqlDbType.Int,dati.IdFasciaOraria),P("@ente",SqlDbType.VarChar,50,dati.EnteProvenienza),P("@data",SqlDbType.DateTime,dati.DataAssegnazione),P("@sesso",SqlDbType.Bit,cf.Uomo),P("@mil",SqlDbType.Bit,dati.Militare),P("@cf",SqlDbType.VarChar,16,dati.CodiceFiscale),P("@foto",SqlDbType.VarChar,250,AppConfig.GetFotoPredefinita(dati.Militare,cf.Uomo)));
                        object comune = ScalarObject(connection, tx, "SELECT TOP 1 ID_comune FROM dbo.Comuni WHERE CodiceFiscale_Comune=@codice", P("@codice",SqlDbType.NVarChar,4,cf.CodiceComune));
                        Esegui(connection, tx, @"INSERT dbo.Anagrafica_Sensibile(IDPersonale,Data_Versione_Profilo,DataNascita,ID_StatoCivile,ID_Comune_Nascita,ID_comune_Residenza,ID_comune_Domicilio)
                            VALUES(@id,@ora,@nascita,2,@comune,0,0)",P("@id",SqlDbType.Int,id),P("@ora",SqlDbType.DateTime,ora),P("@nascita",SqlDbType.DateTime,cf.DataNascita),P("@comune",SqlDbType.Int,comune ?? (object)DBNull.Value));
                        if (dati.Militare) InserisciMilitare(connection,tx,id,ora,dati);
                        else InserisciCivile(connection,tx,id,ora,dati);
                        Esegui(connection, tx, "INSERT dbo.Incarichi(IDPersonale,id_tipo_incarico,Data_inizio,principale,ID_Uff1,ID_Uff2,ID_Uff3) VALUES(@id,-1,@ora,1,@u1,@u2,@u3)",P("@id",SqlDbType.Int,id),P("@ora",SqlDbType.DateTime,ora),P("@u1",SqlDbType.Int,AppConfig.UfficioLivello1Vuoto),P("@u2",SqlDbType.Int,AppConfig.UfficioLivello2Vuoto),P("@u3",SqlDbType.Int,AppConfig.UfficioLivello3Vuoto));
                        tx.Commit(); return id;
                    }
                    catch { tx.Rollback(); throw; }
                }
            }
        }

        private static void InserisciCivile(SqlConnection c,SqlTransaction t,int id,DateTime ora,NuovoPersonaleRequest d)
        { Esegui(c,t,"INSERT dbo.PersCivile(IDPersonale,Data_Versione_Profilo,ID_Asl,ID_TitoloAss,ID_TitoloAtt,ID_sottoarea,FasciaDirig,FasciaRetrib) VALUES(@id,@ora,0,@titolo,@titolo,36,'n/a','n/a')",P("@id",SqlDbType.Int,id),P("@ora",SqlDbType.DateTime,ora),P("@titolo",SqlDbType.Int,d.IdTitoloCivile.Value)); }
        private static void InserisciMilitare(SqlConnection c,SqlTransaction t,int id,DateTime ora,NuovoPersonaleRequest d)
        {
            string sigla=Convert.ToString(ScalarObject(c,t,"SELECT RTRIM(SiglaArma) FROM dbo.FFAA WHERE ID_Arma=@id",P("@id",SqlDbType.Int,d.IdForzaArmata.Value)));
            Esegui(c,t,@"INSERT dbo.Profilo_militare(IDPersonale,Data_Versione_Profilo,Categorico,id_posizione,ID_Grado,ID_Arma,ID_Specialità,ID_Categoria,ID_Ruolo)
                VALUES(@id,@ora,@cat,1,@grado,@arma,@spec,@categoria,@ruolo)",P("@id",SqlDbType.Int,id),P("@ora",SqlDbType.DateTime,ora),P("@cat",SqlDbType.VarChar,1,d.Categorico),P("@grado",SqlDbType.Int,d.IdGrado.Value),P("@arma",SqlDbType.Int,d.IdForzaArmata.Value),P("@spec",SqlDbType.Int,AppConfig.GetSpecialita(sigla,d.Categorico)),P("@categoria",SqlDbType.Int,AppConfig.GetCategoria(sigla,d.Categorico)),P("@ruolo",SqlDbType.Int,AppConfig.GetRuolo(sigla,d.Categorico)));
        }
        private static IList<LookupItem> Leggi(string sql,params SqlParameter[] ps) { var x=new List<LookupItem>(); using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c)){q.Parameters.AddRange(ps);c.Open();using(var r=q.ExecuteReader())while(r.Read())x.Add(new LookupItem{Value=Convert.ToString(r.GetValue(0)),Text=Convert.ToString(r.GetValue(1))});}return x; }
        private static void Esegui(SqlConnection c,SqlTransaction t,string sql,params SqlParameter[] ps){using(var q=new SqlCommand(sql,c,t)){q.Parameters.AddRange(ps);q.ExecuteNonQuery();}}
        private static T Scalar<T>(SqlConnection c,SqlTransaction t,string sql,params SqlParameter[] ps){return (T)Convert.ChangeType(ScalarObject(c,t,sql,ps),typeof(T));}
        private static object ScalarObject(SqlConnection c,SqlTransaction t,string sql,params SqlParameter[] ps){using(var q=new SqlCommand(sql,c,t)){q.Parameters.AddRange(ps);return q.ExecuteScalar();}}
        private static SqlParameter P(string n,SqlDbType t,object v){return new SqlParameter(n,t){Value=v??DBNull.Value};}
        private static SqlParameter P(string n,SqlDbType t,int size,object v){return new SqlParameter(n,t,size){Value=v??DBNull.Value};}
    }
}

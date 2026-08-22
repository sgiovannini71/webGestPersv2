using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebGestPersV2.Models;

namespace WebGestPersV2.Data
{
    public sealed class StoricoModificaListaItem
    {
        public int IdStorico { get; set; }
        public string CampoVariato { get; set; }
        public DateTime? DataModifica { get; set; }
        public string UtenteModificatore { get; set; }
        public string ValoreVecchio { get; set; }
        public string ValoreNuovo { get; set; }

        public string DataModificaTesto
        {
            get { return DataModifica.HasValue ? DataModifica.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }
    }

    public sealed class PersonaleRepository
    {
        public IList<StoricoModificaListaItem> CercaStoricoModifiche(int idPersonale)
        {
            const string sql = @"SELECT ID_Storico, Campo_Variato, Data_Modifica,
                                        Utente_Modificatore, Valore_Vecchio, Valore_Nuovo
                                 FROM dbo.StoricoModifiche
                                 WHERE IDPersonale = @IdPersonale
                                 ORDER BY Data_Modifica DESC, ID_Storico DESC";
            var risultati = new List<StoricoModificaListaItem>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@IdPersonale", SqlDbType.Int).Value = idPersonale;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        risultati.Add(new StoricoModificaListaItem {
                            IdStorico = reader.GetInt32(0),
                            CampoVariato = Testo(reader, 1),
                            DataModifica = Data(reader, 2),
                            UtenteModificatore = Testo(reader, 3),
                            ValoreVecchio = Testo(reader, 4),
                            ValoreNuovo = Testo(reader, 5)
                        });
                    }
                }
            }
            return risultati;
        }

        public IList<StoricoIncaricoListaItem> CercaStoricoIncarichi(int idPersonale)
        {
            const string sql = @"SELECT id_incarico, Incarico, principale, data_inizio, date_fine
                                 FROM dbo.StoricoIncarichi
                                 WHERE IDPersonale = @IdPersonale
                                 ORDER BY id_incarico DESC";
            var risultati = new List<StoricoIncaricoListaItem>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@IdPersonale", SqlDbType.Int).Value = idPersonale;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        risultati.Add(new StoricoIncaricoListaItem {
                            IdIncarico = reader.GetInt32(0), Incarico = Testo(reader, 1),
                            Principale = !reader.IsDBNull(2) && reader.GetBoolean(2),
                            DataInizio = Data(reader, 3), DataFine = Data(reader, 4)
                        });
                    }
                }
            }
            return risultati;
        }

        public IList<IncaricoListaItem> CercaIncarichi(int idPersonale)
        {
            const string sql = @"SELECT i.id_incarico, ti.Descr_incarico, i.principale,
                                        i.Data_inizio, l1.SgUff1, l2.SgUff2, l3.SgUff3,
                                        i.id_tipo_incarico, i.ID_Uff1, i.ID_Uff2, i.ID_Uff3
                                 FROM dbo.Incarichi AS i
                                 INNER JOIN dbo.Tipo_incarichi AS ti
                                     ON i.id_tipo_incarico = ti.id_tipo_incarico
                                 LEFT JOIN dbo.Liv1Uff AS l1
                                     ON i.ID_Uff1 = l1.ID_Uff1
                                 LEFT JOIN dbo.Liv2Uff AS l2
                                     ON i.ID_Uff2 = l2.ID_Uff2 AND i.ID_Uff1 = l2.ID_Uff1
                                 LEFT JOIN dbo.Liv3Uff AS l3
                                     ON i.ID_Uff3 = l3.ID_Uff3 AND i.ID_Uff2 = l3.ID_Uff2 AND i.ID_Uff1 = l3.ID_Uff1
                                 WHERE i.IDPersonale = @IdPersonale
                                 ORDER BY CASE WHEN i.principale = 1 THEN 0 ELSE 1 END,
                                          i.Data_inizio DESC, ti.Descr_incarico";
            var risultati = new List<IncaricoListaItem>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@IdPersonale", SqlDbType.Int).Value = idPersonale;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        risultati.Add(new IncaricoListaItem {
                            IdIncarico = reader.GetInt32(0), Descrizione = Testo(reader, 1),
                            Principale = !reader.IsDBNull(2) && reader.GetBoolean(2), DataInizio = Data(reader, 3),
                            UfficioLivello1 = Testo(reader, 4), UfficioLivello2 = Testo(reader, 5), UfficioLivello3 = Testo(reader, 6),
                            IdTipoIncarico = reader.GetInt32(7), IdUfficio1 = InteroNullable(reader, 8),
                            IdUfficio2 = InteroNullable(reader, 9), IdUfficio3 = InteroNullable(reader, 10)
                        });
                    }
                }
            }
            return risultati;
        }

        public PersonaDettaglio TrovaAttivo(int idPersonale)
        {
            const string sql = @"SELECT ep.IDPersonale, ep.Cognome, ep.Nome, ep.CodiceFiscale,
                                        ep.SessoM, ep.Militare, ep.Stato_Servizio, ep.TelefonoUfficio,
                                        ep.NumeroStanza, ep.UserName, ep.EnteProvenienza,
                                        ep.Data_Ass_Armaereo, ep.Data_Usc_Armaereo, ep.CodiceMensa,
                                        ep.Statino, CONVERT(varchar(max), ep.Note),
                                        g.SiglaGrado, g.Descr_grado, fa.SiglaArma, pm.Categorico,
                                        pm.Matricola, t.Sigla_titolo, t.Descr, a.Area, a.Sottoarea,
                                        pc.Qualifica, pc.FasciaDirig, pc.FasciaRetrib,
                                        ps.Sigla_Pos, cat.DescCategoria, r.DescRuolo, sp.DescSpecialita,
                                        pm.data_decreto, pm.data_economica, pm.num_dercreto,
                                        pm.ScuolaFormazione, pm.data_arruolamento, pm.data_promoz,
                                        pm.Data_Versione_Profilo, asl.Nome_Asl, tass.Sigla_titolo,
                                        tass.Descr, pc.DecrAss, pc.DataGiuridAss, pc.DataEconAss,
                                        pc.DecrAtt, pc.DataGiuridAtt, pc.DataEconAtt, pc.Data_Versione_Profilo,
                                        ans.DataNascita, sc.Descr_stato_civ,
                                        cn.Comune, cn.Provincia, ans.Indirizzo_Residenza,
                                        cr.Comune, cr.Provincia, ans.Telefono_Residenza,
                                        ans.Indirizzo_Domicilio, cd.Comune, cd.Provincia,
                                        ans.Telefono_Domicilio, ans.cellulare, ans.Pass_SVZ,
                                        ans.Data_Pass_SVZ, ans.cmd, ans.Data_cmd,
                                        ans.Modello_AT, ans.Data_AT, ans.Data_Versione_Profilo,
                                        ep.ImgPath, ep.id_Tit_Studio, ep.IDFasciaOraria,
                                        pm.ID_Arma, pm.ID_Grado, pc.ID_Asl, pc.ID_TitoloAss,
                                        pc.ID_TitoloAtt, pc.ID_sottoarea, pm.id_posizione,
                                        pm.ID_Categoria, pm.ID_Ruolo, pm.[ID_Specialità]
                                 FROM dbo.ElencoPersonale AS ep
                                 LEFT JOIN dbo.Profilo_militare AS pm ON ep.IDPersonale = pm.IDPersonale AND ep.Militare = 1
                                 LEFT JOIN dbo.Gradi AS g ON pm.ID_Grado = g.ID_Grado
                                 LEFT JOIN dbo.FFAA AS fa ON pm.ID_Arma = fa.ID_Arma
                                 LEFT JOIN dbo.PosizioniStato AS ps ON pm.id_posizione = ps.id_posizione
                                 LEFT JOIN dbo.Categorie AS cat ON pm.ID_Categoria = cat.ID_Categoria
                                 LEFT JOIN dbo.Ruoli AS r ON pm.ID_Ruolo = r.ID_Ruolo
                                 LEFT JOIN dbo.Specialita AS sp ON pm.[ID_Specialità] = sp.[ID_Specialità]
                                 LEFT JOIN dbo.PersCivile AS pc ON ep.IDPersonale = pc.IDPersonale AND ep.Militare = 0
                                 LEFT JOIN dbo.Titoli AS t ON pc.ID_TitoloAtt = t.ID_Titolo
                                 LEFT JOIN dbo.Titoli AS tass ON pc.ID_TitoloAss = tass.ID_Titolo
                                 LEFT JOIN dbo.Aree AS a ON pc.ID_sottoarea = a.id_sottoarea
                                 LEFT JOIN dbo.ASL AS asl ON pc.ID_Asl = asl.id_Asl
                                 LEFT JOIN dbo.Anagrafica_Sensibile AS ans ON ep.IDPersonale = ans.IDPersonale
                                 LEFT JOIN dbo.Stati_civili AS sc ON ans.ID_StatoCivile = sc.ID_StatoCivile
                                 LEFT JOIN dbo.Comuni AS cn ON ans.ID_Comune_Nascita = cn.ID_comune
                                 LEFT JOIN dbo.Comuni AS cr ON ans.ID_comune_Residenza = cr.ID_comune
                                 LEFT JOIN dbo.Comuni AS cd ON ans.ID_comune_Domicilio = cd.ID_comune
                                 WHERE ep.IDPersonale = @IdPersonale AND ep.Stato_Servizio = 'attivo'";
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@IdPersonale", SqlDbType.Int).Value = idPersonale;
                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read()) return null;
                    return new PersonaDettaglio {
                        IdPersonale = reader.GetInt32(0), Cognome = Testo(reader, 1), Nome = Testo(reader, 2),
                        CodiceFiscale = Testo(reader, 3), Sesso = reader.GetBoolean(4) ? "Maschile" : "Femminile",
                        Militare = reader.GetBoolean(5), StatoServizio = Testo(reader, 6), TelefonoUfficio = Testo(reader, 7),
                        NumeroStanza = Testo(reader, 8), UserName = Testo(reader, 9), EnteProvenienza = Testo(reader, 10),
                        DataAssegnazione = Data(reader, 11), DataUscita = Data(reader, 12), CodiceMensa = Testo(reader, 13),
                        Statino = Testo(reader, 14), Note = Testo(reader, 15), Grado = Testo(reader, 16),
                        DescrizioneGrado = Testo(reader, 17), ForzaArmata = Testo(reader, 18).Trim(), Categorico = Testo(reader, 19),
                        Matricola = Testo(reader, 20), Titolo = Testo(reader, 21), DescrizioneTitolo = Testo(reader, 22),
                        Area = Testo(reader, 23), Sottoarea = Testo(reader, 24), Qualifica = Testo(reader, 25),
                        FasciaDirigenziale = Testo(reader, 26), FasciaRetributiva = Testo(reader, 27),
                        PosizioneStato = Testo(reader, 28), CategoriaMilitare = Testo(reader, 29), Ruolo = Testo(reader, 30),
                        Specialita = Testo(reader, 31), DataGiuridicaMilitare = Data(reader, 32), DataEconomicaMilitare = Data(reader, 33),
                        NumeroDecreto = Testo(reader, 34), ScuolaFormazione = Testo(reader, 35), DataArruolamento = Data(reader, 36),
                        DataPromozione = Data(reader, 37), AggiornamentoProfiloMilitare = Data(reader, 38), Asl = Testo(reader, 39),
                        TitoloAssunzione = Testo(reader, 40), DescrizioneTitoloAssunzione = Testo(reader, 41),
                        DecretoAssunzione = Testo(reader, 42), DataGiuridicaAssunzione = Data(reader, 43),
                        DataEconomicaAssunzione = Data(reader, 44), DecretoAttuale = Testo(reader, 45),
                        DataGiuridicaAttuale = Data(reader, 46), DataEconomicaAttuale = Data(reader, 47),
                        AggiornamentoProfiloCivile = Data(reader, 48), DataNascita = Data(reader, 49), StatoCivile = Testo(reader, 50),
                        ComuneNascita = Comune(Testo(reader, 51), Testo(reader, 52)), IndirizzoResidenza = Testo(reader, 53),
                        ComuneResidenza = Comune(Testo(reader, 54), Testo(reader, 55)), TelefonoResidenza = Testo(reader, 56),
                        IndirizzoDomicilio = Testo(reader, 57), ComuneDomicilio = Comune(Testo(reader, 58), Testo(reader, 59)),
                        TelefonoDomicilio = Testo(reader, 60), Cellulare = Testo(reader, 61), PassaportoServizio = Testo(reader, 62),
                        DataPassaportoServizio = Data(reader, 63), Cmd = Testo(reader, 64), DataCmd = Data(reader, 65),
                        ModelloAt = Testo(reader, 66), DataAt = Data(reader, 67), AggiornamentoDatiSensibili = Data(reader, 68),
                        ImgPath = Testo(reader, 69), IdTitoloStudio = InteroNullable(reader, 70),
                        IdFasciaOraria = InteroNullable(reader, 71), IdForzaArmata = InteroNullable(reader, 72),
                        IdGrado = InteroNullable(reader, 73), IdAsl = InteroNullable(reader, 74),
                        IdTitoloAssunzione = InteroNullable(reader, 75), IdTitoloAttuale = InteroNullable(reader, 76),
                        IdSottoarea = InteroNullable(reader, 77), IdPosizioneMilitare = InteroNullable(reader, 78),
                        IdCategoriaMilitare = InteroNullable(reader, 79), IdRuoloMilitare = InteroNullable(reader, 80),
                        IdSpecialitaMilitare = InteroNullable(reader, 81)
                    };
                }
            }
        }

        private static string Testo(SqlDataReader reader, int indice)
        {
            return reader.IsDBNull(indice) ? string.Empty : Convert.ToString(reader.GetValue(indice));
        }

        private static DateTime? Data(SqlDataReader reader, int indice)
        {
            return reader.IsDBNull(indice) ? (DateTime?)null : reader.GetDateTime(indice);
        }

        private static int? InteroNullable(SqlDataReader reader, int indice)
        {
            return reader.IsDBNull(indice) ? (int?)null : Convert.ToInt32(reader.GetValue(indice));
        }

        private static string Comune(string nome, string provincia)
        {
            if (string.IsNullOrWhiteSpace(nome)) return string.Empty;
            return string.IsNullOrWhiteSpace(provincia) ? nome : nome + " (" + provincia + ")";
        }

        public IList<LookupItem> GradiPerFiltro(int? idArma, string categorico)
        {
            const string sql = @"SELECT g.ID_Grado, RTRIM(f.SiglaArma)+' · '+RTRIM(g.SiglaGrado) + CASE WHEN ISNULL(g.Descr_grado,'')='' THEN '' ELSE ' - '+g.Descr_grado END
                                 FROM dbo.Gradi g INNER JOIN dbo.FFAA f ON f.ID_Arma=g.ID_Arma
                                 WHERE (@Arma IS NULL OR g.ID_Arma=@Arma) AND (@Categorico='' OR g.Categorico=@Categorico)
                                 ORDER BY g.LivelloGerarchico,g.SiglaGrado";
            return LeggiLookup(sql,
                new SqlParameter("@Arma", SqlDbType.Int) { Value = (object)idArma ?? DBNull.Value },
                new SqlParameter("@Categorico", SqlDbType.VarChar, 1) { Value = categorico ?? string.Empty });
        }

        public IList<LookupItem> ProfiliCiviliPerFiltro()
        {
            return LeggiLookup("SELECT ID_Titolo,RTRIM(Sigla_titolo)+CASE WHEN ISNULL(Descr,'')='' THEN '' ELSE ' - '+Descr END FROM dbo.Titoli ORDER BY livello,Sigla_titolo");
        }

        private IList<LookupItem> LeggiLookup(string sql, params SqlParameter[] parametri)
        {
            var risultati = new List<LookupItem>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                if (parametri != null && parametri.Length > 0) command.Parameters.AddRange(parametri);
                connection.Open();
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) risultati.Add(new LookupItem { Value = Convert.ToString(reader.GetValue(0)), Text = Testo(reader, 1) });
            }
            return risultati;
        }

        public IList<PersonaListaItem> Cerca(string testo, int massimoRisultati, string tipoPersonale, int? idGrado, int? idArma, string categorico, int? idProfilo)
        {
            const string sql = @"SELECT TOP (@Massimo)
                                     ep.IDPersonale,
                                     ep.Cognome,
                                     ep.Nome,
                                     ep.CodiceFiscale,
                                     ep.Stato_Servizio,
                                     ep.Militare,
                                     CASE
                                         WHEN ep.Militare = 1 THEN g.SiglaGrado
                                         ELSE t.Sigla_titolo
                                     END AS GradoProfilo,
                                     ti.Descr_incarico AS IncaricoPrincipale,
                                     RTRIM(l1.SgUff1)+CASE WHEN ISNULL(l1.DescUff1,'')='' THEN '' ELSE ' - '+l1.DescUff1 END AS UfficioLivello1,
                                     RTRIM(l2.SgUff2)+CASE WHEN ISNULL(l2.DescUff2,'')='' THEN '' ELSE ' - '+l2.DescUff2 END AS UfficioLivello2,
                                     RTRIM(l3.SgUff3)+CASE WHEN ISNULL(l3.DescUff3,'')='' THEN '' ELSE ' - '+l3.DescUff3 END AS UfficioLivello3
                                 FROM dbo.ElencoPersonale AS ep
                                 LEFT JOIN dbo.Profilo_militare AS pm
                                     ON ep.IDPersonale = pm.IDPersonale AND ep.Militare = 1
                                 LEFT JOIN dbo.Gradi AS g
                                     ON pm.ID_Grado = g.ID_Grado
                                 LEFT JOIN dbo.PersCivile AS pc
                                     ON ep.IDPersonale = pc.IDPersonale AND ep.Militare = 0
                                 LEFT JOIN dbo.Titoli AS t
                                     ON pc.ID_TitoloAtt = t.ID_Titolo
                                 OUTER APPLY (
                                     SELECT TOP (1) i.id_tipo_incarico,i.ID_Uff1,i.ID_Uff2,i.ID_Uff3
                                     FROM dbo.Incarichi i
                                     WHERE i.IDPersonale=ep.IDPersonale AND i.principale=1
                                     ORDER BY i.Data_inizio DESC,i.id_incarico DESC
                                 ) AS ip
                                 LEFT JOIN dbo.Tipo_incarichi AS ti ON ti.id_tipo_incarico=ip.id_tipo_incarico
                                 LEFT JOIN dbo.Liv1Uff AS l1 ON l1.ID_Uff1=ip.ID_Uff1
                                 LEFT JOIN dbo.Liv2Uff AS l2 ON l2.ID_Uff1=ip.ID_Uff1 AND l2.ID_Uff2=ip.ID_Uff2
                                 LEFT JOIN dbo.Liv3Uff AS l3 ON l3.ID_Uff1=ip.ID_Uff1 AND l3.ID_Uff2=ip.ID_Uff2 AND l3.ID_Uff3=ip.ID_Uff3
                                 WHERE ep.Stato_Servizio = 'attivo'
                                   AND (@Testo = '' OR ep.Cognome LIKE @Ricerca OR ep.Nome LIKE @Ricerca OR ep.CodiceFiscale LIKE @Ricerca)
                                   AND (@Tipo = '' OR (@Tipo='M' AND ep.Militare=1) OR (@Tipo='C' AND ep.Militare=0))
                                   AND (@Grado IS NULL OR pm.ID_Grado=@Grado)
                                   AND (@Arma IS NULL OR pm.ID_Arma=@Arma)
                                   AND (@Categorico='' OR pm.Categorico=@Categorico)
                                   AND (@Profilo IS NULL OR pc.ID_TitoloAtt=@Profilo)
                                 ORDER BY ep.Cognome, ep.Nome";
            var risultati = new List<PersonaListaItem>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                string filtro = (testo ?? string.Empty).Trim();
                command.Parameters.Add("@Massimo", SqlDbType.Int).Value = massimoRisultati;
                command.Parameters.Add("@Testo", SqlDbType.VarChar, 50).Value = filtro;
                command.Parameters.Add("@Ricerca", SqlDbType.VarChar, 52).Value = "%" + filtro + "%";
                command.Parameters.Add("@Tipo", SqlDbType.Char, 1).Value = tipoPersonale ?? string.Empty;
                command.Parameters.Add("@Grado", SqlDbType.Int).Value = (object)idGrado ?? DBNull.Value;
                command.Parameters.Add("@Arma", SqlDbType.Int).Value = (object)idArma ?? DBNull.Value;
                command.Parameters.Add("@Categorico", SqlDbType.VarChar, 1).Value = categorico ?? string.Empty;
                command.Parameters.Add("@Profilo", SqlDbType.Int).Value = (object)idProfilo ?? DBNull.Value;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        risultati.Add(new PersonaListaItem {
                            IdPersonale = reader.GetInt32(0), Cognome = reader.GetString(1), Nome = reader.GetString(2),
                            CodiceFiscale = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            StatoServizio = reader.GetString(4),
                            Militare = reader.GetBoolean(5),
                            GradoProfilo = Testo(reader, 6), IncaricoPrincipale = Testo(reader, 7),
                            UfficioLivello1 = Testo(reader, 8), UfficioLivello2 = Testo(reader, 9), UfficioLivello3 = Testo(reader, 10)
                        });
                    }
                }
            }
            return risultati;
        }
    }
}

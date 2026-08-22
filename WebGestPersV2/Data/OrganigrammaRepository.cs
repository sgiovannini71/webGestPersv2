using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebGestPersV2.Data
{
    public sealed class OrganigrammaPersona
    {
        public int IdPersonale { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public bool Militare { get; set; }
        public string GradoProfilo { get; set; }
        public string Incarico { get; set; }
        public int? IdUfficio1 { get; set; }
        public int? IdUfficio2 { get; set; }
        public int? IdUfficio3 { get; set; }
        public string Ufficio1 { get; set; }
        public string Ufficio2 { get; set; }
        public string Ufficio3 { get; set; }
        public int Ordinamento1 { get; set; }
        public int Ordinamento2 { get; set; }
        public int Ordinamento3 { get; set; }
    }

    public sealed class OrganigrammaRepository
    {
        public IList<OrganigrammaPersona> CercaPersonaleAttivo()
        {
            const string sql = @"SELECT ep.IDPersonale, ep.Cognome, ep.Nome, ep.Militare,
                                        CASE WHEN ep.Militare=1 THEN g.SiglaGrado ELSE t.Sigla_titolo END AS GradoProfilo,
                                        ti.Descr_incarico,
                                        ip.ID_Uff1, ip.ID_Uff2, ip.ID_Uff3,
                                        RTRIM(ISNULL(u1.SgUff1,''))+CASE WHEN ISNULL(u1.DescUff1,'')='' THEN '' ELSE ' - '+u1.DescUff1 END,
                                        RTRIM(ISNULL(u2.SgUff2,''))+CASE WHEN ISNULL(u2.DescUff2,'')='' THEN '' ELSE ' - '+u2.DescUff2 END,
                                        RTRIM(ISNULL(u3.SgUff3,''))+CASE WHEN ISNULL(u3.DescUff3,'')='' THEN '' ELSE ' - '+u3.DescUff3 END,
                                        ISNULL(NULLIF(u1.Livello,0),999), ISNULL(NULLIF(u2.Livello,0),999), ISNULL(NULLIF(u3.Livello,0),999)
                                 FROM dbo.ElencoPersonale ep
                                 CROSS APPLY (
                                     SELECT TOP (1) i.id_tipo_incarico,i.ID_Uff1,i.ID_Uff2,i.ID_Uff3
                                     FROM dbo.Incarichi i
                                     WHERE i.IDPersonale=ep.IDPersonale AND i.principale=1
                                     ORDER BY i.Data_inizio DESC,i.id_incarico DESC
                                 ) ip
                                 INNER JOIN dbo.Tipo_incarichi ti ON ti.id_tipo_incarico=ip.id_tipo_incarico
                                 LEFT JOIN dbo.Profilo_militare pm ON pm.IDPersonale=ep.IDPersonale AND ep.Militare=1
                                 LEFT JOIN dbo.Gradi g ON g.ID_Grado=pm.ID_Grado
                                 LEFT JOIN dbo.PersCivile pc ON pc.IDPersonale=ep.IDPersonale AND ep.Militare=0
                                 LEFT JOIN dbo.Titoli t ON t.ID_Titolo=pc.ID_TitoloAtt
                                 LEFT JOIN dbo.Liv1Uff u1 ON u1.ID_Uff1=ip.ID_Uff1
                                 LEFT JOIN dbo.Liv2Uff u2 ON u2.ID_Uff1=ip.ID_Uff1 AND u2.ID_Uff2=ip.ID_Uff2
                                 LEFT JOIN dbo.Liv3Uff u3 ON u3.ID_Uff1=ip.ID_Uff1 AND u3.ID_Uff2=ip.ID_Uff2 AND u3.ID_Uff3=ip.ID_Uff3
                                 WHERE LOWER(LTRIM(RTRIM(ep.Stato_Servizio)))='attivo'
                                 ORDER BY ISNULL(NULLIF(u1.Livello,0),999),u1.SgUff1,
                                          ISNULL(NULLIF(u2.Livello,0),999),u2.SgUff2,
                                          ISNULL(NULLIF(u3.Livello,0),999),u3.SgUff3,
                                          ep.Cognome,ep.Nome";
            var risultati = new List<OrganigrammaPersona>();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        risultati.Add(new OrganigrammaPersona {
                            IdPersonale = reader.GetInt32(0), Cognome = Testo(reader, 1), Nome = Testo(reader, 2),
                            Militare = !reader.IsDBNull(3) && reader.GetBoolean(3), GradoProfilo = Testo(reader, 4),
                            Incarico = Testo(reader, 5), IdUfficio1 = Intero(reader, 6), IdUfficio2 = Intero(reader, 7),
                            IdUfficio3 = Intero(reader, 8), Ufficio1 = Testo(reader, 9), Ufficio2 = Testo(reader, 10),
                            Ufficio3 = Testo(reader, 11), Ordinamento1 = reader.GetInt32(12),
                            Ordinamento2 = reader.GetInt32(13), Ordinamento3 = reader.GetInt32(14)
                        });
                    }
                }
            }
            return risultati;
        }

        private static string Testo(SqlDataReader reader, int indice)
        { return reader.IsDBNull(indice) ? string.Empty : Convert.ToString(reader.GetValue(indice)).Trim(); }

        private static int? Intero(SqlDataReader reader, int indice)
        { return reader.IsDBNull(indice) ? (int?)null : Convert.ToInt32(reader.GetValue(indice)); }
    }
}

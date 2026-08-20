using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebGestPersV2.Models;

namespace WebGestPersV2.Data
{
    public sealed class TabelleGestioneRepository
    {
        private static readonly IDictionary<string, TabellaGestioneDefinizione> Definizioni =
            new Dictionary<string, TabellaGestioneDefinizione>(StringComparer.OrdinalIgnoreCase)
            {
                {"titoli-studio", D("titoli-studio","Titoli di studio","comune","SELECT id_Tit_Studio AS ID,Desc_Tit_Studio AS Descrizione,livello AS Ordinamento FROM dbo.Titoli_Studio ORDER BY livello,Desc_Tit_Studio")},
                {"fasce-orarie", D("fasce-orarie","Fasce orarie","comune","SELECT IDFascia AS ID,Descrizione,Note FROM dbo.TipoFascia ORDER BY Descrizione")},
                {"toponomastica", D("toponomastica","Toponomastica","comune","SELECT particella AS Particella FROM dbo.Toponomastica ORDER BY particella")},
                {"incarichi", D("incarichi","Tipi di incarico","comune","SELECT id_tipo_incarico AS ID,Sigla_incarico AS Sigla,Descr_incarico AS Descrizione,livello AS Ordinamento FROM dbo.Tipo_incarichi ORDER BY livello,Descr_incarico")},
                {"uffici-1", D("uffici-1","Uffici di primo livello","comune","SELECT ID_Uff1 AS ID,SgUff1 AS Sigla,DescUff1 AS Descrizione,livello AS Ordinamento,competenze AS Competenze,Attivo FROM dbo.Liv1Uff ORDER BY livello,SgUff1")},
                {"uffici-2", D("uffici-2","Uffici di secondo livello","comune","SELECT u2.ID_Uff2 AS ID,u1.SgUff1 AS [Ufficio superiore],u2.SgUff2 AS Sigla,u2.DescUff2 AS Descrizione,u2.livello AS Ordinamento,u2.competenze AS Competenze FROM dbo.Liv2Uff u2 LEFT JOIN dbo.Liv1Uff u1 ON u1.ID_Uff1=u2.ID_Uff1 ORDER BY u1.livello,u2.livello,u2.SgUff2")},
                {"uffici-3", D("uffici-3","Uffici di terzo livello","comune","SELECT u3.ID_Uff3 AS ID,u1.SgUff1 AS [Primo livello],u2.SgUff2 AS [Secondo livello],u3.SgUff3 AS Sigla,u3.DescUff3 AS Descrizione,u3.livello AS Ordinamento,u3.competenze AS Competenze FROM dbo.Liv3Uff u3 LEFT JOIN dbo.Liv1Uff u1 ON u1.ID_Uff1=u3.ID_Uff1 LEFT JOIN dbo.Liv2Uff u2 ON u2.ID_Uff2=u3.ID_Uff2 ORDER BY u1.livello,u2.livello,u3.livello,u3.SgUff3")},
                {"qualifiche", D("qualifiche","Qualifiche / profili civili","civile","SELECT ID_Titolo AS ID,Sigla_titolo AS Sigla,Descr AS Descrizione,livello AS Ordinamento FROM dbo.Titoli ORDER BY livello,Sigla_titolo")},
                {"posizioni-economiche", D("posizioni-economiche","Posizioni economiche civili","civile","SELECT id_sottoarea AS ID,Area,Sottoarea,Descrizione,livello AS Ordinamento FROM dbo.Aree ORDER BY Area,Sottoarea")},
                {"asl", D("asl","ASL","civile","SELECT id_Asl AS ID,Nome_Asl AS Nome,Indirizzo_Asl AS Indirizzo,Prefisso,Tel_Asl AS Telefono,Fax_Asl AS Fax,Cap,Comune,Provincia FROM dbo.ASL ORDER BY Nome_Asl")},
                {"gradi", M("gradi","Gradi","SELECT g.ID_Grado AS ID,f.SiglaArma AS [Forza armata],g.Categorico,g.SiglaGrado AS Sigla,g.Descr_grado AS Descrizione,g.livello AS Ordinamento,g.livelloGenerale,g.LivelloGerarchico FROM dbo.Gradi g INNER JOIN dbo.FFAA f ON f.ID_Arma=g.ID_Arma WHERE (@arma IS NULL OR g.ID_Arma=@arma) ORDER BY f.livello,g.LivelloGerarchico")},
                {"ruoli", M("ruoli","Ruoli","SELECT r.ID_Ruolo AS ID,f.SiglaArma AS [Forza armata],r.Categorico,r.SiglaRuolo AS Sigla,r.DescRuolo AS Descrizione,r.livello AS Ordinamento FROM dbo.Ruoli r INNER JOIN dbo.FFAA f ON f.ID_Arma=r.ID_Arma WHERE (@arma IS NULL OR r.ID_Arma=@arma) ORDER BY f.livello,CASE r.Categorico WHEN 'U' THEN 1 WHEN 'S' THEN 2 WHEN 'T' THEN 3 ELSE 4 END,r.SiglaRuolo")},
                {"categorie", M("categorie","Categorie militari","SELECT c.ID_Categoria AS ID,f.SiglaArma AS [Forza armata],c.Categorico,c.SiglaCategoria AS Sigla,c.DescCategoria AS Descrizione,c.livello AS Ordinamento FROM dbo.Categorie c INNER JOIN dbo.FFAA f ON f.ID_Arma=c.ID_Arma WHERE (@arma IS NULL OR c.ID_Arma=@arma) ORDER BY f.livello,CASE c.Categorico WHEN 'U' THEN 1 WHEN 'S' THEN 2 WHEN 'T' THEN 3 ELSE 4 END,c.SiglaCategoria")},
                {"specialita", M("specialita","Specialità","SELECT s.[ID_Specialità] AS ID,f.SiglaArma AS [Forza armata],s.Categorico,s.SiglaSpecialita AS Sigla,s.DescSpecialita AS Descrizione,s.livello AS Ordinamento FROM dbo.Specialita s INNER JOIN dbo.FFAA f ON f.ID_Arma=s.ID_Arma WHERE (@arma IS NULL OR s.ID_Arma=@arma) ORDER BY f.livello,CASE s.Categorico WHEN 'U' THEN 1 WHEN 'S' THEN 2 WHEN 'T' THEN 3 ELSE 4 END,s.SiglaSpecialita")}
            };

        public TabellaGestioneDefinizione Trova(string codice)
        {
            TabellaGestioneDefinizione valore;
            return !string.IsNullOrWhiteSpace(codice) && Definizioni.TryGetValue(codice, out valore) ? valore : null;
        }

        public DataTable Elenco(TabellaGestioneDefinizione definizione, int? idArma)
        {
            var tabella = new DataTable();
            using (var connection = new SqlConnection(Db.ConnectionString))
            using (var command = new SqlCommand(definizione.Sql, connection))
            {
                if (definizione.FiltroForzaArmata)
                    command.Parameters.Add("@arma", SqlDbType.Int).Value = idArma.HasValue ? (object)idArma.Value : DBNull.Value;
                connection.Open();
                using (var adapter = new SqlDataAdapter(command)) adapter.Fill(tabella);
            }
            return tabella;
        }

        public IList<LookupItem> ForzeArmate() { return new PersonaleWriteRepository().ForzeArmate(); }

        public bool ModificaSempliceSupportata(string codice)
        {
            return codice == "titoli-studio" || codice == "fasce-orarie" || codice == "toponomastica" || codice == "incarichi" || codice == "qualifiche";
        }

        public bool ModificaUfficioSupportata(string codice)
        { return codice == "uffici-1" || codice == "uffici-2" || codice == "uffici-3"; }

        public bool ModificaCivileSupportata(string codice) { return codice == "posizioni-economiche" || codice == "asl"; }
        public bool ModificaMilitareSupportata(string codice) { return codice == "gradi" || codice == "ruoli" || codice == "categorie" || codice == "specialita"; }

        public bool ModificaSupportata(string codice)
        { return ModificaSempliceSupportata(codice) || ModificaUfficioSupportata(codice) || ModificaCivileSupportata(codice) || ModificaMilitareSupportata(codice); }

        public IList<LookupItem> UfficiPrimoLivello()
        { return Lookup("SELECT ID_Uff1,SgUff1+' - '+DescUff1 FROM dbo.Liv1Uff ORDER BY livello,SgUff1"); }

        public IList<LookupItem> UfficiSecondoLivello(int idUfficio1)
        { return Lookup("SELECT ID_Uff2,SgUff2+' - '+DescUff2 FROM dbo.Liv2Uff WHERE ID_Uff1=@id ORDER BY livello,SgUff2",new SqlParameter("@id",SqlDbType.Int){Value=idUfficio1}); }

        public string[] LeggiUfficio(string codice,string chiave)
        {
            int id;if(!int.TryParse(chiave,out id))throw new InvalidOperationException("Identificativo non valido.");
            string sql;
            if(codice=="uffici-1")sql="SELECT '', '', SgUff1,DescUff1,CONVERT(varchar(20),livello),competenze,Attivo FROM dbo.Liv1Uff WHERE ID_Uff1=@id";
            else if(codice=="uffici-2")sql="SELECT CONVERT(varchar(20),ID_Uff1),'',SgUff2,DescUff2,CONVERT(varchar(20),livello),competenze,'' FROM dbo.Liv2Uff WHERE ID_Uff2=@id";
            else if(codice=="uffici-3")sql="SELECT CONVERT(varchar(20),ID_Uff1),CONVERT(varchar(20),ID_Uff2),SgUff3,DescUff3,CONVERT(varchar(20),livello),competenze,'' FROM dbo.Liv3Uff WHERE ID_Uff3=@id";
            else throw new InvalidOperationException("Tabella uffici non valida.");
            using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c))
            {q.Parameters.Add("@id",SqlDbType.Int).Value=id;c.Open();using(var r=q.ExecuteReader(CommandBehavior.SingleRow)){if(!r.Read())return null;var x=new string[7];for(int i=0;i<x.Length;i++)x[i]=Testo(r,i);return x;}}
        }

        public void SalvaUfficio(string codice,string chiave,int? idUff1,int? idUff2,string sigla,string descrizione,string livello,string competenze,string attivo)
        {
            sigla=(sigla??"").Trim();descrizione=(descrizione??"").Trim();competenze=(competenze??"").Trim();
            if(string.IsNullOrWhiteSpace(descrizione))throw new InvalidOperationException("La descrizione è obbligatoria.");
            if(string.IsNullOrWhiteSpace(sigla))throw new InvalidOperationException("La sigla è obbligatoria.");
            int ordine; if(!string.IsNullOrWhiteSpace(livello)&&!int.TryParse(livello,out ordine))throw new InvalidOperationException("L'ordinamento deve essere numerico.");
            bool nuovo=string.IsNullOrWhiteSpace(chiave);int id=0;
            if(!nuovo&&!int.TryParse(chiave,out id))throw new InvalidOperationException("Identificativo non valido.");
            if(codice!="uffici-1"&&!idUff1.HasValue)throw new InvalidOperationException("Selezionare l'ufficio di primo livello.");
            if(codice=="uffici-3"&&!idUff2.HasValue)throw new InvalidOperationException("Selezionare l'ufficio di secondo livello.");
            using(var c=new SqlConnection(Db.ConnectionString)){c.Open();SqlCommand q;
                if(codice=="uffici-1")q=new SqlCommand(nuovo?"INSERT dbo.Liv1Uff(SgUff1,DescUff1,livello,competenze,Attivo) VALUES(@s,@d,@l,@c,@a)":"UPDATE dbo.Liv1Uff SET SgUff1=@s,DescUff1=@d,livello=@l,competenze=@c,Attivo=@a WHERE ID_Uff1=@id",c);
                else if(codice=="uffici-2")q=new SqlCommand(nuovo?"INSERT dbo.Liv2Uff(SgUff2,DescUff2,livello,ID_Uff1,competenze) VALUES(@s,@d,@l,@u1,@c)":"UPDATE dbo.Liv2Uff SET SgUff2=@s,DescUff2=@d,livello=@l,ID_Uff1=@u1,competenze=@c WHERE ID_Uff2=@id",c);
                else if(codice=="uffici-3")q=new SqlCommand(nuovo?"INSERT dbo.Liv3Uff(SgUff3,DescUff3,livello,ID_Uff1,ID_Uff2,competenze) VALUES(@s,@d,@l,@u1,@u2,@c)":"UPDATE dbo.Liv3Uff SET SgUff3=@s,DescUff3=@d,livello=@l,ID_Uff1=@u1,ID_Uff2=@u2,competenze=@c WHERE ID_Uff3=@id",c);
                else throw new InvalidOperationException("Tabella uffici non valida.");
                q.Parameters.Add("@s",SqlDbType.VarChar,50).Value=sigla;q.Parameters.Add("@d",SqlDbType.VarChar,255).Value=descrizione;
                q.Parameters.Add("@l",SqlDbType.Int).Value=int.TryParse(livello,out ordine)?(object)ordine:DBNull.Value;q.Parameters.Add("@c",SqlDbType.VarChar,255).Value=string.IsNullOrEmpty(competenze)?(object)DBNull.Value:competenze;
                if(codice=="uffici-1")q.Parameters.Add("@a",SqlDbType.Char,1).Value=attivo=="N"?"N":"S";
                if(codice!="uffici-1")q.Parameters.Add("@u1",SqlDbType.Int).Value=idUff1.Value;
                if(codice=="uffici-3")
                { if(Conteggio(c,"SELECT COUNT(*) FROM dbo.Liv2Uff WHERE ID_Uff2=@u2 AND ID_Uff1=@u1",idUff1.Value,idUff2.Value)!=1)throw new InvalidOperationException("Il secondo livello non appartiene al primo livello selezionato.");q.Parameters.Add("@u2",SqlDbType.Int).Value=idUff2.Value; }
                if(!nuovo)q.Parameters.Add("@id",SqlDbType.Int).Value=id;
                if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Ufficio non trovato o non aggiornato.");
            }
        }

        private static int Conteggio(SqlConnection c,string sql,int u1,int u2){using(var q=new SqlCommand(sql,c)){q.Parameters.Add("@u1",SqlDbType.Int).Value=u1;q.Parameters.Add("@u2",SqlDbType.Int).Value=u2;return Convert.ToInt32(q.ExecuteScalar());}}
        private static IList<LookupItem> Lookup(string sql,params SqlParameter[] ps){var x=new List<LookupItem>();using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c)){q.Parameters.AddRange(ps);c.Open();using(var r=q.ExecuteReader())while(r.Read())x.Add(new LookupItem{Value=Convert.ToString(r.GetValue(0)),Text=Convert.ToString(r.GetValue(1))});}return x;}

        public string[] LeggiSemplice(string codice, string chiave)
        {
            string sql;
            bool chiaveNumerica = codice != "toponomastica";
            switch (codice)
            {
                case "titoli-studio": sql="SELECT Desc_Tit_Studio,CONVERT(varchar(20),livello),'' FROM dbo.Titoli_Studio WHERE id_Tit_Studio=@id"; break;
                case "fasce-orarie": sql="SELECT Descrizione,Note,'' FROM dbo.TipoFascia WHERE IDFascia=@id"; break;
                case "toponomastica": sql="SELECT particella,'','' FROM dbo.Toponomastica WHERE particella=@id"; break;
                case "incarichi": sql="SELECT Sigla_incarico,Descr_incarico,CONVERT(varchar(20),livello) FROM dbo.Tipo_incarichi WHERE id_tipo_incarico=@id"; break;
                case "qualifiche": sql="SELECT Sigla_titolo,Descr,CONVERT(varchar(20),livello) FROM dbo.Titoli WHERE ID_Titolo=@id"; break;
                default: throw new InvalidOperationException("Tabella non modificabile.");
            }
            using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c))
            {
                int id;
                if(chiaveNumerica)
                {
                    if(!int.TryParse(chiave,out id))throw new InvalidOperationException("Identificativo non valido.");
                    q.Parameters.Add("@id",SqlDbType.Int).Value=id;
                }
                else q.Parameters.Add("@id",SqlDbType.VarChar,50).Value=chiave;
                c.Open(); using(var r=q.ExecuteReader(CommandBehavior.SingleRow))
                { if(!r.Read()) return null; return new[]{Testo(r,0),Testo(r,1),Testo(r,2)}; }
            }
        }

        public void SalvaSemplice(string codice, string chiave, string valore1, string valore2, string valore3)
        {
            valore1=(valore1??"").Trim(); valore2=(valore2??"").Trim(); valore3=(valore3??"").Trim();
            if(string.IsNullOrWhiteSpace(valore1)) throw new InvalidOperationException("Il primo campo è obbligatorio.");
            bool nuovo=string.IsNullOrWhiteSpace(chiave); int id=0; int ordinamento;
            using(var c=new SqlConnection(Db.ConnectionString))
            {
                c.Open(); SqlCommand q;
                switch(codice)
                {
                    case "titoli-studio":
                        if(!string.IsNullOrEmpty(valore2)&&!int.TryParse(valore2,out ordinamento))throw new InvalidOperationException("L'ordinamento deve essere numerico.");
                        q=new SqlCommand(nuovo?"INSERT dbo.Titoli_Studio(Desc_Tit_Studio,livello) VALUES(@a,@b)":"UPDATE dbo.Titoli_Studio SET Desc_Tit_Studio=@a,livello=@b WHERE id_Tit_Studio=@id",c);
                        q.Parameters.Add("@a",SqlDbType.VarChar,100).Value=valore1; q.Parameters.Add("@b",SqlDbType.Int).Value=int.TryParse(valore2,out ordinamento)?(object)ordinamento:DBNull.Value; break;
                    case "fasce-orarie":
                        q=new SqlCommand(nuovo?"INSERT dbo.TipoFascia(Descrizione,Note) VALUES(@a,@b)":"UPDATE dbo.TipoFascia SET Descrizione=@a,Note=@b WHERE IDFascia=@id",c);
                        q.Parameters.Add("@a",SqlDbType.VarChar,50).Value=valore1; q.Parameters.Add("@b",SqlDbType.VarChar,255).Value=string.IsNullOrEmpty(valore2)?(object)DBNull.Value:valore2; break;
                    case "toponomastica":
                        q=new SqlCommand(nuovo?"INSERT dbo.Toponomastica(particella) VALUES(@a)":"UPDATE dbo.Toponomastica SET particella=@a WHERE particella=@originale",c);
                        q.Parameters.Add("@a",SqlDbType.VarChar,50).Value=valore1; if(!nuovo)q.Parameters.Add("@originale",SqlDbType.VarChar,50).Value=chiave; break;
                    case "incarichi":
                        if(string.IsNullOrWhiteSpace(valore2))throw new InvalidOperationException("La descrizione è obbligatoria.");
                        if(!string.IsNullOrEmpty(valore3)&&!int.TryParse(valore3,out ordinamento))throw new InvalidOperationException("L'ordinamento deve essere numerico.");
                        q=new SqlCommand(nuovo?"INSERT dbo.Tipo_incarichi(Sigla_incarico,Descr_incarico,livello) VALUES(@a,@b,@c)":"UPDATE dbo.Tipo_incarichi SET Sigla_incarico=@a,Descr_incarico=@b,livello=@c WHERE id_tipo_incarico=@id",c);
                        q.Parameters.Add("@a",SqlDbType.VarChar,15).Value=valore1; q.Parameters.Add("@b",SqlDbType.VarChar,50).Value=valore2; q.Parameters.Add("@c",SqlDbType.Int).Value=int.TryParse(valore3,out ordinamento)?(object)ordinamento:DBNull.Value; break;
                    case "qualifiche":
                        if(!string.IsNullOrEmpty(valore3)&&!int.TryParse(valore3,out ordinamento))throw new InvalidOperationException("L'ordinamento deve essere numerico.");
                        q=new SqlCommand(nuovo?"INSERT dbo.Titoli(Sigla_titolo,Descr,livello) VALUES(@a,@b,@c)":"UPDATE dbo.Titoli SET Sigla_titolo=@a,Descr=@b,livello=@c WHERE ID_Titolo=@id",c);
                        q.Parameters.Add("@a",SqlDbType.VarChar,20).Value=valore1;q.Parameters.Add("@b",SqlDbType.VarChar,200).Value=string.IsNullOrEmpty(valore2)?(object)DBNull.Value:valore2;q.Parameters.Add("@c",SqlDbType.Int).Value=int.TryParse(valore3,out ordinamento)?(object)ordinamento:DBNull.Value;break;
                    default: throw new InvalidOperationException("Tabella non modificabile.");
                }
                if(!nuovo&&codice!="toponomastica")
                { if(!int.TryParse(chiave,out id))throw new InvalidOperationException("Identificativo non valido."); q.Parameters.Add("@id",SqlDbType.Int).Value=id; }
                if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Elemento non trovato o non aggiornato.");
            }
        }

        public string[] LeggiCivile(string codice,string chiave)
        {
            int id;if(!int.TryParse(chiave,out id))throw new InvalidOperationException("Identificativo non valido.");string sql;
            if(codice=="posizioni-economiche")sql="SELECT Area,Sottoarea,Descrizione,CONVERT(varchar(20),livello),'','','','','' FROM dbo.Aree WHERE id_sottoarea=@id";
            else if(codice=="asl")sql="SELECT Nome_Asl,Indirizzo_Asl,CONVERT(varchar(30),Tel_Asl),CONVERT(varchar(20),Prefisso),CONVERT(varchar(30),Fax_Asl),CONVERT(varchar(20),Cap),Comune,Provincia,'' FROM dbo.ASL WHERE id_Asl=@id";
            else throw new InvalidOperationException("Tabella civile non valida.");
            using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c)){q.Parameters.Add("@id",SqlDbType.Int).Value=id;c.Open();using(var r=q.ExecuteReader(CommandBehavior.SingleRow)){if(!r.Read())return null;var x=new string[9];for(int i=0;i<x.Length;i++)x[i]=Testo(r,i);return x;}}
        }

        public void SalvaCivile(string codice,string chiave,string[] v)
        {
            for(int i=0;i<v.Length;i++)v[i]=(v[i]??"").Trim();bool nuovo=string.IsNullOrWhiteSpace(chiave);int id=0;
            if(!nuovo&&!int.TryParse(chiave,out id))throw new InvalidOperationException("Identificativo non valido.");
            using(var c=new SqlConnection(Db.ConnectionString)){c.Open();SqlCommand q;int ordine;
                if(codice=="posizioni-economiche")
                {
                    if(string.IsNullOrEmpty(v[0])||string.IsNullOrEmpty(v[1]))throw new InvalidOperationException("Area e posizione economica sono obbligatorie.");
                    if(!string.IsNullOrEmpty(v[3])&&!int.TryParse(v[3],out ordine))throw new InvalidOperationException("L'ordinamento deve essere numerico.");
                    q=new SqlCommand(nuovo?"INSERT dbo.Aree(Area,Sottoarea,Descrizione,livello) VALUES(@a,@s,@d,@l)":"UPDATE dbo.Aree SET Area=@a,Sottoarea=@s,Descrizione=@d,livello=@l WHERE id_sottoarea=@id",c);
                    q.Parameters.Add("@a",SqlDbType.VarChar,2).Value=v[0];q.Parameters.Add("@s",SqlDbType.VarChar,15).Value=v[1];q.Parameters.Add("@d",SqlDbType.VarChar,50).Value=Vuoto(v[2]);q.Parameters.Add("@l",SqlDbType.Int).Value=int.TryParse(v[3],out ordine)?(object)ordine:DBNull.Value;
                }
                else if(codice=="asl")
                {
                    if(string.IsNullOrEmpty(v[0])||string.IsNullOrEmpty(v[1])||string.IsNullOrEmpty(v[6])||string.IsNullOrEmpty(v[7]))throw new InvalidOperationException("Nome, indirizzo, comune e provincia sono obbligatori.");
                    decimal numero;int intero;ValidaDecimale(v[2],"telefono",out numero);ValidaIntero(v[3],"prefisso",out intero);ValidaDecimale(v[4],"fax",out numero);ValidaIntero(v[5],"CAP",out intero);
                    q=new SqlCommand(nuovo?"INSERT dbo.ASL(Nome_Asl,Indirizzo_Asl,Tel_Asl,Prefisso,Fax_Asl,Cap,Comune,Provincia) VALUES(@n,@i,@t,@p,@f,@c,@co,@pr)":"UPDATE dbo.ASL SET Nome_Asl=@n,Indirizzo_Asl=@i,Tel_Asl=@t,Prefisso=@p,Fax_Asl=@f,Cap=@c,Comune=@co,Provincia=@pr WHERE id_Asl=@id",c);
                    q.Parameters.Add("@n",SqlDbType.VarChar,20).Value=v[0];q.Parameters.Add("@i",SqlDbType.VarChar,100).Value=v[1];
                    var telefono=q.Parameters.Add("@t",SqlDbType.Decimal);telefono.Precision=15;telefono.Scale=0;telefono.Value=Decimale(v[2]);
                    q.Parameters.Add("@p",SqlDbType.Int).Value=Intero(v[3]);var fax=q.Parameters.Add("@f",SqlDbType.Decimal);fax.Precision=15;fax.Scale=0;fax.Value=Decimale(v[4]);
                    q.Parameters.Add("@c",SqlDbType.Int).Value=Intero(v[5]);q.Parameters.Add("@co",SqlDbType.VarChar,20).Value=v[6];q.Parameters.Add("@pr",SqlDbType.VarChar,20).Value=v[7];
                }
                else throw new InvalidOperationException("Tabella civile non valida.");
                if(!nuovo)q.Parameters.Add("@id",SqlDbType.Int).Value=id;if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Elemento non trovato o non aggiornato.");
            }
        }

        private static object Vuoto(string x){return string.IsNullOrEmpty(x)?(object)DBNull.Value:x;}
        private static object Decimale(string x){decimal n;return decimal.TryParse(x,out n)?(object)n:DBNull.Value;}
        private static object Intero(string x){int n;return int.TryParse(x,out n)?(object)n:DBNull.Value;}
        private static void ValidaDecimale(string x,string nome,out decimal n){n=0;if(!string.IsNullOrEmpty(x)&&!decimal.TryParse(x,out n))throw new InvalidOperationException("Il campo "+nome+" deve essere numerico.");}
        private static void ValidaIntero(string x,string nome,out int n){n=0;if(!string.IsNullOrEmpty(x)&&!int.TryParse(x,out n))throw new InvalidOperationException("Il campo "+nome+" deve essere numerico.");}

        public string[] LeggiMilitare(string codice,string chiave)
        {
            int id;if(!int.TryParse(chiave,out id))throw new InvalidOperationException("Identificativo non valido.");string sql;
            if(codice=="gradi")sql="SELECT CONVERT(varchar(20),ID_Arma),Categorico,SiglaGrado,Descr_grado,CONVERT(varchar(20),livello),CONVERT(varchar(20),livelloGenerale),CONVERT(varchar(20),LivelloGerarchico) FROM dbo.Gradi WHERE ID_Grado=@id";
            else if(codice=="ruoli")sql="SELECT CONVERT(varchar(20),ID_Arma),Categorico,SiglaRuolo,DescRuolo,CONVERT(varchar(20),livello),'','' FROM dbo.Ruoli WHERE ID_Ruolo=@id";
            else if(codice=="categorie")sql="SELECT CONVERT(varchar(20),ID_Arma),Categorico,SiglaCategoria,DescCategoria,CONVERT(varchar(20),livello),'','' FROM dbo.Categorie WHERE ID_Categoria=@id";
            else if(codice=="specialita")sql="SELECT CONVERT(varchar(20),ID_Arma),Categorico,SiglaSpecialita,DescSpecialita,CONVERT(varchar(20),livello),'','' FROM dbo.Specialita WHERE [ID_Specialità]=@id";
            else throw new InvalidOperationException("Tabella militare non valida.");
            using(var c=new SqlConnection(Db.ConnectionString))using(var q=new SqlCommand(sql,c)){q.Parameters.Add("@id",SqlDbType.Int).Value=id;c.Open();using(var r=q.ExecuteReader(CommandBehavior.SingleRow)){if(!r.Read())return null;var x=new string[7];for(int i=0;i<x.Length;i++)x[i]=Testo(r,i);return x;}}
        }

        public void SalvaMilitare(string codice,string chiave,int? idArma,string categorico,string sigla,string descrizione,string livello,string livelloGenerale,string livelloGerarchico)
        {
            categorico=(categorico??"").Trim();sigla=(sigla??"").Trim();descrizione=(descrizione??"").Trim();
            if(!idArma.HasValue)throw new InvalidOperationException("Selezionare la forza armata.");
            if(categorico!="U"&&categorico!="S"&&categorico!="T")throw new InvalidOperationException("Selezionare un categorico valido.");
            if(string.IsNullOrEmpty(sigla))throw new InvalidOperationException("La sigla è obbligatoria.");
            int ordine,generale,gerarchico;ValidaIntero(livello,"ordinamento",out ordine);ValidaIntero(livelloGenerale,"livello generale",out generale);ValidaIntero(livelloGerarchico,"livello gerarchico",out gerarchico);
            if(codice=="gradi"&&string.IsNullOrWhiteSpace(livello))throw new InvalidOperationException("L'ordinamento del grado è obbligatorio.");
            bool nuovo=string.IsNullOrWhiteSpace(chiave);int id=0;if(!nuovo&&!int.TryParse(chiave,out id))throw new InvalidOperationException("Identificativo non valido.");
            using(var c=new SqlConnection(Db.ConnectionString)){c.Open();SqlCommand q;
                if(codice=="gradi")q=new SqlCommand(nuovo?"INSERT dbo.Gradi(Categorico,ID_Arma,Descr_grado,SiglaGrado,livello,livelloGenerale,LivelloGerarchico) VALUES(@cat,@arma,@d,@s,@l,@lg,@lh)":"UPDATE dbo.Gradi SET Categorico=@cat,ID_Arma=@arma,Descr_grado=@d,SiglaGrado=@s,livello=@l,livelloGenerale=@lg,LivelloGerarchico=@lh WHERE ID_Grado=@id",c);
                else if(codice=="ruoli")q=new SqlCommand(nuovo?"INSERT dbo.Ruoli(Categorico,ID_Arma,SiglaRuolo,DescRuolo,livello) VALUES(@cat,@arma,@s,@d,@l)":"UPDATE dbo.Ruoli SET Categorico=@cat,ID_Arma=@arma,SiglaRuolo=@s,DescRuolo=@d,livello=@l WHERE ID_Ruolo=@id",c);
                else if(codice=="categorie")q=new SqlCommand(nuovo?"INSERT dbo.Categorie(Categorico,ID_Arma,SiglaCategoria,DescCategoria,livello) VALUES(@cat,@arma,@s,@d,@l)":"UPDATE dbo.Categorie SET Categorico=@cat,ID_Arma=@arma,SiglaCategoria=@s,DescCategoria=@d,livello=@l WHERE ID_Categoria=@id",c);
                else if(codice=="specialita")q=new SqlCommand(nuovo?"INSERT dbo.Specialita(Categorico,ID_Arma,SiglaSpecialita,DescSpecialita,livello) VALUES(@cat,@arma,@s,@d,@l)":"UPDATE dbo.Specialita SET Categorico=@cat,ID_Arma=@arma,SiglaSpecialita=@s,DescSpecialita=@d,livello=@l WHERE [ID_Specialità]=@id",c);
                else throw new InvalidOperationException("Tabella militare non valida.");
                q.Parameters.Add("@cat",SqlDbType.VarChar,1).Value=categorico;q.Parameters.Add("@arma",SqlDbType.Int).Value=idArma.Value;
                q.Parameters.Add("@s",SqlDbType.VarChar,codice=="gradi"?20:10).Value=sigla;q.Parameters.Add("@d",SqlDbType.VarChar,50).Value=Vuoto(descrizione);
                q.Parameters.Add("@l",SqlDbType.Int).Value=Intero(livello);
                if(codice=="gradi"){q.Parameters.Add("@lg",SqlDbType.Int).Value=Intero(livelloGenerale);q.Parameters.Add("@lh",SqlDbType.Int).Value=Intero(livelloGerarchico);}
                if(!nuovo)q.Parameters.Add("@id",SqlDbType.Int).Value=id;if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Elemento non trovato o non aggiornato.");
            }
        }

        private static string Testo(SqlDataReader r,int i){return r.IsDBNull(i)?"":Convert.ToString(r.GetValue(i));}

        private static TabellaGestioneDefinizione D(string codice,string titolo,string ambito,string sql)
        { return new TabellaGestioneDefinizione { Codice=codice,Titolo=titolo,Ambito=ambito,Sql=sql }; }
        private static TabellaGestioneDefinizione M(string codice,string titolo,string sql)
        { var d=D(codice,titolo,"militare",sql); d.FiltroForzaArmata=true; return d; }
    }
}

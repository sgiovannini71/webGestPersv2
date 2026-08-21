<%@ Page Title="Report IN/OUT" Language="C#" MasterPageFile="~/Site.Master" Inherits="WebGestPersV2.HomePage" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Text" %>
<script runat="server">
 public sealed class Movimento
 {
  public int IdPersonale { get; set; } public string Cognome { get; set; } public string Nome { get; set; }
  public string TipoPersonale { get; set; } public string GradoProfilo { get; set; } public string StatoServizio { get; set; }
  public DateTime DataEvento { get; set; } public string DataEventoTesto { get { return DataEvento.ToString("dd/MM/yyyy"); } }
 }
 protected override void OnLoad(EventArgs e) { base.OnLoad(e); if (!IsPostBack) Carica(); }
 protected void Period_Changed(object sender,EventArgs e) { Carica(); }
 protected void ExportIncoming_Click(object sender,EventArgs e) { Esporta(Leggi(true,Mesi(IncomingPeriod.SelectedValue)),"Personale_entrato.csv","Data ingresso"); }
 protected void ExportOutgoing_Click(object sender,EventArgs e) { Esporta(Leggi(false,Mesi(OutgoingPeriod.SelectedValue)),"Personale_uscito.csv","Data uscita"); }
 private void Carica()
 {
  try { IList<Movimento> a=Leggi(true,Mesi(IncomingPeriod.SelectedValue)),u=Leggi(false,Mesi(OutgoingPeriod.SelectedValue)); IncomingGrid.DataSource=a;IncomingGrid.DataBind();IncomingCount.Text=a.Count+" risultati";OutgoingGrid.DataSource=u;OutgoingGrid.DataBind();OutgoingCount.Text=u.Count+" risultati"; }
  catch(Exception ex) { Message.Text=Server.HtmlEncode(ex.Message);Message.Visible=true; }
 }
 private IList<Movimento> Leggi(bool ingressi,int mesi)
 {
  if(mesi!=3&&mesi!=6&&mesi!=12&&mesi!=24&&mesi!=36)throw new InvalidOperationException("Intervallo temporale non valido.");
  string campo=ingressi?"ep.Data_Ass_Armaereo":"ep.Data_Usc_Armaereo";
  string sql=@"SELECT ep.IDPersonale,ep.Cognome,ep.Nome,ep.Militare,ep.Stato_Servizio,CASE WHEN ep.Militare=1 THEN g.SiglaGrado ELSE t.Sigla_titolo END,"+campo+@" FROM dbo.ElencoPersonale ep LEFT JOIN dbo.Profilo_militare pm ON pm.IDPersonale=ep.IDPersonale AND ep.Militare=1 LEFT JOIN dbo.Gradi g ON g.ID_Grado=pm.ID_Grado LEFT JOIN dbo.PersCivile pc ON pc.IDPersonale=ep.IDPersonale AND ep.Militare=0 LEFT JOIN dbo.Titoli t ON t.ID_Titolo=pc.ID_TitoloAtt WHERE "+campo+@">=DATEADD(MONTH,-@mesi,GETDATE()) ORDER BY "+campo+" DESC,ep.Cognome,ep.Nome";
  var x=new List<Movimento>(); string cs=ConfigurationManager.ConnectionStrings["DipendentiDB"].ConnectionString;
  using(var c=new SqlConnection(cs))using(var q=new SqlCommand(sql,c)){q.Parameters.Add("@mesi",SqlDbType.Int).Value=mesi;c.Open();using(var r=q.ExecuteReader())while(r.Read())x.Add(new Movimento{IdPersonale=r.GetInt32(0),Cognome=T(r,1),Nome=T(r,2),TipoPersonale=r.GetBoolean(3)?"Militare":"Civile",StatoServizio=T(r,4),GradoProfilo=T(r,5),DataEvento=r.GetDateTime(6)});}return x;
 }
 private void Esporta(IList<Movimento> dati,string file,string dataHeader) { var s=new StringBuilder();s.AppendLine("ID;Cognome;Nome;Grado/Profilo;Tipo;Stato servizio;"+dataHeader);foreach(var x in dati)s.AppendLine(string.Join(";",x.IdPersonale,Csv(x.Cognome),Csv(x.Nome),Csv(x.GradoProfilo),Csv(x.TipoPersonale),Csv(x.StatoServizio),x.DataEvento.ToString("dd/MM/yyyy")));Response.Clear();Response.ContentType="text/csv";Response.ContentEncoding=Encoding.UTF8;Response.AddHeader("Content-Disposition","attachment; filename="+file);Response.BinaryWrite(Encoding.UTF8.GetPreamble());Response.Write(s.ToString());Response.End(); }
 private static string T(SqlDataReader r,int i) { return r.IsDBNull(i)?"":Convert.ToString(r.GetValue(i)).Trim(); }
 private static string Csv(string x) { return "\""+(x??"").Replace("\"","\"\"")+"\""; }
 private static int Mesi(string x) { int n;if(int.TryParse(x,out n))return n;throw new InvalidOperationException("Selezionare un periodo valido."); }
</script>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Report IN/OUT</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href="Indice.aspx">← Torna ai report</a><div class="page-heading"><p class="eyebrow">Report IN/OUT</p><h1>Personale in entrata e uscita</h1><p>Movimenti registrati nel periodo selezionato, calcolato rispetto alla data corrente.</p></div><asp:Label ID="Message" runat="server" CssClass="message error" Visible="false" />
 <div class="movement-grid">
  <section class="card report-panel"><h2>Personale entrato</h2><div class="report-toolbar"><label>Periodo</label><asp:DropDownList ID="IncomingPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Period_Changed"><asp:ListItem Value="3">Ultimi 3 mesi</asp:ListItem><asp:ListItem Value="6">Ultimi 6 mesi</asp:ListItem><asp:ListItem Value="12" Selected="True">Ultimi 12 mesi</asp:ListItem><asp:ListItem Value="24">Ultimi 24 mesi</asp:ListItem><asp:ListItem Value="36">Ultimi 36 mesi</asp:ListItem></asp:DropDownList><asp:Button ID="ExportIncoming" runat="server" Text="Esporta CSV" CssClass="button secondary" OnClick="ExportIncoming_Click" /></div><asp:Label ID="IncomingCount" runat="server" CssClass="result-count" /><asp:GridView ID="IncomingGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessun ingresso nel periodo selezionato."><Columns><asp:BoundField DataField="Cognome" HeaderText="Cognome" /><asp:BoundField DataField="Nome" HeaderText="Nome" /><asp:BoundField DataField="GradoProfilo" HeaderText="Grado/Profilo" /><asp:BoundField DataField="TipoPersonale" HeaderText="Tipo" /><asp:BoundField DataField="DataEventoTesto" HeaderText="Data ingresso" /></Columns></asp:GridView></section>
  <section class="card report-panel"><h2>Personale uscito</h2><div class="report-toolbar"><label>Periodo</label><asp:DropDownList ID="OutgoingPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Period_Changed"><asp:ListItem Value="3">Ultimi 3 mesi</asp:ListItem><asp:ListItem Value="6">Ultimi 6 mesi</asp:ListItem><asp:ListItem Value="12" Selected="True">Ultimi 12 mesi</asp:ListItem><asp:ListItem Value="24">Ultimi 24 mesi</asp:ListItem><asp:ListItem Value="36">Ultimi 36 mesi</asp:ListItem></asp:DropDownList><asp:Button ID="ExportOutgoing" runat="server" Text="Esporta CSV" CssClass="button secondary" OnClick="ExportOutgoing_Click" /></div><asp:Label ID="OutgoingCount" runat="server" CssClass="result-count" /><asp:GridView ID="OutgoingGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessuna uscita nel periodo selezionato."><Columns><asp:BoundField DataField="Cognome" HeaderText="Cognome" /><asp:BoundField DataField="Nome" HeaderText="Nome" /><asp:BoundField DataField="GradoProfilo" HeaderText="Grado/Profilo" /><asp:BoundField DataField="TipoPersonale" HeaderText="Tipo" /><asp:BoundField DataField="DataEventoTesto" HeaderText="Data uscita" /></Columns></asp:GridView></section>
 </div>
</asp:Content>

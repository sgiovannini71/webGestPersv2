<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebGestPersV2.HomePage" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Home</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="hero">
        <p class="eyebrow">Nuova applicazione</p>
        <h1>Gestione del personale</h1>
        <p>Gestione delle informazioni anagrafiche, dei profili e degli incarichi del personale.</p>
        <div class="home-actions"><a class="button" href="Personale/Elenco.aspx">Apri elenco personale</a><a class="button secondary" href="PersonaleEsterno/Elenco.aspx">Personale esterno</a><asp:HyperLink ID="NonActivePersonnelLink" runat="server" NavigateUrl="~/Personale/NonAttivi.aspx" CssClass="button secondary">Personale non attivo</asp:HyperLink><a class="button secondary" href="Organigramma.aspx">Personale per unità</a><a class="button secondary" href="<%= Server.HtmlEncode(OrganizationChartUrl) %>" target="_blank" rel="noopener noreferrer">Organigramma</a><a class="button secondary" href="Help/Default.aspx">Help</a></div>
    </section>
    <section class="home-reports" aria-labelledby="reports-title">
        <div class="section-heading"><p class="eyebrow">Analisi ed esportazioni</p><h2 id="reports-title">Report</h2><p>Prospetti operativi derivati dai report della versione legacy.</p></div>
        <div class="report-card-grid">
            <a class="card report-card" href="Report/InOut.aspx"><span class="report-mark">IN/OUT</span><h3>Personale in entrata e uscita</h3><p>Movimenti del personale per intervalli temporali ed esportazioni.</p></a>
            <a class="card report-card" href="Report/Chart.aspx"><span class="report-mark">CHART</span><h3>Composizione del personale</h3><p>Consistenza e distribuzione per tipologia, sesso, FFAA e profilo.</p></a>
            <a class="card report-card" href="Report/CasellePostali.aspx"><span class="report-mark">POSTA</span><h3>Caselle postali</h3><p>Associazioni, copertura e anomalie delle caselle del personale.</p></a>
            <a class="card report-card" href="Report/Ttoo.aspx"><span class="report-mark">TTOO</span><h3>Prospetti TTOO</h3><p>Estrazioni operative, struttura degli uffici e organigramma.</p></a>
            <a class="card report-card" href="Report/Feo.aspx"><span class="report-mark">FEO</span><h3>Forza effettiva organica</h3><p>Cruscotto e distribuzione del personale attivo fino al terzo livello.</p></a>
        </div>
        <a class="button reports-button" href="Report/Indice.aspx">Apri tutti i report</a>
    </section>
</asp:Content>

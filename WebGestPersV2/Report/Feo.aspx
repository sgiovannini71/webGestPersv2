<%@ Page Title="FEO" Language="C#" MasterPageFile="~/Site.Master" Inherits="WebGestPersV2.HomePage" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">FEO</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href="Indice.aspx">← Torna ai report</a>
 <div class="page-heading"><p class="eyebrow">Forza effettiva organica</p><h1>Report FEO</h1><p>Analisi del personale attivo effettivamente assegnato alle unità organizzative tramite l’incarico principale.</p></div>
 <nav class="section-nav"><a href="#lettura">Cosa rappresenta</a><a href="#funzioni">Funzioni</a><a href="FeoCruscotto.aspx">Cruscotto</a><a href="FeoDistribuzione.aspx?livello=1">Distribuzione</a><a href="TtooConsistenza.aspx">Analisi organico</a></nav>
 <section id="lettura" class="card detail-section anchor-section"><h2>Cosa rappresenta</h2><p>La FEO rappresenta la forza realmente presente: comprende il personale con stato di servizio attivo e considera l’unità organizzativa indicata nell’incarico principale. I conteggi sono distinti tra personale militare e civile.</p><h3>Cosa è possibile analizzare</h3><p>È possibile osservare la consistenza complessiva, confrontare il peso dei reparti, approfondire divisioni e sezioni e verificare, attraverso il confronto con le TTOO, coperture, vacanze ed eccedenze.</p></section>
 <div id="funzioni" class="report-card-grid anchor-section" style="margin-top:18px">
  <section class="card report-card"><span class="report-mark">DIREZIONALE</span><h3>Cruscotto FEO</h3><p>Indicatori complessivi, reparto maggiore, consistenza media e graduatoria dei reparti.</p><a class="button reports-button" href="FeoCruscotto.aspx">Apri cruscotto</a></section>
  <section class="card report-card"><span class="report-mark">STRUTTURA</span><h3>Distribuzione FEO</h3><p>Distribuzione di militari e civili con navigazione fino al terzo livello.</p><a class="button reports-button" href="FeoDistribuzione.aspx?livello=1">Esplora struttura</a></section>
  <section class="card report-card"><span class="report-mark">ORGANICO</span><h3>Analisi TTOO/FEO</h3><p>Confronto tra personale previsto e presente, con scoperture, eccedenze e copertura.</p><a class="button reports-button" href="TtooConsistenza.aspx">Analizza organico</a></section>
 </div>
</asp:Content>

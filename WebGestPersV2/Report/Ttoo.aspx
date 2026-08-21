<%@ Page Title="TTOO" Language="C#" MasterPageFile="~/Site.Master" Inherits="WebGestPersV2.HomePage" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">TTOO</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href="Indice.aspx">← Torna ai report</a>
 <div class="page-heading"><p class="eyebrow">Tabelle teoriche organiche ordinative</p><h1>Gestione TTOO</h1><p>Definizione delle posizioni previste e confronto con la forza effettivamente presente nelle unità organizzative.</p></div>
 <nav class="section-nav" aria-label="Navigazione TTOO"><a href="#lettura">Cosa rappresenta</a><a href="#funzioni">Funzioni</a><a href="TtooPosizioni.aspx">Posizioni TTOO</a><a href="TtooConsistenza.aspx">Confronto TTOO/FEO</a><a href="TtooConsistenza.aspx?livello=1">1° livello</a><a href="TtooConsistenza.aspx?livello=2">2° livello</a><a href="TtooConsistenza.aspx?livello=3">3° livello</a></nav>
 <section id="lettura" class="card detail-section anchor-section"><h2>Cosa rappresenta</h2><p>Le TTOO descrivono l’organico teoricamente previsto per ciascuna unità organizzativa. Ogni posizione indica dove è prevista una risorsa, quale incarico deve ricoprire e, secondo il tipo di personale, la categoria militare e le Forze Armate ammesse oppure il profilo civile richiesto.</p><h3>Cosa è possibile analizzare</h3><p>Da questa sezione si possono amministrare le posizioni previste e confrontarle con la forza effettiva organica (FEO), individuando unità coperte, parzialmente coperte, scoperte o in eccedenza ai diversi livelli della struttura.</p></section>
 <div id="funzioni" class="help-grid anchor-section">
  <section class="card help-card"><span class="report-mark">GESTIONE</span><h2>Posizioni TTOO</h2><p>Inserimento, consultazione, modifica e disattivazione delle posizioni previste, fino all’ufficio di terzo livello.</p><a class="button" href="TtooPosizioni.aspx">Gestisci posizioni</a></section>
  <section class="card help-card"><span class="report-mark">REPORT</span><h2>Confronto TTOO/FEO</h2><p>Confronto tra personale previsto ed effettivo, scoperture, eccedenze e percentuale di copertura.</p><a class="button" href="TtooConsistenza.aspx">Apri confronto</a></section>
 </div>
</asp:Content>

<%@ Page Title="Help" Language="C#" MasterPageFile="~/Site.Master" Inherits="WebGestPersV2.HomePage" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Help</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href="../Default.aspx">← Torna alla pagina principale</a>
 <div class="page-heading"><p class="eyebrow">Centro assistenza</p><h1>Come possiamo aiutarti?</h1><p>Scegli la guida più adatta al tuo ruolo.</p></div>
 <div class="help-grid">
  <section class="card help-card"><p class="help-icon" aria-hidden="true">?</p><h2>Guida per l’utilizzatore</h2><p>Consultazione del personale, lettura dei profili, ricerca e interpretazione delle informazioni visualizzate.</p><a class="button" href="Utente.aspx">Apri la guida utente</a></section>
  <section class="card help-card"><p class="help-icon" aria-hidden="true">⚙</p><h2>Guida per amministratori e sviluppatori</h2><p>Permessi, gestione dei dati, configurazione, architettura applicativa, database e indicazioni per la manutenzione.</p><a class="button" href="Amministrazione.aspx">Apri la guida tecnica</a></section>
 </div>
</asp:Content>

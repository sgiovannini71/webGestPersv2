<%@ Page Title="Personale non attivo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NonAttivi.aspx.cs" Inherits="WebGestPersV2.Personale.NonAttivi" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Personale non attivo</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href="../Default.aspx">← Torna alla pagina principale</a>
 <div class="page-heading"><p class="eyebrow">Amministrazione</p><h1>Personale non attivo</h1><p>Ricerca il personale con stato di servizio diverso da attivo e accedi alla modifica per l’eventuale riattivazione.</p></div>
 <section class="card">
  <div class="people-filters">
   <div><asp:Label runat="server" AssociatedControlID="SearchText" Text="Cognome, nome o codice fiscale"/><asp:TextBox ID="SearchText" runat="server" MaxLength="50" /></div>
   <div><asp:Label runat="server" AssociatedControlID="ServiceStatus" Text="Stato di servizio"/><asp:DropDownList ID="ServiceStatus" runat="server" /></div>
   <div class="people-filter-actions"><asp:Button ID="SearchButton" runat="server" Text="Applica filtri" CssClass="button" OnClick="SearchButton_Click"/><asp:Button ID="ResetButton" runat="server" Text="Azzera filtri" CssClass="button secondary" OnClick="ResetButton_Click"/></div>
  </div>
  <asp:Label ID="Message" runat="server" CssClass="message error" Visible="false" />
  <asp:Label ID="ResultCount" runat="server" CssClass="result-count" />
  <asp:GridView ID="PeopleGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessuna persona non attiva trovata."><Columns>
   <asp:BoundField DataField="IdPersonale" HeaderText="ID"/><asp:BoundField DataField="Cognome" HeaderText="Cognome"/><asp:BoundField DataField="Nome" HeaderText="Nome"/><asp:BoundField DataField="CodiceFiscale" HeaderText="Codice fiscale"/><asp:BoundField DataField="GradoProfilo" HeaderText="Grado/Profilo"/><asp:BoundField DataField="StatoServizio" HeaderText="Stato di servizio"/><asp:HyperLinkField Text="Modifica / Riattiva" DataNavigateUrlFields="IdPersonale" DataNavigateUrlFormatString="Modifica.aspx?id={0}&amp;from=nonattivi" />
  </Columns></asp:GridView>
 </section>
</asp:Content>

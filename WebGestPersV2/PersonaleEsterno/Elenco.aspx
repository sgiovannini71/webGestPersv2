<%@ Page Title="Personale esterno" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Elenco.aspx.cs" Inherits="WebGestPersV2.PersonaleEsterno.Elenco" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Personale esterno</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <div class="page-heading"><div><p class="eyebrow">Area dedicata</p><h1>Personale esterno</h1><p>Elenco separato del personale di ditte esterne che opera presso le unità organizzative.</p></div></div>
 <section class="card">
  <div class="searchbar"><asp:Label runat="server" AssociatedControlID="SearchText" Text="Cognome, nome, codice fiscale o ditta"/><asp:TextBox ID="SearchText" runat="server" MaxLength="100"/><asp:Button ID="SearchButton" runat="server" Text="Cerca" CssClass="button" OnClick="SearchButton_Click"/></div>
  <div class="people-list-actions"><asp:CheckBox ID="IncludeInactive" runat="server" Text=" Mostra anche i non attivi" AutoPostBack="true" OnCheckedChanged="Filter_Changed"/><asp:HyperLink ID="NewLink" runat="server" NavigateUrl="~/PersonaleEsterno/Nuovo.aspx" CssClass="button">Nuovo personale esterno</asp:HyperLink></div>
  <asp:Label ID="Message" runat="server" CssClass="message error" Visible="false"/><asp:Label ID="ResultCount" runat="server" CssClass="result-count"/>
  <asp:GridView ID="ExternalGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessuna persona esterna trovata."><Columns>
   <asp:BoundField DataField="Id" HeaderText="ID"/><asp:HyperLinkField DataTextField="Cognome" HeaderText="Cognome" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="Modifica.aspx?id={0}"/><asp:BoundField DataField="Nome" HeaderText="Nome"/><asp:BoundField DataField="Ditta" HeaderText="Ditta"/><asp:BoundField DataField="Telefono" HeaderText="Telefono"/><asp:BoundField DataField="Email" HeaderText="E-mail"/><asp:BoundField DataField="Stato" HeaderText="Stato"/>
  </Columns></asp:GridView>
 </section>
</asp:Content>

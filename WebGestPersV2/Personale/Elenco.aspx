<%@ Page Title="Personale" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Elenco.aspx.cs" Inherits="WebGestPersV2.Personale.Elenco" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Personale</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading"><div><p class="eyebrow">Anagrafica</p><h1>Elenco personale</h1></div></div>
    <section class="card">
        <div class="searchbar">
            <asp:Label ID="SearchLabel" runat="server" AssociatedControlID="SearchTextBox" Text="Cognome, nome o codice fiscale" />
            <asp:TextBox ID="SearchTextBox" runat="server" MaxLength="50" />
            <asp:Button ID="SearchButton" runat="server" Text="Cerca" CssClass="button" OnClick="SearchButton_Click" />
        </div>
        <asp:Label ID="ErrorMessage" runat="server" CssClass="message error" Visible="false" />
        <asp:GridView ID="PeopleGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessun nominativo trovato.">
            <Columns>
                <asp:BoundField DataField="IdPersonale" HeaderText="ID" />
                <asp:HyperLinkField DataTextField="Cognome" HeaderText="Cognome" DataNavigateUrlFields="IdPersonale" DataNavigateUrlFormatString="Dettaglio.aspx?id={0}" />
                <asp:BoundField DataField="Nome" HeaderText="Nome" />
                <asp:BoundField DataField="CodiceFiscale" HeaderText="Codice fiscale" />
                <asp:CheckBoxField DataField="Militare" HeaderText="Militare" />
                <asp:BoundField DataField="GradoProfilo" HeaderText="Grado/Profilo" />
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>

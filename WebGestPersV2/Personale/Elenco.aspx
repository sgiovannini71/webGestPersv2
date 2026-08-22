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
        <div class="people-filters">
            <div><asp:Label runat="server" AssociatedControlID="PersonnelType" Text="Tipo personale"/><asp:DropDownList ID="PersonnelType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PersonnelType_Changed"><asp:ListItem Value="">Tutto</asp:ListItem><asp:ListItem Value="M">Militare</asp:ListItem><asp:ListItem Value="C">Civile</asp:ListItem></asp:DropDownList></div>
            <asp:Panel ID="MilitaryFilters" runat="server" CssClass="people-filter-group">
                <div><asp:Label runat="server" AssociatedControlID="ArmedForce" Text="Forza Armata"/><asp:DropDownList ID="ArmedForce" runat="server" AutoPostBack="true" OnSelectedIndexChanged="MilitaryFilter_Changed"/></div>
                <div><asp:Label runat="server" AssociatedControlID="CategoryCode" Text="Categorico"/><asp:DropDownList ID="CategoryCode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="MilitaryFilter_Changed"><asp:ListItem Value="">Tutti</asp:ListItem><asp:ListItem Value="U">Ufficiali (U)</asp:ListItem><asp:ListItem Value="S">Sottufficiali (S)</asp:ListItem><asp:ListItem Value="T">Truppa (T)</asp:ListItem></asp:DropDownList></div>
                <div><asp:Label runat="server" AssociatedControlID="MilitaryGrade" Text="Grado"/><asp:DropDownList ID="MilitaryGrade" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed"/></div>
            </asp:Panel>
            <asp:Panel ID="CivilianFilters" runat="server" CssClass="people-filter-group">
                <div><asp:Label runat="server" AssociatedControlID="CivilianProfile" Text="Profilo civile"/><asp:DropDownList ID="CivilianProfile" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed"/></div>
            </asp:Panel>
            <div class="people-filter-actions"><asp:Button ID="ResetFilters" runat="server" Text="Azzera filtri" CssClass="button secondary" OnClick="ResetFilters_Click"/></div>
        </div>
        <div class="people-list-actions"><asp:Button ID="ExportExcelButton" runat="server" Text="Esporta elenco in Excel" CssClass="button secondary" OnClick="ExportExcelButton_Click" /></div>
        <asp:Label ID="ErrorMessage" runat="server" CssClass="message error" Visible="false" />
        <asp:Label ID="ResultCount" runat="server" CssClass="result-count" />
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

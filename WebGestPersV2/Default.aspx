<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebGestPersV2.HomePage" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Home</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="hero">
        <p class="eyebrow">Nuova applicazione</p>
        <h1>Gestione del personale</h1>
        <p>Prima approssimazione della nuova WebGestPers, compatibile con Visual Studio 2019.</p>
        <a class="button" href="Personale/Elenco.aspx">Apri elenco personale</a>
    </section>
</asp:Content>

<%@ Page Title="Accesso negato" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccessoNegato.aspx.cs" Inherits="WebGestPersV2.Account.AccessoNegato" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Accesso negato</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="card access-denied">
        <p class="eyebrow">Sicurezza</p>
        <h1>Utente non autorizzato</h1>
        <p>L’identità Windows corrente non dispone delle autorizzazioni necessarie per accedere a questa funzione.</p>
    </section>
</asp:Content>

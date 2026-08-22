<%@ Page Title="Nuovo personale esterno" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Nuovo.aspx.cs" Inherits="WebGestPersV2.PersonaleEsterno.Nuovo" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Nuovo personale esterno</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href="Elenco.aspx">← Torna al personale esterno</a><div class="page-heading"><p class="eyebrow">Inserimento</p><h1>Nuovo personale esterno</h1><p>Registra i dati essenziali, la ditta e l'unità organizzativa di assegnazione.</p></div>
 <asp:ValidationSummary runat="server" CssClass="message error" ValidationGroup="external"/><asp:Label ID="Message" runat="server" CssClass="message error" Visible="false"/>
 <section class="card form-card"><div class="edit-grid">
  <label>Cognome*</label><asp:TextBox ID="LastName" runat="server" MaxLength="100"/><asp:RequiredFieldValidator runat="server" ControlToValidate="LastName" ErrorMessage="Cognome obbligatorio." ValidationGroup="external" CssClass="field-error"/>
  <label>Nome*</label><asp:TextBox ID="FirstName" runat="server" MaxLength="100"/><asp:RequiredFieldValidator runat="server" ControlToValidate="FirstName" ErrorMessage="Nome obbligatorio." ValidationGroup="external" CssClass="field-error"/>
  <label>Codice fiscale</label><asp:TextBox ID="TaxCode" runat="server" MaxLength="16"/><span></span>
  <label>Ditta*</label><asp:DropDownList ID="Company" runat="server"/><asp:RequiredFieldValidator runat="server" ControlToValidate="Company" InitialValue="" ErrorMessage="Ditta obbligatoria." ValidationGroup="external" CssClass="field-error"/>
  <label>Telefono</label><asp:TextBox ID="Phone" runat="server" MaxLength="20"/><span></span><label>Stanza</label><asp:TextBox ID="Room" runat="server" MaxLength="20"/><span></span>
  <label>Ufficio 1° livello</label><asp:DropDownList ID="Office1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Office1_Changed"/><span></span>
  <label>Ufficio 2° livello</label><asp:DropDownList ID="Office2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Office2_Changed"/><span></span>
  <label>Ufficio 3° livello</label><asp:DropDownList ID="Office3" runat="server"/><span></span>
 </div></section><div class="form-actions"><asp:Button ID="SaveButton" runat="server" Text="Crea" CssClass="button" ValidationGroup="external" OnClick="SaveButton_Click"/><a class="button secondary" href="Elenco.aspx">Annulla</a></div>
</asp:Content>

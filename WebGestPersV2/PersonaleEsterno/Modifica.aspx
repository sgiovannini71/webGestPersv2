<%@ Page Title="Scheda personale esterno" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Modifica.aspx.cs" Inherits="WebGestPersV2.PersonaleEsterno.Modifica" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Scheda personale esterno</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href="Elenco.aspx">← Torna al personale esterno</a><div class="page-heading"><p class="eyebrow">Scheda e modifica</p><h1><asp:Literal ID="Heading" runat="server"/></h1><p>Dati essenziali e collocazione organizzativa del collaboratore esterno.</p></div>
 <asp:ValidationSummary runat="server" CssClass="message error" ValidationGroup="external"/><asp:Label ID="Message" runat="server" CssClass="message error" Visible="false"/>
 <section class="card form-card"><div class="edit-grid">
  <label>Cognome*</label><asp:TextBox ID="LastName" runat="server" MaxLength="100"/><asp:RequiredFieldValidator runat="server" ControlToValidate="LastName" ErrorMessage="Cognome obbligatorio." ValidationGroup="external" CssClass="field-error"/>
  <label>Nome*</label><asp:TextBox ID="FirstName" runat="server" MaxLength="100"/><asp:RequiredFieldValidator runat="server" ControlToValidate="FirstName" ErrorMessage="Nome obbligatorio." ValidationGroup="external" CssClass="field-error"/>
  <label>Codice fiscale</label><asp:TextBox ID="TaxCode" runat="server" MaxLength="16"/><span></span>
  <label>Ditta*</label><asp:DropDownList ID="Company" runat="server"/><asp:RequiredFieldValidator runat="server" ControlToValidate="Company" InitialValue="" ErrorMessage="Ditta obbligatoria." ValidationGroup="external" CssClass="field-error"/>
  <label>Telefono</label><asp:TextBox ID="Phone" runat="server" MaxLength="20"/><span></span><label>Stanza</label><asp:TextBox ID="Room" runat="server" MaxLength="20"/><span></span>
  <label>Stato</label><asp:CheckBox ID="Active" runat="server" Text=" Persona esterna attiva"/><span>Deselezionare per mantenere il record nello storico senza mostrarlo nell'elenco ordinario.</span>
  <label>Ufficio 1° livello</label><asp:DropDownList ID="Office1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Office1_Changed"/><span></span>
  <label>Ufficio 2° livello</label><asp:DropDownList ID="Office2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Office2_Changed"/><span></span>
  <label>Ufficio 3° livello</label><asp:DropDownList ID="Office3" runat="server"/><span></span>
 </div></section>
 <div class="form-actions"><asp:Button ID="SaveButton" runat="server" Text="Salva modifiche" CssClass="button" ValidationGroup="external" OnClick="SaveButton_Click"/><asp:Button ID="DeactivateButton" runat="server" Text="Disattiva" CssClass="button secondary" CausesValidation="false" OnClick="DeactivateButton_Click" OnClientClick="return confirm('Confermare la disattivazione della persona esterna?');"/><a class="button secondary" href="Elenco.aspx">Annulla</a></div>
 <section class="card detail-section"><h2>Storico modifiche</h2><asp:GridView ID="HistoryGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessuna modifica registrata."><Columns><asp:BoundField DataField="Campo" HeaderText="Campo"/><asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}"/><asp:BoundField DataField="Utente" HeaderText="Utente"/><asp:BoundField DataField="Prima" HeaderText="Valore precedente"/><asp:BoundField DataField="Dopo" HeaderText="Nuovo valore"/></Columns></asp:GridView></section>
</asp:Content>

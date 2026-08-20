<%@ Page Title="Modifica personale" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Modifica.aspx.cs" Inherits="WebGestPersV2.Personale.Modifica" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Modifica personale</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href='<%= "Dettaglio.aspx?id=" + Request.QueryString["id"] %>'>← Torna al dettaglio</a>
 <div class="page-heading"><p class="eyebrow">Amministrazione</p><h1>Modifica personale</h1></div>
 <asp:ValidationSummary runat="server" CssClass="message error" ValidationGroup="edit" />
 <asp:Label ID="Message" runat="server" CssClass="message error" Visible="false" />
 <section class="card form-card"><div class="edit-grid">
  <label>Cognome*</label><asp:TextBox ID="LastName" runat="server" MaxLength="30" /><asp:RequiredFieldValidator runat="server" ControlToValidate="LastName" ErrorMessage="Cognome obbligatorio." ValidationGroup="edit" CssClass="field-error" />
  <label>Nome*</label><asp:TextBox ID="FirstName" runat="server" MaxLength="30" /><asp:RequiredFieldValidator runat="server" ControlToValidate="FirstName" ErrorMessage="Nome obbligatorio." ValidationGroup="edit" CssClass="field-error" />
  <label>Codice fiscale*</label><asp:TextBox ID="TaxCode" runat="server" MaxLength="16" /><asp:RequiredFieldValidator runat="server" ControlToValidate="TaxCode" ErrorMessage="Codice fiscale obbligatorio." ValidationGroup="edit" CssClass="field-error" />
  <label>Data assegnazione</label><asp:TextBox ID="AssignmentDate" runat="server" TextMode="Date" /><span></span>
  <label>Ente provenienza</label><asp:TextBox ID="SourceOrganization" runat="server" MaxLength="50" /><span></span>
  <label>Telefono ufficio</label><asp:TextBox ID="OfficePhone" runat="server" MaxLength="15" /><span></span>
  <label>Numero stanza</label><asp:TextBox ID="Room" runat="server" MaxLength="5" /><span></span>
  <label>Titolo di studio</label><asp:DropDownList ID="EducationTitle" runat="server" /><span></span>
  <label>Fascia oraria</label><asp:DropDownList ID="TimeBand" runat="server" /><span></span>
 </div></section>
 <asp:Panel ID="MilitaryPanel" runat="server" CssClass="card form-card detail-section" Visible="false"><h2>Profilo militare</h2><div class="edit-grid">
  <label>Posizione/Stato*</label><asp:DropDownList ID="MilitaryPosition" runat="server" /><span></span>
  <label>Forza armata*</label><asp:DropDownList ID="ArmedForce" runat="server" AutoPostBack="true" OnSelectedIndexChanged="MilitaryFilter_Changed" /><span></span>
  <label>Categorico*</label><asp:DropDownList ID="Category" runat="server" AutoPostBack="true" OnSelectedIndexChanged="MilitaryFilter_Changed"><asp:ListItem Value="U">Ufficiale</asp:ListItem><asp:ListItem Value="S">Sottufficiale</asp:ListItem><asp:ListItem Value="T">Truppa</asp:ListItem></asp:DropDownList><span></span>
  <label>Grado*</label><asp:DropDownList ID="Rank" runat="server" /><span></span>
  <label>Categoria militare*</label><asp:DropDownList ID="MilitaryCategory" runat="server" /><span></span>
  <label>Ruolo*</label><asp:DropDownList ID="MilitaryRole" runat="server" /><span></span>
  <label>Specialità*</label><asp:DropDownList ID="MilitarySpecialty" runat="server" /><span></span>
  <label>Matricola</label><asp:TextBox ID="RegistrationNumber" runat="server" MaxLength="20" /><span></span>
  <label>Numero decreto</label><asp:TextBox ID="DecreeNumber" runat="server" MaxLength="20" /><span></span>
  <label>Scuola di formazione</label><asp:TextBox ID="TrainingSchool" runat="server" MaxLength="50" /><span></span>
  <label>Data giuridica</label><asp:TextBox ID="MilitaryLegalDate" runat="server" TextMode="Date" /><span></span>
  <label>Data economica</label><asp:TextBox ID="MilitaryEconomicDate" runat="server" TextMode="Date" /><span></span>
  <label>Data arruolamento</label><asp:TextBox ID="EnlistmentDate" runat="server" TextMode="Date" /><span></span>
  <label>Data promozione</label><asp:TextBox ID="PromotionDate" runat="server" TextMode="Date" /><span></span>
 </div></asp:Panel>
 <asp:Panel ID="CivilianPanel" runat="server" CssClass="card form-card detail-section" Visible="false"><h2>Profilo civile</h2><div class="edit-grid">
  <label>ASL*</label><asp:DropDownList ID="HealthAuthority" runat="server" /><span></span>
  <label>Profilo all'assunzione*</label><asp:DropDownList ID="HiringTitle" runat="server" /><span></span>
  <label>Decreto/contratto assunzione</label><asp:TextBox ID="HiringDecree" runat="server" MaxLength="20" /><span></span>
  <label>Data giuridica assunzione</label><asp:TextBox ID="HiringLegalDate" runat="server" TextMode="Date" /><span></span>
  <label>Data economica assunzione</label><asp:TextBox ID="HiringEconomicDate" runat="server" TextMode="Date" /><span></span>
  <label>Profilo attuale*</label><asp:DropDownList ID="CurrentTitle" runat="server" /><span></span>
  <label>Decreto/contratto attuale</label><asp:TextBox ID="CurrentDecree" runat="server" MaxLength="20" /><span></span>
  <label>Data giuridica attuale</label><asp:TextBox ID="CurrentLegalDate" runat="server" TextMode="Date" /><span></span>
  <label>Data economica attuale</label><asp:TextBox ID="CurrentEconomicDate" runat="server" TextMode="Date" /><span></span>
  <label>Area di inquadramento*</label><asp:DropDownList ID="CivilianArea" runat="server" /><span></span>
  <label>Fascia dirigenziale</label><asp:DropDownList ID="ManagementBand" runat="server"><asp:ListItem Value="n/a">n/a</asp:ListItem><asp:ListItem Value="1^">1^</asp:ListItem><asp:ListItem Value="2^">2^</asp:ListItem></asp:DropDownList><span></span>
  <label>Fascia retributiva dirigenti</label><asp:DropDownList ID="PayBand" runat="server"><asp:ListItem Value="n/a">n/a</asp:ListItem><asp:ListItem Value="1^">1^</asp:ListItem><asp:ListItem Value="2^">2^</asp:ListItem><asp:ListItem Value="3^">3^</asp:ListItem><asp:ListItem Value="4^">4^</asp:ListItem><asp:ListItem Value="5^">5^</asp:ListItem><asp:ListItem Value="6^">6^</asp:ListItem><asp:ListItem Value="7^">7^</asp:ListItem></asp:DropDownList><span></span>
 </div></asp:Panel>
 <div class="form-actions"><asp:Button ID="SaveButton" runat="server" Text="Salva modifiche" CssClass="button" ValidationGroup="edit" OnClick="SaveButton_Click" /><asp:HyperLink ID="CancelLink" runat="server" CssClass="button secondary">Annulla</asp:HyperLink></div>
</asp:Content>

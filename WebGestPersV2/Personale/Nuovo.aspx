<%@ Page Title="Nuovo personale" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Nuovo.aspx.cs" Inherits="WebGestPersV2.Personale.Nuovo" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Nuovo personale</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <div class="page-heading"><p class="eyebrow">Amministrazione</p><h1>Nuovo personale</h1></div>
 <asp:ValidationSummary ID="Summary" runat="server" CssClass="message error" ValidationGroup="insert" />
 <asp:Label ID="Message" runat="server" CssClass="message error" Visible="false" />
 <section class="card form-card"><div class="edit-grid">
  <label>Tipo personale*</label><asp:DropDownList ID="PersonnelType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PersonnelType_Changed"><asp:ListItem Value="">-- seleziona --</asp:ListItem><asp:ListItem Value="C">Civile</asp:ListItem><asp:ListItem Value="M">Militare</asp:ListItem></asp:DropDownList><span class="field-hint">Selezionare civile o militare</span>
  <label>Cognome*</label><asp:TextBox ID="LastName" runat="server" MaxLength="30" /><asp:RequiredFieldValidator runat="server" ControlToValidate="LastName" ErrorMessage="Cognome obbligatorio." ValidationGroup="insert" CssClass="field-error" />
  <label>Nome*</label><asp:TextBox ID="FirstName" runat="server" MaxLength="30" /><asp:RequiredFieldValidator runat="server" ControlToValidate="FirstName" ErrorMessage="Nome obbligatorio." ValidationGroup="insert" CssClass="field-error" />
  <label>Codice fiscale*</label><asp:TextBox ID="TaxCode" runat="server" MaxLength="16" /><asp:RequiredFieldValidator runat="server" ControlToValidate="TaxCode" ErrorMessage="Codice fiscale obbligatorio." ValidationGroup="insert" CssClass="field-error" />
  <label>Data assegnazione*</label><asp:TextBox ID="AssignmentDate" runat="server" TextMode="Date" /><asp:RequiredFieldValidator runat="server" ControlToValidate="AssignmentDate" ErrorMessage="Data assegnazione obbligatoria." ValidationGroup="insert" CssClass="field-error" />
  <label>Ente provenienza</label><asp:TextBox ID="SourceOrganization" runat="server" MaxLength="50" /><span></span>
  <label>Titolo di studio*</label><asp:DropDownList ID="EducationTitle" runat="server" /><asp:RequiredFieldValidator runat="server" ControlToValidate="EducationTitle" InitialValue="" ErrorMessage="Titolo di studio obbligatorio." ValidationGroup="insert" CssClass="field-error" />
  <label>Fascia oraria*</label><asp:DropDownList ID="TimeBand" runat="server" /><asp:RequiredFieldValidator runat="server" ControlToValidate="TimeBand" InitialValue="" ErrorMessage="Fascia oraria obbligatoria." ValidationGroup="insert" CssClass="field-error" />
 </div></section>
 <asp:Panel ID="CivilianFields" runat="server" CssClass="card form-card detail-section" Visible="false"><h2>Profilo civile</h2><div class="edit-grid"><label>Famiglia professionale*</label><asp:DropDownList ID="CivilianTitle" runat="server" /><asp:RequiredFieldValidator runat="server" ControlToValidate="CivilianTitle" InitialValue="" ErrorMessage="Famiglia professionale obbligatoria." ValidationGroup="insert" CssClass="field-error" /></div></asp:Panel>
 <asp:Panel ID="MilitaryFields" runat="server" CssClass="card form-card detail-section" Visible="false"><h2>Profilo militare</h2><div class="edit-grid">
  <label>Forza armata*</label><asp:DropDownList ID="ArmedForce" runat="server" AutoPostBack="true" OnSelectedIndexChanged="MilitaryFilter_Changed" /><span></span>
  <label>Categorico*</label><asp:DropDownList ID="Category" runat="server" AutoPostBack="true" OnSelectedIndexChanged="MilitaryFilter_Changed"><asp:ListItem Value="">-- seleziona --</asp:ListItem><asp:ListItem Value="U">Ufficiale</asp:ListItem><asp:ListItem Value="S">Sottufficiale</asp:ListItem><asp:ListItem Value="T">Truppa</asp:ListItem></asp:DropDownList><span></span>
  <label>Grado*</label><asp:DropDownList ID="Rank" runat="server" /><span></span>
 </div></asp:Panel>
 <div class="form-actions"><asp:Button ID="CreateButton" runat="server" Text="Crea personale" CssClass="button" ValidationGroup="insert" OnClick="CreateButton_Click" /><a class="button secondary" href="Elenco.aspx">Annulla</a></div>
</asp:Content>

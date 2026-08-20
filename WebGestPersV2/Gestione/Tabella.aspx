<%@ Page Title="Tabella di gestione" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Tabella.aspx.cs" Inherits="WebGestPersV2.Gestione.Tabella" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server"><asp:Literal ID="PageTitle" runat="server" /></asp:Content>
<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server"><style>.management-filter{display:flex;align-items:center;gap:14px;margin-bottom:18px}.management-filter label{font-weight:600}.management-filter select{min-width:280px;height:40px;border:1px solid #cbd5e1;border-radius:7px;padding:0 10px}.table-card{overflow-x:auto}</style></asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <a class="back-link" href="Indice.aspx">← Tabelle di gestione</a>
 <div class="page-heading"><p class="eyebrow">Amministrazione</p><h1><asp:Literal ID="Heading" runat="server" /></h1></div>
 <asp:Panel ID="MilitaryFilter" runat="server" CssClass="card management-filter" Visible="false">
  <label for="<%= ArmedForce.ClientID %>">Forza armata</label><asp:DropDownList ID="ArmedForce" runat="server" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged" />
 </asp:Panel>
 <asp:Label ID="Message" runat="server" CssClass="message error" Visible="false" />
 <asp:Panel ID="EditPanel" runat="server" CssClass="card form-card" Visible="false">
  <h2><asp:Literal ID="EditTitle" runat="server" /></h2><asp:HiddenField ID="OriginalKey" runat="server" />
  <div class="edit-grid">
   <asp:Label ID="Field1Label" runat="server" AssociatedControlID="Field1" /><asp:TextBox ID="Field1" runat="server" /><span></span>
   <asp:Label ID="Field2Label" runat="server" AssociatedControlID="Field2" /><asp:TextBox ID="Field2" runat="server" /><span></span>
   <asp:Label ID="Field3Label" runat="server" AssociatedControlID="Field3" /><asp:TextBox ID="Field3" runat="server" /><span></span>
  </div><div class="form-actions"><asp:Button ID="SaveButton" runat="server" Text="Salva" CssClass="button" OnClick="SaveButton_Click" /><a class="button secondary" href='<%= "Tabella.aspx?tipo=" + Server.UrlEncode(Request.QueryString["tipo"]) %>'>Annulla</a></div>
 </asp:Panel>
 <asp:Panel ID="OfficePanel" runat="server" CssClass="card form-card" Visible="false">
  <h2><asp:Literal ID="OfficeEditTitle" runat="server" /></h2><asp:HiddenField ID="OfficeOriginalKey" runat="server" />
  <div class="edit-grid">
   <asp:Label ID="Parent1Label" runat="server" AssociatedControlID="ParentOffice1">Ufficio di primo livello*</asp:Label><asp:DropDownList ID="ParentOffice1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ParentOffice1_Changed" /><span></span>
   <asp:Label ID="Parent2Label" runat="server" AssociatedControlID="ParentOffice2">Ufficio di secondo livello*</asp:Label><asp:DropDownList ID="ParentOffice2" runat="server" /><span></span>
   <label for="<%= OfficeCode.ClientID %>">Sigla*</label><asp:TextBox ID="OfficeCode" runat="server" MaxLength="50" /><span></span>
   <label for="<%= OfficeDescription.ClientID %>">Descrizione*</label><asp:TextBox ID="OfficeDescription" runat="server" MaxLength="255" /><span></span>
   <label for="<%= OfficeOrder.ClientID %>">Ordinamento</label><asp:TextBox ID="OfficeOrder" runat="server" /><span></span>
   <label for="<%= OfficeCompetences.ClientID %>">Competenze</label><asp:TextBox ID="OfficeCompetences" runat="server" MaxLength="255" TextMode="MultiLine" Rows="3" /><span></span>
   <asp:Label ID="ActiveLabel" runat="server" AssociatedControlID="OfficeActive">Attivo</asp:Label><asp:DropDownList ID="OfficeActive" runat="server"><asp:ListItem Value="S">Sì</asp:ListItem><asp:ListItem Value="N">No</asp:ListItem></asp:DropDownList><span></span>
  </div><div class="form-actions"><asp:Button ID="OfficeSaveButton" runat="server" Text="Salva" CssClass="button" OnClick="OfficeSaveButton_Click" /><a class="button secondary" href='<%= "Tabella.aspx?tipo=" + Server.UrlEncode(Request.QueryString["tipo"]) %>'>Annulla</a></div>
 </asp:Panel>
 <asp:Panel ID="CivilEditPanel" runat="server" CssClass="card form-card" Visible="false">
  <h2><asp:Literal ID="CivilEditTitle" runat="server" /></h2><asp:HiddenField ID="CivilOriginalKey" runat="server" />
  <div class="edit-grid">
   <asp:Label ID="CivilLabel1" runat="server" AssociatedControlID="CivilField1" /><asp:TextBox ID="CivilField1" runat="server" /><span></span>
   <asp:Label ID="CivilLabel2" runat="server" AssociatedControlID="CivilField2" /><asp:TextBox ID="CivilField2" runat="server" /><span></span>
   <asp:Label ID="CivilLabel3" runat="server" AssociatedControlID="CivilField3" /><asp:TextBox ID="CivilField3" runat="server" /><span></span>
   <asp:Label ID="CivilLabel4" runat="server" AssociatedControlID="CivilField4" /><asp:TextBox ID="CivilField4" runat="server" /><span></span>
   <asp:Label ID="CivilLabel5" runat="server" AssociatedControlID="CivilField5" /><asp:TextBox ID="CivilField5" runat="server" /><span></span>
   <asp:Label ID="CivilLabel6" runat="server" AssociatedControlID="CivilField6" /><asp:TextBox ID="CivilField6" runat="server" /><span></span>
   <asp:Label ID="CivilLabel7" runat="server" AssociatedControlID="CivilField7" /><asp:TextBox ID="CivilField7" runat="server" /><span></span>
   <asp:Label ID="CivilLabel8" runat="server" AssociatedControlID="CivilField8" /><asp:TextBox ID="CivilField8" runat="server" /><span></span>
  </div><div class="form-actions"><asp:Button ID="CivilSaveButton" runat="server" Text="Salva" CssClass="button" OnClick="CivilSaveButton_Click" /><a class="button secondary" href='<%= "Tabella.aspx?tipo=" + Server.UrlEncode(Request.QueryString["tipo"]) %>'>Annulla</a></div>
 </asp:Panel>
 <asp:Panel ID="MilitaryEditPanel" runat="server" CssClass="card form-card" Visible="false">
  <h2><asp:Literal ID="MilitaryEditTitle" runat="server" /></h2><asp:HiddenField ID="MilitaryOriginalKey" runat="server" />
  <div class="edit-grid">
   <label for="<%= MilitaryArmedForce.ClientID %>">Forza armata*</label><asp:DropDownList ID="MilitaryArmedForce" runat="server" /><span></span>
   <label for="<%= MilitaryGrouping.ClientID %>">Categorico*</label><asp:DropDownList ID="MilitaryGrouping" runat="server"><asp:ListItem Value="">-- seleziona --</asp:ListItem><asp:ListItem Value="U">Ufficiale</asp:ListItem><asp:ListItem Value="S">Sottufficiale</asp:ListItem><asp:ListItem Value="T">Truppa</asp:ListItem></asp:DropDownList><span></span>
   <label for="<%= MilitaryCode.ClientID %>">Sigla*</label><asp:TextBox ID="MilitaryCode" runat="server" /><span></span>
   <label for="<%= MilitaryDescription.ClientID %>">Descrizione</label><asp:TextBox ID="MilitaryDescription" runat="server" MaxLength="50" /><span></span>
   <label for="<%= MilitaryOrder.ClientID %>">Ordinamento</label><asp:TextBox ID="MilitaryOrder" runat="server" /><span></span>
   <asp:Label ID="GeneralLevelLabel" runat="server" AssociatedControlID="MilitaryGeneralLevel">Livello generale</asp:Label><asp:TextBox ID="MilitaryGeneralLevel" runat="server" /><span></span>
   <asp:Label ID="HierarchyLevelLabel" runat="server" AssociatedControlID="MilitaryHierarchyLevel">Livello gerarchico</asp:Label><asp:TextBox ID="MilitaryHierarchyLevel" runat="server" /><span></span>
  </div><div class="form-actions"><asp:Button ID="MilitarySaveButton" runat="server" Text="Salva" CssClass="button" OnClick="MilitarySaveButton_Click" /><a class="button secondary" href='<%= "Tabella.aspx?tipo=" + Server.UrlEncode(Request.QueryString["tipo"]) %>'>Annulla</a></div>
 </asp:Panel>
 <asp:HyperLink ID="NewButton" runat="server" CssClass="button" Visible="false">Nuovo elemento</asp:HyperLink>
 <section class="card table-card detail-section"><asp:GridView ID="Items" runat="server" AutoGenerateColumns="true" CssClass="data-grid" GridLines="None" EmptyDataText="Nessun elemento presente." OnRowDataBound="Items_RowDataBound" /></section>
</asp:Content>

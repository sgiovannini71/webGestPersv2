<%@ Page Title="Gestione incarichi" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Incarichi.aspx.cs" Inherits="WebGestPersV2.Personale.Incarichi" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Gestione incarichi</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <asp:HyperLink ID="BackLink" runat="server" CssClass="back-link">← Torna al dettaglio</asp:HyperLink>
 <div class="page-heading"><p class="eyebrow">Personale</p><h1>Incarichi di <asp:Literal ID="FullName" runat="server" /></h1></div>
 <asp:Label ID="Message" runat="server" CssClass="message" Visible="false" />
 <section class="card detail-section">
  <div class="form-actions"><asp:HyperLink ID="NewLink" runat="server" CssClass="button">Nuovo incarico</asp:HyperLink></div>
  <asp:GridView ID="AssignmentsGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessun incarico associato.">
   <Columns>
    <asp:BoundField DataField="Descrizione" HeaderText="Incarico" /><asp:CheckBoxField DataField="Principale" HeaderText="Principale" />
    <asp:BoundField DataField="DataInizioTesto" HeaderText="Data inizio" /><asp:BoundField DataField="UfficioLivello1" HeaderText="1° livello" />
    <asp:BoundField DataField="UfficioLivello2" HeaderText="2° livello" /><asp:BoundField DataField="UfficioLivello3" HeaderText="3° livello" />
    <asp:TemplateField HeaderText=""><ItemTemplate>
     <asp:HyperLink runat="server" CssClass="button secondary" NavigateUrl='<%# UrlModifica(Eval("IdIncarico")) %>'>Modifica</asp:HyperLink>
     <asp:HyperLink runat="server" CssClass="button secondary" NavigateUrl='<%# UrlChiusura(Eval("IdIncarico")) %>' Visible='<%# !Convert.ToBoolean(Eval("Predefinito")) %>'>Chiudi</asp:HyperLink>
    </ItemTemplate></asp:TemplateField>
   </Columns>
  </asp:GridView>
 </section>
 <asp:Panel ID="ClosePanel" runat="server" CssClass="card form-card detail-section" Visible="false">
  <h2>Chiudi incarico</h2>
  <p>L’incarico sarà spostato nello storico e non comparirà più tra quelli attivi.</p>
  <div class="edit-grid">
   <label>Data fine incarico*</label><asp:TextBox ID="EndDate" runat="server" TextMode="Date" /><asp:RequiredFieldValidator runat="server" ControlToValidate="EndDate" ErrorMessage="Data fine incarico obbligatoria." ValidationGroup="closeAssignment" CssClass="field-error" />
  </div>
  <div class="form-actions"><asp:Button ID="CloseButton" runat="server" Text="Conferma chiusura" CssClass="button" ValidationGroup="closeAssignment" OnClick="CloseButton_Click" /><asp:HyperLink ID="CloseCancelLink" runat="server" CssClass="button secondary">Annulla</asp:HyperLink></div>
 </asp:Panel>
 <asp:Panel ID="EditPanel" runat="server" CssClass="card form-card detail-section" Visible="false">
  <h2><asp:Literal ID="FormTitle" runat="server" /></h2>
  <asp:ValidationSummary runat="server" CssClass="message error" ValidationGroup="assignment" />
  <div class="edit-grid">
   <label>Tipo incarico*</label><asp:DropDownList ID="AssignmentType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="AssignmentType_Changed" /><asp:RequiredFieldValidator runat="server" ControlToValidate="AssignmentType" InitialValue="" ErrorMessage="Tipo incarico obbligatorio." ValidationGroup="assignment" CssClass="field-error" />
   <label>Data inizio</label><asp:TextBox ID="StartDate" runat="server" TextMode="Date" /><span></span>
   <label>Incarico principale</label><asp:CheckBox ID="IsPrimary" runat="server" Text="Sì" /><span class="field-hint">Gli altri incarichi saranno impostati come non principali.</span>
   <label>Ufficio di 1° livello<asp:Literal ID="Office1RequiredMark" runat="server" /></label><asp:DropDownList ID="Office1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Office1_Changed" /><asp:RequiredFieldValidator ID="Office1Validator" runat="server" ControlToValidate="Office1" InitialValue="" ErrorMessage="Ufficio di primo livello obbligatorio." ValidationGroup="assignment" CssClass="field-error" />
   <asp:Panel ID="DefaultAssignmentHint" runat="server" CssClass="field-hint" Visible="false"><span></span><span>Per l'incarico n/a vengono utilizzati automaticamente gli uffici predefiniti.</span><span></span></asp:Panel>
   <label>Ufficio di 2° livello</label><asp:DropDownList ID="Office2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Office2_Changed" /><span></span>
   <label>Ufficio di 3° livello</label><asp:DropDownList ID="Office3" runat="server" /><span></span>
  </div>
  <div class="form-actions"><asp:Button ID="SaveButton" runat="server" Text="Salva incarico" CssClass="button" ValidationGroup="assignment" OnClick="SaveButton_Click" /><asp:HyperLink ID="CancelLink" runat="server" CssClass="button secondary">Annulla</asp:HyperLink></div>
 </asp:Panel>
 <section class="card detail-section">
  <h2>Storico incarichi</h2>
  <asp:GridView ID="HistoryGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessun incarico storico presente.">
   <Columns><asp:BoundField DataField="Incarico" HeaderText="Incarico" /><asp:CheckBoxField DataField="Principale" HeaderText="Principale" /><asp:BoundField DataField="DataInizioTesto" HeaderText="Data inizio" /><asp:BoundField DataField="DataFineTesto" HeaderText="Data fine" /></Columns>
  </asp:GridView>
 </section>
</asp:Content>

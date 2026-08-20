<%@ Page Title="Tabelle di gestione" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Indice.aspx.cs" Inherits="WebGestPersV2.Gestione.Indice" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Tabelle di gestione</asp:Content>
<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server"><style>.management-groups{display:grid;gap:18px}.management-links{display:grid;grid-template-columns:repeat(auto-fit,minmax(210px,1fr));gap:10px}.management-links a{display:block;padding:12px 14px;border:1px solid #e2e8f0;border-radius:8px;color:#2457d6;text-decoration:none;font-weight:600;background:#f8fafc}.management-links a:hover{background:#2457d6;color:#fff}.management-groups h2{margin-top:0}</style></asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
 <div class="page-heading"><p class="eyebrow">Amministrazione</p><h1>Tabelle di gestione</h1><p>Configurazioni utilizzate nelle schede del personale e negli incarichi.</p></div>
 <div class="management-groups">
  <section class="card"><h2>Comuni a civile e militare</h2><div class="management-links">
   <a href="Tabella.aspx?tipo=titoli-studio">Titoli di studio</a><a href="Tabella.aspx?tipo=fasce-orarie">Fasce orarie</a><a href="Tabella.aspx?tipo=toponomastica">Toponomastica</a><a href="Tabella.aspx?tipo=incarichi">Tipi di incarico</a><a href="Tabella.aspx?tipo=uffici-1">Uffici 1° livello</a><a href="Tabella.aspx?tipo=uffici-2">Uffici 2° livello</a><a href="Tabella.aspx?tipo=uffici-3">Uffici 3° livello</a>
  </div></section>
  <asp:Panel ID="CivilianTables" runat="server" CssClass="card"><h2>Personale civile</h2><div class="management-links">
   <a href="Tabella.aspx?tipo=qualifiche">Qualifiche / profili</a><a href="Tabella.aspx?tipo=posizioni-economiche">Posizioni economiche</a><a href="Tabella.aspx?tipo=asl">ASL</a>
  </div></asp:Panel>
  <asp:Panel ID="MilitaryTables" runat="server" CssClass="card"><h2>Personale militare</h2><div class="management-links">
   <a href="Tabella.aspx?tipo=gradi">Gradi</a><a href="Tabella.aspx?tipo=ruoli">Ruoli</a><a href="Tabella.aspx?tipo=categorie">Categorie</a><a href="Tabella.aspx?tipo=specialita">Specialità</a>
  </div></asp:Panel>
 </div>
</asp:Content>

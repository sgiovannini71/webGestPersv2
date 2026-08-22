<%@ Page Title="Personale per unità organizzativa" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Organigramma.aspx.cs" Inherits="WebGestPersV2.Organigramma" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Personale per unità organizzativa</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <a class="back-link" href="Default.aspx">← Torna alla pagina principale</a>
    <div class="page-heading">
        <p class="eyebrow">Struttura organizzativa</p>
        <h1>Personale per unità organizzativa</h1>
        <p>Visualizza il personale attivo in base all’incarico principale e naviga gli uffici fino al terzo livello.</p>
    </div>
    <asp:Panel ID="ErrorPanel" runat="server" CssClass="message error" Visible="false"><asp:Literal ID="ErrorMessage" runat="server" /></asp:Panel>
    <section class="card organigramma-tools" aria-label="Strumenti organigramma">
        <div class="field"><label for="orgSearch">Cerca persona o ufficio</label><input id="orgSearch" type="search" placeholder="Cognome, nome, incarico o ufficio" /></div>
        <div class="field"><label for="orgType">Tipo personale</label><select id="orgType"><option value="">Tutto</option><option value="militare">Militare</option><option value="civile">Civile</option></select></div>
        <div class="form-actions"><button class="button secondary" id="orgExpand" type="button">Espandi tutto</button><button class="button secondary" id="orgCollapse" type="button">Comprimi tutto</button><button class="button secondary" id="orgReset" type="button">Azzera filtri</button><asp:Button ID="ExportExcelButton" runat="server" Text="Esporta in Excel" CssClass="button" OnClick="ExportExcelButton_Click" /></div>
    </section>
    <asp:HiddenField ID="ExportSearch" runat="server" />
    <asp:HiddenField ID="ExportType" runat="server" />
    <div class="org-summary" aria-live="polite"><strong><asp:Literal ID="PersonnelCount" runat="server" /></strong> persone attive associate a un’unità organizzativa · <span id="orgVisibleCount"></span></div>
    <asp:Panel ID="EmptyPanel" runat="server" CssClass="message info" Visible="false">Nessun dipendente attivo con incarico principale è associato alle unità organizzative.</asp:Panel>
    <div id="orgChart" class="org-chart"><asp:Literal ID="OrganizationTree" runat="server" /></div>
    <script>
    (function () {
        var chart=document.getElementById('orgChart'), search=document.getElementById('orgSearch'), type=document.getElementById('orgType'), visible=document.getElementById('orgVisibleCount');
        if(!chart) return;
        function branches(){return chart.querySelectorAll('.org-branch');}
        function setOpen(branch,open){var body=branch.querySelector(':scope > .org-branch-body'),button=branch.querySelector(':scope > .org-unit > .org-toggle');if(!body)return;body.hidden=!open;if(button){button.setAttribute('aria-expanded',open?'true':'false');button.textContent=open?'−':'+';}}
        Array.prototype.forEach.call(branches(),function(branch){var button=branch.querySelector(':scope > .org-unit > .org-toggle');if(button)button.addEventListener('click',function(){setOpen(branch,button.getAttribute('aria-expanded')!=='true');});});
        function officeMatches(person,q){if(!q)return false;var branch=person.closest('.org-branch');while(branch){var title=branch.querySelector(':scope > .org-unit');if(title&&title.textContent.toLowerCase().indexOf(q)>=0)return true;branch=branch.parentElement.closest('.org-branch');}return false;}
        function filter(){var q=(search.value||'').toLowerCase().trim(), selected=type.value, shown=0;
            document.getElementById('<%= ExportSearch.ClientID %>').value=q;document.getElementById('<%= ExportType.ClientID %>').value=selected;
            Array.prototype.forEach.call(chart.querySelectorAll('.org-person'),function(person){var textMatch=!q||person.textContent.toLowerCase().indexOf(q)>=0||officeMatches(person,q), ok=textMatch&&(!selected||person.getAttribute('data-type')===selected);person.hidden=!ok;if(ok)shown++;});
            Array.prototype.forEach.call(Array.prototype.slice.call(branches()).reverse(),function(branch){var own=branch.querySelectorAll('.org-person:not([hidden])').length>0, title=branch.querySelector(':scope > .org-unit'), titleMatch=q&&title&&title.textContent.toLowerCase().indexOf(q)>=0;branch.hidden=!(own||titleMatch);if((q||selected)&&!branch.hidden)setOpen(branch,true);});
            visible.textContent=shown+' visualizzate';
        }
        search.addEventListener('input',filter);type.addEventListener('change',filter);
        document.getElementById('orgExpand').addEventListener('click',function(){Array.prototype.forEach.call(branches(),function(x){setOpen(x,true);});});
        document.getElementById('orgCollapse').addEventListener('click',function(){Array.prototype.forEach.call(branches(),function(x){setOpen(x,false);});});
        document.getElementById('orgReset').addEventListener('click',function(){search.value='';type.value='';Array.prototype.forEach.call(branches(),function(x){x.hidden=false;setOpen(x,true);});filter();});
        filter();
    }());
    </script>
</asp:Content>

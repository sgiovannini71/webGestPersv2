<%@ Page Title="Dettaglio personale" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dettaglio.aspx.cs" Inherits="WebGestPersV2.Personale.Dettaglio" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Dettaglio personale</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <a class="back-link" href="Elenco.aspx">← Torna all’elenco</a>
    <asp:Panel ID="ErrorPanel" runat="server" CssClass="message error" Visible="false">
        <asp:Literal ID="ErrorMessage" runat="server" />
    </asp:Panel>
    <asp:Panel ID="DetailsPanel" runat="server" Visible="false">
        <div class="profile-heading">
            <div class="photo-card">
                <asp:Image ID="ProfilePhoto" runat="server" CssClass="profile-photo" AlternateText="Foto del dipendente" />
            </div>
            <div class="page-heading">
                <p class="eyebrow"><asp:Literal ID="PersonnelType" runat="server" /></p>
                <h1><asp:Literal ID="FullName" runat="server" /></h1>
                <asp:HyperLink ID="EditLink" runat="server" CssClass="button" Visible="false">Modifica dati</asp:HyperLink>
                <a class="button secondary" href="#storico-modifiche">Storico modifiche</a>
            </div>
        </div>
        <nav class="section-nav" aria-label="Sezioni del profilo">
            <a href="#dati-generali">Dati generali</a>
            <a href="#dati-sensibili">Dati sensibili</a>
            <a href="#profilo">Profilo</a>
            <a href="#caselle-posta">Caselle di posta</a>
            <a href="#incarichi">Incarichi</a>
            <a href="#storico-incarichi">Storico incarichi</a>
            <a href="#storico-modifiche">Storico modifiche</a>
            <a href="#note">Note</a>
        </nav>
        <section id="dati-generali" class="card detail-section anchor-section">
            <h2>Dati generali</h2>
            <dl class="detail-grid">
                <dt>ID personale</dt><dd><asp:Literal ID="PersonnelId" runat="server" /></dd>
                <dt>Codice fiscale</dt><dd><asp:Literal ID="TaxCode" runat="server" /></dd>
                <dt>Sesso</dt><dd><asp:Literal ID="Gender" runat="server" /></dd>
                <dt>Stato servizio</dt><dd><asp:Literal ID="ServiceStatus" runat="server" /></dd>
                <dt>Telefono ufficio</dt><dd><asp:Literal ID="OfficePhone" runat="server" /></dd>
                <dt>Stanza</dt><dd><asp:Literal ID="Room" runat="server" /></dd>
                <dt>Ente provenienza</dt><dd><asp:Literal ID="SourceOrganization" runat="server" /></dd>
                <dt>Data assegnazione</dt><dd><asp:Literal ID="AssignmentDate" runat="server" /></dd>
                <dt>Data uscita</dt><dd><asp:Literal ID="ExitDate" runat="server" /></dd>
            </dl>
        </section>
        <section id="dati-sensibili" class="card detail-section anchor-section">
            <h2>Dati sensibili</h2>
            <dl class="detail-grid">
                <dt>Data di nascita</dt><dd><asp:Literal ID="BirthDate" runat="server" /></dd>
                <dt>Comune di nascita</dt><dd><asp:Literal ID="BirthTown" runat="server" /></dd>
                <dt>Stato civile</dt><dd><asp:Literal ID="MaritalStatus" runat="server" /></dd>
                <dt>Cellulare</dt><dd><asp:Literal ID="MobilePhone" runat="server" /></dd>
            </dl>
            <h3>Residenza</h3>
            <dl class="detail-grid">
                <dt>Indirizzo</dt><dd><asp:Literal ID="ResidenceAddress" runat="server" /></dd>
                <dt>Comune</dt><dd><asp:Literal ID="ResidenceTown" runat="server" /></dd>
                <dt>Telefono</dt><dd><asp:Literal ID="ResidencePhone" runat="server" /></dd>
            </dl>
            <h3>Domicilio</h3>
            <dl class="detail-grid">
                <dt>Indirizzo</dt><dd><asp:Literal ID="DomicileAddress" runat="server" /></dd>
                <dt>Comune</dt><dd><asp:Literal ID="DomicileTown" runat="server" /></dd>
                <dt>Telefono</dt><dd><asp:Literal ID="DomicilePhone" runat="server" /></dd>
            </dl>
            <h3>Documenti</h3>
            <dl class="detail-grid">
                <dt>Passaporto di servizio</dt><dd><asp:Literal ID="ServicePassport" runat="server" /></dd>
                <dt>Data passaporto</dt><dd><asp:Literal ID="ServicePassportDate" runat="server" /></dd>
                <dt>CMD</dt><dd><asp:Literal ID="CmdValue" runat="server" /></dd>
                <dt>Data CMD</dt><dd><asp:Literal ID="CmdDate" runat="server" /></dd>
                <dt>Modello AT</dt><dd><asp:Literal ID="AtModel" runat="server" /></dd>
                <dt>Data AT</dt><dd><asp:Literal ID="AtDate" runat="server" /></dd>
                <dt>Ultimo aggiornamento</dt><dd><asp:Literal ID="SensitiveDataUpdatedAt" runat="server" /></dd>
            </dl>
        </section>
        <div id="profilo" class="anchor-section">
        <asp:Panel ID="MilitaryPanel" runat="server" CssClass="card detail-section">
            <h2>Profilo militare</h2>
            <dl class="detail-grid">
                <dt>Grado</dt><dd><asp:Literal ID="Rank" runat="server" /></dd>
                <dt>Descrizione grado</dt><dd><asp:Literal ID="RankDescription" runat="server" /></dd>
                <dt>Forza armata</dt><dd><asp:Literal ID="ArmedForce" runat="server" /></dd>
                <dt>Categorico</dt><dd><asp:Literal ID="Category" runat="server" /></dd>
                <dt>Posizione/Stato</dt><dd><asp:Literal ID="MilitaryStatus" runat="server" /></dd>
                <dt>Categoria</dt><dd><asp:Literal ID="MilitaryCategory" runat="server" /></dd>
                <dt>Ruolo</dt><dd><asp:Literal ID="Role" runat="server" /></dd>
                <dt>Specialità</dt><dd><asp:Literal ID="Specialty" runat="server" /></dd>
                <dt>Matricola</dt><dd><asp:Literal ID="RegistrationNumber" runat="server" /></dd>
                <dt>Data giuridica</dt><dd><asp:Literal ID="MilitaryLegalDate" runat="server" /></dd>
                <dt>Data economica</dt><dd><asp:Literal ID="MilitaryEconomicDate" runat="server" /></dd>
                <dt>Numero decreto</dt><dd><asp:Literal ID="DecreeNumber" runat="server" /></dd>
                <dt>Scuola di formazione</dt><dd><asp:Literal ID="TrainingSchool" runat="server" /></dd>
                <dt>Data arruolamento</dt><dd><asp:Literal ID="EnlistmentDate" runat="server" /></dd>
                <dt>Data promozione</dt><dd><asp:Literal ID="PromotionDate" runat="server" /></dd>
                <dt>Ultimo aggiornamento</dt><dd><asp:Literal ID="MilitaryUpdatedAt" runat="server" /></dd>
            </dl>
        </asp:Panel>
        <asp:Panel ID="CivilianPanel" runat="server" CssClass="card detail-section">
            <h2>Profilo civile</h2>
            <h3>Profilo di assunzione</h3>
            <dl class="detail-grid">
                <dt>ASL</dt><dd><asp:Literal ID="HealthAuthority" runat="server" /></dd>
                <dt>Famiglia professionale</dt><dd><asp:Literal ID="HiringTitle" runat="server" /></dd>
                <dt>Descrizione</dt><dd><asp:Literal ID="HiringTitleDescription" runat="server" /></dd>
                <dt>Decreto/contratto</dt><dd><asp:Literal ID="HiringDecree" runat="server" /></dd>
                <dt>Data giuridica</dt><dd><asp:Literal ID="HiringLegalDate" runat="server" /></dd>
                <dt>Data economica</dt><dd><asp:Literal ID="HiringEconomicDate" runat="server" /></dd>
            </dl>
            <h3>Profilo attuale</h3>
            <dl class="detail-grid">
                <dt>Famiglia professionale</dt><dd><asp:Literal ID="TitleCode" runat="server" /></dd>
                <dt>Descrizione</dt><dd><asp:Literal ID="TitleDescription" runat="server" /></dd>
                <dt>Decreto/contratto</dt><dd><asp:Literal ID="CurrentDecree" runat="server" /></dd>
                <dt>Data giuridica</dt><dd><asp:Literal ID="CurrentLegalDate" runat="server" /></dd>
                <dt>Data economica</dt><dd><asp:Literal ID="CurrentEconomicDate" runat="server" /></dd>
                <dt>Area</dt><dd><asp:Literal ID="Area" runat="server" /></dd>
                <dt>Sottoarea</dt><dd><asp:Literal ID="Subarea" runat="server" /></dd>
                <dt>Fascia dirigenziale</dt><dd><asp:Literal ID="ManagementBand" runat="server" /></dd>
                <dt>Fascia retributiva</dt><dd><asp:Literal ID="PayBand" runat="server" /></dd>
                <dt>Ultimo aggiornamento</dt><dd><asp:Literal ID="CivilianUpdatedAt" runat="server" /></dd>
            </dl>
        </asp:Panel>
        </div>
        <section id="caselle-posta" class="card detail-section anchor-section">
            <h2>Caselle di posta</h2>
            <div class="form-actions"><asp:HyperLink ID="MailboxesEditLink" runat="server" CssClass="button" Visible="false">Gestisci caselle di posta</asp:HyperLink></div>
            <asp:GridView ID="MailboxesGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessuna casella postale associata."><Columns><asp:BoundField DataField="Denominazione" HeaderText="Indirizzo e-mail"/><asp:BoundField DataField="Tipo" HeaderText="Tipo"/></Columns></asp:GridView>
        </section>
        <section id="incarichi" class="card detail-section anchor-section">
            <h2>Incarichi</h2>
            <div class="form-actions"><asp:HyperLink ID="AssignmentsEditLink" runat="server" CssClass="button" Visible="false">Gestisci incarichi</asp:HyperLink></div>
            <asp:GridView ID="AssignmentsGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessun incarico associato.">
                <Columns>
                    <asp:BoundField DataField="Descrizione" HeaderText="Incarico" />
                    <asp:CheckBoxField DataField="Principale" HeaderText="Principale" />
                    <asp:BoundField DataField="DataInizioTesto" HeaderText="Data inizio" />
                    <asp:BoundField DataField="UfficioLivello1" HeaderText="1° livello" />
                    <asp:BoundField DataField="UfficioLivello2" HeaderText="2° livello" />
                    <asp:BoundField DataField="UfficioLivello3" HeaderText="3° livello" />
                </Columns>
            </asp:GridView>
        </section>
        <section id="storico-incarichi" class="card detail-section anchor-section">
            <h2>Storico incarichi</h2>
            <asp:GridView ID="AssignmentHistoryGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessun incarico storico presente.">
                <Columns>
                    <asp:BoundField DataField="Incarico" HeaderText="Incarico" />
                    <asp:CheckBoxField DataField="Principale" HeaderText="Principale" />
                    <asp:BoundField DataField="DataInizioTesto" HeaderText="Data inizio" />
                    <asp:BoundField DataField="DataFineTesto" HeaderText="Data fine" />
                </Columns>
            </asp:GridView>
        </section>
        <section id="storico-modifiche" class="card detail-section anchor-section">
            <h2>Storico modifiche</h2>
            <p class="field-hint">Elenca le modifiche registrate sui dati del dipendente, dalla più recente.</p>
            <asp:GridView ID="ModificationHistoryGrid" runat="server" AutoGenerateColumns="false" CssClass="data-grid" GridLines="None" EmptyDataText="Nessuna modifica registrata.">
                <Columns>
                    <asp:BoundField DataField="DataModificaTesto" HeaderText="Data e ora" />
                    <asp:BoundField DataField="CampoVariato" HeaderText="Campo modificato" />
                    <asp:BoundField DataField="ValoreVecchio" HeaderText="Valore precedente" />
                    <asp:BoundField DataField="ValoreNuovo" HeaderText="Nuovo valore" />
                    <asp:BoundField DataField="UtenteModificatore" HeaderText="Utente" />
                </Columns>
            </asp:GridView>
        </section>
        <section id="note" class="card detail-section anchor-section"><h2>Note</h2><asp:Literal ID="Notes" runat="server" /></section>
    </asp:Panel>
</asp:Content>

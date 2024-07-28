<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Main.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Account.CreateProjectViewModel>"%>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Account" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="System.Globalization" %>

<asp:content id="Content1" contentplaceholderid="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="ms.lang" content="<%= CultureInfo.CurrentUICulture.TwoLetterISOLanguageName %>" />
    <meta name="ms.loc" content="<%= Model.CurrentUICultureRegion %>" />
    <meta name="ms.shortidmsdn" content="vstsprojcreate" />
    <script type="text/javascript"<%= Html.GenerateNonce(true) %>> var _pageContext = { nextUrl: "<%: Model.NextUrl %>", scenario: "<%: Model.Scenario %>" };</script>
</asp:content>
<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <%
        Html.PageTitle(AccountResources.CreatingYourProject);
        Html.UseScriptModules("Account/Scripts/TFS.CreateProject");
        Html.UseCSS("Account.CreateProject");
    %>
</asp:content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="create-project-view" class="create-project-view <%: Model.IsCompact ? " create-project-ideview" : " create-project-webview" %>">
        <div id="new-project-container">
            <% if (Model.IsCompact)
                { %>
                    <div id="logoHeader" class="vslogo"></div>
            <% } %>
            <!-- Control rendered from js -->
        </div>
        <div class="project-info-contanier">
            <h1><%= AccountResources.CreateProjectInfoTitle %></h1>
            <p><%= AccountResources.CreateProjectInfoProjectNameHelp %></p>
            <p><%= AccountResources.CreateProjectInfoAgileHelp %></p>
            <p><%= AccountResources.CreateProjectInfoScrumHelp %></p>
            <p><%= AccountResources.CreateProjectInfoCMMIHelp %></p>
            <p><%= AccountResources.CreateProjectInfoGitHelp %></p>
            <p><%= AccountResources.CreateProjectInfoTFVCHelp %></p>
            <p><%= AccountResources.CreateProjectInfoFooter %></p>
        </div>
        <div id="create-project-progress" class="create-project-progress"></div>
    </div>
</asp:Content>

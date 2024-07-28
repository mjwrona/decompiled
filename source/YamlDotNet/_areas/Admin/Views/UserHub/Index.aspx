<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Account" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="TfsControls=Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.PageTitle(AccountServerResources.UserHubTitle);
        Html.AddHubViewClass("no-title");
        Html.UseScriptModules("Account/Scripts/TFS.Account");
    %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%:Url.Themed("account.css") %>" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="HubContent" runat="server">
    <% Html.UpgradeToFullVersionMessage(); %>
    <% if (ViewData["SpsUsersUrl"] != null)
        { %>
            <div class="hub-content" role="main">
                <div class="spsUserHub-receiver"></div>
                <script type="application/json" class="sps-account-url"><%= ViewData["SpsAccountUrl"] %></script>
                <iframe src="<%= ViewData["SpsUsersUrl"] %>" class="userHub-iframe"></iframe>
            </div>
    <% }
    else
    { %>
    <div class="userHub-account-view">
        <div id="userHubTitle" class="left-panel-title"><%: AccountServerResources.UsersHubHeader %></div>
        <div id="commonMessage" class="common-message"></div>
        <div id="userCol">
            <div id="menuBar">
            </div>
            <div id="panel" class="panel-region">
            </div>
        </div>
    </div>
    <div id="licenseDataContainer" class="license-view">
        <div id="scrollContainer" class="license-scroll">
            <script type="application/json" class="permissions-context"><%= ViewData["PermissionsData"] %></script>
            <div id="userTitle" class="license-panel-title"><%:AccountServerResources.HubDisplayText%></div>
            <div id="userSection" class="users-section">
            </div>

            <div id="licenseSection">
            </div>
            <div id="msdnSection" class="msdn-section">
            </div>
            <div id="pricingInfoLink" class="pricing-info">
            </div>
        </div>
    </div>
    <%  } %>
</asp:Content>

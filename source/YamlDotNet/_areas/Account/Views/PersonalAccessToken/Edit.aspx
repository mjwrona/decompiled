<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageDivided.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Account.PersonalAccessTokenModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Account" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:content ID="HeadContentStyles" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%:StaticResources.Versioned.Content.GetLocation("Details.Common.css") %>" type="text/css" rel="stylesheet" />
    <link href="<%:StaticResources.Versioned.Content.GetLocation("Details.Security.Common.css") %>" type="text/css" rel="stylesheet" />
    <link href="<%:StaticResources.Versioned.Content.GetLocation("Details.Security.Token.css") %>" type="text/css" rel="stylesheet" />
</asp:content>

<asp:Content ID="DocumentBegin" ContentPlaceHolderID="DocumentBegin" runat="server">
<% 
    Html.PageTitle(AccountResources.TokenAddPageTitle);
    Html.UseScriptModules("Account/Scripts/TFS.Details.Security.Common.Controls");
    Html.UseScriptModules("Account/Scripts/TFS.Details.Security.Tokens.Controls");
    Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.SSHPublicKeys);
    Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.PATPaginationAndFiltering);
%>
</asp:Content>

<asp:Content ID="LeftHubContent" ContentPlaceHolderID="LeftHubContent" runat="server">
    <div id="security-area-nav" class="details-area-nav" role="navigation" aria-label="<%: AccountServerResources.SecurityNavigationLabel %>"></div>
</asp:Content>

<asp:Content ID="RightHubContent" ContentPlaceHolderID="RightHubContent" runat="server">
    <div id="commonMessage"></div>
    <div class="main-column add-token-form has-footer" role="main" aria-labelledby="pat-header">
        <h1 class="header-title" id="pat-header"><%: Model.AuthorizationId == null ? AccountResources.TokenAddPageTitle : Model.Description %> </h1>
        <div class="clearfix">
            <div class="column span6 normal-space-container">
                <div class="normal-space-container">
                    <% Html.RenderPartial("UrlJsonIsland"); %>
                    <% Html.RenderPartial("EditTemplate", Model); %>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageDivided.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Account.PublicKeyIndexModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Account" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>

<asp:Content ID="HeadContentStyles" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%:StaticResources.Versioned.Content.GetLocation("Details.Common.css") %>" type="text/css" rel="stylesheet" />
    <link href="<%:StaticResources.Versioned.Content.GetLocation("Details.Security.Common.css") %>" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="DocumentBegin" ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.PageTitle(AccountResources.SSH_IndexPageTitle);
        Html.UseScriptModules("Account/Scripts/TFS.Details.Security.Common.Controls");
        Html.UseScriptModules("Account/Scripts/TFS.Details.Security.PublicKeys.Controls");
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.SSHPublicKeys);
    %>
</asp:Content>

<asp:Content ID="LeftHubContent" ContentPlaceHolderID="LeftHubContent" runat="server">
    <div id="security-area-nav" class="details-area-nav" role="navigation" aria-label="<%: AccountServerResources.SecurityNavigationLabel %>"></div>
</asp:Content>

<asp:Content ID="RightHubContent" ContentPlaceHolderID="RightHubContent" runat="server">
    <div id="commonMessage"></div>
    <div class="main-column has-footer" role="main">
        <div class="clearfix">
            <div class="column span6 normal-space-container">
                <div class="normal-space-container">
                    <div class="header-content" style="padding-left: 10px;">
                        <h1 class="header-title"><%: AccountResources.SSH_IndexPageTitle %> </h1>

                        <div class="header-text-wrapper featureHeader section-box">
                            <span class="header-text"><%: AccountServerResources.SSH_KeyFormDescription %> <a href="<%: AccountServerResources.SSH_DescriptionLearnMoreUrl %>"><%: AccountResources.LearnMore %></a></span>
                        </div>
                    </div>

                    <% Html.RenderPartial("UrlJsonIsland"); %>

                    <div class="token-notice"></div>

                    <div class="keyHub-key-view">
                        <% if(Model.PublicKeysDisabled) { %>
                            <div class="disabled-message"><%: AccountServerResources.SSH_PublicKeysDisabled %></div>
                        <% } else { %>
                            <div class="display-fingerprint"></div>
                            <div class="key-menu"></div>
                            <div class="key-grid"></div>
                        <% } %>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

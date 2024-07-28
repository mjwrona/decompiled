<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Main.Master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.CollectionViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:Content ID="DocumentBegin" ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.ContentTitle(Model.DisplayName); %>
	<% Html.UseScriptModules("Admin/Scripts/TFS.Admin.Controls"); %>
	<% Html.UseScriptModules("Admin/Scripts/TFS.Admin.Common"); %>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="admin-overview" role="main" aria-level="1">
        <div class="collection-overview collection-overview-control vertical-fill-layout">
            <% if (!TfsWebContext.IsHosted) { %>
            <div class="collection-management fixed-header"><%: AdminServerResources.OnPremiseCollectionInfo %></div>
            <% } %>
            <div class="overview fill-content">
                <% if (!TfsWebContext.TfsRequestContext.IsFeatureEnabled(FeatureAvailabilityFlags.CollectionAdminWebUi)) { %>
                <div class="overview-profile">
                    <% if (TfsWebContext.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment) { %>
                        <div class="header" role="heading" aria-level="2">
                            <span class="account-collection"><%:AdminServerResources.Account %></span>
                            <span class="profile-name"> / <%:Model.DisplayName %></span>
                        </div>
                    <% } else { %>
                        <div class="header" role="heading" aria-level="2">
                            <span class="account-collection"><%:AdminServerResources.Collection %></span>
                            <span class="profile-name"> / <%:Model.DisplayName %></span>
                        </div>
                    <% } %>
                    <div class="browse-info">
                        <div class="form-pair">
                            <div class="form-key"><%: AdminResources.Description %></div>
                            <div class="form-value"><%: Model.Description %></div>
                        </div>
                    </div>
                </div>                
                <div class="overview-detail content">
                <% } else { %>
                <div class="overview-detail overview-detail-no-left-pane content">
                <% } %>
                    <div class="fixed-header">
                        <div class="header" role="heading" aria-level="2"><%: AdminResources.Projects %></div>
                        <div class="actions-control toolbar"></div>
                    </div>
                    <div class="fill-content">
                        <div class="overview-grid-wrapper">
                            <div class="projects-grid"></div>
                        </div>
                    </div>
                </div>
            </div>
            <%= Html.CollectionOverviewOptions(Model) %>
        </div>
    </div>
</asp:Content>

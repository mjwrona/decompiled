<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.TeamProjectModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>

<div class="browse-project-control">
    <div class="browse-info">
        <%: Html.IdentityImage(Model.DefaultTeamId, new { @addClass="large-identity-picture" }) %>
        <div class="form-inline">
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.ProjectName %></div>
                <div class="form-value"><strong><%: Model.DisplayName %></strong></div>
            </div>
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Collection %></div>
                <div class="form-value"><%: Model.CollectionName %></div>
            </div>
            <% if (!String.IsNullOrEmpty(Model.Description)) { %>
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Description %></div>
                <div class="form-value"><%: Model.Description %></div>
            </div>
            <% } %>
        </div>
    </div>
    <% if (ViewData.ContainsKey("showTasks") && (Boolean)ViewData["showTasks"]) { %>
        <div class="browse-tasks-header"><%: AdminServerResources.AdministrationTasks %></div>
        <ul class="browse-tasks">
            <li><a href="<%: Url.Action("members", "security", new {routeArea = TfsRouteArea.Admin}) %>"><%: AdminServerResources.ManageProjectSecurityTask %></a></li>
            <li><a href="#" class="create-team"><%: AdminServerResources.CreateTeamTask %></a></li>
            <li><a href="<%: Url.Action("index", "home", new {routeArea = TfsRouteArea.Admin}) %>"><%: AdminServerResources.ViewProjectAdmin %></a></li>
        </ul>
    <% } %>
    <%= Html.TfsWebContext(TfsWebContext) %>
</div>
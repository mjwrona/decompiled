<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.CollectionViewModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>

<div class="browse-collection-control">
    <div class="browse-info">
        <div class="form-inline">
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.CollectionName %></div>
                <div class="form-value"><strong><%: Model.DisplayName %></strong></div>
            </div>
            <div class="form-pair">
                <div class="form-key"><%: AdminServerResources.CollectionStatus %></div>
                <div class="form-value"><%: Model.Status %></div>
            </div>
            <% if (Model.IsOnline) { %>
                <div class="form-pair">
                    <div class="form-key"><%: AdminResources.TeamProjects %></div>
                    <div class="form-value project-count"><%: Model.TeamProjects.Count %></div>
                </div>
                <% if (!String.IsNullOrEmpty(Model.Description)) { %>
                <div class="form-pair">
                    <div class="form-key"><%: AdminResources.Description %></div>
                    <div class="form-value"><%: Model.Description %></div>
                </div>
                <% } %>
            <% } %>
            <% else { %>
                <div class="form-pair">
                    <div class="form-key"><%: AdminServerResources.CollectionStatusReason %></div>
                    <div class="form-value project-count"><%: Model.StatusReason %></div>
                </div>
            <% } %>
        </div>
    </div>
    <% if (Model.IsOnline && ViewData.ContainsKey("showTasks") && (Boolean)ViewData["showTasks"]) { %>
        <div class="browse-tasks-header"><%: AdminServerResources.AdministrationTasks %></div>
        <ul class="browse-tasks">
            <li><a href="<%: Url.RouteUrl(TfsRouteWellKnownNames.AdminServiceHostControllerAction, "index", "security", new { serviceHost = Model.CollectionServiceHost, routeArea = TfsRouteArea.Admin }) %>"><%: AdminServerResources.ManageCollectionSecurityTask %></a></li>
            <li><a href="#" class="create-project"><%: AdminServerResources.CreateProjectTask %></a></li>
            <li><a href="<%: Url.RouteUrl(TfsRouteWellKnownNames.AdminServiceHost, "index", "home", new { serviceHost = Model.CollectionServiceHost, routeArea = TfsRouteArea.Admin }) %>"><%: AdminServerResources.ViewCollectionAdmin %></a></li>
        </ul>
    <% } %>
    <%= Html.BrowseCollectionOptions(Model) %>
    <%= Html.TfsWebContext(TfsWebContext) %>
</div>
<%@ Control Language="C#"
        Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.TeamViewModel>"%>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models" %>

<div class="browse-team-control">
    <div class="browse-info">
        <%: Html.IdentityImage(Model.Identity.TeamFoundationId, new { @addClass="large-identity-picture" }) %>
        <div class="form-inline">
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.TeamName %></div>
                <div class="form-value"><strong><%: Model.Identity.FriendlyDisplayName %></strong></div>
            </div>
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Project %></div>
                <div class="form-value"><%: Model.Identity.Scope %></div>
            </div>
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Members %></div>
                <div class="form-value member-count"><%: Model.Identity.MemberCount %></div>
            </div>
            <% if (!String.IsNullOrEmpty(Model.Identity.Description))
               { %>
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Description %></div>
                <div class="form-value"><%: Model.Identity.Description %></div>
            </div>
            <% } %>
            <% if (ViewData.ContainsKey("showAdmins") && (Boolean)ViewData["showAdmins"]) { %>
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Administrators %></div>
                <div class="form-value team-admins-list">
                    <%: Html.TeamViewModelOptions(Model) %>
                    <input type="hidden" />
                </div>
            </div>
            <% } %>
        </div>
    </div>
    <% if (ViewData.ContainsKey("showTasks") && (Boolean)ViewData["showTasks"]) { %>
        <div class="browse-tasks-header"><%: AdminServerResources.AdministrationTasks %></div>
        <ul class="browse-tasks">
            <li><a href="<%: Url.Action("index", "home", new {routeArea = TfsRouteArea.Admin}) %>"><%: AdminServerResources.ViewTeamAdmin %></a></li>
        </ul>
    <% } %>
</div>
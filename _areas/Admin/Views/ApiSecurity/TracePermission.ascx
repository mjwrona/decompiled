<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.TracePermissionModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.AdminEngagement.WebApi" %>

<div class="trace-permission-page">
    <div class="info-section">
        <div>
            <% if (!String.IsNullOrEmpty(Model.TitlePrefix)) { %>
            <span class="dialog-header">
                <%: Model.TitlePrefix %>
            </span>
            <% } %>
            <span class="<%: String.IsNullOrEmpty(Model.TitlePrefix) ? "dialog-header" : String.Empty %>">
                <%: Model.Title %>
            </span>
        </div>
        <p class="description"><%: AdminServerResources.TracePermissionDialogDescription %></p>
        <div class="form-inline">
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Permission %></div>
                <div class="form-value"><%: Model.PermissionName %></div>
            </div>
            <div class="form-pair">
                <div class="form-key"><%: AdminServerResources.EffectivePermission %></div>
                <div class="form-value effective-value"><%: Model.UserPermission %></div>
            </div>
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Identity %></div>
                <div class="form-value"><%: Model.UserDisplayName %></div>
            </div>
        </div>
    </div>
    <div class="account-section-control groups-section">
        <div class="groups-section-header">
            <p class="main-header"><%: Model.InheritanceType == InheritanceType.Group ? AdminServerResources.TracePermissionGroupMemberInheritance : AdminServerResources.NodeInheritance %></p>
            <span class="secondary-guidance"><%: Model.InheritanceType == InheritanceType.Group ? AdminServerResources.TracePermissionGroupsDescription : AdminServerResources.NodeInheritanceDescription %></span>
        </div>
        <div class="content">
            <% if (String.IsNullOrEmpty(Model.Error)) { %>
                <table>
                <% foreach (var v in Model.AffectingGroups) { %>
                    <tr>
                        <td></td>
                    <% if(Model.InheritanceType == InheritanceType.Group) { %>
                        <td class="key"><%: v.Key.DisplayName %></td>                
                    <% } else {%>
                        <td class="key"><%: Model.TokenDisplayName %></td>
                        <td class="key-subtitle"><%: v.Key.DisplayName %></td>
                    <% } %>
                        <td class="value permission"><%: PermissionModel.GetPermissionDisplayString(v.Value) %></td>
                    </tr>
                <% } %>
                </table>
            <% } else { %>
                <p class="trace-permission-error"><%: Model.Error %></p>
            <% } %>
        </div>
    </div>
</div>
<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.SecurityModel>"%>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server"%>

<div class="security-view vertical-fill-layout">    
    <div class="fill-content">
        <div class="splitter horizontal hub-splitter">
            <div class="leftPane">
                <div class="hub-pivot-content">
                    <script type="application/json" class="permissions-context"><%= ViewData["PermissionsData"] %></script>
                    <div class="identity-list-section vertical-fill-layout">
                        <div class="identity-search-box fixed-header">
                            <div class="toolbar"></div>
                            <% if (!TfsWebContext.TfsRequestContext.IsFeatureEnabled(FeatureAvailabilityFlags.AadGroupsAdminUi) || !(bool)ViewData["NewAdminUi"])
                            { %>
                                <div class="identity-search-control"></div>
                            <% }
                            else
                            { %>
                                <div class="ip-groups-search-container"></div>
                            <% } %>
                        </div>
                        <div class="main-identity-grid fill-content"></div>
                    </div>
                </div>
            </div>
            <div class="handleBar"></div>
            <div class="rightPane">
                <div class="hub-pivot">
                    <div class="permission-header">
                        <div class="header"><%: AdminServerResources.AccessControlSummary %></div>
                        <div class="secondary-guidance"><%: AdminServerResources.AccessControlHeader %></div>
                    </div>
                </div>
                <div class="hub-pivot-content">
                    <div class="identity-details-section">
                        <div id="identityInfo" class="security-info"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%: Html.SecurityViewOptions(Model) %>
    <%: Html.JsonIsland(ViewData["NewAdminUi"], new {@class = "enable-new-admin-ui"}) %>
    <%: Html.JsonIsland(ViewData["MinTemplateNewAddDialog"], new {@class = "enable-new-add-dialog-min-template"}) %>
</div>
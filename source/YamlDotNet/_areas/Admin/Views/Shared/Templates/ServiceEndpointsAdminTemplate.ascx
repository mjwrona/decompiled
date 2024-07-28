<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<script id="service_endpoint_admin_tab" type="text/html">
    <div class="service-endpoint-admin-hosts-view">
        <div class="hub-pivot">
            <div class="views">
                <ul class="empty pivot-view" role="tablist">
                    <li role="tab" data-bind="css: { selected: showDetails() }, attr: { 'aria-selected': showDetails(), 'tabindex': ((showDetails() == true) ? '0' : '-1') }, event: { keydown: onMenuKeyDown }, click: onShowDetails">
                        <a role="button" tabindex="-1"><%:AdminServerResources.ServiceEndPointDetailsTab %></a>
                    </li>
                    <li role="tab" data-bind="css: { selected: showRoles() }, attr: { 'aria-selected': showDetails(), 'tabindex': ((showRoles() == true) ? '0' : '-1') }, event: { keydown: onMenuKeyDown }, click: onShowRoles">
                        <a role="button" tabindex="-1"><%:AdminServerResources.ServiceEndPointRolesTab %></a>
                    </li>
                    <li role="tab" data-bind="css: { selected: showExecutionHistory() }, attr: { 'aria-selected': showDetails(), 'tabindex': ((showExecutionHistory() == true) ? '0' : '-1') }, event: { keydown: onMenuKeyDown }, click: onShowExecutionHistory">
                        <a role="button" tabindex="-1"><%:AdminServerResources.ServiceEndPointExecutionHistoryTab %></a>
                    </li>
                    <li role="tab" data-bind="visible: showPolicyTab, css: { selected: showPolicy() }, attr: { 'aria-selected': showPolicy(), 'tabindex': ((showPolicy() == true) ? '0' : '-1') }, event: { keydown: onMenuKeyDown }, click: onShowPolicy">
                        <a role="button" tabindex="-1"><%:AdminServerResources.ServiceEndPointPolicyTab %></a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="hub-pivot-content">
            <div class="details" role="region" aria-label="endpoint-details" data-bind="visible: showDetails()">
            </div>
            <div class="roles" role="region" aria-label="endpoint-roles" data-bind="visible: showRoles()">
            </div>
            <div class="execution-history" role="region" aria-label="endpoint-execution-history" data-bind="visible: showExecutionHistory()">
            </div>
            <div class="policy" role="region" aria-label="endpoint-policy" data-bind="visible: showPolicy()">
            </div>
        </div>
    </div>
</script>

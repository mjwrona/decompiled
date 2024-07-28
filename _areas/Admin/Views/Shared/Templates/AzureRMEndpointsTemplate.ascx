<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Services details template--%>
<script id="azurerm-connected-service-details" type="text/html">
    <div class="connected-service-details">
        <!-- ko if: name() -->
        <div class="group">
            <div class="header" role="heading" aria-level="2"><%:AdminServerResources.InformationHeader %></div>
            <p><%:AdminResources.ConnectionType %> <span data-bind="text: connectionType"></span></p>
            <p><%:AdminResources.CreatedBy %> <span data-bind="text: createdBy"></span></p>
            <p><%:AdminServerResources.ConnectingUsing %> <span data-bind="text: connectedUsing"></span></p>
        </div>
        <div class="group">
            <div class="header" role="heading" aria-level="2"><%:AdminServerResources.AzureSubscriptionActionsText %></div>
            <div class="content">
                <p><%:AdminServerResources.AzureSubscriptionListOfActionsText %></p>
                <ul>
                    <li data-bind="click: updateAuthentication, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="update-service-action"><%:AdminServerResources.UpdateServiceConfiguration %></a></li>
                    <li data-bind="visible: shouldEnableManageEndpointRoles"><a href="#" data-bind="click: manageEndpointRoles"><%:AdminServerResources.ManageEndpointRoles %> </a></li>
                    <li data-bind="visible: shouldEnableManageServicePrincipal"><a href="#" data-bind="click: manageServicePrincipal"><%:AdminServerResources.ManageServicePrincipal %> </a></li>
                    <li data-bind="click: disconnectService, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="disconnect-action"><%:AdminServerResources.AzureSubscriptionDisconnectText %></a></li>
                    <!-- ko if: shareAcrossProjectsEnabled() -->
                    <li data-bind="click: shareAcrossProjects, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="share-action"><%:AdminResources.ShareAcrossProjects %></a></li>
                    <!-- /ko -->
                </ul>
            </div>
        </div>
        <!-- /ko -->
    </div>
</script>

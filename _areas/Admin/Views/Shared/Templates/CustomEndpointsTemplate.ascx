<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Services details template--%>
<script id="custom-connected-service-details" type="text/html">
    <div class="custom-connected-service-details">
        <!-- ko if: name() -->
        <div class="group">
            <div class="header" role="heading" aria-level="2"><%:AdminServerResources.InformationHeader %></div>
            <p><%:AdminResources.ConnectionType %> <span data-bind="text: connectionType"></span> 
                <!-- ko if: connectionType() == "Jenkins" -->
                <span class="help icon icon-info-white" title="<%:AdminResources.JenkinsLicenseText %>" />
                <!-- /ko -->
            </p>
            
            <p><%:AdminResources.CreatedBy %> <span data-bind="text: createdBy"></span></p>
            <p><%:AdminServerResources.ConnectingUsing %> <span data-bind="text: connectedUsing"></span></p>
            <!-- ko if: !isReady -->
            <p><%:AdminServerResources.Warning %> <span data-bind="text: warning"></span></p>
            <!-- /ko -->

        </div>
        <div class="group">
            <div class="header" role="heading" aria-level="2"><%:AdminServerResources.Actions %></div>
            <div class="content">
                <p><%:AdminServerResources.AvailableActionsOnConnection %></p>
                <ul>
                    <!-- ko if: !isExtensionDisabled() -->
                    <li data-bind="click: updateConnection, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="update-service-action"><%:AdminServerResources.UpdateConnection %></a></li>
                    <!-- /ko -->
                    <li data-bind="click: disconnectService, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="disconnect-action"><%:AdminResources.Disconnect %></a></li>
                    <!-- ko if: shareAcrossProjectsEnabled() -->
                    <li data-bind="click: shareAcrossProjects, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="share-action"><%:AdminResources.ShareAcrossProjects %></a></li>
                    <!-- /ko -->
                </ul>
            </div>
        </div>
        <!-- /ko -->
    </div>
</script>

<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Dialog template--%>
<script id="add_chef_connections_dialog" type="text/html">
    <div class="add_chef_connections_dialog services_dialog">
        <div id="connectionId" class="connectionId" data-bind="value: id, disable: isUpdate()"></div>
        <table>
            <tr>
                <td>
                    <label for="connectionName"><%:AdminResources.ConnectionName %></label></td>
                <td>
                    <input class="textbox" data-bind="value: name" required id="connectionName" type="text" /></td>
            </tr>
            <tr>
                <td>
                    <label for="serverUrl"><%:AdminServerResources.ServerUrlChef %></label></td>
                <td>
                    <input class="textbox" data-bind="value: serverUrl" required id="serverUrl" type="text" /></td>
            </tr>
            <tr>
                <td>
                    <label for="username"><%:AdminServerResources.NodeUserName %></label></td>
                <td>
                    <input class="textbox" required id="username" type="text" /></td>
            </tr>
            <tr>
                <td>
                    <label for="clientKey"><%:AdminServerResources.ClientKey %></label></td>
                <td>
                    <textarea class="textbox" required id="clientKey" rows="6" cols="30"></textarea>
                    <span class="getting-started-lighttext getting-started-vertical-small"><%:AdminServerResources.CopyChefClientKey %></span>
                </td>
            </tr>
        </table>
        <div class="error-messages-div">
            <div data-bind="foreach: errors">
                <span data-bind="text: $data"></span>
                <br />
            </div>
        </div>
    </div>
</script>

<%--Services details template--%>
<script id="chef-connected-service-details" type="text/html">
    <div class="chef-connected-service-details">
        <!-- ko if: name() -->
        <div class="group">
            <div class="header" role="heading" aria-level="2"><%:AdminServerResources.InformationHeader %></div>
            <p> <%:AdminResources.ConnectionType %> <span data-bind="text: connectionType"></span></p>
            <p> <%:AdminResources.CreatedBy %> <span data-bind="text: createdBy"></span></p>
            <p> <%:AdminServerResources.ConnectingUsing %> <span data-bind="text: connectedUsing"></span></p>
        </div>
        <div class="group">
            <div class="header" role="heading" aria-level="2"><%:AdminServerResources.Actions %></div>
            <div class="content">
                <p><%:AdminServerResources.AvailableActionsOnConnection %></p>
                <ul>
                    <li data-bind="click: updateConnection, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="update-service-action"><%:AdminServerResources.UpdateConnection %></a></li>
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
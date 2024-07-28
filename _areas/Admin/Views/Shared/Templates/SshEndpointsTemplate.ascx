<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Dialog template--%>
<script id="add_ssh_connections_dialog" type="text/html">
    <div class="add_ssh_connections_dialog services_dialog">
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
                    <label for="host"><%:AdminResources.SshHost %></label></td>
                <td class='value-field'>
                    <input class="textbox" data-bind="value: host" required id="host" type="text" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: sshHostHelpMarkdown, minWidth: 400, pivotSiblingCssClass: 'value-field' }"></td>
            </tr>
            <tr>
                <td>
                    <label for="port"><%:AdminResources.SshPort %></label></td>
                <td class='value-field'>
                    <input class="textbox" data-bind="value: port" id="port" type="text" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: sshPortHelpMarkdown, minWidth: 400, pivotSiblingCssClass: 'value-field' }"></td>
            </tr>
            <tr>
                <td>
                    <label for="username"><%:AdminResources.UserName %></label></td>
                <td>
                    <input class="textbox" required id="username" type="text" data-bind="value: userName" /></td>
            </tr>
            <tr>
                <td>
                    <label for="pwd"><%:AdminResources.SshPassword %></label></td>
                <td class='value-field'>
                    <input class="textbox" required id="pwd" type="password" data-bind="attr: { placeholder: isUpdate() ? '********' : '' }" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: sshPasswordHelpMarkdown, minWidth: 400, pivotSiblingCssClass: 'value-field' }"></td>
            </tr>
             <tr>
                <td>
                    <label for="key"><%:AdminResources.SshKey %></label></td>
                <td class='value-field'>
                    <textarea class="textbox" id="key" rows="6" cols="30" data-bind="value: privateKey, attr: { placeholder: isUpdate() ? '********' : '' }"></textarea>
                    <p>
                        <a class="file-input-container" href="#" data-bind="click: loadTextFileContent, triggerClickOnKeyUp: loadTextFileContent"><%:AdminResources.LoadKeyFile %></a>
                    </p>
                </td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: sshKeyHelpMarkdown, minWidth: 400, pivotSiblingCssClass: 'value-field' }"></td>
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
<script id="ssh-connected-service-details" type="text/html">
    <div class="svn-connected-service-details">
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
        <!-- ko if: !name() -->
        <p><%:AdminServerResources.ConnectedServiceWelcome %></p>
        <!-- /ko -->
    </div>
</script>
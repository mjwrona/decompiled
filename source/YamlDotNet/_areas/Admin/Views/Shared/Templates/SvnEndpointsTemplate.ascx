<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Dialog template--%>
<script id="add_svn_connections_dialog" type="text/html">
    <div class="add_svn_connections_dialog services_dialog">
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
                    <label for="serverUrl"><%:AdminResources.SvnRepositoryUrl %></label></td>
                <td class='value-field'>
                    <input class="textbox" data-bind="value: serverUrl" required id="serverUrl" type="text" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: svnServerUrlHelpMarkdown, minWidth: 400, pivotSiblingCssClass: 'value-field' }"></td>
            </tr>
            <tr>
                <td/>
                <td class='value-field'>
                    <span>
                        <input class="checkbox" data-bind="checked: acceptUntrustedCerts" id="acceptUntrustedCerts" type="checkbox" />
                        <label for="acceptUntrustedCerts"><%:AdminResources.AcceptUntrustedCerts %></label>
                    </span></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: svnAcceptUntrustedCertsHelpMarkdown, minWidth: 400, pivotSiblingCssClass: 'value-field' }"></td>
            </tr>
            <tr>
                <td>
                    <label for="realmName"><%:AdminServerResources.RealmName %></label></td>
                <td class='value-field'>
                    <input class="textbox" data-bind="value: realmName" id="realmName" type="text" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: svnRealmNameHelpMarkdown, minWidth: 400, pivotSiblingCssClass: 'value-field' }"></td>
            </tr>
            <tr>
                <td>
                    <label for="username"><%:AdminResources.UserName %></label></td>
                <td>
                    <input class="textbox" id="username" type="text" data-bind="value: userName" /></td>
            </tr>
            <tr>
                <td>
                    <label for="pwd"><%:AdminServerResources.SubversionPassword %></label></td>
                <td>
                    <input class="textbox" id="pwd" type="password" data-bind="attr: { placeholder: isUpdate() ? '********' : '' }" /></td>
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
<script id="svn-connected-service-details" type="text/html">
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
    </div>
</script>
<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Dialog template--%>
<script id="add_gcp_connections_dialog" type="text/html">
    <div class="add_gcp_connections_dialog services_dialog">
        <div id="connectionId" class="connectionId" data-bind="value: id, disable: isUpdate()"></div>
        <table>
            <tr>
                <td>
                    <label for="connectionName"><%:AdminResources.ConnectionName %></label></td>
                <td>
                    <input class="textbox" data-bind="value: name" required id="connectionName" type="text" /></td>
            </tr>
            <!-- ko if: isUpdate() -->
            <tr>
                <td>
                    <label for="audience"><%:AdminResources.Audience %></label></td>
                <td>
                    <input class="textbox" type="text" readonly="readonly" id="audience" data-bind="value: audience"/>
            </tr>
            <tr>
                <td>
                    <label><%:AdminResources.Issuer %></label></td>
                <td>
                    <input class="textbox" type="text" readonly="readonly"  data-bind="value: issuer"/>
            </tr>
             <tr>
                <td>
                    <label for="projectid"><%:AdminResources.projectid %></label></td>
                <td>
                    <input class="textbox" type="text" readonly="readonly" id="projectid" data-bind="value: projectid"/>
            </tr>
            <tr>
                <td>
                    <label for="privatekey"><%:AdminServerResources.privatekey %></label></td>
                <td>
                    <input class="textbox"  placeholder="*****" required id="privatekey" type="text" /></td>
            </tr>
            <!-- /ko -->
            <tr>
                <td>
                    <label for="scope"><%:AdminServerResources.scope %></label></td>
                <td>
                    <input class="textbox"  id="scope" type="text" data-bind="value: scope"/></td>
            </tr>
            <tr>
                <td>
                    <label for="certificate"><%:AdminServerResources.certificate %></label></td>
                <td>
                    <textarea class="textbox"   required id="certificate" rows="5" cols="10" data-bind="attr: { placeholder: isUpdate() ? '*****' : '' }"></textarea>
                </td>
            </tr>
            <tr>
                    <td><a href="https://console.cloud.google.com/iam-admin/serviceaccounts">Get JSON File</a></td>
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
<script id="gcp-connected-service-details" type="text/html">
    <div class="gcp-connected-service-details">
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
<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Dialog template--%>
<script id="add_deployment_environments_dialog" type="text/html">
    <div class="add_deployment_environments_dialog services_dialog">
    <div id="connectionId" class="connectionId" data-bind="value: id, disable: isUpdate()"></div>
        <table>
            <tr>
                <td>
                    <label for="subscription-id"><%:AdminServerResources.AzureSubscriptionIdText %></label></td>
                <td>
                    <input class="textbox" data-bind="value: subscriptionid, disable: isUpdate()" required id="subscription-id" type="text" /></td>
            </tr>
            <tr>
                <td>
                    <label for="subscription-name"><%:AdminServerResources.AzureSubscriptionNameText %></label></td>
                <td>
                    <input class="textbox" data-bind="value: name" required id="subscription-name" type="text" /></td>
            </tr>
            <tr>
                <td>
                    <label for="auth-kind"><%:AdminServerResources.AzureSubscriptionAuthText %></label></td>
                <td>
                    <span>
                        <input class="auth-kind" required type="radio" name="cert" value="0" data-bind="checked: authChoice" />
                        <%:AdminServerResources.AzureSubscriptionCertificateText %></span>
                    <span>
                        <input class="auth-kind" required type="radio" name="cert" value="1" data-bind="checked: authChoice" />
                        <%:AdminServerResources.AzureSubscriptionCredentialsText %></span>
                    <!-- ko if: isServicePrincipalEnabled() -->
                    <span>
                        <input class="auth-kind" required type="radio" name="cert" value="2" data-bind="checked: authChoice" />
                        <%:AdminServerResources.AzureSubscriptionPrincipalText %></span>
                    <!-- /ko -->
                </td>
            </tr>
        </table>
        <div data-bind="template: { name: authTemplate() }" />
        <div class="error-messages-div">
            <div data-bind="foreach: errors">
                <span data-bind="text: $data"></span>
                <br />
            </div>
        </div>
    </div>
</script>

<%--Certificate template--%>
<script type="text/html" id="CertificateTemplate">
    <table>
        <tr>
            <td>
                <label for="subscription-cert"><%:AdminServerResources.AzureSubscriptionSubscriptionCertificateText %></label></td>
            <td>
                <textarea class="textbox" required id="subscription-cert" rows="6" cols="30"></textarea>
                <span data-bind="html: '<%:AdminServerResources.SubscriptionCertTip %>'" class="getting-started-lighttext getting-started-vertical-small"></span>
            </td>
        </tr>
    </table>
</script>

<%--Credential template--%>
<script type="text/html" id="CredentialTemplate">
    <table>
        <tr>
            <td>
                <label for="username"><%:AdminServerResources.AzureSubscriptionUsernameText %></label></td>
            <td>
                <input class="textbox" required id="username" type="text" /></td>
        </tr>
        <tr>
            <td>
                <label for="pwd"><%:AdminServerResources.AzureSubscriptionPasswordText %></label></td>
            <td>
                <input class="textbox" required id="pwd" type="password" /></td>
        </tr>
        <tr>
            <td>
                <label for="pwd-check"><%:AdminServerResources.AzureSubscriptionReenterPasswordText %></label></td>
            <td>
                <input class="textbox" required id="pwd-check" type="password" /></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <span data-bind="html: '<%:AdminServerResources.AzureCredentialsNoteHtml %>'"></span>
            </td>
        </tr>
    </table>
</script>

<%--Service Principal template--%>
<script type="text/html" id="ServicePrincipalTemplate">
   <table>
        <tr>
            <td>
                <label for="ServicePrincipalId"><%:AdminServerResources.AzureServicePrincipalIdText %></label></td>
            <td>
                <input class="textbox" required id="ServicePrincipalId" type="text" /></td>
        </tr>
        <tr>
            <td>
                <label for="ServicePrincipalKey"><%:AdminServerResources.AzureServicePrincipalKeyText %></label></td>
            <td>
                <input class="textbox" required id="ServicePrincipalKey" type="password" /></td>
        </tr>
        <tr>
            <td>
                <label for="TenantId"><%:AdminServerResources.AzureTenantId %></label></td>
            <td>
                <input class="textbox" required id="TenantId" type="text" /></td>
        </tr>
         <tr>
            <td>&nbsp;</td>
            <td>
                <span data-bind="html: '<%:AdminServerResources.ServicePrincipalTip %>'" class="getting-started-lighttext getting-started-vertical-small"></span>
            </td>
        </tr>
    </table>
</script>

<%--Services details template--%>
<script id="connected-service-details" type="text/html">
    <div class="connected-service-details">
        <!-- ko if: name() -->
        <div class="group">
            <div class="header" role="heading" aria-level="2"><%:AdminServerResources.InformationHeader %></div>
            <p> <%:AdminResources.ConnectionType %> <span data-bind="text: connectionType"></span></p>
                <div class="content">
                <p data-bind="visible: deploymentEnvironments().length == 0 && showDeploymentEnvironment()"><i><%:AdminServerResources.AzureSubscriptionNoDeploymentEnvironmentsText %></i></p>
                <div data-bind="visible: deploymentEnvironments().length > 0">
                    <p ><%:AdminServerResources.AzureSubscriptionListOfDeploymentEnvironmentsText %></p>
                    <ul data-bind="foreach: deploymentEnvironments">
                        <li>
                            <span data-bind="text: $data"></span>
                        </li>
                    </ul>
                </div>
            </div>
            <p> <%:AdminResources.CreatedBy %> <span data-bind="text: createdBy"></span></p>
            <p> <%:AdminServerResources.ConnectingUsing %> <span data-bind="text: connectedUsing"></span></p>
        </div>
        <div class="group">
            <div class="header" role="heading" aria-level="2"><%:AdminServerResources.AzureSubscriptionActionsText %></div>
            <div class="content">
                <p><%:AdminServerResources.AzureSubscriptionListOfActionsText %></p>
                <ul>
                    <li data-bind="click: updateAuthentication, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="update-service-action"><%:AdminServerResources.UpdateServiceConfiguration %></a></li>
                    <li data-bind="visible: serviceUri()"><a target="_blank" href="#" data-bind="attr: { href: serviceUri }"><%:AdminServerResources.AzureSubscriptionManageText %></a></li>
                    <li data-bind="click: disconnectService, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="disconnect-action"><%:AdminServerResources.AzureSubscriptionDisconnectText %></a></li>
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
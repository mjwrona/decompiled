<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Dialog template--%>
<script id="add_bitbucket_connections_dialog" type="text/html">
    <div class="add_bitbucket_connections_dialog services_dialog">
        <div id="connectionId" class="connectionId" data-bind="value: id, disable: isUpdate()"></div>
        <table>
            <tr data-bind="visible: isHosted()">
                <td><%:AdminServerResources.BitbucketChooseAuthorizationLabel %></td>
                <td>
                    <span>
                        <input class="auth-kind" required type="radio" id="ChoiceOAUTH" style="margin-left: 1px" name="cert" value="0" data-bind="checked: tokenChoice, disable: isUpdate() || isAuthorizing()" />
                        <label for="ChoiceOAUTH" style="display: inline"><%:AdminServerResources.BitbucketGrantAuthorizationLabel %></label>
                    </span>
                    <span>
                        <input class="auth-kind" required type="radio" id="ChoiceBasic" name="cert" value="1" data-bind="checked: tokenChoice, disable: isUpdate() || isAuthorizing()" />
                        <label for="ChoiceBasic" style="display: inline"><%:AdminServerResources.BitbucketBasicAuthorizationLabel %></label>
                    </span>
                </td>
            </tr>
            <tr>
                <td><div><label for="connectionName"><%:AdminServerResources.ConnectionName %></label></div></td>
                <td><div><input class="textbox" data-bind="value: name" required id="connectionName" type="text" /></div></td>
            </tr>
            <tr data-bind="visible: isHosted() && tokenChoice() == 0">
                <td />
                <td>
                    <div style="height: 40px;">
                        <div data-bind="visible: tokenChoice() == 0" style="height: 100%">
                            <div data-bind="visible: !isAuthorizing() && !authorizationCompleted() && !isUpdate()">
                                <button id="bitbucket-authorize-button"><%:AdminServerResources.AuthorizeButtonText %></button>
                            </div>
                            <div data-bind="visible: isAuthorizing()">
                                <img style="vertical-align: top; margin-right: 2px;" src="<%= StaticResources.Versioned.Content.GetLocation("spinner.gif") %>"/>
                                <span><%:AdminServerResources.AuthorizingInProgressText %></span>
                            </div>
                            <div class="bitbucket-oauth-success" data-bind="visible: authorizationSucceeded() || isUpdate()">
                                <span class="icon icon-tfs-build-status-succeeded" style="vertical-align: top"/>
                                <span data-bind="text: authorizationCompletedText"/>
                            </div>
                            <div class="bitbucket-oauth-error" data-bind="visible: authorizationCompleted() && !authorizationSucceeded()">
                                <span class="icon icon-tfs-build-status-failed" style="vertical-align: top"/>
                                <span data-bind="text: authorizationCompletedText"/>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
            <tr data-bind="visible: !isHosted() || tokenChoice() == 1">
                <td>
                    <label for="username"><%:AdminResources.UserName %></label>
                </td>
                <td>
                    <input class="textbox" id="username" type="text" data-bind="value: userName" />
                </td>
            </tr>
            <tr data-bind="visible: !isHosted() || tokenChoice() == 1">
                <td>
                    <label for="pwd"><%:AdminResources.Password %></label>
                </td>
                <td>
                    <input class="textbox" id="pwd" type="password" data-bind="attr: { placeholder: isUpdate() ? '********' : '' }" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td><div style="margin-bottom: 10px"><a href="<%:AdminServerResources.BitbucketLearnMoreLink %>" target="_blank"><%:AdminServerResources.BitbucketLearnMoreLinkText %></a></div></td>
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
<script id="bitbucket-connected-service-details" type="text/html">
    <div class="bitbucket-connected-service-details">
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
                    <li tabindex="0" data-bind="click: updateConnection, event: { keydown: onKeyDown }"><a role="button"><%:AdminServerResources.UpdateConnection %></a></li>
                    <li tabindex="0" data-bind="click: disconnectService, event: { keydown: onKeyDown }"><a role="button" class="disconnect-action"><%:AdminResources.Disconnect %></a></li>
                    <!-- ko if: shareAcrossProjectsEnabled() -->
                    <li data-bind="click: shareAcrossProjects, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="share-action"><%:AdminResources.ShareAcrossProjects %></a></li>
                    <!-- /ko -->
                </ul>
            </div>
        </div>
        <!-- /ko -->
    </div>
</script>
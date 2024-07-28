<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<%--Dialog template--%>
<script id="add_github_connections_dialog" type="text/html">
    <div class="add_github_connections_dialog services_dialog">
        <div id="connectionId" class="connectionId" data-bind="value: id"></div>
        <table>
            <tr data-bind="visible: isHosted() && marketAppFeatureEnabled">
                <td><%:AdminServerResources.GitHubAuthenticateAsLabel %></td>
                <td>
                    <span>
                        <input class="auth-kind" required type="radio" id="authenticateUser" style="margin-left: 1px" name="auth-as" value="1" data-bind="checked: authAsChoice, disable: disableUpdateTokenBasedEndpoint()" />
                        <label for="authenticateUser" style="display: inline"><%:AdminServerResources.GitHubAuthenticateAsUser %></label>
                    </span>
                    <span>
                        <input class="auth-kind" required type="radio" id="authenticateApp" name="auth-as" value="0" data-bind="checked: authAsChoice, disable: disableUpdateTokenBasedEndpoint()" />
                        <label for="authenticateApp" style="display: inline"><%:AdminServerResources.GitHubAuthenticateAsMarketplaceApp %></label>
                    </span>
                </td
            </tr>
            <tr data-bind="visible: isHosted() && authAsChoice() == 0 && marketAppFeatureEnabled && !disableUpdateTokenBasedEndpoint()">
                <td><%:AdminServerResources.GitHubOrganizationLabel %></td>
                <td>
                    <div style="width: 95%">
                        <div class="select-org-dropdown select-dropdown">
                    </div>
                    <div data-bind="visible: appInstallationsLoaded()" style="margin-top: 2px">
                        <span id="install-app-help-text">
                    </div>
                </td>
            </tr>
            <tr data-bind="visible: isHosted() && authAsChoice() == 1">
                <td><%:AdminServerResources.GitHubChooseAuthorizationLabel %></td>
                <td>
                    <div style="width: 95%">
                        <div class="select-auth-choice-dropdown select-dropdown">
                    </div>
                </td>
            </tr>
            <tr data-bind="visible: tokenChoice() != 2">
                <td>
                    <div><label for="connectionName"><%:AdminServerResources.ConnectionName %></label></div>
                </td>
                <td>
                    <div><input class="textbox" data-bind="value: name" required id="connectionName" type="text" /></div>
                </td>
            </tr>

            <tr data-bind="visible: isHosted() && authAsChoice() == 0 && marketAppFeatureEnabled && !disableUpdateTokenBasedEndpoint()">
                <td></td>
                <td>
                    <div style="height: 40px;">
                        <div style="height: 100%">
                            <div data-bind="visible: !isAuthorizing() && !appInstallationsLoaded()">
                                <button id="github-load-installation-button"><%:AdminServerResources.AuthorizeButtonText %></button>
                            </div>
                            <div data-bind="visible: isAuthorizing()">
                                <img style="vertical-align: top; margin-right: 2px;" src="<%= StaticResources.Versioned.Content.GetLocation("spinner.gif") %>" />
                                <span><%:AdminServerResources.AuthorizingInProgressText %></span>
                            </div>
                            <div class="github-oauth-success" data-bind="visible: authorizationSucceeded() && appInstallationsLoaded()">
                                <span class="icon icon-tfs-build-status-succeeded" style="vertical-align: top" />
                                <span data-bind="text: authorizationCompletedText" />
                            </div>
                            <div class="github-oauth-error" data-bind="visible: authorizationCompleted() && !authorizationSucceeded() && appInstallationsLoaded()">
                                <span class="icon icon-tfs-build-status-failed" style="vertical-align: top" />
                                <span data-bind="text: authorizationCompletedText" />
                            </div>
                        </div>
                    </div>
                </td>
            </tr>

            <tr data-bind="visible: tokenChoice() != 2 && authAsChoice() == 1">
                <td>
                    <div data-bind="visible: tokenChoice() == 1">
                        <div data-bind="visible: isHosted()">
                            <span id="github-token-label"><%:AdminServerResources.GitHubTokenLabel %></span>
                        </div>
                        <div data-bind="visible: !isHosted()">
                            <span id="github-pat-label"><%:AdminServerResources.GitHubPersonalAccessTokenLabel %></span>
                        </div>
                    </div>
                </td>
                <td>
                    <div style="height: 40px;">
                        <div data-bind="visible: tokenChoice() == 0" style="height: 100%">
                            <div data-bind="visible: !isAuthorizing() && !authorizationCompleted()">
                                <button id="github-authorize-button"><%:AdminServerResources.AuthorizeButtonText %></button>
                            </div>
                            <div data-bind="visible: isAuthorizing()">
                                <img style="vertical-align: top; margin-right: 2px;" src="<%= StaticResources.Versioned.Content.GetLocation("spinner.gif") %>" />
                                <span><%:AdminServerResources.AuthorizingInProgressText %></span>
                            </div>
                            <div class="github-oauth-success" data-bind="visible: authorizationSucceeded()">
                                <span class="icon icon-tfs-build-status-succeeded" style="vertical-align: top" />
                                <span data-bind="text: authorizationCompletedText" />
                            </div>
                            <div class="github-oauth-error" data-bind="visible: authorizationCompleted() && !authorizationSucceeded()">
                                <span class="icon icon-tfs-build-status-failed" style="vertical-align: top" />
                                <span data-bind="text: authorizationCompletedText" />
                            </div>
                        </div>
                        <div data-bind="visible: tokenChoice() == 1" style="height: 100%">
                            <div>
                                <input class="textbox" required id="accessToken" type="text" data-bind="attr: { 'aria-labelledby': isHosted() ? 'github-token-label' : 'github-pat-label' }" /></div>
                            <div style="margin-top: 2px"><span><b><%:AdminServerResources.GitHubRecommendedScopes %></b></span></div>
                        </div>
                    </div>
                </td>
            </tr>
            <tr data-bind="visible: tokenChoice() != 2">
                <td></td>
                <td>
                    <div style="margin-bottom: 10px"><a href="<%:AdminServerResources.GitHubLearnMoreLink %>" target="_blank"><%:AdminServerResources.GitHubLearnMoreLinkText %></a></div>
                </td>
            </tr>
            <tr data-bind="visible: disableUpdateTokenBasedEndpoint()">
                <td></td>
                <td>
                    <div style="height: 40px">
                        <span class="icon icon-tfs-build-status-failed" style="vertical-align: top" />
                        <span><%:AdminServerResources.GitHubLaunchServiceEndpointWarning %></span>
                    </div>
                </td>
            </tr>
            <tr data-bind="visible: tokenChoice() == 2">
                <td></td>
                <td>
                    <div style="margin-bottom: 10px"><a href="<%:AdminServerResources.GitHubManageAppLink %>" target="_blank"><%:AdminServerResources.GitHubManageAppLinkText %></a></div>
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
<script id="github-connected-service-details" type="text/html">
    <div class="github-connected-service-details">
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
                    <!-- ko if: (authorizationScheme == "PersonalAccessToken" || authorizationScheme == "OAuth") -->
                    <li data-bind="click: shareAcrossProjects, event: { keydown: onKeyDown }"><a tabindex="0" role="button" class="share-action"><%:AdminResources.ShareAcrossProjects %></a></li>
                    <!-- /ko -->
                    <!-- /ko -->
                </ul>
            </div>
        </div>
        <!-- /ko -->
    </div>
</script>
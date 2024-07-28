<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Build.WebApi" %>

<script id="buildvnext_repository_editor_git" type="text/html">
    <!-- ko with: baseViewModel -->
    <table class="buildvnext-definition-repo buildvnext-variables-grid-table">
        <tbody>
            <tr>
                <td class="option-label connection-id-label"><label><%:BuildServerResources.ExternalGitConnectionLabelText %></label></td>
                <td class="option-value connection-id-value">
                    <select data-bind="visible: hasServiceEndpoints(), options: computedServiceEndpoints, optionsText: 'name', value: serviceEndpoint"></select>
                    <input type="text" class="invalid" data-bind="enable: false, visible: !hasServiceEndpoints()"/>
                </td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: gitRepositoryConnectionMarkdown }" /> 
                <td class="connection-details"><a href="#" class="icon icon-refresh buildvnext-hovered" title="<%: BuildResources.Refresh %>" data-bind="click: refreshServiceEndpoints" /></td>
                <td class="connection-details"><a href="#" class="git-manage-link" target="_blank" title="<%:BuildServerResources.ConnectionManageLinkText %>"><%:BuildServerResources.ConnectionManageLinkText %></a></td>
            </tr>
            <tr>
                <td class="option-label"><label><%:BuildServerResources.BuildRepositoryNameLabel %></label></td>
                <td class="option-value"><input type="text" data-bind="value: name, valueUpdate: 'afterkeydown'" /></td>
            </tr>
            <tr>
                <td><label><%:BuildServerResources.BuildRepositoryGitDefaultBranchLabel %></label></td>
                <td><input type="text" data-bind="value: defaultBranch, valueUpdate: 'afterkeydown'" /></td>
            </tr>
            <tr>
                <td><label for="checkout-submodules"><%:BuildServerResources.BuildRepositoryCheckoutSubmodulesLabel %></label></td>
                <td><input id="checkout-submodules" type="checkbox" data-bind="checked: checkoutSubmodules" /></td>
            </tr>
            <tr>
                <td><label for="gitLfs-support"><%:BuildServerResources.BuildRepositoryGitLfsSupportLabel %></label></td>
                <td><input id="gitLfs-support" type="checkbox" data-bind="checked: gitLfsSupport" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: gitLfsSupportHelpMarkdown }" />
            </tr>
            <tr>
                <td><label for="skip-syncSource"><%:BuildServerResources.BuildRepositorySkipSyncSourceLabel %></label></td>
                <td><input id="skip-syncSource" type="checkbox" data-bind="checked: skipSyncSource" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: skipSyncSourceHelpMarkdown }" />
            </tr>
        </tbody>
    </table>
    <table class="buildvnext-definition-repo build-repo-misc-options">
        <tbody>
            <tr>
                <td class="shallow-label"><label for="shallow-repository"><%:BuildServerResources.BuildRepositoryShallowRepositoryLabel %></label></td>
                <td class="shallow-checkbox"><input id="shallow-repository" type="checkbox" data-bind="checked: shallowRepository" /></td>
                <td class="shallow-label-depth"><label class="second-col-label" for="fetch-depth_textbox"><%:BuildServerResources.BuildRepositoryFetchDepathLabel %></label></td>
                <td class="shallow-depth-input"><input id="fetch-depth_textbox" type="text" data-bind="enable: shallowRepository, value: fetchDepth, valueUpdate: 'afterkeydown', css: { 'invalid': _fetchDepthInvalid() }" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: shallowRepositoryHelpMarkdown }" />
            </tr>
            <tr>
                <td><label><%: BuildServerResources.BuildRepositoryCleanLabel %></label></td>
                <td><div class="repository-clean"></div></td>
                <td><label class="second-col-label"><%: BuildServerResources.BuildRepositoryCleanOptionsLabel %></label></td>
                <td>
                    <select id="repository-cleanOptions" data-bind="value: cleanOptions">
                        <option value="<%: (Int32)RepositoryCleanOptions.Source %>"><%: BuildServerResources.BuildRepositoryCleanOptionsSourcesLabel %></option>
                        <option value="<%: (Int32)RepositoryCleanOptions.SourceAndOutputDir %>"><%: BuildServerResources.BuildRepositoryCleanOptionsSourcesAndOutputDirLabel %></option>
                        <option value="<%: (Int32)RepositoryCleanOptions.SourceDir%>"><%: BuildServerResources.BuildRepositoryCleanOptionsSourcesDirLabel %></option>
                        <option value="<%: (Int32)RepositoryCleanOptions.AllBuildDir%>"><%: BuildServerResources.BuildRepositoryCleanOptionsAllBuildDirLabel %></option>
                    </select>
                </td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: cleanOptionHelpMarkDown }" />
            </tr>
        </tbody>
    </table>
    <!--/ko -->
</script>
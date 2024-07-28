<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Build.WebApi" %>

<script id="buildvnext_repository_editor_tfgit" type="text/html">
    <!-- ko with: baseViewModel -->
    <table class="buildvnext-definition-repo buildvnext-variables-grid-table">
        <tbody>
            <tr>
                <td class="option-label"><label><%: BuildServerResources.BuildRepositoryRepoLabel %></label></td>
                <td class="option-value">
                    <div class="repositories" data-bind="tfgitRepositorySelectorControl: { 'repository': gitRepository, 'onItemChanged': onItemChanged }"></div>
                </td>
            </tr>
            <tr>
                <td><label><%: BuildServerResources.BuildRepositoryGitDefaultBranchLabel %></label></td>
                <td><div data-bind="tfgitVersionSelectorControl: repositoryContext, observable: branch, repoOptions: { disableTags: true, popupOptions: { elementAlign: 'left-top', baseAlign: 'left-bottom' } }"></div></td>
            </tr>
            <tr>
                <td><label for="label-sources"><%: BuildServerResources.BuildRepositoryLabelSourcesLabel %></label></td>
                <td>
                    <select id="label-sources" data-bind="value: labelSources">
                        <option value="<%: (Int32)BuildResult.None %>"><%: BuildServerResources.BuildRepositoryLabelSourcesOptionNone %></option>
                        <option value="<%: (Int32)(BuildResult.Succeeded | BuildResult.PartiallySucceeded) %>"><%: BuildServerResources.BuildRepositoryLabelSourcesOptionSuccess %></option>
                        <option value="<%: (Int32)(BuildResult.Succeeded | BuildResult.PartiallySucceeded | BuildResult.Canceled | BuildResult.Failed) %>"><%: BuildServerResources.BuildRepositoryLabelSourcesOptionAlways %></option>
                    </select>
                </td>
            </tr>
            <tr data-bind="visible: labelSources() > 0">
                <td class="option-label"><label for="build-format"><%:BuildServerResources.SourceLabelFormat %></label></td>
                <td class="option-value"><input id="build-format" type="text" data-bind="css: { 'invalid': _isSourceLabelFormatInvalid() }, value: sourceLabelFormat, valueUpdate: 'afterkeydown'" /></td>
            </tr>
            <tr>
                <td class="option-label"><label for="report-build-status"><%:BuildResources.ReportBuildStatus %></label></td>
                <td class="option-value"><input id="report-build-status" type="checkbox" data-bind="checked: reportBuildStatus" /></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: reportBuildStatusHelpMarkdown }" />
            </tr>
            <tr>
                <td><label for="checkout-submodules"><%: BuildServerResources.BuildRepositoryCheckoutSubmodulesLabel %></label></td>
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

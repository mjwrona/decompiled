<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Build.WebApi" %>

<script id="buildvnext_repository_editor_tfvc" type="text/html">
    <!-- ko with: baseViewModel -->
    <table class="buildvnext-definition-repo buildvnext-variables-grid-table">
        <tbody>
            <tr>
                <td class="option-label"><label for="repository-name"><%:BuildServerResources.BuildRepositoryNameLabel %></label></td>
                <td class="option-value"><input id="repository-name" type="text" data-bind="value: name, valueUpdate: 'afterkeydown'" /></td>
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
                <td class="option-value"><input id="build-format" type="text" data-bind="value: sourceLabelFormat, valueUpdate: 'afterkeydown'" /></td>
            </tr>
        </tbody>
    </table>
    <table class="buildvnext-definition-repo build-repo-misc-options">
        <tbody>
            <tr>
                <td class="option-label"><label><%: BuildServerResources.BuildRepositoryCleanLabel %></label></td>
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
    <h3><%:BuildServerResources.TfvcRepoMappingsHeader %></h3>
    <table class="tfvc-mappings buildvnext-variables-grid-table">
        <thead>
            <tr>
                <th class="remove-icon"></th>
                <th><%:BuildServerResources.WorkspaceMappingsType %></th> 
                <th colspan="3"><%:BuildServerResources.WorkspaceMappingsServerPath %></th>
                <th></th>
                <th colspan="3"><%:BuildServerResources.WorkspaceMappingsLocalPath %></th>
            </tr>
        </thead>
        <tbody data-bind="foreach: mappings">
            <tr>
                <td class="local-path"><a title="Remove this mapping" href="#" data-bind="click: $parent.removeMapping"><span class="icon icon-delete-grey-f1-background red-delete-icon-hover"></span></a></td>
                <td class="local-path">
                    <select data-bind="value: mappingType">
                        <option value="map" selected><%:BuildServerResources.TfvcMapMappingText %></option>
                        <option value="cloak"><%:BuildServerResources.TfvcCloakMappingText %></option>
                    </select>
                </td>
                <td colspan="3">
                    <input type="text" data-bind="value: serverPath, valueUpdate: ['blur', 'afterkeydown'], css: { 'invalid': isServerPathInvalid() }"/>
                </td>
                <td>
                    <button class="file-path" data-bind="click: onSourcePickerClick">...</button>
                </td>
                <td class="local-path">
                    <label data-bind="visible: localPathVisible">$(build.sourcesDirectory)\</label>
                </td>
                <td colspan="3">
                    <input type="text" data-bind="value: displayedLocalPath, valueUpdate: ['blur', 'afterkeydown'], css: { 'invalid': isLocalPathInvalid() }, visible: localPathVisible" />
                </td>
            </tr>
        </tbody>
        <tfoot>
            <tr>
                <td><a title="Add Mapping" href="#" data-bind="click: addMapping"><span class="icon icon-add"></span></a></td>
                <td colspan="2"><a title="Add Mapping" href="#" data-bind="click: addMapping"><%:BuildServerResources.TfvcRepoAddMappingButtonText %></a></td>
            </tr>                            
        </tfoot>
    </table>
    <!--/ko -->
</script>
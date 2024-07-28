<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Build.WebApi" %>

<script id="buildvnext_repository_editor_svn" type="text/html">
    <!-- ko with: baseViewModel -->
    <table class="buildvnext-definition-repo buildvnext-variables-grid-table">
        <tbody>
            <tr>
                <td class="option-label connection-id-label"><label><%:BuildServerResources.SvnConnectionLabelText %></label></td>
                <td class="option-value connection-id-value">
                    <select data-bind="visible: hasConnections(), options: computedConnections, optionsText: 'name', value: connection"></select>
                    <input type="text" class="invalid" data-bind="enable: false, visible: !hasConnections()"/>
                </td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: svnRepositoryConnectionMarkDown }" /> 
                <td class="connection-details"><a href="#" class="icon icon-refresh buildvnext-hovered" title="<%: BuildResources.Refresh %>" data-bind="click: refreshConnectedServices" /></td>
                <td class="connection-details"><a href="#" class="svn-manage-link" target="_blank" title="<%:BuildServerResources.ConnectionManageLinkText %>"><%:BuildServerResources.ConnectionManageLinkText %></a></td>
            </tr>
            <tr>
                <td class="option-label"><label><%:BuildServerResources.BuildRepositorySvnDefaultBranchLabel %></label></td>
                <td class="option-value">
                    <div data-bind="css: { invalid: !defaultBranch() }">
                        <input type="text" data-bind="value: defaultBranch, valueUpdate: 'afterkeydown'" />
                    </div>
                </td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: svnBranchHelpMarkdown }" />
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
                        <option value="<%: (Int32)RepositoryCleanOptions.AllBuildDir%>"><%: BuildServerResources.BuildRepositoryCleanOptionsAllBuildDirLabel %></option>
                    </select>
                </td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: cleanOptionHelpMarkDown }" />
            </tr>
        </tbody>
    </table>
    <h3><%:BuildServerResources.SvnRepoMappingsHeader %></h3>
    <table class="tfvc-mappings buildvnext-variables-grid-table">
        <thead data-bind="visible: mappings().length > 0">
            <tr>
                <th class="variable-remove"></th>
                <th class="variable-name"><%:BuildServerResources.BuildDefinitionSvnMapServerPathHeader %></th>
                <th class="variable-value"><%:BuildServerResources.BuildDefinitionSvnMapLocalPathHeader %></th>
                <th></th>
                <th class="variable-value"><%:BuildServerResources.BuildDefinitionSvnMapRevisionHeader %></th>
                <th class="variable-value"><%:BuildServerResources.BuildDefinitionSvnMapDepthHeader %></th>
                <th class="variable-value"><%:BuildServerResources.BuildDefinitionSvnMapIgnoreExternalsHeader %><th>
            </tr>
        </thead>
        <tbody data-bind="foreach: mappings, visible: mappings().length > 0">           
            <tr>
                <td class="local-path">
                    <a title="Remove this mapping" href="#" data-bind="click: $parent.removeMapping">
                        <span class="icon icon-delete-grey-f1-background red-delete-icon-hover"></span>
                    </a>
                </td>
                <td>
                    <div data-bind="css: { invalid: !_isValidServerPath() }">
                        <input type="text" data-bind="value: serverPath, valueUpdate: ['blur', 'afterkeydown']"/>
                    </div>
                </td>
                <td class="local-path">
                    <label>$(build.sourcesDirectory)/</label>
                </td>
                <td>
                    <div data-bind="css: { invalid: !_isValidLocalPath() }">
                        <input type="text" data-bind="value: localPath, valueUpdate: ['blur', 'afterkeydown']"/>
                    </div>
                </td>
                <td>
                    <div data-bind="css: { invalid: !_isValidRevision() }">
                        <input class="svn-revision-value" type="text" data-bind="value: revision, valueUpdate: ['blur', 'afterkeydown']"/>
                    </div>
                </td>
                <td class="local-path">
                    <select data-bind="value: depth">
                        <option value="0">Empty</option>
                        <option value="1">Files</option>
                        <option value="2">Children</option>
                        <option value="3">Infinity</option>
                    </select>
                </td>
                <td class="local-path">
                    <input type="checkbox" data-bind="checked: ignoreExternals"/>
                </td>
            </tr>                            
        </tbody>
        <tfoot>
            <tr>
                <td><a title="Add Mapping" href="#" data-bind="click: addMapping"><span class="icon icon-add"></span></a></td>
                <td colspan="5"><a title="Add Mapping" href="#" data-bind="click: addMapping"><%:BuildServerResources.BuildDefinitionSvnAddMappingButtonText %></a></td>
                <td class="helpMarkDown" data-bind="showTooltip: { text: svnMappingHelpMarkdown, minWidth: 500 }" />
            </tr>                            
        </tfoot>
    </table>
    <!--/ko -->
</script>
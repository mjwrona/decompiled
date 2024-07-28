<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script id="queue_definition_dialog_demands_content" type="text/html">
    <table class="buildvnext-definition-general-demands buildvnext-variables-grid-table">
        <thead>
            <tr>
                <th class="remove-icon"></th>
                <th class="demand-name"><%:BuildServerResources.BuildDefinitionDemandNameHeader %></th>
                <th class="demand-type"><%:BuildServerResources.BuildDefinitionDemandTypeHeader %></th>
                <th class="demand-value"><%:BuildServerResources.BuildDefinitionDemandValueHeader %></th>
            </tr>
        </thead>
        <tbody data-bind="foreach: demands">
            <tr>
                <td>
                    <a href="#" data-bind="click: $parent.removeDemand.bind($parent)">
                        <span class="icon icon-delete-grey-f1-background red-delete-icon-hover" aria-label="<%:BuildServerResources.BuildDefinitionRemoveDemandText %>"></span>
                    </a>
                </td>
                <td>
                    <input type="text" data-bind="value: name, triggerFocus: true, attr: { 'aria-label': inputAriaLabel }, id: name, valueUpdate: 'afterkeydown'" /></td>
                <td>
                    <select data-bind="value: type">
                        <option value="exists"><%:BuildServerResources.BuildDefinitionDemandExistsText %></option>
                        <option value="equals"><%:BuildServerResources.BuildDefinitionDemandEqualsText %></option>
                    </select>
                </td>
                <td>
                    <input type="text" data-bind="visible: valueVisible, attr: { 'aria-label': inputValueAriaLabel }, value: value, valueUpdate: 'afterkeydown'" /></td>
            </tr>
        </tbody>
        <tbody>
            <tr>
                <td colspan="4">
                    <button data-bind="click: addDemand">
                        <span class="icon icon-add"></span>&nbsp;
                        <%:BuildServerResources.BuildDefinitionAddDemandText %>
                    </button>
                </td>
            </tr>
        </tbody>
    </table>
</script>

<script id="queue_definition_dialog_variables_content" type="text/html">
    <!-- ko with: queueTimeVariables -->
    <table class="filter">
        <tbody data-bind="foreach: variables">
            <!-- ko ifnot: isImplicit -->
            <!-- ko if: allowOverride -->
            <tr>
                <td>
                    <a href="#" data-bind="click: $parent.removeVariable, visible: canRemove">
                        <span class="icon icon-delete-grey-f1-background red-delete-icon-hover" aria-label="<%:BuildServerResources.GridRemoveVariableText %>"></span>
                    </a>
                </td>
                <!-- ko if: canRemove -->
                <td class="data-column">
                    <%--ko's hasfocus binding won't work here as this element is added to the DOM, looks like it works only for existing DOM elements--%>
                    <%--So, use custom triggerFocus binding. It's fine if multiple elements has true, since latest one wins.--%>
                    <input type="text" data-bind="value: name, valueUpdate: 'afterkeydown', triggerFocus: true, attr: { 'aria-label': inputAriaLabel }, disable: isImplicit, css: { disabled: isImplicit, 'invalid': isNameInvalid() }" />
                </td>
                <!-- /ko -->
                <!-- ko ifnot: canRemove -->
                <td class="data-column">
                    <label data-bind="attr: { 'for': name }, text: name" />
                </td>
                <!-- /ko -->
                <td>
                    <!-- ko ifnot: showSecretPlaceholder -->
                    <input class="queue-time-variable-input" data-bind="attr: { 'type': inputType, 'aria-label': inputValueAriaLabel }, value: value, valueUpdate: 'afterkeydown'" />
                    <!-- /ko -->
                    <!-- ko if: showSecretPlaceholder -->
                    <input class="queue-time-variable-input" data-bind="attr: { 'aria-label': inputValueAriaLabel }" type="password" value="********" disabled="disabled" />
                    <!-- /ko -->
                </td>
                <td class="queue-time-secret-td">
                    <!-- ko if: showSecretPlaceholder -->
                    <span class="icon icon-restricted-2" data-bind="css: { 'not-secret': !isSecret() }, click: onSecretClick" title="<%: BuildServerResources.GridSecretVariableText %>"></span>
                    <!-- /ko -->
                </td>
            </tr>
            <!-- /ko -->
            <!-- /ko -->
        </tbody>
        <tbody>
            <tr>
                <td colspan="4">
                    <button data-bind="click: addVariable">
                        <span class="icon icon-add"></span>&nbsp;
                        <%:BuildServerResources.GridAddVariableText %>
                    </button>
                </td>
            </tr>
        </tbody>
    </table>
    <!-- /ko -->
</script>

<script id="queue_definition_dialog" type="text/html">
    <div class="queue-build">
        <!-- ko if: warningMessage -->
        <div class="queue-build-messagebar">
            <span data-bind="text: warningMessage"></span>
        </div>
        <!-- /ko -->
        <table class="filter">
            <tr>
                <td colspan="2">
                    <label for="queue"><%:BuildServerResources.QueueBuildQueueTitle %></label>
                    <select id="queue" class="queue" data-bind="options: queues, optionsText: 'name', optionsValue: 'id'"></select>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <label for="branch"><%:BuildServerResources.QueueBuildBranchTitle %></label>
                    <input type="text" id="branch" data-bind="value: selectedBranch, valueUpdate: 'afterkeydown'" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <label for="commit"><%:BuildServerResources.QueueDefinitionDialogSourceVersionTitle %></label>
                    <input type="text" id="commit" data-bind="value: sourceVersion" />
                </td>
            </tr>
        </table>
        <div class="queue-build-dialog-tabs tfs_knockout_hubpageexplorerpivot_holder"></div>
        <!-- ko foreach: tabs -->
        <div class="tab-content-container queue-dialog-tab" data-bind="visible: isSelected">
            <div data-bind="applyTemplate: { templateName: templateName, viewModel: $data }"></div>
        </div>
        <!-- /ko -->
    </div>
</script>

<script type="text/html" id="queue_xaml_build_dialog">
    <div class="queue-build">
        <table class="filter">
            <tr>
                <td colspan="2">
                    <label for="source">
                        <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildWhatToBuild %></label>
                    <select id="source" class="source">
                        <option value="latest" selected="selected"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildWhatToBuildLatest %></option>
                        <option value="latest-with-shelveset"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildWhatToBuildLatestWithShelveset %></option>
                    </select>
                </td>
            </tr>
            <tr class="shelveset-picker-container">
                <td>
                    <label for="source">
                        <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildShelvesetTitle %></label>
                    <input type="text" id="shelveset" class="shelveset" />
                </td>
                <td style="width: 1%">
                    <button class="browse">...</button></td>
            </tr>
            <tr class="check-in-container">
                <td colspan="2">
                    <input type="checkbox" id="check-in" class="check-in" />
                    <label for="check-in" class="check-in">
                        <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildCheckinChanges %></label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <label for="controller">
                        <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildControllerTitle %></label>
                    <select id="controller" class="controller">
                    </select>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <label for="priority">
                        <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildPriorityTitle %></label>
                    <select id="priority" class="priority">
                        <option value="high"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityHigh %></option>
                        <option value="abovenormal"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityAboveNormal %></option>
                        <option value="normal" selected="selected"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityNormal %></option>
                        <option value="belownormal"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityBelowNormal %></option>
                        <option value="low"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityLow %></option>
                    </select>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <label for="drop-folder">
                        <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildDropLocationTitle %></label>
                    <input type="text" id="drop-folder" class="drop-folder" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <label for="msbuild-args">
                        <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildMSBuildArgsTitle %></label>
                    <input type="text" id="msbuild-args" class="msbuild-args" />
                </td>
            </tr>
        </table>
    </div>
</script>

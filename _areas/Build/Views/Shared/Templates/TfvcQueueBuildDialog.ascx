<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script id="tfvc_queue_definition_dialog" type="text/html">
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
                    <label for="commit"><%:BuildServerResources.QueueDefinitionDialogSourceVersionTitle %></label>
                    <input type="text" id="commit" data-bind="value: sourceVersion" />
                </td>
                <td class="helpMarkDown queue-definition-dialog-helpmarkdown" data-bind="showTooltip: { text: sourceVersionHelpMarkDown }" />
            </tr>
            <tr>
                <td colspan="2">
                    <label for="branch"><%:BuildServerResources.QueueBuildShelvesetTitle %></label>
                    <input type="text" id="branch" data-bind="value: sourceOptions.shelvesetName" />
                </td>
                <td class="browse-col">
                    <button data-bind="click: sourceOptions.onShelvePickerClick.bind($data)">...</button></td>
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
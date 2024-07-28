<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script id="tfgit_queue_definition_dialog" type="text/html">
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
                    <div data-bind="tfgitVersionSelectorControl: sourceOptions.repositoryContext, observable: selectedBranch, repoOptions: { disableTags: false, popupOptions: { elementAlign: 'left-top', baseAlign: 'left-bottom' } }"></div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <label for="commit"><%:BuildServerResources.QueueBuildCommitIdTitle %></label>
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
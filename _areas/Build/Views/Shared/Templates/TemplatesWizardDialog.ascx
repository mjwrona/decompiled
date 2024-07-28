<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<%
    // Include required templates here so that loading this templated would be enough to get all templates
    // This template is loaded by Code tab as well, so load templates from areas relative path

    Html.RenderPartial("~/_areas/Build/Views/Shared/Templates/TfGitRepositoryEditor.ascx");
    Html.RenderPartial("~/_areas/Build/Views/Shared/Templates/TfvcRepositoryEditor.ascx");
    Html.RenderPartial("~/_areas/Build/Views/Shared/Templates/GitHubRepositoryEditor.ascx");
    Html.RenderPartial("~/_areas/Build/Views/Shared/Templates/GitRepositoryEditor.ascx");
    Html.RenderPartial("~/_areas/Build/Views/Shared/Templates/SvnRepositoryEditor.ascx");
%>

<script id="loading_templates" type="text/html">
    <div data-bind="html: title"></div>
</script>

<script id="add_templates_dialog_content" type="text/html">
    <ul class="templates-list" data-bind="foreach: templateVMs, attr: { 'id': id }">
        <li class="templates-list-item" data-bind="attr: { 'id': id }, css: { 'templates-list-item-selected': id === $parent.selectedTemplateId() }, click: $parent.onTemplateClick">
            <!-- ko if: iconUrl -->
            <div class="icon">
                <img data-bind="attr: { src: iconUrl }" />
            </div>
            <!-- /ko -->
            <div class="templates-list-item-details">
                <div class="dialog-item-title" data-bind="text: friendlyName"></div>
                <div class="templates-list-item-description" data-bind="attr: { title: description }, text: description"></div>
                <!-- ko if: deleteFunction && canDelete -->
                <div class="templates-list-item-delete">
                    <button data-bind="click: deleteCommand" value="Delete"><%:BuildServerResources.DeleteTemplateText %></button>
                </div>
                <!-- /ko -->
            </div>
        </li>
    </ul>
</script>

<script id="select_template_page" type="text/html">
    <div class="template-tabs">
        <ul class="pivot-view pivot-tabs" data-bind="foreach: tabs">
            <li data-bind="css: { 'selected': isSelected }, visible: isVisible, attr: { 'title': title, 'tabindex': 0 }, click: $parent._onTabClick, triggerClickOnKeyUp: $parent._onTabClick">
                <a data-bind="text: text"></a>
            </li>
        </ul>
    </div>
    <div class="add-task-templates-dialog">
        <!-- ko foreach: tabs -->
        <div class="tab-content-container" data-bind="visible: isSelected">
            <div data-bind="applyTemplate: { templateName: templateName, viewModel: $data }"></div>
        </div>
        <!-- /ko -->
    </div>
    <div class="template-empty-def">
        <!-- ko if: emptyDefinitionTab -->
        <div data-bind='applyTemplate: { templateName: emptyDefinitionTab().templateName, viewModel: emptyDefinitionTab }'></div>
        <!-- /ko -->
    </div>
</script>

<script id="selected_definition_build_repo_source" type="text/html">
    <div data-bind="createFolderManageReactComponent: { 'showDialog': showFolderDialog(), 'title': folderPathDialogTitle, 'refreshIfNotInitialized': true, 'showDialogActions': true, 'defaultPath': folderPath(), 'okManageDialogCallBack': function (args) { onFolderManageDialogOkClick(args); }, 'onManageDialogDissmiss': function () { onFolderManageDialogDismiss() } }"></div>
    <!-- ko if: showFolderDialog -->
    <%--Note: since we are using office fabric dialog with webplatform's dialog widget, content is interfering with the new popup dialog, no amount of z-index combinations appears to fix this--%>
    <%-- as a work-around doing this made it working again--%>
    <div><%: BuildResources.WaitingForUserInput %></div>
    <!-- /ko -->
    <!-- ko ifnot: showFolderDialog -->
    <div class="repo-source-options">
        <div class="repository-source" data-bind="visible: !hideRepoPicker()">
            <label><%: BuildServerResources.TemplateWizardRepositorySourceLabel %></label>
            <div class="repository-source-blocks" data-bind="foreach: repositorySourceBlocks">
                <div class="repository-source-block" data-bind="attr: { id: id }, click: $parent.repositorySourceClicked.bind($data, $parent), css: { 'repository-source-block-selected': $parent.selectedRepositorySourceId() == id }">
                    <div data-bind="attr: { 'class': 'block-image icon-' + id + '-logo-' + (id == $parent.selectedRepositorySourceId() ? 'white' : 'greyscale') }"></div>
                    <span class="block-text" data-bind="attr: { title: text }, text: text"></span>
                </div>
            </div>
        </div>

        <div data-bind="applyTemplate: { templateName: 'wizard-repository-source-project', viewModel: $data }"></div>

        <div class="ci" data-bind="visible: labelForCI">
            <input id="ci" type="checkbox" data-bind="checked: enableCI, disable: templateHasCITrigger" />
            <label for="ci" data-bind="text: labelForCI"></label>
        </div>

        <div class="queue">
            <span class="queue-select-text">
                <label><%: BuildServerResources.TemplateWizardDefaultAgentQueueLabel %> </label>
                <a data-bind="attr: { href: queueManageLink }" target="_blank" title="<%: BuildServerResources.TemplateWizardManageQueuesLabel %>"><%: BuildServerResources.TemplateWizardManageQueuesLabel %></a>
                <span class="icon build-icon-external-link"></span>
            </span>
            <select id="queues" data-bind="foreach: queues, value: selectedQueueId" class="queues">
                <option data-bind="value: id, text: name"></option>
            </select>
            <a href="#" class="icon build-icon-refresh buildvnext-hovered" title="<%: BuildResources.Refresh %>" data-bind="click: refreshQueues"></a>
        </div>
        <div class="folder" data-bind="visible: !hideFolderPicker()">
            <label for="folder"><%: BuildResources.BuildSelectFolderLabel %></label>
            <div class="folder-input" data-bind="createComboControl: { 'options': { 'change': function (args) { onFolderComboInputChange(args); }, 'source': folderPathsSource() }, 'observable': folderPath }"></div>
            <button class="folder-path" data-bind="click: onFolderPickerClick"><%: BuildResources.BuildPickFolderLabel %></button>
        </div>
    </div>
    <!-- /ko -->
</script>

<script id="wizard-repository-source-project" type="text/html">
    <div class="repo-selection" data-bind="visible: showRepoSelector()">
        <label><%: BuildServerResources.BuildRepositoryRepoLabel %></label>
        <div class="repository-picker-holder">
            <div class="repository-picker" data-bind="css: { 'disabled-div': hideRepoPicker() }, tfgitRepositorySelectorControl: { 'repository': repository, 'onItemChanged': onItemChanged, 'showTfvc': 'true' }"></div>
            <div class="branch-picker-holder" data-bind="visible: showBranchPicker">
                <label><%: BuildServerResources.BuildRepositoryGitDefaultBranchLabel %></label>
                <div class="branch-picker" data-bind="tfgitVersionSelectorControl: repositoryContext, observable: selectedBranch, repoOptions: { disableTags: true, popupOptions: { elementAlign: 'left-top', baseAlign: 'left-bottom' } }"></div>
            </div>
        </div>
    </div>
</script>

<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script type="text/html" id="buildvnext_view_explorer_hubtitle">
    <span data-bind="text: hubTitle"></span>
    <!-- ko if: canEditDefinition -->
    <span>| </span><a href="#" class="edit-definition"><%:BuildServerResources.BuildDefinitionEditLinkText %></a>
    <!-- /ko -->
    <p class="description" data-bind="text: description"></p>
</script>

<script type="text/html" id="buildvnext_view_explorer_rightpane">
    <!-- ko with: listView -->
    <div class="hub-pivot">
        <div class="views">
            <div class="build-explorer-rightpane-tabs tfs_knockout_hubpageexplorerpivot_holder"></div>
        </div>
        <div class="filters">
            <div class="build-list-tags"></div>
        </div>
    </div>

    <div class="buildvnext-view-right-pane-content">
        <div class="toolbar hub-pivot-toolbar"></div>
        <div class="buildvnext-view-right-pane">
            <!-- ko foreach: tabs -->
            <div class="tab-content-container" data-bind="visible: isSelected">
                <div data-bind="applyTemplate: { templateName: templateName, viewModel: $data }"></div>
            </div>
            <!-- /ko -->
        </div>
    </div>
    <!-- /ko -->
</script>

<script type="text/html" id="buildvnext_view_result_hubtitle">
    <span class="build-summary-page-hub-title">
        <!-- ko if: definitionName -->
        <a data-bind="text: definitionName, click: onDefinitionNameClicked, attr: { href: definitionSummaryUrl }"></a>
        <!-- /ko -->

        <!-- ko if: currentTimelineRecordName -->
        <!-- ko if: definitionName -->
        <span>/ </span>
        <!-- /ko -->
        <a role="button"  tabindex="0" data-bind="text: buildNumber, click: onBuildTitleClicked, triggerClickOnKeyUp: onBuildTitleClicked" aria-label="<%: BuildResources.JumpToRootBuild %>"></a>
        <!-- /ko -->

        <!-- ko ifnot: currentTimelineRecordName -->
        <!-- ko if: definitionName -->
        <span>/ </span>
        <!-- /ko -->
        <span role="heading" data-bind="text: buildNumber"></span>
        <!-- ko if: currentTimelineRecordResult -->
        <span>- </span><span data-bind="text: currentTimelineRecordResult"></span>
        <!-- /ko -->
        <!-- /ko -->

        <!-- ko if: parentTimelineRecordName -->
        <span>/ </span><a role="button" tabindex="0" data-bind="text: parentTimelineRecordName, click: onParentTimelineRecordClicked, triggerClickOnKeyUp: onParentTimelineRecordClicked" aria-label="<%: BuildResources.JumpToParentTimeLineText %>"></a>
        <!-- /ko -->
        <!-- ko if: currentTimelineRecordName -->
        <span>/ </span><span data-bind="text: currentTimelineRecordName"></span>
        <!-- /ko -->
    </span>
</script>

<script type="text/html" id="buildvnext_view_result_rightpane">
    <!-- ko with: selectedView -->
    <!-- ko if: showErrorPanel() -->
    <div class="inline-error" data-bind="text: errorMessage"></div>
    <!-- /ko -->
    <!-- /ko -->

    <!-- ko ifnot: selectedView().showErrorPanel() -->
    <div data-bind="template: 'buildvnext_details_header'" class="buildvnext-details-header"></div>
    <!-- ko with: selectedView -->
    <div class="hub-pivot">
        <div class="views">
            <div class="build-details-tabs tfs_knockout_hubpageexplorerpivot_holder"></div>
        </div>
        <!-- ko if: showCopyButton -->
        <div class="buildvnext-view-copy-button">
            <button class="bowtie-tooltipped tooltip-align-top bowtie-tooltipped-sw multiple-toggle-copy-button" data-bind="click: _onCopyButtonClick, attr: { 'aria-label': _copyLabel}, event: { blur: _resetCopy }"><span class="bowtie-icon bowtie-edit-copy"></span></button>
        </div>
        <!-- /ko -->
    </div>
    <div class="buildvnext-view-right-pane-content">
        <div class="buildvnext-view-right-pane">
            <!-- ko ifnot: showErrorPanel() -->
            <!-- ko foreach: tabs -->
            <div class="tab-content-container" data-bind="visible: isSelected">
                <div data-bind="applyTemplate: { templateName: templateName, viewModel: $data, cssClass: 'tab-content-holder' }"></div>
            </div>
            <!-- /ko -->
            <!-- /ko -->
        </div>
    </div>
    <!-- /ko -->
    <!-- /ko -->
</script>

<script type="text/html" id="buildvnext_details_tab_section">
    <!-- ko if: contributions().length == 0 || contributingToExistingSection()-->
    <div class="custom-section">
        <div data-bind="applyTemplate: { templateName: template, viewModel: $vmparent, customData: { displayName: displayName }, fadeIn: true }"></div>
        <div class="content" data-bind="foreach: messages">
            <div class="summary-item" data-bind="renderSummaryMarkdown: { 'markDown': $data }"></div>
        </div>
    </div>
    <!-- /ko -->

    <!-- ko if: contributions().length > 0 -->
    <div data-bind="foreach: contributions">
        <div class="custom-contributed-section">
            <!-- ko ifnot: $parent.contributingToExistingSection() -->
            <div data-bind="applyTemplate: { templateName: $parent.template, viewModel: $parent, customData: { displayName: $parent.displayName }, fadeIn: true }"></div>
            <!-- /ko -->
            <div data-bind="attr: { id: $data.id }, enhanceResultsViewContributions: { contribution: $data }" class="hub-external"></div>
        </div>
    </div>
    <!-- /ko -->
</script>

<script type="text/html" id="buildvnext_details_sections">
    <div class="summary-section-holder-left" data-bind="foreach: sections">
        <!-- ko if: column() == 0 && isVisible -->
        <div data-bind="applyTemplate: { templateName: 'buildvnext_details_tab_section', viewModel: $data, parentIndex: '0', applyBindingsOnlyOnce: true, fadeIn: true }"></div>
        <!-- /ko -->
    </div>
    <div class="summary-section-holder-right" data-bind="foreach: sections">
        <!-- ko if: column() == 1 && isVisible -->
        <div data-bind="applyTemplate: { templateName: 'buildvnext_details_tab_section', viewModel: $data, parentIndex: '0', applyBindingsOnlyOnce: true, fadeIn: true }"></div>
        <!-- /ko -->
    </div>
</script>

<script type="text/html" id="buildvnext_details_section_header">
    <div class="summary-section">
        <h2 class="summary-section-header" data-bind="text: $vmcustom.displayName"></h2>
    </div>
</script>


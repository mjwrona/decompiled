<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script type="text/html" id="buildvnext_details_console_tab2">
    <!-- ko with: $data.pipelineQueueViewModel-->
    <div class="buildvnext-licensing-queue" data-bind="visible: showPipelineQueue">
        <div class="buildvnext-licensing-queue__message" data-bind="html: licensingPipelineMessage"></div>
        <div class="buildvnext-licensing-queue__details">
            <div class="buildvnext-licensing-queue__positionTitle"><%= BuildServerResources.LicensingPipelineHeader %></div>
            <span class="buildvnext-licensing-queue__position" data-bind="text: queuePosition"></span>
            <span class="buildvnext-licensing-queue__positionQualifier"><%= BuildServerResources.QueuePositionQualifierText %></span>
            <div class='horizontal-title-separator'></div>
            <a class="buildvnext-licensing-queue__buyMore" data-bind="attr: { href: buyMoreLink }" target="_blank"><%= BuildServerResources.BuyMorePipelines %></a>
            <div>
                <div class='horizontal-title-separator adjust-margin'></div>
                <div class='pipeline-plan-groups-queue-dialog-placeholder'></div>
                <div tabindex="0" data-bind="event: { keydown: onLinkKeyDown, click: showPlanGroupsQueueDialog }" class="buildvnext-licensing-queue__show-pipelines pipeline-plan-groups-queue-dialog-open-link as-link" role="button"><%:BuildServerResources.PipelinesPlanGroupsQueueLinkText %></div>
            </div>
        </div>
    </div>
    <!-- /ko -->
    <!-- ko ifnot: $data.pipelineQueueViewModel.showPipelineQueue -->
        <!-- ko with: $data.selectedJob -->
        <div class="buildvnext-jobs-console" data-bind="visible: loaded() && !started()">
            <span class="buildvnext-jobs-console__title" data-bind="text: title" />
            <br />
            <ul class="buildvnext-jobs-console__agents" data-bind="foreach: enabledAgents">
                <li class="buildvnext-jobs-console__item" data-bind="css: itemCss">
                    <!-- ko if: manageLink -->
                    <a class="buildvnext-jobs-console__item__name" data-bind="attr: { href: manageLink }, text: name" target="_blank"></a>
                    <!-- /ko -->
                    <!-- ko ifnot: manageLink -->
                    <span class="buildvnext-jobs-console__item__name" data-bind="text: name"></span>
                    <!-- /ko -->
                    <span class="buildvnext-jobs-console__item__position" data-bind="text: position"></span>
                    <span class="buildvnext-jobs-console__item__positionQualifier" data-bind="visible: enabled"><%= BuildServerResources.QueuePositionQualifierText %></span>
                    <!-- ko if: !active() || !statusLink() -->
                    <span class="buildvnext-jobs-console__item__status" data-bind="text: statusText"></span>
                    <!-- /ko -->
                    <!-- ko if: active && statusLink -->
                    <a class="buildvnext-jobs-console__item__status buildvnext-jobs-console__item__status--running" data-bind="attr: { href: statusLink }, text: statusText" target="_blank"></a>
                    <!-- /ko -->
                </li>
            </ul>
            <!-- ko if: enabledAgents().length > 0 && disabledAgents().length > 0 -->
            <span class="buildvnext-jobs-console__title"><%:BuildServerResources.DisabledAgentsTitle %></span>
            <!-- /ko -->
            <ul class="buildvnext-jobs-console__agents" data-bind="foreach: disabledAgents">
                <li class="buildvnext-jobs-console__item" data-bind="css: itemCss">
                    <!-- ko if: manageLink -->
                    <a class="buildvnext-jobs-console__item__name" data-bind="attr: { href: manageLink }, text: name" target="_blank"></a>
                    <!-- /ko -->
                    <!-- ko ifnot: manageLink -->
                    <span class="buildvnext-jobs-console__item__name" data-bind="text: name"></span>
                    <!-- /ko -->
                    <span class="buildvnext-jobs-console__item__position" data-bind="text: position"></span>
                    <span class="buildvnext-jobs-console__item__positionQualifier" data-bind="visible: enabled"><%= BuildServerResources.QueuePositionQualifierText %></span>
                    <!-- ko if: !active() || !statusLink() -->
                    <span class="buildvnext-jobs-console__item__status" data-bind="text: statusText"></span>
                    <!-- /ko -->
                    <!-- ko if: active && statusLink -->
                    <a class="buildvnext-jobs-console__item__status buildvnext-jobs-console__item__status--running" data-bind="attr: { href: statusLink }, text: statusText" target="_blank"></a>
                    <!-- /ko -->
                </li>
            </ul>
        </div>
        <!-- /ko -->
    <!-- /ko -->
</script>

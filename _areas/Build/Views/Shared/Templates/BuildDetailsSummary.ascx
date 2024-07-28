<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<script type="text/html" id="buildvnext_details_summary_tab_builddetails">
    <div class="summary-section">
        <h2 class="summary-section-header" data-bind="text: $vmcustom.displayName"></h2>

        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummaryDefinitionLabel %></label>
            <a data-bind="text: definitionName, click: onDefinitionClick, attr: { href: definitionSummaryUrl }"></a>
            <!-- ko if: canEditDefinition -->
            <a data-bind="click: onDefinitionEdit, attr: { href: definitionEditUrl, 'aria-label': definitionNameEditLabel }"><%: BuildServerResources.BuildSummaryDefinitionEditText %></a>
            <!-- /ko -->
        </div>
        <!-- ko if: canShowSourceBranch -->
        <div class="summary-item">
            <label data-bind="text: sourceBranchLabel"></label>
            <!-- ko if: canLinkSourceBranch -->
            <a data-bind="text: sourceBranch, click: onBranchClick, attr: { href: sourceBranchUrl }"></a>
            <!-- /ko -->
            <!-- ko ifnot: canLinkSourceBranch -->
            <span data-bind="text: sourceBranch"></span>
            <!-- /ko -->
        </div>
        <!-- /ko -->
        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummarySourceVersionLabel %></label>
            <!-- ko if: linkSourceVersion -->
            <a data-bind="text: sourceVersion, click: onSourceVersionClick, attr: { href: sourceVersionUrl }"></a>
            <!-- /ko -->
            <!-- ko ifnot: linkSourceVersion -->
            <span data-bind="text: sourceVersion"></span>
            <!-- /ko -->
        </div>
        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummaryRequestedByLabel %></label>
            <span data-bind="text: requestedBy"></span>
        </div>
        <!-- ko if: queueName -->
        <div class="summary-item">
            <label><%: BuildServerResources.QueueNameText %></label>
            <a data-bind="text: queueName, attr: { href: queueManageLink, 'aria-label': queueManageLabel }"></a>
        </div>
        <!-- /ko -->
        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummaryQueueTimeLabel%></label>
            <span data-bind="text: queueTime"></span>
        </div>
        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummaryStartTimeLabel%></label>
            <span data-bind="text: startTime"></span>
        </div>
        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummaryFinishTimeLabel %></label>
            <span data-bind="text: finishTime"></span>
        </div>
        <!-- ko if: deleted -->
        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummaryDeletedTimeLabel %></label>
            <span data-bind="text: dateDeleted"></span>
        </div>
        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummaryDeletedByLabel%></label>
            <span data-bind="text: deletedBy"></span>
        </div>
        <div class="summary-item">
            <label><%: BuildServerResources.BuildSummaryDeletedReasonLabel%></label>
            <span data-bind="text: deletedReason"></span>
        </div>
        <!-- /ko -->
        <div class="summary-item">
            <label><%: BuildServerResources.RetainedStateText %></label>
            <span data-bind="text: retainStateText"></span>
        </div>
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_issues">
    <div class="summary-section" data-bind="visible: canShowIssues">
        <h2 class="summary-section-header" data-bind="text: $vmcustom.displayName"></h2>

        <!-- ko if: hasValidationResults -->
        <div class="summary-item">
            <!-- ko foreach: validationResults -->
            <div class="build-issue">
                <div class="icon build-issue-icon" data-bind="css: iconCssClass" />
                <div class="build-issue-detail" data-bind="text: message"></div>
            </div>
            <!-- /ko -->
        </div>
        <!-- /ko -->
        <%--Issues built from jobs--%>
        <!-- ko foreach: jobs -->
        <!-- ko if: hasIssues -->
        <div class="summary-item">
            <div class="summary-item-header">
                <span data-bind="text: name" />
            </div>
            <!-- ko foreach: issues -->
            <div class="build-issue" data-bind="template: { name: getTemplateName() }"></div>
            <!-- /ko -->
        </div>
        <!-- /ko -->
        <!-- /ko -->
        <%--Issues coming from xaml builds--%>
        <!-- ko if: (xamlBuildIssues().length > 0) -->
        <div class="summary-item-header">
            <span><%: BuildResources.BuildSequenceTitle %></span>
        </div>
        <div class="summary-item">
            <!-- ko foreach: xamlBuildIssues -->
            <div class="build-issue" data-bind="template: { name: getTemplateName() }"></div>
            <!-- /ko -->
            <!-- ko if: xamlBuildIssuesTruncated -->
            <div class="build-issue"><%: BuildResources.IssuesTruncated %></div>
            <!-- /ko -->
        </div>
        <!-- /ko -->
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_changes">
    <div class="summary-section">
        <h2 class="summary-section-header" data-bind="text: $vmcustom.displayName"></h2>

        <!-- ko ifnot: changesLoaded -->
        <div class="status-indicator">
            <div class="status">
                <table>
                    <tr>
                        <td>
                            <span class="icon big-status-progress"></span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- /ko -->

        <!-- ko if: changesLoaded -->

        <!-- ko if: changesMessage -->
        <span data-bind="text: changesMessage"></span>
        <!-- /ko -->

        <!-- ko foreach: changes -->
        <div class="buildvnext-associated-change summary-item">
            <a data-bind="text: changeText, click: openChange, attr: { href: changeUrl }"></a>
            <span data-bind="text: authoredBy"></span>
            <br />
            <pre class="message" data-bind="text: message"></pre>
            <span class="message-more-link" data-bind="visible: messageTruncated, click: showMore"><%: PresentationResources.Ellipsis %></span>
        </div>
        <!-- /ko -->

        <!-- /ko -->
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_deployments">
    <div class="summary-section" data-bind="visible: canShowDeployments">
        <h2 class="summary-section-header">
            <!--ko text: $vmcustom.displayName-->
            <!--/ko-->
            <a href="#" class="icon icon-refresh buildvnext-hovered" title="<%: BuildResources.Refresh %>" data-bind="click: refreshDeployments" />
        </h2>

        <!-- ko ifnot: deploymentsLoaded -->
        <div class="status-indicator">
            <div class="status">
                <table>
                    <tr>
                        <td>
                            <span class="icon big-status-progress"></span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- /ko -->

        <!-- ko if: deploymentsLoaded -->

        <!-- ko if: (deploymentBuilds().length > 0) -->
        <div class="summary-item-header">
            <%: BuildServerResources.DeploymentBuildLabel %>
        </div>
        <!-- ko foreach: deploymentBuilds -->
        <table>
            <tr>
                <td class="message"><span data-bind="text: statusMessage"></span></td>
                <td class="link"><a data-bind="attr: { href: linkValue, title: linkText }, text: linkText" target="_blank"></a></td>
            </tr>
        </table>
        <!-- /ko -->
        <!-- /ko -->

        <!-- ko if: (deploymentDeploys().length > 0) -->
        <div class="summary-item-header">
            <%: BuildServerResources.DeploymentDeployLabel %>
        </div>
        <!-- ko foreach: deploymentDeploys -->
        <div class="buildvnext-deployment-details summary-item">
            <div class="message" data-bind="text: message"></div>
        </div>
        <!-- /ko -->
        <!-- /ko -->

        <!-- ko if: (deploymentTests().length > 0) -->
        <div class="summary-item-header">
            <%: BuildServerResources.DeploymentTestLabel %>
        </div>
        <table class="test-run-table buildvnext-variables-grid-table">
            <tbody data-bind="foreach: deploymentTests">
                <tr>
                    <td class="link"><a data-bind="attr: { href: linkValue, title: linkText }, text: linkText" target="_blank"></a></td>
                    <td class="status">
                        <span data-bind="css: statusClass"></span>
                        <span data-bind="text: statusText"></span>
                    </td>
                    <td class="result"><span data-bind="text: resultText"></span></td>
                </tr>
            </tbody>
        </table>
        <!-- /ko -->

        <!-- /ko -->
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_tags">
    <div class="summary-section" data-bind="visible: canShowTags">
        <h2 class="summary-section-header" data-bind="text: $vmcustom.displayName"></h2>

        <div class="summary-item">
            <span class="build-list-tags" data-bind="createEnhancement: { controlType: 'tags', viewModel: tagsViewModel, controlInitialized: tagControlInitialized, eventHandlers: [{ type: 'add', eventHandler: addTag }, { type: 'delete', eventHandler: deleteTag }] }"></span>
        </div>
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_codecoverage">
    <div class="summary-section">
        <h2 class="summary-section-header">
            <!--ko text: $vmcustom.displayName-->
            <!--/ko-->
            <a href="#" class="icon icon-refresh buildvnext-hovered" title="<%: BuildResources.Refresh %>" data-bind="click: refreshCodeCoverage" />
        </h2>
        <!-- ko ifnot: codeCoveragesLoaded -->
        <div class="status-indicator">
            <div class="status">
                <table>
                    <tr>
                        <td>
                            <span class="icon big-status-progress"></span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- /ko -->

        <!-- ko if: codeCoveragesLoaded -->

        <!-- ko if: codeCoveragesMessage -->
        <span data-bind="html: codeCoveragesMessage"></span>
        <!-- /ko -->

        <!-- ko foreach: codeCoverages -->
        <div class="buildvnext-codecoverage summary-item">
            <!-- ko if: lastError -->
            <span class="build-failure-icon-color bowtie-icon bowtie-edit-delete"></span>
            <span data-bind="text: lastError"></span>
            <!-- /ko -->
            <div data-bind="createCollapsiblePanel: { viewModel: $data, headerText: $data.summary, headerTextCssClass: 'codecoverage-item-summary', templateName: 'buildvnext_details_summary_tab_codecoverage_details', collapseByDefault: false }"></div>
        </div>
        <!-- /ko -->

        <!-- ko foreach: codeCoverageSummaries -->
        <div class="buildvnext-codecoverage summary-item">
            <!-- ko if: lastError -->
            <span class="build-failure-icon-color bowtie-icon bowtie-edit-delete"></span>
            <span data-bind="text: lastError"></span>
            <!-- /ko -->
            <div data-bind="createCollapsiblePanel: { viewModel: $data, headerText: $data.summary, headerTextCssClass: 'codecoverage-item-summary', templateName: 'buildvnext_details_summary_tab_codecoveragesummary_details', collapseByDefault: false }"></div>
        </div>
        <!-- /ko -->

        <!-- /ko -->
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_codecoverage_details">
    <div class="codecoverage-item-details">
        <!-- ko ifnot: url -->
        <span><%: BuildServerResources.BuildCodeCoverageNoResults %></span>
        <!-- /ko -->
        <!-- ko foreach: modules -->
        <div class="buildvnext-codecoverage-module">
            <span data-bind="text: summary"></span>
        </div>
        <!-- /ko -->
        <!-- ko if: url -->
        </br>
        <a data-bind="click: downloadCC"><%: BuildServerResources.DownloadBuildCodeCoverageResults %></a>
        <!-- /ko -->
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_codecoveragesummary_details">
    <div class="codecoverage-item-details">
        <!-- ko if: summary -->
        <!-- ko foreach: summaries -->
        <div class="buildvnext-codecoverage-summary-item">
            <span data-bind="text: $data"></span>
            <span data-bind="text: $parent.differences[$index()]"></span>
        </div>
        <!-- /ko -->
        </br>       
        <!-- /ko -->

        <div class="codecoverage-item-downloadlink">
            <!-- ko if: url -->
            <a data-bind="click: downloadCCSummary"><%: BuildServerResources.DownloadBuildCodeCoverageResults %></a>
            <!-- /ko -->

            <!-- ko ifnot: url -->
            <span><%: BuildServerResources.BuildCodeCoverageNoResults %></span>
            <!-- /ko -->

            <!-- ko if: summaryUrl  -->
            <a data-bind="attr: { href: summaryUrl }" target="_blank" href="#"><%: BuildServerResources.ViewBuildCodeCoverageResults %></a>
            <!-- /ko -->

            </br>
        </div>
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_associatedWorkitems">
    <div class="summary-section">
        <h2 class="summary-section-header" data-bind="text: $vmcustom.displayName"></h2>

        <!-- ko ifnot: workItemsLoaded -->
        <div class="status-indicator">
            <div class="status">
                <table>
                    <tr>
                        <td>
                            <span class="icon big-status-progress"></span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- /ko -->

        <!-- ko if: workItemsLoaded -->

        <!-- ko if: workItemsMessage -->
        <span data-bind="text: workItemsMessage"></span>
        <!-- /ko -->

        <!-- ko foreach: workItems -->
        <div class="buildvnext-associated-workitem summary-item">
            <a data-bind="text: id, attr: { href: url }" target="_blank" href="#"></a>
            <span class="work-item-title" data-bind="text: title"></span>
            <div class="work-item-summary" data-bind="text: fullStatus"></div>
        </div>
        <!-- /ko -->

        <!-- /ko -->
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab_diagnosticlogs">
    <div class="summary-section" data-bind="visible: canShowDiagnosticLogs">
        <h2 class="summary-section-header" data-bind="text: $vmcustom.displayName"></h2>
        <table class="diagnostic-logs">
            <thead>
                <tr>
                    <th></th>
                    <th><%: BuildServerResources.BuildDiagnosticLogsPhase %></th>
                    <th><%: BuildServerResources.BuildDiagnosticLogsAgent %></th>
                </tr>
            </thead>
            <tbody data-bind="foreach: diagnosticLogs">
                <tr>
                    <!-- ko with: phaseLink -->
                    <td><span class="failed-phase" data-bind="if: phaseFailed()"><%: BuildServerResources.BuildDiagnosticLogsPhaseFailed %><span></td>
                    <td class="details"><a data-bind="text: name(), attr: { href: url() }" target="_blank" href="#"></a></td>
                    <!-- /ko -->
                    <td class="details"><a class="agent-name" data-bind="text: agentName, attr: { href: agentUrl }" target="_blank" href="#"></a></td>
                </tr>
                
            </tbody>
        </table>
    </div>
</script>

<script type="text/html" id="buildvnext_details_summary_tab">
    <!-- ko if: $data -->
    <div class="buildvnext-build-summary build-details">
        <!-- ko with: summary -->
        <div data-bind="applyTemplate: { templateName: 'buildvnext_details_sections', viewModel: $data }"></div>
        <!-- /ko -->
    </div>
    <!-- /ko -->
</script>

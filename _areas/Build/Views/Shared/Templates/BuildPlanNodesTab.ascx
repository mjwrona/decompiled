<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script id="buildvnext_plan_nodes_tab" type="text/html">
    <span class="status icon" data-bind="css: statusIconClass"></span>
    <!-- ko if: buildNumber -->
    <span class="tree-heading"><a tabIndex="0" role="button" data-bind="text: buildNumber, click: onBuildTitleClicked, triggerClickOnKeyUp: onBuildTitleClicked" aria-label="<%: BuildResources.JumpToRootBuild %>"></a></span>
    <!-- /ko -->

    <!-- ko if: nodesTree -->
    <div data-bind="template: { name: 'buildvnext_plan_nodes_tree', data: nodesTree }"></div>
    <!-- /ko -->
</script>
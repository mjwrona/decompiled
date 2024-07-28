<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>

<script id="buildvnext_definition_tree" type="text/html">
    <div class="buildvnext-tree" data-bind="renderTreeView: { treeNodes: nodes, onNodeClick: _onClick, menuOptions: { getMenuOptions: getMenuOptions } }"></div>
</script>

<script id="buildvnext_definition_tree_custom_node" type="text/html">
    <div class="build-definition-tree-custom-node">
        <!-- ko if: $data.isFavNode -->
        <!-- ko if: $data.buildStatusIconCssClass -->
        <span class="icon favnode-build-status" data-bind="css: buildStatusIconCssClass"></span>
        <!-- /ko -->
        <div class="ago" data-bind="text: buildStatusText, attr: { title: buildStatusText }"></div>
        <div class="build-histogram histogram definition-histogram" data-bind="createEnhancement: { controlType: 'histogram', viewModel: $data.histogram }"></div>
        <!-- /ko -->
    </div>
</script>
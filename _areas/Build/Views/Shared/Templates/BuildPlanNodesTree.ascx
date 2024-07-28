<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script id="buildvnext_plan_nodes_tree" type="text/html">
    <div class="buildvnext-plan-nodes-tree" data-bind="renderTreeView: { treeNodes: nodes, onNodeClick: _onClick }"></div>
</script>

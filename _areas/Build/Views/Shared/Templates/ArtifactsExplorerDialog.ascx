<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script id="artifacts_explorer_dialog" type="text/html">
    <div data-bind="template: { name: 'artifacts_explorer_tree', data: fileTree }"></div>
</script>

<script id="artifacts_explorer_tree" type="text/html">
    <div data-bind="renderTreeView: { treeNodes: nodes, onNodeClick: _onClick, menuOptions: { getMenuOptions: getMenuOptions } }"></div>
</script>

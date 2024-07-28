<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>

<script id="configuration_definition_tree" type="text/html">
    <div class="configuration-tree">
        <ul class="tree-children" data-bind="template: { name: 'configuration_definition_tree_node', foreach: nodes }"></ul>
    </div>
</script>

<script id="configuration_definition_tree_node" type="text/html">
    <li class="node configuration-node configuration-definition-node" data-bind="css: { 'dirty': dirty(), 'expanded': expanded(), 'collapsed': !expanded(), 'selected': selected() }">
        <div class="node-content" data-bind="css: $data.nodeContentCssClass, event: { mouseover: _onMouseover, mouseout: _onMouseout, click: _onClick, keydown: _onKeyDown }, attr: { 'tabindex': ((selected() || (isFolder() && !expanded())) ? '0' : '-1') } " >
            <div class="icon tree-icon" data-bind="css: { 'invisibleIcon': !isFolder() }, click: _onTreeIconClick"></div>
            <div class="link" data-bind="css: cssClass">
                <label data-bind="text: text"></label>
                <div class="node-remove"></div>
            </div>
        </div>
        <ul class="tree-children" data-bind="template: { name: 'configuration_definition_tree_node', foreach: nodes }, visible: expanded() && nodes().length !== 0"></ul>
    </li>
</script>

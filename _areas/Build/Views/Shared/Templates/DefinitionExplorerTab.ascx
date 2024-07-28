<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script id="buildvnext_definition_explorer_tree" type="text/html">
    <div data-bind="template: { name: 'buildvnext_definition_tree', data: definitionTree }"></div>
</script>
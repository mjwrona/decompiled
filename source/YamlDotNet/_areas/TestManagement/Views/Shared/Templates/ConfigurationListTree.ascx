<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<script id="configurations_list_explorer_tree" type="text/html">
    <div data-bind="template: { name: 'configuration_definition_tree', data: definitionTree }"></div>
</script>
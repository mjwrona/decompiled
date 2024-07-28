<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<object>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>

<%: Html.ScriptModules("Agile/Scripts/Common/SprintCapacityDashboard") %>
<%: Html.DashboardTile(Model, "sprint-capacity-summary-control") %>

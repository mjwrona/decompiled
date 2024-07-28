<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<object>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>

<%: Html.ScriptModules("Agile/Scripts/Common/Controls") %>
<%: Html.DashboardTile(Model, Html.BurnDownChart) %>

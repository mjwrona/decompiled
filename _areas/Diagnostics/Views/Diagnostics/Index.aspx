<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Diagnostics" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
	<% Html.ContentTitle(DiagnosticsResources.DiagnosticsPageTitle); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    
    <p style="margin-left: 1em;">
        <span><%: DiagnosticsResources.ScriptDebugMode %>: </span>
        <%: Html.ActionLink(Html.DebugEnabled() ? DiagnosticsResources.Enabled : DiagnosticsResources.Disabled, "EnableScriptDebugMode", new { disable = Html.DebugEnabled() }) %>
    </p>
    <p style="margin-left: 1em;">
        <span><%: DiagnosticsResources.TracePointCollectorLabel %>: </span>
        <%: Html.ActionLink(Html.IsTracePointCollectorEnabled() ? DiagnosticsResources.Enabled : DiagnosticsResources.Disabled, "EnableTracePointCollector", new { disable = Html.IsTracePointCollectorEnabled() }) %>
    </p>

    <script type="text/javascript"<%= Html.GenerateNonce(true) %>>
        require(["VSS/Diag"], function (TFS_DIAG) {
             TFS_DIAG.logTracePoint("Diagnostics.Index.pageLoadComplete");
         });
    </script>

</asp:Content>
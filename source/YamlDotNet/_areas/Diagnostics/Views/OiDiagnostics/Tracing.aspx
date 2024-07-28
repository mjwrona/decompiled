<%@ page title="" language="C#" masterpagefile="~/_views/Shared/Masters/HubPageExplorerPivot.master" inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Diagnostics" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:content contentplaceholderid="PageBegin" runat="server">
	<% 
        Html.ContentTitle("ServerTracing");
        Html.AddHubViewClass("tracing-view");
        Html.UseScriptModules("Diagnostics/Scripts/TFS.Diag.Tracing");
    %>
</asp:content>
<asp:content contentplaceholderid="LeftHubContent" runat="server">
    <div class="tracing-view-left-pane">
        <div class="tracing-filters">
            <div class="filter-toolbar"></div>
            <div class="tracing-filter-list"></div>
        </div>
    </div>
</asp:content>
<asp:content ID="Content4" contentplaceholderid="HubPivotViews" runat="server">
<%: Html.PivotViews(new[] { "traces" }) %>
</asp:content>
<asp:content contentplaceholderid="RightHubContent" runat="server">
    <div class="tracing-view-right-pane">
        <div class="tracing-content">
            <div class="trace-viewer"></div>
        </div>
    </div>
</asp:content>



<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Diagnostics" %>

<asp:Content ContentPlaceHolderID="PageBegin" runat="server">
	<% 
        Html.ContentTitle(DiagnosticsResources.ActivityLog);
        Html.UseScriptModules("Diagnostics/Scripts/TFS.Diag.ActivityLog.Controls");
    %>
</asp:Content>
<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="activity-log-view">
        <div class="activity-log-container">
            <div class="activity-log-filters"></div>
            <div class="activity-log-content"></div>
        </div>
    </div>
</asp:Content>

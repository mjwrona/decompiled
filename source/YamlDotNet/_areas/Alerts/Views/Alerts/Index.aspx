<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Alerts" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(AlertsServerResources.AlertsPageTitle);
        Html.UseScriptModules("Alerts/Scripts/TFS.Alerts.Controls", "Admin/Scripts/TFS.Admin.Common"); 
    %>
</asp:Content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <div class="hub-no-content-gutter alerts-manage-page-content">
        <div class="alerts-manage-view"></div>
    </div>
</asp:content>
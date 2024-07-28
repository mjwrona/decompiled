<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorer.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Alerts" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(AlertsResources.AlertsAdminPageTitle);
        Html.UseScriptModules("Alerts/Scripts/TFS.Alerts.Controls", "Admin/Scripts/TFS.Admin.Common"); 
        Html.AddHubViewClass("alerts-advanced-view");
    %>
</asp:Content>

<asp:content contentplaceholderid="HubBegin" runat="server">
    <%:Html.AlertsAdminViewOptions() %>
</asp:content>

<asp:content contentplaceholderid="LeftHubContent" runat="server">
    <div class="hub-no-content-gutter left-hub-container">
        <div class="alerts-tree"></div>
        <% if (Html.IsAlertAdministrator()) { %>
            <div class="alerts-admin-actions">
              <p class="alerts-leftpane-header"><%: AlertsServerResources.AdministratorActionsHeader %></p>
              <label for="alertsIdentityPickerTextBox"><%: AlertsServerResources.FindAlertsForUserLabel %></label>
              <div id="alertsIdentityControl"></div>
            </div>
        <% } %>
        <div class="alerts-quick-alerts">
          <p class="alerts-leftpane-header"><%: AlertsResources.QuickAlertsHeader %></p>
          <div id="quickAlertsList"></div>
        </div>
    </div>
</asp:content>

<asp:content contentplaceholderid="RightHubContent" runat="server">
    <div class="splitter vertical alerts-manage-splitter">
        <div class="leftPane alerts-grid-pane"></div>
        <div class="handleBar"></div>
        <div class="rightPane alerts-editor-pane"></div>
    </div>
</asp:content>

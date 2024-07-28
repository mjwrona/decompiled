<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Main.Master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.ProcessViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%:Url.Themed("Process.css") %>" type="text/css" rel="stylesheet" />
    <% Html.RenderPartial("TabStripControl"); %>
</asp:Content>

<asp:Content ID="DocumentBegin" ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.ContentTitle(Model.DisplayName); %>
    <% Html.UseScriptModules("Admin/Scripts/TFS.Admin.Process", "Admin/Scripts/TFS.Admin.Controls", "Admin/Scripts/TFS.Admin.Common"); %>
    <% Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebaccessProcessHierarchy); %>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="process-view">
        <div class="process-overview">
            <table class="process-overview-detail">
                <tr class="detail-row">
                    <td class="process-overview-header">
                        <div class="header"><%: AdminResources.Process %></div>
                        <div class="subheader bowtie"><%= AdminServerResources.ProcessSubheader %></div>
                        <div class="process-error"></div>
                        <div class="actions-control toolbar process-toolbar"></div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="process-grid"></div>
                    </td>
                </tr>
            </table>
            <%= Html.ProcessOverviewOptions(Model) %>
        </div>
        <div class="process-details" style="display: none">
            <div class="process-details-error"></div>

            <div class="admin-process-nav">
                <div class="process-details-selector-container"></div>
                <div class="process-details-pivot">
                    <%
                        var hubPivots = new List<PivotView>();

                        hubPivots.Add(new PivotView(AdminResources.Overview)
                        {
                            Id = "overview",
                            Link = Url.FragmentAction("overview")
                        });
                        hubPivots.Add(new PivotView(AdminResources.WorkItemTypes)
                        {
                            Id = "workitemtypes",
                            Link = Url.FragmentAction("workitemtypes")
                        });
                        hubPivots.Add(new PivotView(AdminResources.Fields)
                        {
                            Id = "fields",
                            Link = Url.FragmentAction("fields")
                        });

                        hubPivots.Add(new PivotView(AdminResources.BacklogLevels)
                        {
                            Id = "backloglevels",
                            Link = Url.FragmentAction("backloglevels")
                        });

                        hubPivots.Add(new PivotView(AdminResources.Security)
                        {
                            Id = "security",
                            Link = Url.FragmentAction("security")
                        });
                    %>
                    <%: Html.PivotViews(hubPivots, new { @class = "process-details-tabs" })%>
                </div>
                <div class="inherited-process-message" style="display: none;">
                    <div class="message-area-control info-message" style="display: flex; border-color: transparent">
                        <div class="message-icon"><span class="icon icon-info"></span></div>
                        <div class="message-header"><span><%: AdminServerResources.SystemProcessCannotBeCustomized %> <a class="create-inherited-process"><%: AdminServerResources.CreateAnInheritedProcess %></a>.</span></div>
                    </div>
                </div>
                <div class="custom-process-message" style="display: none;">
                    <div class="message-area-control info-message" style="display: flex; border-color: transparent">
                        <div class="message-icon"><span class="icon icon-info"></span></div>
                        <div class="message-header"><span><%: AdminServerResources.CustomProcessesCannotBeCustomizedUI %> <a href="https://go.microsoft.com/fwlink/?LinkId=534262"><%: AdminServerResources.ImportAndExportOfCustomProcesses %></a>.</span></div>
                    </div>
                </div>
            </div>
            <div class="process-details-pivot-container">
                <div class="process-details-pivot-content">
                    <div class="process-details-overview-view page"></div>
                    <div class="process-details-security-view page"></div>
                    <div class="process-details-fields-view page"></div>
                    <div class="process-details-states-view page"></div>
                    <div class="process-details-links-view page"></div>
                    <div class="process-details-backlog-levels page"></div>
                    <div class="process-details-workitemtypes-view page">
                        <%= Html.ProcessWorkItemTypeOptions(Model) %>
                    </div>
                    <div class="process-details-behaviors-view"></div>
                </div>
            </div>

            <%= Html.ProcessOverviewOptions(Model) %>

            <div class="picklist-control-options">
                <%= Html.ProcessAddFieldOptions(Model) %>
            </div>
        </div>
    </div>
</asp:Content>

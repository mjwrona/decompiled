<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerTriSplitPivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Implements Interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.UseAreaScriptModules(AgileAreaModules.ModuleNames);
        Html.AddHubViewClass("backlog-view");
        Html.UseScriptModules("Agile/Scripts/TFS.Agile.ProductBacklog.View");
        Html.IncludeFeatureFlagState(WebAccessAgileFeatureFlags.FeatureFlagNames);
        Html.IncludeContributions(AgileContributions.Backlog);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    <div class="backlog-page-title-area">
        <h1 id="backlog-page-title-header"></h1>
    </div>
    <div class="small-chart-group-container"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContentBegin" runat="server">
    <% if (ViewBag.BacklogPayload != null)
        { %>
    <%:Html.DataContractJsonIsland<BacklogViewModel>(ViewBag.BacklogPayload as BacklogViewModel, new { @class = "backlog-payload" }) %>
    <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <% Html.BasicUserMessage(); %>
    <% Html.NewBacklogLevelVisibilityNotSetMessage(); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftHubContent" runat="server">
    <%: Html.BacklogViewControl("team-backlog-view") %>
    <%: Html.SprintViewControl("team-iteration-view") %>
    <div class="recycle-bin"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
    <%-- TODO: BacklogPivotFilters.ascx should be called BacklogPivotViews.ascx--%>
    <% Html.RenderPartial("BacklogPivotFilters", new BacklogPivotFiltersViewModel
        {
            SelectedPivot = BacklogPivot.Backlog
        });
    %></asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server">
    <% 
        if(Html.isAdvanceBacklogManagementFeatureAvailable()) {
    %>

    <%--The real selected value will be set on the client--%>
    <%: Html.PivotFilter(AgileViewResources.ProductBacklog_PivotControl_ShowForecast,
            new[] {
                new PivotFilterItem(AgileProductBacklogServerResources.Backlog_ON, FilterConstants.On) { Selected = true },                 
                new PivotFilterItem(AgileProductBacklogServerResources.Backlog_OFF, FilterConstants.Off) { Selected = false }
            },
            new { @class = "show-forecast-filter agile-important-hidden" }) %>
    <% 
        }
    %>
    <%: Html.PivotFilter(
                AgileViewResources.ProductBacklog_PivotControl_ShowParents,
                new[] {
                    new PivotFilterItem(AgileProductBacklogServerResources.Backlog_Parents_Show, FilterConstants.On) { Selected = true },
                    new PivotFilterItem(AgileProductBacklogServerResources.Backlog_Parents_Hide, FilterConstants.Off) { Selected = false }
                },
                new { @class = "show-parents-filter agile-important-hidden" }) %>
    <%: Html.PivotFilter(
                AgileViewResources.ProductBacklog_PivotControl_ShowHideInProgress,
                new[] {
                    new PivotFilterItem(AgileProductBacklogServerResources.Backlog_InProgress_Show, FilterConstants.On) { Selected = true },
                    new PivotFilterItem(AgileProductBacklogServerResources.Backlog_InProgress_Hide, FilterConstants.Off) { Selected = false }
                },
                new { @class = "show-inprogress-filter agile-important-hidden" }) %>
    <div class="backlogs-right-pane-pivot-filter agile-important-hidden"></div>
    <%: Html.PivotFilter(
            new PivotFilter() { 
                Behavior = PivotFilterBehavior.Dropdown, 
                HtmlAttributes = new { @class = "backlog-pivot-filter agile-important-hidden" }, 
                Text = AgileViewResources.Backlog_FilterTitle }) %>
    <%: Html.MenuBar(
        new[]{
            Html.GetCommonSettingsConfigMenuItem(),
            Html.GetFullScreenMenuItem()
        }, new { @class = "backlogs-common-menubar agile-important-hidden" }    ) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="CenterHubContent" runat="server">
    <div class="main-content flex-container">
        <%: Html.TeamSettingsData() %>
        <%: Html.BugsBehaviorUIState() %>

        <div class="backlog-header">
            <div class="toolbar hub-pivot-toolbar agile-important-hidden"></div>
            <div class="toolbar filter-bar"></div>

            <div class="agile-portfolio-management-notification agile-important-hidden"></div>
            <div class="grid-status-message"></div>
            <% Html.MissingProposedStateInBugsWarning(); %>

            <div class="panel-region">
                <div class="add-panel-data"></div>
                <div class="forecasting-input-container hidden"></div>
            </div>
        </div>

        <div class="backlog-content-view">
            <div class="productbacklog-grid-results"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FarRightHubContent" runat="server">
    <div class="backlogs-default-tool-panel-container">
        <div class="backlogs-tool-panel-header">
            <h1 class="mapping-pane-title"></h1>

            <div class="hub-pivot">
                <div class="filters">
                    <%: Html.PivotFilter(
                        new PivotFilter() { 
                            Behavior = PivotFilterBehavior.Dropdown, 
                            HtmlAttributes = new { @class = "team-filter agile-important-hidden" }, 
                            Text = AgileViewResources.Mapping_PivotFilter_Team }) %>
                </div>
            </div>
        </div>
        <div class="backlogs-tool-panel-content">
            <div class="toolbar"></div>
            <div class="product-backlog-mapping-panel-status-indicator"></div>
            <div class="query-result-grid">
                <%: Html.JsonIsland(new { showContextMenu = false, allowMultiSelect = false }, new {@class="options"}) %>
            </div>
        </div>
    </div>
    <div class="backlogs-extension-tool-panel-container"></div>
</asp:Content>

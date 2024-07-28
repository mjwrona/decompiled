<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerPivot.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Agile.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Implements Interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.UseAreaScriptModules(AgileAreaModules.ModuleNames);
        Html.ContentTitle((string)ViewData[ViewDataConstants.SprintName]);
        Html.AddHubViewClass("board-view");
        Html.IncludeFeatureFlagState(WebAccessAgileFeatureFlags.FeatureFlagNames);
        Html.UseScriptModules("Agile/Scripts/TFS.Agile.TaskBoard");
        Html.UseScriptModules("Agile/Scripts/TFS.Agile.IterationBoard.View");
        Html.UseScriptModules("Agile/Scripts/TFS.Agile.IterationPivotFilters.View");
        Html.UseScriptModules("Agile/Scripts/Common/SprintDates");
        Html.IncludeContributions(AgileContributions.IterationBoard);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    <%: Html.AgileContext((SprintInformation)ViewData[ViewDataConstants.SprintInformation]) %>
    <%: Html.BacklogContext((BacklogContext)ViewData[ViewDataConstants.BacklogContextInformation]) %>
    <h1 id="iteration-board-title-id"><%: Html.ContentTitle() %></h1>
    <div class="sprint-title-right-container">
        <%: Html.SprintDatesControl() %>
        <%: Html.BurnDownChart() %>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftHubContent" runat="server">
    <%: Html.BacklogViewControl("team-backlog-view", "board") %>
    <%: Html.SprintViewControl("team-iteration-view") %>
    <div class="recycle-bin"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
    <% Html.RenderPartial("IterationPivotFilters", new IterationPivotFiltersViewModel
        {
            SelectedPivot = IterationPivot.Board,
        });
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server">
    <%: Html.PivotFilter(new PivotFilter(){ Behavior = PivotFilterBehavior.Dropdown, HtmlAttributes = new { @class = "group-filter" }, Text = AgileViewResources.TaskBoard_PivotFilter_GroupBy } ) %>
    <%: Html.PivotFilter(new PivotFilter(){ Behavior = PivotFilterBehavior.Dropdown, HtmlAttributes = new { @class = "person-filter" }, Text = AgileViewResources.TaskBorard_PivotFilter_Person } ) %>
    <%: Html.MenuBar(
        new[]{
            Html.GetCommonSettingsConfigMenuItem(),
            Html.GetFullScreenMenuItem()
        }, new { @class = "taskboard-menubar agile-important-hidden" }    ) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="RightHubContent" runat="server">
    <div class="iteration-main-content">
        <div class="data">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.IterationBacklogOptions], new {@class = "iteration-backlog-options"}) %>
            <%: Html.TeamSettingsData() %>
            <%: Html.BugsBehaviorUIState() %>
        </div>
    </div>
    <div id="taskboard" class="taskboard">
        <%: Html.JsonIsland(ViewData[ViewDataConstants.TaskBoardData]) %>
        <%: Html.JsonIsland(ViewData[ViewDataConstants.BoardCardSettings], new {@class = "taskboard-card-settings" }) %>
    </div>
</asp:Content>
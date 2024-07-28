<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerPivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<BacklogBoardViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Agile.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Common" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="System.Web.Optimization" %>

<%@ Implements Interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.UseAreaScriptModules(AgileAreaModules.ModuleNames);
        Html.ContentTitle((string)ViewData[ViewDataConstants.ViewTitle]);
        Html.AddHubViewClass("boards-backlog-view");
        Html.AddHubViewClass("backlog-view");
        Html.UseScriptModules("Agile/Scripts/Board/BoardsControls");
        Html.IncludeFeatureFlagState(WebAccessAgileFeatureFlags.FeatureFlagNames);
        Html.IncludeContributions(AgileContributions.Board);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">    
    
    <%: Html.JsonIsland(new { SignalRHubUrl = ViewData["SignalrHubUrl"] }, new { @class = "signalr-hub-url" }) %>
    <%: Html.BacklogContext(Model.BacklogContext) %>
    <%: Html.BoardModel(Model.BoardModel) %>

    <div id="board-page-title-id" class="board-page-title-area">
        <%: Html.ContentTitle()%>
    </div>
    <div class="small-chart-group-container">
        <%: Html.VelocityChart() %>
        <%: Html.CumulativeFlowChart() %>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <% Html.BasicUserMessage(); %>
    <% Html.NewBacklogLevelVisibilityNotSetMessage(); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftHubContent" runat="server">
    <%: Html.BacklogViewControl("team-backlog-view", "board") %>
    <%: Html.SprintViewControl("team-iteration-view") %>
    <div class="recycle-bin"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
    <% Html.RenderPartial("BacklogPivotFilters", new BacklogPivotFiltersViewModel
        {
            SelectedPivot = BacklogPivot.Board,
        });
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="RightHubContent" runat="server">
    <% Html.RenderPartial("BoardMainContent"); %>
    <%: Html.BugsBehaviorUIState() %>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageEnd" runat="server">
</asp:Content>

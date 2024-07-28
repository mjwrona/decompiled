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

<asp:Content ID="Content1" ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle((string)ViewData[ViewDataConstants.SprintName]);
        Html.AddHubViewClass("board-view");
        Html.IncludeFeatureFlagState(WebAccessAgileFeatureFlags.FeatureFlagNames);
        Html.UseScriptModules("Agile/Scripts/Common/Controls");
        Html.UseScriptModules("Agile/Scripts/Common/SprintDates");
        Html.UseScriptModules("Agile/Scripts/TFS.Agile.IterationBoardNoData.View");
        Html.UseScriptModules("Agile/Scripts/TFS.Agile.IterationPivotFilters.View");
        Html.IncludeContributions(AgileContributions.IterationBoard);
    %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HubTitleContent" runat="server">
    <%: Html.AgileContext((SprintInformation)ViewData[ViewDataConstants.SprintInformation]) %>
    <%: Html.BacklogContext((BacklogContext)ViewData[ViewDataConstants.BacklogContextInformation]) %>
    <%: Html.ContentTitle() %>
    <div class="sprint-title-right-container">
        <%: Html.SprintDatesControl() %>
        <%: Html.BurnDownChart() %>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftHubContent" runat="server">
    <%: Html.BacklogViewControl("team-backlog-view", "board") %>
    <%: Html.SprintViewControl("team-iteration-view") %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="HubPivotViews" runat="server">
    <% Html.RenderPartial("IterationPivotFilters", new IterationPivotFiltersViewModel
        {
            SelectedPivot = IterationPivot.Board,
        });
    %>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="RightHubContent" runat="server">
    <%: Html.JsonIsland(this.ViewData[ViewDataConstants.IterationBacklogOptions], new {@class = "iteration-backlog-options"}) %>
    <%: Html.TeamSettingsData() %>
        
    <!-- Making the empty content gutter behavior consistent with Iteration implementation -->
    <% if ((bool)ViewData[ViewDataConstants.ShowNoContentGutter])
        {
            Html.RenderPartial("NoContentGutter", ViewData[ViewDataConstants.NoContentGutterModel]);
        } %>
</asp:Content>

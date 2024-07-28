<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerPivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Implements Interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(TestManagementResources.TestRunExplorerPageTitle);
        Html.UseScriptModules("TestManagement/Scripts/TFS.TestManagement.RunsView.RunExplorer.View"); 
        Html.UseAreaCSS("TestManagement");
        Html.AddHubViewClass("testmanagement-runexplorer-view");
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTCMTestPlanLiteHub);
    %>
</asp:Content>

<asp:content ID="Content3" contentplaceholderid="LeftHubContent" runat="server">
    <div class="goto-run-action-container">
        <div class="goto-run-action bowtie"></div>
    </div>
    <div class="test-sidebar-content-separator"></div>
    <div class="hub-pivot-content">
        <div class="testmanagement-runexplorer-treetoolbar toolbar"></div>
        <div class="testmanagement-runexplorer-treesplitter"></div>
    </div>
    <% if (this.ViewData.ContainsKey("WorkItemColors"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["WorkItemColors"], new { @class = "__workItemColors" }) %>
    <% } %>
    <% if (this.ViewData.ContainsKey("WorkItemTypeCategories"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["WorkItemTypeCategories"], new { @class = "__workItemTypeCategories" }) %>
    <% } %>
    <% if (this.ViewData.ContainsKey("TestSessions"))
        { %>
    <%: Html.RestApiJsonIsland(this.ViewData["TestSessions"], new { @class = "__testSessions" }) %>
    <% } %>
    <% if (this.ViewData.ContainsKey("ExploratorySessionUserSettings"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["ExploratorySessionUserSettings"], new { @class = "__exploratorySessionUserSettings" }) %>
    <% } %>
</asp:content>

<asp:content contentplaceholderid="HubPivotViews" runat="server">
    <%
        IList<PivotView> rightPivots = new List<PivotView>();
        string pivotClass = "explorer-right-hub-view-tabs";

        rightPivots.Add(new PivotView(TestManagementServerResources.RunSummaryChartsTabTitle)
        {
            Id = "runCharts",
            Link = Url.FragmentAction("runCharts"),
            Disabled = true
        });
        rightPivots.Add(new PivotView(TestManagementServerResources.ResultsQueryTabTitle)
        {
            Id = "resultQuery",
            Link = Url.FragmentAction("resultQuery"),
            Disabled = true
        });
        rightPivots.Add(new PivotView(TestManagementServerResources.QueryFilterTabTitle)
        {
            Id = "resultQueryEditor",
            Link = Url.FragmentAction("resultQueryEditor"),
            Disabled = true
        });        
        rightPivots.Add(new PivotView(TestManagementServerResources.ResultSummaryTabTitle)
        {
            Id = "resultSummary",
            Link = Url.FragmentAction("resultSummary"),
            Disabled = true
        });
        rightPivots.Add(new PivotView(TestManagementServerResources.ResultHistoryTabTitle)
        {
            Id = "resultHistory",
            Link = Url.FragmentAction("resultHistory"),
            Disabled = true
        });
        rightPivots.Add(new PivotView(TestManagementServerResources.TestRunsTabTitle)
        {
            Id = "runQuery",
            Link = Url.FragmentAction("runQuery"),
            Disabled = true
        });
        rightPivots.Add(new PivotView(TestManagementServerResources.QueryFilterTabTitle)
        {
            Id = "runQueryEditor",
            Link = Url.FragmentAction("runQueryEditor"),
            Disabled = true
        });
        
    %>
    <%: Html.PivotViews(rightPivots, new { @class = pivotClass }, "ms.vss-test-web.test-reporting-pivot-tab", false)%>
</asp:content>

<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server">     
   <div class="pivot-filter session-team-filter" style="display: none"></div>
   <%: Html.SessionOwnerFilter() %>
   <%: Html.SessionPeriodFilter() %>
   <%: Html.QuerySelectorFilter() %>   
</asp:Content>

<asp:content ID="Content1" contentplaceholderid="RightHubContent" runat="server">
    <div class="explorer-right-hub-content"></div>
</asp:content>
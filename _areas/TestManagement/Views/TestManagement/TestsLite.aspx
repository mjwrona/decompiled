<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerTriSplitPivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.UseScriptModules("TestManagement/Scripts/TFS.TestManagement.TestLiteView");
        Html.UseAreaCSS("TestManagement");
        Html.AddHubViewClass("test-hub-lite-view");

        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.TCMUseNewIdentityPicker);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTestManagementPointCountFeatureDisabled);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTCMTestPlanLiteHub);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTCMEnableXtForEdge);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.QuickStartXTPromotion2);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTestManagementEnableDesktopScreenShot);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTestManagementReactBasedRunWithOptionsDialog);
    %>
    

</asp:Content>

<asp:Content ID="LeftHubContent" ContentPlaceHolderID="LeftHubContent" runat="server">
    <% 
        Html.RenderPartial("Templates/RunWithOptionsDialog");
    %>
    <% if (this.ViewData.ContainsKey("workitemtype-colors"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["workitemtype-colors"], new { @class = "workitemtype-colors" }) %>
    <% } %>
    <div class="testmanagement-view-left-pane">
        <div class="testmanagement-testplans-pane" role="banner">
            <div class="testmanagement-plans-combo"></div>
            <div class="testmanagement-testplans-filter"></div>
        </div>
        <div class="test-plans-suites-toolbar toolbar" role="banner"></div>
        <div class="test-sidebar-content-separator"></div>
        <div class="testmanagement-suites-tree" role="banner"></div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <%: Html.IsAdvancedExtensionEnabled(TfsWebContext) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
    <%
        IList<PivotView> rightPivots = new List<PivotView>();
        string pivotClass = "test-items-tabs";

        rightPivots.Add(new PivotView(TestManagementServerResources.TestHubTestsTabTitle)
        {
            Id = "tests",
            Link = Url.FragmentAction("tests", "my")
        });
        rightPivots.Add(new PivotView(TestManagementServerResources.TestHubChartsTabTitle)
        {
            Id = "charts",
            Link = Url.FragmentAction("charts", "my")
        });
    %>
    <%: Html.PivotViews(rightPivots, new { @class = pivotClass }, "ms.vss-test-web.test-plan-pivot-tabs", false)%>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server">
    <%: Html.TestPointOutcomeFilter() %>
    <%: Html.TesterFilter() %>
    <%: Html.ConfigurationFilter() %>
    <%: Html.ViewFilter(TfsWebContext) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="CenterHubContent" runat="server">
    <div class="test-view-right-pane">
        <div class="leftPane" role="banner">
            <div class="toolbar hub-pivot-toolbar"></div>
            <div class="test-view-filter-bar"></div>
            <div class="test-view-grid-area"></div>
            <div class="test-edit-grid-area"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FarRightHubContent" runat="server">
    <div class="far-right-pane-title"></div>    
    <div class="hub-pivot far-right-pane-pivot">
        <div class="filters">
            <%: Html.PaneFilter(TfsWebContext) %>
            <%: Html.PositionFilter(TfsWebContext, ViewData) %>
        </div>
    </div>
    <div class="work-item-form"></div>
    <div class="rightPane hub-no-content-gutter">
        <div class="test-case-details-pane" role="banner">
            
            <div class="far-right-pane-status-indicator"></div>
            <div class="far-right-pane-view-explorer">
                <div class="far-right-pane-toolbar toolbar"></div>
                <div class="far-right-pane-list-container"></div>
            </div>
        </div>
    </div>
</asp:Content>
<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerTriSplitPivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.UseScriptModules("TestManagement/Scripts/TFS.TestManagement.TestView");
        Html.UseAreaCSS("TestManagement");
        Html.AddHubViewClass("test-hub-view");

        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.TCMUseNewIdentityPicker);

        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTCMTestPlanLiteHub);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTCMEnableXtForEdge);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.QuickStartXTPromotion2);
    %>
</asp:Content>
<asp:Content ID="LeftHubContent" ContentPlaceHolderID="LeftHubContent" runat="server">
    <% 
        Html.RenderPartial("Templates/RunWithOptionsDialog");
    %>
    <% if (this.ViewData.ContainsKey("TestPlans"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["TestPlans"], new { @class = "__allTestPlans" }) %>
    <% } %>

    <% if (this.ViewData.ContainsKey("TestSuitesForSelectedPlan"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["TestSuitesForSelectedPlan"], new { @class = "__allSuitesOfSelectedPlan" }) %>
    <% } %>

    <% if (this.ViewData.ContainsKey("TestPointsForSelectedSuite"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["TestPointsForSelectedSuite"], new { @class = "__allTestPointsOfSelectedSuite" }) %>
    <% } %>
     <% if (this.ViewData.ContainsKey("TeamFieldForLastSelectedTestPlan"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["TeamFieldForLastSelectedTestPlan"], new { @class = "__allTeamFieldForLastSelectedTestPlan" }) %>
    <% } %>
    <% if (this.ViewData.ContainsKey("DefaultPlanQuery"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["DefaultPlanQuery"], new { @class = "__defaultPlanQuery" }) %>
    <% } %>
    <% if (this.ViewData.ContainsKey("TestPlanQueryFromRegistry"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["TestPlanQueryFromRegistry"], new { @class = "__testPlanQueryFromRegistry" }) %>
    <% } %>
    <% if (this.ViewData.ContainsKey("workitemtype-colors"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["workitemtype-colors"], new { @class = "workitemtype-colors" }) %>
    <% } %>

    <div class="testmanagement-view-left-pane">
        <div class="testmanagement-testplans-pane">
            <div class="testmanagement-plans-combo"></div>
            <div class="testmanagement-testplans-filter"></div>
        </div>
        <div class="test-plans-suites-toolbar toolbar"></div>
        <div class="test-sidebar-content-separator"></div>
        <div class="testmanagement-suites-tree"></div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <%: Html.IsAdvancedExtensionEnabled(TfsWebContext) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
    <%: Html.TestTabItems(Url) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server">
    <%: Html.TestPointOutcomeFilter() %>
    <%: Html.TesterFilter() %>
    <%: Html.ConfigurationFilter() %>
    <%: Html.ViewFilter(TfsWebContext) %>
</asp:Content>


<asp:Content ContentPlaceHolderID="CenterHubContent" runat="server">
    <div class="test-view-right-pane">
        <div class="leftPane">
            <div class="toolbar hub-pivot-toolbar"></div>
            <div class="test-view-filter-bar"></div>
            <div class="test-view-grid-area"></div>
            <div class="test-edit-grid-area"></div>

        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="FarRightHubContent" runat="server">
    <div class="work-item-form"></div>
    <div class="rightPane hub-no-content-gutter">
        <div class="test-case-details-pane">
            <div class="far-right-pane-title"></div>
            <div class="far-right-pane-status-indicator"></div>
            <div class="far-right-pane-view-explorer">
                <div class="far-right-pane-toolbar toolbar"></div>
                <div class="far-right-pane-list-container"></div>
            </div>
        </div>
    </div>
    <div class="hub-pivot far-right-pane-pivot">
        <div class="filters">
            <%: Html.PaneFilter(TfsWebContext) %>
            <%: Html.PositionFilter(TfsWebContext, ViewData) %>
        </div>
    </div>
    
    <% if (this.ViewData.ContainsKey("panePosition"))
        { %>
    <%: Html.DataContractJsonIsland(this.ViewData["panePosition"], new { @class = "__panePositionSetting" }) %>
    <% } %>
</asp:Content>
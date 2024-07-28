<%@ Page Title=""
         Language="C#"
         MasterPageFile="~/_views/Shared/Masters/HubPage.master"
         Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Presentation.DashboardDefaultViewModel>"
%>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Dashboards" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Dashboards.Common" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Implements Interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>


<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.TCMUseNewIdentityPicker);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.GalleryPromotion);
        Html.UseCSS("dashboard");
        Html.AddHubViewClass("dashboard-landing");
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
     <%: Html.CreateDefaultDashboardIdJsonIsland(JsonIslandDomClassNames.DefaultDashboardId, Model.DefaultDashboardId.ToString()) %>
     <%: Html.CreateDefaultDashboardWidgetsJsonIsland(JsonIslandDomClassNames.DefaultDashboardWidgets, Model.DefaultDashboardWidgets) %>
    <%: Html.CreateMaxWidgetsJsonIsland(JsonIslandDomClassNames.MaxWidgetsPerDashboards, Model.MaxWidgetsPerDashboard) %>
    <%: Html.CreateMaxDashboardsJsonIsland(JsonIslandDomClassNames.MaxDashboardsPerGroup, Model.MaxDashboardPerGroup) %>
    <%: Html.CreateIsStakeholderJsonIsland(JsonIslandDomClassNames.IsStakeholder, Model.IsStakeholder) %>
</asp:Content>

<asp:content contentplaceholderid="HubTitle" runat="server">
    <div class="hub-progress pageProgressIndicator"></div>
</asp:content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBegin" runat="server">
        <%
        Html.UseAreaScriptModules(
            "Dashboards/Scripts/LandingView"
        );
      %>

      <%-- The Controls.Common module is required for DismissableMessageAreaControl which is used to display upgrade notifications --%>
      <%
         Html.UseScriptModules(
            "Dashboards/Scripts/L2Menu",
            "Presentation/Scripts/TFS/TFS.UI.Controls.Common"
        );
    %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HubContent" runat="server">
    <div id="container-without-scroll">
        <%--
            All messages are within #container-without-scroll (required for blade system) so that #container-with-scroll (which holds the
            Dashboard view) can resize vertically to fill available space. Messages are outside #container-with-scroll so that they are
            always displayed even if we scroll (in edit or view mode).
        --%>
        <% Html.StakeholderMessage(); %>
        <div class="dashboard-notification-message"></div>
        <div id="curtain"></div>
        <div id="container-with-scroll">
            <div class="team-dashboard-view" role="main"></div>
        </div>
        <div id="dashboard-edit-menu"></div>
    </div>
     <%-- The style is set to none because we want to avoid any glitch of showing the Blade Menu when we enter the page --%>
    <div id="blade-menu" style="display: none"></div>
</asp:Content>
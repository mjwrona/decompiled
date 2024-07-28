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
        Html.AddHubViewClass("board-view");
        Html.IncludeFeatureFlagState(WebAccessAgileFeatureFlags.FeatureFlagNames);
        Html.IncludeContributions(AgileContributions.Backlog);
        Html.UseScriptModules("Agile/Scripts/Common/Controls");
        Html.UseScriptModules("Agile/Scripts/TFS.Agile.BacklogContributedTab.View");
    %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HubTitleContent" runat="server">
    <%: Html.AgileContext((SprintInformation)ViewData[ViewDataConstants.SprintInformation]) %>
    <%: Html.BacklogContext((BacklogContext)ViewData[ViewDataConstants.BacklogContextInformation]) %>
    <div class="product-contributed-tab-title">    
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftHubContent" runat="server">
    <!-- TODO: What should be action for BacklogViewControl -->
    <%: Html.BacklogViewControl("team-backlog-view", "backlog") %>
    <%: Html.SprintViewControl("team-iteration-view") %>
</asp:Content>
<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server">    
        <%: Html.MenuBar(
        new[]{
            Html.GetFullScreenMenuItem()
        }, new { @class = "backlogs-common-menubar agile-important-hidden" }    ) %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HubPivotViews" runat="server">
    <!-- todo change to backlog -->
    <% Html.RenderPartial("BacklogPivotFilters", new BacklogPivotFiltersViewModel
        {
            SelectedPivot =  (BacklogPivot) ViewData[ViewDataConstants.TabContributionSelectedPivotFilter]
        });
    %>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="RightHubContent" runat="server">    
    <%: Html.TeamSettingsData() %>
</asp:Content>

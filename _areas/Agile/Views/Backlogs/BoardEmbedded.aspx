<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master"
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

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.UseAreaScriptModules(AgileAreaModules.ModuleNames);
        Html.ContentTitle((string)ViewData[ViewDataConstants.ViewTitle]);
        Html.AddHubViewClass("backlog-view");
        Html.AddHubViewClass("embedded");
        Html.UseScriptModules("Agile/Scripts/Board/BoardsControls");
        Html.IncludeFeatureFlagState(WebAccessAgileFeatureFlags.FeatureFlagNames);
        Html.IncludeContributions(AgileContributions.Board);
        Html.Chromeless(true);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    
    <%: Html.JsonIsland(new { SignalRHubUrl = ViewData["SignalrHubUrl"] }, new { @class = "signalr-hub-url" }) %>
    <%: Html.BacklogContext(Model.BacklogContext) %>
    <%: Html.BoardModel(Model.BoardModel) %>
    <div class="board-page-title-area">
        <%: Html.ContentTitle()%>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="hub-pivot">
        <div class="views"></div>
        <div class="filters"></div>
    </div>
    <div class="hub-pivot-content">
        <% Html.RenderPartial("BoardMainContent"); %>
    </div>
</asp:Content>

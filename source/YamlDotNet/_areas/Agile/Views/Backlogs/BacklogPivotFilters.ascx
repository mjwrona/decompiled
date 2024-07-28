<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<BacklogPivotFiltersViewModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%: Html.JsonIsland(this.ViewData[ViewDataConstants.AnchoredLevel], new {@class = "product-backlog-anchored-level"}) %>
<%: Html.JsonIsland(this.ViewData[ViewDataConstants.ShowParents], new {@class = "product-backlog-showparents"}) %>
<%: Html.JsonIsland(this.ViewData[ViewDataConstants.TabContributionsModel], new {@class = "agile-tab-contributions-model"}) %>
<%: Html.JsonIsland(Model.SelectedPivot, new {@class = "product-backlog-selected-pivot"}) %>

<%: 
Html.PivotViews(new[]
{
    new PivotView(AgileViewResources.Backlogs_PivotView_Backlog)
    {
        Id = "backlog",
        Link = Url.ActionWithParameters("backlog", "backlogs", new { routeArea = TfsRouteArea.Root }),
        Selected = Model.SelectedPivot == BacklogPivot.Backlog
    },
    new PivotView(AgileViewResources.Boards_PivotView)
    {
        Id = "board",
        Link = Url.ActionWithParameters("board", "backlogs", new { routeArea = TfsRouteArea.Root }),
        Selected = Model.SelectedPivot == BacklogPivot.Board
    }
}, new
{
    @class = (Model.SelectedPivot == BacklogPivot.Backlog) ? "productbacklog-view-tabs" : "productbacklog-view-tabs"
}, "ms.vss-work-web.product-backlog-tabs", false) /* Do not enhance the control here, it will be enhanced in call to  enhanceBacklogPivotView*/

%>
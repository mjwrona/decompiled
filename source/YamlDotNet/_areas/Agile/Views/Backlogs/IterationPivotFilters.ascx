<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<IterationPivotFiltersViewModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Common" %>

<%: Html.JsonIsland(this.ViewData[ViewDataConstants.SelectedIteration], new {@class = "iteration-backlog-selected-iteration"}) %>
<%: Html.JsonIsland(Model.SelectedPivot, new {@class = "iteration-backlog-selected-pivot"}) %>
<%: Html.JsonIsland(this.ViewData[ViewDataConstants.TabContributionsModel], new {@class = "agile-tab-contributions-model"}) %>

<%: Html.IterationPivotFilters(Model.SelectedPivot) %>

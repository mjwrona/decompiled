<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<BacklogBoardViewModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Common" %>

<%: Html.TeamSettingsData() %>

<div class="agile-board-filter-area"></div>
<div class="agile-board-message-area"></div>
<div class="agile-board"></div>

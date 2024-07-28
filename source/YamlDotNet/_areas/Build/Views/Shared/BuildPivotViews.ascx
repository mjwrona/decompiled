<%@ Control Language="C#"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Build.BuildViewModel>" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>


<%
    IList<PivotView> rightPivots = new List<PivotView>();

    rightPivots.Add(new PivotView(BuildResources.QueuedBuildsTitle)
    { 
        Id = "queued",
        Link = Url.FragmentAction("queued"),
        Disabled=true
    });

    rightPivots.Add(new PivotView(BuildResources.CompletedBuildsTitle)
    { 
        Id = "completed",
        Link = Url.FragmentAction("completed"),
        Disabled = true
    });

    rightPivots.Add(new PivotView(BuildServerResources.DeployedBuildsTitle)
    { 
        Id = "deployed",
        Link = Url.FragmentAction("deployed"),
        Disabled = true
    });

    rightPivots.Add(new PivotView(BuildResources.BuildDetailSummaryTitle)
    { 
        Id = "summary",
        Link = Url.FragmentAction("summary"),
        Disabled = true
    });

    rightPivots.Add(new PivotView(BuildResources.BuildDetailLogTitle)
    { 
        Id = "log",
        Link = Url.FragmentAction("log"),
        Disabled = true
    });

    rightPivots.Add(new PivotView(BuildResources.BuildDetailDiagnosticsTitle)
    { 
        Id = "diagnostics",
        Link = Url.FragmentAction("diagnostics"),
        Disabled = true
    });

%>
<%: Html.PivotViews(rightPivots, new { @class = "build-tabs" })%>

<div class="build-detail-view hidden">
    <div class="header">
        <div class="title"></div>
    </div>
    <div class="content">
        <div class="logmenu" style="display: none"></div>
        <div class="logcontent" style="display: none"></div>
    </div>
</div>

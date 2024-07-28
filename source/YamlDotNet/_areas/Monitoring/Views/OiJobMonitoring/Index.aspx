<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master" 
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Monitoring" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(MonitoringServerResources.MonitoringPageTitle);
        Html.UseScriptModules("Monitoring/Scripts/TFS.Monitoring.View");
        Html.AddHubViewClass("monitoring-view");
    %>
</asp:Content>

<asp:content ID="Content3" contentplaceholderid="HubPivotViews" runat="server">
    <%
        IList<PivotView> pivots = new List<PivotView>();
        pivots.Add(new PivotView(MonitoringServerResources.TabTitleJobSummary)
        {
            Id = "summary",
            Link = Url.FragmentAction("summary")
        });
        pivots.Add(new PivotView(MonitoringServerResources.TabTitleJobQueue)
        { 
            Id = "queue",
            Link = Url.FragmentAction("queue")
        });
        pivots.Add(new PivotView(MonitoringResources.TabTitleJobHistory)
        {
            Id = "history",
            Link = Url.FragmentAction("history")
        });
    %>
    <%: Html.PivotViews(pivots, new { @class = "job-tabs" })%>
</asp:content>

<asp:content ID="HubContent3" contentplaceholderid="HubContent" runat="server">
    <div class="hub-content monitoring-content"></div>
</asp:content>
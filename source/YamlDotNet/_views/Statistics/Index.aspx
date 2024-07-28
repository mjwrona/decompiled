<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerPivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle("Activity Statistics");
        Html.AddHubViewClass("activity-stats-view");
        Html.UseScriptModules("Presentation/Scripts/TFS/TFS.Diag.ActivityStats.Controls");        
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <%:Html.SecurityOptions() %>
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftHubContent" runat="server">
    <div class="activity-stats-explorer">          
        <div class="activity-stats-toolbar"></div>      
        <div class="activity-stats-container"></div>
    </div>
</asp:Content>

<asp:content contentplaceholderid="HubPivotViews" runat="server">

    <%
        var rightPivots = new List<PivotView>();

        rightPivots.Add(new PivotView("Summary")
        {
            Id = "summary",
            Link = Url.FragmentAction("summary"),
            Disabled = false
        });
        rightPivots.Add(new PivotView("Chart")
        {
            Id = "chart",
            Link = Url.FragmentAction("chart"),
            Disabled = false
        });   
             
    %>
    <%: Html.PivotViews(rightPivots, new { @class = "activitystats-explorer-tabs" })%>
</asp:content>

<asp:Content ContentPlaceHolderID="RightHubContent" runat="server">
    <div class="activity-stats-content">                
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="HubEnd" runat="server">
</asp:Content>


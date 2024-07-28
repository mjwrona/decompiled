<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<VersionControlViewModel>" %>

<%@ import namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="TfsControls=Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.VersionControl" %>

<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <% 
        using (WebPerformanceTimer.StartMeasure(TfsWebContext.RequestContext, "GitController.PullRequestDetail.DocumentBegin"))
        {
            Html.UseAreaCSS("VersionControl");
            Html.IncludeContributions("ms.vss-code-web.pull-request-detail-form");
            Html.UseAreaScriptModules("VersionControl/Scripts/VCAreaBundle");
            Html.UseScriptModules("VersionControl/Scripts/Views/PullRequestReviewView");
            Html.AddHubViewClass("vc-pullrequest-review-view");
            Html.UseScriptModules("VersionControl/Scripts/Components/PullRequestReview/Tabs/" + ViewData["SelectedTab"] + "Tab");
        }
    %>
</asp:content>

<asp:Content ContentPlaceHolderID="PageBegin" runat="server">
    <% if (Html.DebugEnabled()) { %>
    <script type="text/javascript" src="<%:StaticResources.ThirdParty.Scripts.GetLocation("jquery.signalR-upd.2.2.0.js") %>"<%= Html.GenerateNonce(true) %>></script>
    <% } else { %>
    <script type="text/javascript" src="<%:StaticResources.ThirdParty.Scripts.GetLocation("jquery.signalR-upd.2.2.0.min.js") %>"<%= Html.GenerateNonce(true) %>></script>
    <% } %>
    <script type="text/javascript" src="<%:ViewData["SignalrHubUrl"] %>"<%= Html.GenerateNonce(true) %>></script>
</asp:Content>

<asp:content contentplaceholderid="HubBegin" runat="server">
    <div class="vc-pullrequest-short-title"></div>
    <%: Html.VCViewOptions(Model) %>
</asp:content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    <div class="vc-pullrequest-details-titleArea"></div>
    <div class="left-splitter-options"><%: Html.StatefulSplitterOptions(ControlExtensions.LeftHubSplitterKey) %></div>
</asp:Content>

<asp:content ID="HubPivotViews" contentplaceholderid="HubPivotViews" runat="server">
    <div class="vc-pullrequest-pivotArea">
    <%
        // note that here we are building each tab link
        // FragmentAction is going to generate the "hash" portion of the URL.
        // for example: #_a=overview
        // note that we append the "?" so we can strip the query string that comes in on the URL but still make a relative path
        var pivots = new List<PivotView>();

        pivots.Add(new PivotView
        {
            Text = VCResources.PullRequest_Pivot_Overview,
            Id = "overview",
            Link = "?" + Url.FragmentAction("Overview")
        });

        pivots.Add(new PivotView
        {
            Text = VCResources.PullRequest_Pivot_Files,
            Id = "files",
            Link = "?" +  Url.FragmentAction("Files")
        });

        pivots.Add(new PivotView
        {
            Text = VCResources.PullRequest_Pivot_Updates,
            Id = "updates",
            Link = "?" + Url.FragmentAction("Updates")
        });

        pivots.Add(new PivotView
        {
            Text = VCResources.PullRequest_Pivot_Commits,
            Id = "commits",
            Link = "?" + Url.FragmentAction("Commits")
        });
    %>
    <%: Html.PivotViews(pivots, new { @class = "vc-pullrequest-tabs" }, "ms.vss-code-web.pr-tabs") %>
    </div>
</asp:content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <div class="versioncontrol-pullrequests-content">
    </div>
</asp:content>


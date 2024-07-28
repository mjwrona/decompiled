<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<VersionControlViewModel>" %>

<%@ import namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="TfsControls=Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.VersionControl" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <% 
        Html.UseAreaCSS("VersionControl");
        Html.UseAreaScriptModules("VersionControl/Scripts/VCAreaBundle");
        Html.UseScriptModules("VersionControl/Scripts/Views/Empty");
        Html.AddHubViewClass("vc-empty-view");
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
    <div class="vc-empty-short-title"></div>
    <%: Html.VCViewOptions(Model) %>
</asp:content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    <div class="vc-empty-titleArea"></div>
    <div class="left-splitter-options"><%: Html.StatefulSplitterOptions(ControlExtensions.LeftHubSplitterKey) %></div>
</asp:Content>

<asp:content ID="HubPivotViews" contentplaceholderid="HubPivotViews" runat="server">
    <div class="vc-empty-pivotArea">
    <%
        var pivots = new List<PivotView>();

        pivots.Add(new PivotView
        {
            Text = VCResources.PullRequest_Pivot_Overview,
            Id = "overview",
            Link = "?" + Url.FragmentAction("Overview")
        });
    %>
    <%: Html.PivotViews(pivots, new { @class = "vc-empty-tabs" })%>
    </div>
</asp:content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <div class="versioncontrol-empty-content">
    </div>
</asp:content>


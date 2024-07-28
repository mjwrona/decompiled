<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerPivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<VersionControlViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="TfsControls=Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.VersionControl" %>
<%@ Implements Interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>

<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <% 
        Html.UseCSS("BuildStyles", "DistributedTasksLibrary");
        Html.UseAreaCSS("VersionControl");
        Html.UseAreaScriptModules("VersionControl/Scripts/VCAreaBundle");
        Html.UseScriptModules("VersionControl/Scripts/Views/ExplorerView");
        Html.AddHubViewClass("versioncontrol-explorer-view"); 
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessQuickStartCodeSearchPromotion);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessQuickStartCodeSearchFeature);
        Html.IncludeContributions("ms.vss-search-platform.entity-type-collection", "ms.vss-code-search.code-entity-type");
        Html.IncludeContributions("ms.vss-code-web.explorer-view");
    %>
</asp:content>

<asp:content contentplaceholderid="HubBegin" runat="server">
    <%:Html.VCViewOptions(Model) %>
    <%:Html.AreaLocations((string[])(ViewBag.areaLocations)) %>
</asp:content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    <div class="vc-page-title-area">
        <div class="vc-branches-container"></div>
        <div class="vc-path-explorer-container"></div>
        <div class="vc-status-container"></div>
    </div>
</asp:Content>

<asp:content contentplaceholderid="LeftHubContent" runat="server">
    <div class="version-control-item-left-pane">
        <div class="source-node-container">
            <div class="source-explorer-tree"></div>
        </div>
    </div>
</asp:content>
<asp:content contentplaceholderid="HubPivotViews" runat="server">
<%
        var hubPivots = new List<PivotView>();

        hubPivots.Add(new PivotView(VCResources.Contents)
            { 
                Id = "contents",
                Link = Url.FragmentAction("contents"),
                Selected = true
            });
        hubPivots.Add(new PivotView(VCResources.History)
            { 
                Id = "history",
                Link = Url.FragmentAction("history"),
            });
        hubPivots.Add(new PivotView(VCResources.Compare)
            { 
                Id = "compare",
                Link = Url.FragmentAction("compare"),
                Disabled = true
            });
        hubPivots.Add(new PivotView(VCResources.Annotate)
            {
                Id = "annotate",
                Link = Url.FragmentAction("annotate"),
                Disabled = true
            }); 
    %>
    <%: Html.PivotViews(hubPivots, new { @class = "vc-explorer-tabs" })%>
</asp:content>
<asp:content contentplaceholderid="HubPivotFilters" runat="server">
</asp:content>
<asp:content contentplaceholderid="RightHubContent" runat="server">
    <div class="version-control-item-right-pane">
    </div>
    <% Html.RenderPartial("Templates/Status"); %>
</asp:content>

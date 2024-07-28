<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master"
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
        Html.UseScriptModules("VersionControl/Scenarios/Explorer/ExplorerView");
        Html.AddHubViewClass("versioncontrol-explorer-view");
        Html.IncludeContributions("ms.vss-search-platform.entity-type-collection", "ms.vss-code-search.code-entity-type");
        Html.IncludeContributions("ms.vss-code-web.explorer-view");
    %>
</asp:content>

<asp:content contentplaceholderid="HubBegin" runat="server">
    <%:Html.VCViewOptions(Model) %>
    <%:Html.AreaLocations((string[])(ViewBag.areaLocations)) %>
</asp:content>

<asp:Content ContentPlaceHolderID="PageEnd" runat="server">
    <% Html.RenderPartial("Templates/Status"); %>
</asp:Content>

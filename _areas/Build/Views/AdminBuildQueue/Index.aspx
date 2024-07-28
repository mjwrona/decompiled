<%@ Page Title="" Language="C#" MasterPageFile="../Shared/BuildSetting.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<BuildSettingsAdminViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="TfsControls=Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>
<%@ Implements Interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>

<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <% 
        Html.UseScriptModules("Admin/Scripts/TFS.Admin.Security", "Build/Scripts/Controls.Admin", "Build/Scripts/AdminView", "Build/Scripts/KnockoutExtensions");
        Html.AddHubViewClass("buildvnext-admin-view");
        
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.FilePathArtifactsAndSymbolsDeleteFeature);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.BuildAndReleaseResourceLimits);
    %>
</asp:content>

<asp:content contentplaceholderid="HubBegin" runat="server">
    <%: Html.BuildAdminViewOptions(Model) %>
</asp:content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <div class="buildvnext-admin-content" style="height: 100%;">
    </div>

    <%
        if (TfsWebContext.IsFeatureAvailable(FeatureAvailabilityFlags.BuildAndReleaseResourceLimits))
        {
            Html.RenderPartial("Templates/AdminResourceLimitsTab");
        }
    %>
</asp:content>
<%@ Page Title="" Language="C#" MasterPageFile="../Shared/BuildExplorer.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        // title is determined by the selected definition
        Html.ContentTitle("");

        // this gets rendered by the Html.ScriptModules() call after the PageBegin section
         Html.UseScriptModules(
            "Build/Scripts/KnockoutExtensions",
            "Build/Scripts/Views.Explorer",
            "Build/Scripts/Views.Definition",
            "Build/Scripts/Views.Details",
            "Build/Scripts/Views.Index");

        // set class name for the main view
        Html.AddHubViewClass("buildvnext-view");
        // Add the Feature flag for the Build Summary Email (Send Mail) functionality
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessBuildvNextEmailBuildSummary);

        // Add the Feature flag for showing preview features in the task UI
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.DistributedTaskTaskPreview);

        // Add the Feature flag for showing the gated check-in trigger UI
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.GatedCheckInTrigger);

        // Add the Feature flag for all definitions tab
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.BuildAllDefinitionsTab);

        // Add the Feature flag for showing licensing queue position in build console tab
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.BuildAndReleaseResourceLimits);

        Html.Chromeless(true);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <%:Html.SecurityOptions() %>
    <% if (ViewData["Tfs-HideBuildExplorerWarningBanner"] == null || String.Equals(ViewData["Tfs-HideBuildExplorerWarningBanner"].ToString(), "false", StringComparison.OrdinalIgnoreCase)) { %>
    <div class="explorer-warning-message"></div>
    <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitle" runat="server">
    <div class="hub-title ko-target"></div>
    <div class="hub-progress pageProgressIndicator"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftHubContent" runat="server">
    <div class="buildvnext-view-left-pane-content left-toolbar"></div>
    <div class="buildvnext-view-left-pane-content definition-tree" data-bind="template: 'buildvnext_definition_explorer_tree', visible: visible"></div>
    <div class="buildvnext-view-left-pane-content plan-tree" data-bind="template: 'buildvnext_plan_nodes_tab', visible: visible"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="RightHubContent" runat="server">
    <% 
        Html.RenderPartial("Templates/ArtifactsExplorerDialog");
        Html.RenderPartial("Templates/BuildDetailsArtifacts");
        Html.RenderPartial("Templates/BuildDetailsConsole");
        Html.RenderPartial("Templates/BuildDetailsHeader");
        Html.RenderPartial("Templates/BuildDetailsLogs");
        Html.RenderPartial("Templates/BuildDetailsSummary");
        Html.RenderPartial("Templates/BuildDetailsCustomTab");
        Html.RenderPartial("Templates/BuildDetailsTimeline");
        Html.RenderPartial("Templates/BuildDetailsXamlTabs");
        Html.RenderPartial("Templates/BuildIssues");
        Html.RenderPartial("Templates/BuildPlanNodesTab");
        Html.RenderPartial("Templates/BuildPlanNodesTree");
        Html.RenderPartial("Templates/BuildViews");
        Html.RenderPartial("Templates/DefinitionExplorerTab");
        Html.RenderPartial("Templates/DefinitionRetentionTab");
        Html.RenderPartial("Templates/DefinitionHistoryTab");
        Html.RenderPartial("Templates/DefinitionOptionsTab");
        Html.RenderPartial("Templates/DefinitionTree");
        Html.RenderPartial("Templates/DefinitionVariablesTab");
        Html.RenderPartial("Templates/SimpleProcessDefinitionTab");
        Html.RenderPartial("Templates/Explorer_Queued");
        Html.RenderPartial("Templates/Explorer_Completed");
        Html.RenderPartial("Templates/Explorer_Deleted");
        Html.RenderPartial("Templates/PivotFilter");
        Html.RenderPartial("Templates/QueueBuildDialog");
        Html.RenderPartial("Templates/TfGitQueueBuildDialog");
        Html.RenderPartial("Templates/TfvcQueueBuildDialog");
        Html.RenderPartial("Templates/TemplatesWizardDialog"); /*Note:this loads some tab templates as needed */

    %>
    <%:Html.ValidTimeZones() %>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="../Shared/BuildReact.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        // set class name for the main view
        // let's move away from using "buildvnext"
        Html.AddHubViewClass("build-view");
        Html.AddHubViewClass("build-definitions-view");

        // Add the Feature flag for all definitions tab
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.BuildAllDefinitionsTab);

        Html.UseScriptModules("Build/Scenarios/Definitions/View");
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <%:Html.SecurityOptions() %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    <div class="build-titleArea"></div>
    <div class="build-common-component-queuebuild"></div>
    <div class="build-common-component-renamedefinition"></div>
    <div class="build-common-component-savedefinition"></div>

    <div class="build-alldefinitions-move-component-foldermanage"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="build-pivot-content-container build-definitions-view-content"></div>
    <%
        // ko templates. feel free to phase these out by replacing them with react components!
        Html.RenderPartial("Templates/TemplatesWizardDialog");
        Html.RenderPartial("Templates/QueueBuildDialog");
        Html.RenderPartial("Templates/TfGitQueueBuildDialog");
        Html.RenderPartial("Templates/TfvcQueueBuildDialog");
    %>
</asp:Content>

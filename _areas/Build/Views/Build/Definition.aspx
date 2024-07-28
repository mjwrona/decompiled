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
        Html.AddHubViewClass("build-definition-view");
        Html.AddHubViewClass("bowtie");

        Html.UseScriptModules("Build/Scenarios/Definition/View");

        Html.IncludeContributions("ms.vss-build-web.build-definition-hub-tab-group");
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <%:Html.SecurityOptions() %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    <div class="build-titleArea"></div>
    <div class="build-common-component-queuebuild"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="build-pivot-content-container build-definition-view-content"></div>

    <%
        // ko templates. feel free to phase these out by replacing them with react components!
        Html.RenderPartial("Templates/TemplatesWizardDialog");
        Html.RenderPartial("Templates/QueueBuildDialog");
        Html.RenderPartial("Templates/TfGitQueueBuildDialog");
        Html.RenderPartial("Templates/TfvcQueueBuildDialog");
    %>
</asp:Content>

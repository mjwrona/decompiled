<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="System.Security.Policy" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:content ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%:Url.Themed("DistributedTasksLibrary.css")%>" type="text/css" rel="stylesheet" />
</asp:content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        Html.ContentTitle(AdminResources.Services);
        Html.UseScriptModules("Admin/Scripts/TFS.Admin.Controls", "Admin/Scripts/TFS.Admin.ServiceEndpoints.Controls", "Admin/Scripts/TFS.Admin.ServiceEndpoints");
        Html.AddHubViewClass("services-view");
    %>
</asp:Content>

<asp:content contentplaceholderid="HubTitle" runat="server">
</asp:content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <div class="resources-content">
        <div class="splitter horizontal hub-splitter">
            <div class="leftPane" role="complementary">
                <div class="resources-left-pane">
                    <div class="resources-left-pane-toolbar toolbar"></div>
                    <div class="search-input-wrapper"></div>
                    <div class="resources-view tree-pane">
                        <div class="resources tree">
                        </div>
                    </div>
                </div>
            </div>
            <div class="handleBar"></div>
            <div class="rightPane" role="region" aria-label="endpoint-page">
            <div class="hub-title header" role="heading" aria-level="1">
            </div>
                <div class="resources-right-pane"></div>
            </div>
        </div>
    </div>

    <% Html.RenderPartial("Templates/ServiceEndpointsAdminTemplate"); %>
    <% Html.RenderPartial("Templates/ExternalGitEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/AzureSubscriptionsTemplate"); %>
    <% Html.RenderPartial("Templates/ChefEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/GenericEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/GitHubEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/BitbucketEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/SshEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/SvnEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/CustomEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/AzureRMEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/GcpEndpointsTemplate"); %>
    <% Html.RenderPartial("Templates/DockerRegistryTemplate"); %>
    <% Html.RenderPartial("Templates/KubernetesTemplate"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
<%
        var hubPivots = new List<PivotView>();
        hubPivots.Add(new PivotView(AdminServerResources.EndpointTabTitle)
        {
            Id = "resources",
            Link = Url.FragmentAction("resources", null)
        });

        hubPivots.Add(new PivotView(AdminServerResources.AzureXamlBuildServicesTabTitle)
        {
            Id = "connectedservices",
            Link = Url.FragmentAction("connectedservices", null)
        });
    %>
    <%: Html.PivotViews(hubPivots, new { @class = "endpoints-tab" })%>
</asp:Content>

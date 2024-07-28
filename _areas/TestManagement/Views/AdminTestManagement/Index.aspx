<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server"%>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.UseScriptModules("TestManagement/Scripts/TFS.TestManagement.Admin"); %>
    <% Html.UseAreaCSS("TestManagement"); %>
    <% Html.AddHubViewClass("test-admin-view"); %>
</asp:Content>

<asp:content contentplaceholderid="HubPivotViews" runat="server">
    <%: Html.PivotViews(new[]
                {
                    new PivotView(TestManagementResources.RetentionActionName)
                    {
                        Id = "retention",
                        Link = Url.FragmentAction("retention", null)
                    }
                }, new { @class = "admin-view-tabs" })
    %>
</asp:content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <% 
        Html.RenderPartial("Templates/RetentionSettings");
    %>

    <div class="retention-settings-container bowtie"></div>
</asp:content>

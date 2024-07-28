<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.ContentTitle((string)ViewData[ViewDataConstants.Title]); %>
    <% Html.UseScriptModules("Agile/Scripts/Admin/TFS.Admin.AreaIterations"); %>
    <% Html.UseCSS("Agile"); %>
</asp:Content>

<%-- TODO: Waiting on answer from Gregg about the whitespace under the content title --%>
<asp:content contentplaceholderid="HubPivotViews" runat="server">
<%: Html.PivotViews(new[] { (string)ViewData[ViewDataConstants.Title] }) %>
</asp:content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <% Html.RenderPartial("AdminAreaIterations", this.ViewData); %>
</asp:content>

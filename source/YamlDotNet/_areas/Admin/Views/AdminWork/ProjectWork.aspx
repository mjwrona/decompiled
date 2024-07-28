<%@ Page
    Title=""
    Language="C#"
    MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.ProjectAdminWorkModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        Html.UseScriptModules("Agile/Scripts/Admin/ProjectWork");
        Html.AddHubViewClass("work-view");
        Html.UseCSS("Agile");
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitle" runat="server">
    <%: Html.HubTitle((string)ViewData[ViewDataConstants.ViewTitle]) %>
    <%  if (!string.IsNullOrEmpty(Model.processName))
        {
            Html.RenderPartial("CustomizeProcessMessageBar", Model.processName);
        }
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
    <%: Html.WorkProjectPivotViews() %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="project-admin-work">
        <%= Html.JsonIsland(Model, new { @class = "options" }, SecureJsonResult.DefaultMaxJsonLength) %>

        <div class="iterations-control"></div>
        <div class="areas-control"></div>
    </div>
</asp:Content>
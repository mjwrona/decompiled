<%@ Page
    Title=""
    Language="C#"
    MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.AdminWorkModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking" %>
<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.UseScriptModules("Agile/Scripts/Admin/Work");
        Html.UseCSS("Agile");
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitle" runat="server">
    <%: Html.HubTitle((string)ViewData[ViewDataConstants.ViewTitle]) %>
    <%  if (!string.IsNullOrEmpty(Model.ProjectWorkModel.processName))
        {
            Html.RenderPartial("CustomizeProcessMessageBar", Model.ProjectWorkModel.processName);
        }
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
    <%: Html.WorkPivotViews(Model) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="team-admin-work bowtie">
        <%= Html.TeamSettingsData(Model.TeamViewModel) %>
        <%= Html.JsonIsland(Model.ProjectWorkModel, new { @class = "project-work-model" }, SecureJsonResult.DefaultMaxJsonLength) %>
        <div class="team-settings-control"><%= Html.TeamSettingsControlOptions(Model.TeamViewModel) %></div>
        <div class="team-iterations-control"></div>
        <% if (Model.IsTeamFieldAreaPath)
            { %>
        <div class="team-areas-control"></div>
        <% } else { %>
        <%: Html.TeamField(this.ViewData["TeamFieldData"], "team-field-control") %>
        <% } %>
        <div class="templates-control"></div>

    </div>
</asp:Content>
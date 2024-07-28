<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorer.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminLicensesViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="TfsControls=Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(AdminServerResources.AccessLevelsTitle);
        Html.UseScriptModules("Admin/Scripts/TFS.Admin.Controls"); 
        Html.AddHubViewClass("licenses-view");
    %>
</asp:Content>

<asp:content contentplaceholderid="LeftHubContent" runat="server">
    <div class="hub-pivot toolbar admin-license-toolbar">
        <%: Html.MenuBar(new[] { new TfsControls.MenuItem(AdminServerResources.ExportLicenses, "export-licenses-action"){ ShowIcon = false } })%>
    </div>
    <div class="hub-pivot-content">
        <div class="licenses-list-control">
            <%= Html.AdminLicensesViewOptions(Model) %>
            <input type="hidden" />
        </div>
    </div>
</asp:content>

<asp:Content ID="RightHubContent" ContentPlaceHolderID="RightHubContent" runat="server">
    <div class="licenses-info"></div>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.BrowseControlModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(TfsWebContext.IsHosted ? AdminServerResources.AdministerAccount : AdminServerResources.AdministerServer);
        Html.UseScriptModules("Admin/Scripts/TFS.Admin.Controls");
        Html.AddHubViewClass("control-panel-hub-view");
    %>
</asp:Content>


<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
    <%: Html.ContentTitle() %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="control-panel-container vertical-fill-layout">
        <div class="control-panel-hub-header fixed-header">
            <div class="header"><%: TfsWebContext.NavigationContext.ServiceHost.Name %></div>
            <div class="subheader"><%: AdminServerResources.ControlPanelDescription %></div>
        </div>
        <div class="fill-content">
            <% Html.RenderPartial("BrowseControl", Model); %>
        </div>
    </div>
</asp:Content>
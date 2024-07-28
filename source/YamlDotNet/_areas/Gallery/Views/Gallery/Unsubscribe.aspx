<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.UseScriptModules("Gallery/Client/Pages/Unsubscribe/Unsubscribe.View"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Gallery3rdParty", "Gallery"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <%  Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.View"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.ScriptModules("VSS/Error") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <% Response.WriteFile("~/_views/Shared/SubscribeDialog.html"); %>
  <div class="user-mail-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.UserMailAddress], new {@class = ViewDataConstants.UserMailAddress}) %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.UserSettings], new {@class = ViewDataConstants.UserSettings}) %>
  </div>
  <div class="main-content unsubscribe-page">
  </div>
</asp:Content>
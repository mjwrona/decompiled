<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.SecurityModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        Html.ContentTitle((string)ViewData["Title"]);
        Html.UseScriptModules("Admin/Scripts/TFS.Admin.Controls");
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <% Html.RenderPartial("SecurityMin", Model); %>
</asp:Content>

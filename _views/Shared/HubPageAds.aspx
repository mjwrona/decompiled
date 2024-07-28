<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Main.master" 
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Models.AdPageModel>" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.UseScriptModules("Presentation/Scripts/TFS/TFS.UI.Controls.Common"); %>
    <% Html.AddHubViewClass(Model.ClassName); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageBegin" runat="server" />

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.IFrameControl(Model.Url) %>
</asp:Content>
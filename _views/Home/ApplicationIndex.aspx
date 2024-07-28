<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>

<%@ Implements Interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</asp:Content>
<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(WACommonResources.PageTitleServerHome);
        Html.AddHubViewClass("home-view");
        Html.UseScriptModules("Presentation/Scripts/TFS/TFS.Host.UI.AccountHomeView");
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitle" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="responsive-grid-container">
        <% Html.RenderGrid(); %>
    </div>
    <div class="account-home-view">
        <%: Html.AccountHomeViewData() %>
    </div>
</asp:Content>

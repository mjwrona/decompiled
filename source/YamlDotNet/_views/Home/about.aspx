<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
	<% Html.ContentTitle(PresentationServerResources.AboutPageTitle); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
        <%
        string version = "";
        try
        {
            var fileVersionAttr = typeof(TfsWebContext).Assembly.GetCustomAttributes(false).OfType<System.Reflection.AssemblyFileVersionAttribute>().FirstOrDefault();
            version = fileVersionAttr != null ? fileVersionAttr.Version : "";
        }
        catch{}
        %>
    <div class="hub-no-content-gutter">
        <h2>Azure DevOps Server</h2>
        <p class="version"><%: string.Format(PresentationServerResources.AboutVersionFormat, version) %></p>
        <p class="copyright"><%: PresentationServerResources.FooterCopyright %></p>
    </div>
</asp:Content>
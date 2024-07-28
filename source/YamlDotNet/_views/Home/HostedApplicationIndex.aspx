<%@ page title="" language="C#" masterpagefile="~/_views/Shared/Masters/HubPage.master"
    inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Models.GettingStartedModel>" %>

<%@ import namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ import namespace="Microsoft.TeamFoundation.Integration.Server" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Models" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ implements interface="Microsoft.TeamFoundation.Server.WebAccess.IChangesMRUNavigationContext" %>

<asp:content id="Content1" contentplaceholderid="HeadContent" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <%: Html.EnsightenTrackingScript() %>
</asp:content>
<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(WACommonResources.HostedWelcomePageTitle);
        Html.AddHubViewClass("home-view");
        Html.UseScriptModules("Account/Scripts/TFS.Account");
        Html.UseScriptModules("Presentation/Scripts/TFS/TFS.Host.UI.AccountHomeView");
    %>
</asp:content>

<asp:content contentplaceholderid="HubTitle" runat="server">
</asp:content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <% Html.UpgradeToFullVersionMessage(); %>
    <%: Html.AccountTrialInformation() %>
    <div class="responsive-grid-container">
        <% Html.RenderGrid(); %>
    </div>
    <div class="account-home-view">
        <%: Html.AccountHomeViewData() %>
    </div>
        
    <%= Html.GettingStartedOptions(Model) %>
</asp:content>

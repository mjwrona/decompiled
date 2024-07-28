<%@ page language="C#" masterpagefile="~/_views/Shared/Masters/HubPagePivot.Master"
    inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage" %>

<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ import namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <% 
        Html.ContentTitle(AdminResources.Settings);
        Html.UseScriptModules("Admin/Scripts/TFS.Admin.Controls");
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.SSHPublicKeys);
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.AnonymousAccessFeatureName);
    %>

</asp:content>

<asp:content contentplaceholderid="HubPivotViews" runat="server">
    <%: Html.PivotViews(new[] 
        { 
            new PivotView(AdminServerResources.AccountTab)
            { 
                Id = "settings"
            }
        }, new
        {
            @class = "account-settings-pivot-view"
        }) %>
</asp:content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <%: Html.AccountAadInformation() %>
    <div class="account-settings-container">
        <div class="toolbar hub-pivot-toolbar"></div>
        <div class="message-area-container">
            <div class="message-area"></div>
        </div>
        <div class="settings-control"></div>
    </div>
</asp:content>

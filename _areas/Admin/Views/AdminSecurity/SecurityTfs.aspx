<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.SecurityModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        Html.ContentTitle((string)ViewData["Title"]);
        Html.UseScriptModules("Admin/Scripts/TFS.Admin.Controls");

        // Add the Feature flag for the Group Rules check when loading groups
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.GroupLicensingRule);  
        // Add the Feature flag for the AAD Admin Group Ui
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.AadGroupsAdminUi);
        // Add the Feature flag for identity picker cache
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.IdentityPickerClientPerformance);
        // Add the feature flag state for not displaying the list of all groups in scope to avoid timeouts
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.DoNotPopulateIdentityGrid);
    %>    
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <% Html.RenderPartial("SecurityMin", Model); %>
</asp:Content>

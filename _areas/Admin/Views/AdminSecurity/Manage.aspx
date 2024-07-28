<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerPivot.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.ManageViewModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server"%>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
	<% 
        Html.ContentTitle(ViewData["Title"] as string);
        Html.UseScriptModules("Admin/Scripts/TFS.Admin.Controls", "Admin/Scripts/TFS.Admin.Common");
        Html.AddHubViewClass("manage-identities-view");
        if (TfsWebContext.NavigationContext.TopMostLevel == NavigationContextLevels.Team)
        {
            Html.AddHubViewClass("team-view");
        }

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

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <script type="application/json" class="permissions-context"><%= ViewData["PermissionsData"] %></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitleContent" runat="server">
   <div class="label"></div>
   <div class="image"></div>
</asp:Content>

<asp:content contentplaceholderid="LeftHubContent" runat="server">
    <%
        if (!TfsWebContext.TfsRequestContext.IsFeatureEnabled(FeatureAvailabilityFlags.AadGroupsAdminUi))
        {
    %>
    <div class="hub-pivot">
        <div class="views">
            <%  if (TfsWebContext.NavigationContext.TopMostLevel != NavigationContextLevels.Team)
    {
        List<PivotView> filters = new List<PivotView>();
        filters.Add(new PivotView(AdminServerResources.ShowAllGroups)
        {
            Id = "groups",
            Title = AdminServerResources.ShowAllGroupsTooltip
        });
        filters.Add(new PivotView(AdminServerResources.ShowAllUsers)
        {
            Id = "users",
            Title = AdminServerResources.ShowAllUsersTooltip
        });
            %>
                <%: Html.PivotViews(filters, new { @class = "change-groups-filter" }) %>
            <%  } %>
        </div>
    </div>
    <% 
        } 
    %>
    <div class="hub-pivot-content">
        <div class="identity-list-section vertical-fill-layout">
            <div class="identity-search-box fixed-header">
                <div id="manage-identities-create-group-container" class="bowtie">
                    <button id="manage-identities-create-group"/>
                </div>
                <% if (!TfsWebContext.TfsRequestContext.IsFeatureEnabled(FeatureAvailabilityFlags.AadGroupsAdminUi))
                { %>
                    <div class="identity-search-control"></div>
                <% }
                else
                { %>
                    <div class="ip-groups-search-container"></div>
                <% } %>
            </div>
            <div class="main-identity-grid fill-content"></div>
        </div>
    </div>
</asp:content>

<asp:Content contentplaceholderid="HubPivot" runat="server">
    <div class="hub-pivot">
        <div class="views">
            <%: Html.PivotViews(new[]
                {
                    new PivotView(AdminResources.Permissions)
                    {
                        Id = "summary",
                        Link = Url.FragmentAction("summary", null)
                    },
                    new PivotView(AdminResources.Members)
                    {
                        Id = "members",
                        Link = Url.FragmentAction("members", null)
                    },
                    new PivotView(AdminServerResources.MemberOf)
                    {
                        Id = "memberOf",
                        Link = Url.FragmentAction("memberOf", null)
                    }
                }, new { @class = "manage-view-tabs" })
            %>
        </div>
    </div>
</asp:Content>

<asp:content contentplaceholderid="RightHubContent" runat="server">
    <div class="identity-details-section">
        <div id="identityInfo" class="manage-info"></div>
        <%: Html.JsonIsland(ViewData["HasSingleCollectionAdmin"], new {@class = "show-single-pca-warning"}) %>
    </div>
</asp:content>

<asp:content contentplaceholderid="HubEnd" runat="server">
    <%: Html.ManageViewOptions(Model) %>
</asp:content>

<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.UseScriptModules("Gallery/Client/Pages/Category/Category.View"); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableVersionRangeOnSearchPage); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.TileImpressionsSearchPage); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.ShowPublishExtensions); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.ShowNewZeroSearchResultsExperience); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableVsForMac); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableCertifiedPublisherUIChanges); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableSortByInstallCountUI); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePlatformSpecificExtensionsForVSCode); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableTargetPlatformFilterDropdown); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableVerifiedPublisherDomain); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <%: Html.GetPageTitle(
    ((PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]).GalleryPage,
    (PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MetaTagPlaceholder" runat="server">
    <%: Html.RenderPageMetadata(
    ((PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]).GalleryPage,
    (PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Gallery3rdParty", "Gallery"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.View", "Gallery/Client/Common/Telemetry", "Gallery/Client/Controls/ErrorControl/ErrorControl.View"
           , "Gallery/Client/Service/VSSGallery/VSSGallery", "Gallery/Client/Service/VSGallery/VSGallery", "VSS/FeatureAvailability/RestClient", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%:Html.ScriptModules("VSS/Error") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%-- TODO: Check if there is a better approach than WriteFile, and if we really need html, rather than ascx :
         , RenderPartial doesn't include .html files properly.
    --%>
    <% Response.WriteFile("~/_views/Shared/Image.html"); %>
    <% Response.WriteFile("~/_views/Shared/ItemTile.html"); %>
    <% Response.WriteFile("~/_views/Shared/RatingControl.html"); %>
    <% Response.WriteFile("~/_views/Shared/VSItemTile.html"); %>
    <% Response.WriteFile("~/_views/Shared/ItemBanner.html"); %>
    <% Response.WriteFile("~/_views/Shared/ItemGrid.html"); %>
    <% Response.WriteFile("~/_views/Shared/ItemRow.html"); %>
    <% Response.WriteFile("~/_views/Shared/CategorizedList.html"); %>
    <% Response.WriteFile("~/_views/Shared/Category.html"); %>
    <% Response.WriteFile("~/_views/Shared/BreadCrumb.html"); %>
    <% Response.WriteFile("~/_views/Shared/ErrorControl.html"); %>
    <% Response.WriteFile("~/_views/Shared/FilterControl.html"); %>
    <% Response.WriteFile("~/_views/Shared/SearchBar.html"); %>
    <% Response.WriteFile("~/_views/Shared/SeeMore.html"); %>
    <% Response.WriteFile("~/_views/Shared/ZeroSearchResults.html"); %>
    <% Response.WriteFile("~/_views/Shared/NewImage.html"); %>
    <div class="user-mail-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.UserMailAddress], new {@class = ViewDataConstants.UserMailAddress}) %>
    </div>
    <div>
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.ProductCategories], new {@class = ViewDataConstants.ProductCategories}) %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.ValidCategories], new {@class = ViewDataConstants.ValidCategories})%>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.TargetPlatforms], new {@class = ViewDataConstants.TargetPlatforms})%>
    </div>
    <div class="main-content category-main-content">
        <div class="category-content">
        </div>
    </div>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.WebApi" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <%: Html.GetPageTitle(GalleryPages.PublisherProfile, (PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MetaTagPlaceholder" runat="server">
    <%: Html.RenderPageMetadata(GalleryPages.PublisherProfile, (PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Core", "Gallery", "fabric"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.AddBodyClass("publisher-profile-page");
        Html.UseScriptModules("Gallery/Client/Pages/PublisherProfile/PublisherProfile.View");
        Html.UseCSS("jQueryUI-Modified");
        Html.UseCSS("VSS.Features.Controls");
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableCertifiedPublisherUIChanges);
        Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableVsForMac);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.View", "Gallery/Client/Common/Telemetry", "Gallery/Client/Controls/ErrorControl/ErrorControl.View"
             , "Gallery/Client/Service/VSSGallery/VSSGallery", "Gallery/Client/Service/VSGallery/VSGallery", "VSS/FeatureAvailability/RestClient", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.ScriptModules("VSS/Error") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.PublisherProfilePublisherDetails], new { @class = ViewDataConstants.PublisherProfilePublisherDetails } ) %>
    <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.PublisherProfileVsExtensionsResult], new { @class = ViewDataConstants.PublisherProfileVsExtensionsResult} ) %>
    <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.PublisherProfileVscodeExtensionsResult], new { @class = ViewDataConstants.PublisherProfileVscodeExtensionsResult} ) %>
    <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.PublisherProfileVstsExtensionsResult], new { @class = ViewDataConstants.PublisherProfileVstsExtensionsResult} ) %>
    <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.PublisherProfileVsForMacExtensionsResult], new { @class = ViewDataConstants.PublisherProfileVsForMacExtensionsResult} ) %>
    <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.PublisherProfileExtensionQueryPageSize], new { @class = ViewDataConstants.PublisherProfileExtensionQueryPageSize} ) %>
    <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.PublisherProfileQueryWithDisplayName], new { @class = ViewDataConstants.PublisherProfileQueryWithDisplayName} ) %>
    <div id="PublisherProfilePageContainer" role="main"></div>
    <script type="text/javascript" <%=Html.GenerateNonce(true) %>>
    $(window).on('load', function () {
        $(".vss-PivotBar--bar .vss-PivotBar--itemLink").removeAttr("tabindex");
    });
    </script>
</asp:Content>

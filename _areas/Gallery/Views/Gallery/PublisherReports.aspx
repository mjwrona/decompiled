<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <%: string.Format("{0} | {1}", GalleryResources.ReportsPageTitle, GalleryResources.PageTitle) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Core", "Gallery", "fabric"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ShowFooter(false);
        Html.UseScriptModules("Gallery/Client/Pages/PublisherReports/PublisherReportsView");
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableAcquisitionTab);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableAcquisitionTabForPaid);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableAQnATab);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableIntAcquisitionTab);
        Html.IncludeFeatureFlagState(GalleryFeatureFlags.ShowQnA);
        Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableQnABypass);
        Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableQnABypassForVSTS);
        Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableSalesTransactionsTab);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableSupportRequestFeature);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePlatformSpecificExtensionsUIForManagePages);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableVSConsolidationUIForManagePages);
    %>
    <% Html.UseCSS("jQueryUI-Modified"); %>
    <% Html.UseCSS("VSS.Features.Controls"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.View", "Gallery/Client/Common/Telemetry", "Gallery/Client/Controls/ErrorControl/ErrorControl.View"
             , "Gallery/Client/Service/VSSGallery/VSSGallery", "Gallery/Client/Service/VSGallery/VSGallery", "VSS/FeatureAvailability/RestClient", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.ScriptModules("VSS/Error") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vss-item-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtension], new {@class = ViewDataConstants.VSSExtension}) %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.UninstallEvents], new {@class = ViewDataConstants.UninstallEvents}) %>
    </div>
    <div class="disable-contact-option">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.DisableContactOption], new {@class = ViewDataConstants.DisableContactOption}) %>
    </div>
    <div class="can-update-extension-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.CanUpdateExtension], new {@class = ViewDataConstants.CanUpdateExtension}) %>
    </div>
    <div class="vss-item-properties-container">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemProperties], new {@class = ViewDataConstants.VSSItemProperties}) %>
    </div>
    <div class="vss-item-offer">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtensionOffer], new {@class = ViewDataConstants.VSSExtensionOffer}) %>
    </div>
    <div Id="GalleryExtensionReport">

    </div>
    <div class="supported-target-platforms-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.TargetPlatforms], new {@class = ViewDataConstants.TargetPlatforms}) %>
    </div>
    <div class="manage-vscode-records-per-page-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.ManageVSCodeReportsRecordsPerPage], new {@class = ViewDataConstants.ManageVSCodeReportsRecordsPerPage}) %>
    </div>
</asp:Content>

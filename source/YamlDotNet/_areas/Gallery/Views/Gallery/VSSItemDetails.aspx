<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <%: Html.GetPageTitle(GalleryPages.ItemDetailsPage, (PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MetaTagPlaceholder" runat="server">
    <%: Html.RenderPageMetadata(GalleryPages.ItemDetailsPage, (PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Gallery3rdParty", "Gallery", "fabric"); %>
    <% if(Html.IsCaptchaEnabledOnReviewAndRating() || Html.IsCaptchaEnabledOnQnA()) { %>
            <script src="<%: Html.GetReCaptchaUrl() %>" async defer></script>
            <script <%= Html.GenerateNonce(true) %>>
                function onSubmit(token) {
                    var tokenElement = document.getElementById("tokenId")
                    tokenElement.value = token;
                    tokenElement.click();
                }

                window.resetReCaptcha = function () {
                    grecaptcha.reset();
                }

                window.validate = function () {
                    grecaptcha.execute();                  
                }
            </script>
     <% } %>
</asp:Content>
    
<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.UseScriptModules("Gallery/Client/Pages/VSSItemDetails/VSSItemDetails.View"); %>    
    <% Html.AddBodyClass("gallery-page-item-details"); %>
    <% Html.UseCSS("jQueryUI-Modified"); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.PublisherReply); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.ShowVSItemLink); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.ShowLargeThumbnailAsBrandingIcon); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.AllowNewAccountAPI); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.LogGetStartedCount); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableVersionHistoryViewForVS); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableVersionHistoryViewForVSCode); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaInReviewAndRating); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaInQnA); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.ShowQnA); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableQnABypass); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableQnABypassForVSTS); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.PrivacyPage); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableByolForMarketplace); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableReportsLinkForVSCode); %>
    <% Html.IncludeFeatureFlagState(GalleryFeatureFlags.EnableReportsLinkForVSIde); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableNewTokenAcquisitionExperience); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableNewTokenAcquisitionExperienceForOffers); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableNewTokenAcquisitionExperienceForTestManager); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableSupportRequestFeature); %>
    <% if (!WebContext.IsHosted) { %>
       <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableNewAcquisitionOnPremExperience); %>
    <% } %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePublisherProfilePage); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableCertifiedPublisherUIChanges); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableSeeMoreButtonOnVersionHistoryTab); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableVerifiedPublisherDomain); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReferralLinkRedirectionWarningPopup); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.View", "Gallery/Client/Common/Telemetry", "Gallery/Client/Controls/ErrorControl/ErrorControl.View"
           , "Gallery/Client/Service/VSSGallery/VSSGallery", "Gallery/Client/Service/VSGallery/VSGallery", "VSS/FeatureAvailability/RestClient", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.TfsAntiForgeryToken() %>  
    <%: Html.ScriptModules("VSS/Error") %>    
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <% Response.WriteFile("~/_views/Shared/VSSItemDetails.html"); %>
    <% Response.WriteFile("~/_views/Shared/VSCodeCopyCommand.html"); %>
    <% Response.WriteFile("~/_views/Shared/Image.html"); %>
    <% Response.WriteFile("~/_views/Shared/Carousel.html"); %>
    <% Response.WriteFile("~/_views/Shared/ItemTile.html"); %>
    <% Response.WriteFile("~/_views/Shared/RatingControl.html"); %>   
    <% Response.WriteFile("~/_views/Shared/ItemBanner.html"); %>
    <% Response.WriteFile("~/_views/Shared/BreadCrumb.html"); %>
    <% Response.WriteFile("~/_views/Shared/ErrorControl.html"); %>
    <% Response.WriteFile("~/_views/Shared/NewImage.html"); %>
       
    <% if(Html.IsCaptchaEnabledOnReviewAndRating() || Html.IsCaptchaEnabledOnQnA()) { %>
            <div class="g-recaptcha" <%= Html.GenerateNonce(true) %> data-sitekey="<%: Html.GetReCaptchaSiteKey() %>" data-callback="onSubmit" data-size="invisible"></div>
    <% } %>
    <%  if ((bool)this.ViewData[ViewDataConstants.IsModalInstall]) { %>
        <% Response.WriteFile("~/_views/Shared/FilterControl.html"); %>
        <% Response.WriteFile("~/_views/Shared/Spinner.html"); %>
    <% } %>
    <% Response.WriteFile("~/_views/Shared/spinner.html"); %>

    <% if (Html.DebugEnabled()) { %>
    <script type="text/javascript" <%= Html.GenerateNonce(true) %> src="<%:StaticResources.ThirdParty.Scripts.GetLocation("bootstrap.js") %>"></script>
    <% } else { %>
    <script type="text/javascript" <%= Html.GenerateNonce(true) %> src="<%:StaticResources.ThirdParty.Scripts.GetLocation("bootstrap.min.js") %>"></script>
    <% } %>
    <div class="main-content item-details-main-content">
        <i class="centered big-spinner bowtie-icon bowtie-spinner"></i>
    </div>
    <div class="vss-item-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtension], new {@class = ViewDataConstants.VSSExtension}) %>
    </div>
    <div class="vss-item-token">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtensionToken], new {@class = ViewDataConstants.VSSExtensionToken}) %>
    </div>
    <div class="vss-item-offer">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtensionOffer], new {@class = ViewDataConstants.VSSExtensionOffer}) %>
    </div>
    <div class="vss-item-offer-meter-price-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtensionOfferMeterPrice], new {@class = ViewDataConstants.VSSExtensionOfferMeterPrice}) %>
    </div>
    <div class="vss-item-offer-meter-price-details-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtensionOfferMeterPriceCurrency], new {@class = ViewDataConstants.VSSExtensionOfferMeterPriceCurrency}) %>
    </div>
    <div class="vss-item-overview-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemOverview], new {@class = ViewDataConstants.VSSItemOverview}) %>
    </div>
    <div class="vss-item-pricing-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemPricing], new {@class = ViewDataConstants.VSSItemPricing}) %>
    </div>
    <div class="vss-item-properties-container">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemProperties], new {@class = ViewDataConstants.VSSItemProperties}) %>
    </div>
    <div class="is-migrated-container">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.IsMigrated], new {@class = ViewDataConstants.IsMigrated}) %>
    </div>
    <div class="vss-item-badges-container">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemBadges], new {@class = ViewDataConstants.VSSItemBadges}) %>
    </div>
    <div class="user-login-url-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.UserLoginUrl], new {@class = ViewDataConstants.UserLoginUrl}) %>
    </div>
    <div class="is-connected-server-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.IsConnectedServer], new {@class = ViewDataConstants.IsConnectedServer}) %>
    </div>
    <div class="pinned-user-review-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.PinnedUserReview], new {@class = ViewDataConstants.PinnedUserReview}) %>
    </div>
    <div class="csp-user">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.IsCspUser], new { @class = ViewDataConstants.IsCspUser }) %>
    </div>
    <div class="vss-item-scope-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemScope], new {@class = ViewDataConstants.VSSItemScope}) %>
    </div>
    <% if (this.ViewData[ViewDataConstants.VSSProjectCollections] != null) { %>
        <div class="VSSProjectCollections">
        <%: Html.DataContractJsonIsland(this.ViewData[ViewDataConstants.VSSProjectCollections], new {@class = ViewDataConstants.VSSProjectCollections}) %>
        </div>
    <% } %>
    <% if (this.ViewData[ViewDataConstants.OrgPublisherDisplayNames] != null) { %>
        <div class="OrgPublisherDisplayNames">
        <%: Html.DataContractJsonIsland(this.ViewData[ViewDataConstants.OrgPublisherDisplayNames], new {@class = ViewDataConstants.OrgPublisherDisplayNames}) %>
        </div>
    <% } %>
    <div class="CreateNewSubscription">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSSubscriptionCreationUrl], new {@class = ViewDataConstants.VSSSubscriptionCreationUrl}) %>
    </div>
    <% if (this.ViewData[ViewDataConstants.NeedsAADAuth] != null) { %>
        <div class="needs-aad-auth-data">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.NeedsAADAuth], new {@class = ViewDataConstants.NeedsAADAuth}) %>
        </div>
    <% } %>
    <div class="can-update-extension-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.CanUpdateExtension], new {@class = ViewDataConstants.CanUpdateExtension}) %>
    </div>
     <div class="has-publisher-role-reader-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.HasPublisherRoleReader], new {@class = ViewDataConstants.HasPublisherRoleReader}) %>
    </div>
    <% if (this.ViewData[ViewDataConstants.PublishJobId] != null) { %>
        <div class="vss-publish-jobid-data">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.PublishJobId], new {@class = ViewDataConstants.PublishJobId}) %>
        </div>
    <% } %>
    <% if (this.ViewData[ViewDataConstants.OnPremRedirectURL] != null) { %>
        <div class="onprem-redirect-url-data">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.OnPremRedirectURL], new {@class = ViewDataConstants.OnPremRedirectURL}) %>
        </div>
    <% } %>

    <% if (!WebContext.IsHosted && !String.IsNullOrEmpty((string)this.ViewData[ViewDataConstants.MarketplaceProductionUrl])) { %>
        <div class="market-url">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.MarketplaceProductionUrl], new { @class = ViewDataConstants.MarketplaceProductionUrl }) %>
        </div>
    <% } %>

    <% if (!WebContext.IsHosted && !String.IsNullOrEmpty((string)this.ViewData[ViewDataConstants.MarketplaceBrowseUrl])) { %>
        <div class="market-url">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.MarketplaceBrowseUrl], new { @class = ViewDataConstants.MarketplaceBrowseUrl }) %>
        </div>
    <% } %>
    <% if (WebContext.IsHosted && !String.IsNullOrEmpty((string)this.ViewData[ViewDataConstants.GalleryBrowseUrl])) { %>
        <div class="market-url">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.GalleryBrowseUrl], new { @class = ViewDataConstants.GalleryBrowseUrl }) %>
        </div>
    <% } %>

    <% if (WebContext.IsHosted) { %>
        <div class="msdn-browse-url">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.MsdnUrl], new { @class = ViewDataConstants.MsdnUrl }) %>
        </div>
    <% } %>

    <% if (WebContext.IsHosted && this.ViewData[ViewDataConstants.IsSupportedForOnPremVersion] != null) { %>
        <div class="onprem-support">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.IsSupportedForOnPremVersion], new { @class = ViewDataConstants.IsSupportedForOnPremVersion }) %>
        </div>
    <% } %>

    <div class="vss-install-target-id">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSInstallTargetId], new {@class = ViewDataConstants.VSSInstallTargetId}) %>
    </div>
     <div >
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSInstallContext], new {@class = ViewDataConstants.VSSInstallContext}) %>
    </div>
    <% if (this.ViewData[ViewDataConstants.WorksWith] != null) { %>
        <div class="worksWithStrings">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.WorksWith], new {@class = ViewDataConstants.WorksWith }) %>
        </div>
    <% } %>
    <% if (this.ViewData[ViewDataConstants.TargetPlatforms] != null) { %>
        <div class="presentTargetPlatformsContainer">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.TargetPlatforms], new {@class = ViewDataConstants.TargetPlatforms }) %>
        </div>
    <% } %>
    <% if (this.ViewData[ViewDataConstants.VsixId] != null) { %>
        <div class="vsixId-container">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VsixId], new {@class = ViewDataConstants.VsixId}) %>
        </div>
    <% } %>
    <script type="text/javascript" <%=Html.GenerateNonce(true) %>>
        $(window).on('load', function () {
            setTimeout( function () {
                $('.rnr-top-container .reviews-container .rnr-review-column .rating-control.light .ms-FocusZone').each(function () {
                    var $input = jQuery(this);
                    $input.removeAttr("tabindex");
                    $input.removeAttr("data-is-focusable");
                });
            }, 500);
            });
    </script>
</asp:Content>

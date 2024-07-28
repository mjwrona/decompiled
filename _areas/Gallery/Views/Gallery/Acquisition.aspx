<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.WebApi" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Gallery3rdParty", "Gallery", "fabric"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">     
    <% Html.ShowFooter(false); %>
    <% Html.AddBodyClass("acquisition-page"); %>
    <% Html.UseScriptModules("Gallery/Client/Pages/Acquisition/Acquisition.View"); %>
    <% if (!WebContext.IsHosted) { %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/CollectionSelectionStep", "Gallery/Client/Scenarios/Acquisition/Steps/CollectionSelectionStepForConnectedFlow"); %>
    <% } %>
    <% else if (Html.isPaidResource((PublishedExtension)this.ViewData[ViewDataConstants.VSSExtension])) { %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/AccountSelectionStepForResource", "Gallery/Client/Scenarios/Acquisition/Steps/BuyCustomizationStepForResource", "Gallery/Client/Scenarios/Acquisition/Steps/ReviewStepForAccountScope", "Gallery/Client/Scenarios/Acquisition/Steps/ReviewStep", "Gallery/Client/Scenarios/Acquisition/Steps/SubscriptionSelectionStepForAccountScope"); %>
    <% } %>
    <% else if (Html.isPaidOffers((PublishedExtension)this.ViewData[ViewDataConstants.VSSExtension])) { %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/BuyCustomizationStepForOffers", "Gallery/Client/Scenarios/Acquisition/Steps/BuyCustomizationStepForAnnualOffers"); %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/ReviewStepForOfferScope","Gallery/Client/Scenarios/Acquisition/Steps/ReviewStepForAnnualOfferScope", "Gallery/Client/Scenarios/Acquisition/Steps/ReviewStep"); %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/SubscriptionSelectionStepForOffer"); %> 
    <% } %> 
    <% else if ((WebContext.IsHosted && ViewData[ViewDataConstants.ServerContext] == null) && (Html.isPaidExtension((PublishedExtension)this.ViewData[ViewDataConstants.VSSExtension], (ExtensionOfferDetails)this.ViewData[ViewDataConstants.VSSExtensionOffer]) && (!Html.isByolEnforced((PublishedExtension)this.ViewData[ViewDataConstants.VSSExtension])))) { %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/AccountSelectionStepForPaidExtension"); %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/BuyCustomizationStepForExtension"); %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/ReviewStepForAccountScope", "Gallery/Client/Scenarios/Acquisition/Steps/ReviewStep"); %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/SubscriptionSelectionStepForAccountScope"); %> 
    <% } %>
    <% else if (WebContext.IsHosted && ViewData[ViewDataConstants.ServerContext] != null) { %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/AccountSelectionStepInConnectedContext"); %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/BuyCustomizationStepInConnectedContext"); %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/ReviewStepInConnectedContext", "Gallery/Client/Scenarios/Acquisition/Steps/ReviewStep"); %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/SubscriptionSelectionStepInConnectedContext"); %> 
    <% } %>
    <%  else { %>
        <% Html.UseScriptModules("Gallery/Client/Scenarios/Acquisition/Steps/AccountSelectionStepForFreeExtension"); %>
    <% } %>  
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableNewTokenAcquisitionExperience); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableByolForMarketplace); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableNewTokenAcquisitionExperienceForOffers); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableNewTokenAcquisitionExperienceForTestManager); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableNewEmsAcquisitionOptions); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableCommerceServiceRouting); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableCommerceServiceFallback); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.CallNewGetAccountsVersionAPI); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("react", "react-dom", "Gallery/Client/Pages/Common/Base.Minimal.View", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%:Html.TfsAntiForgeryToken() %>
    <%: Html.ScriptModules("VSS/Error") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div id="AcquisitionWorkflowContainer" role="main"></div>
    <div class="vss-item-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtension], new {@class = ViewDataConstants.VSSExtension}) %>
    </div>
    <% if (this.ViewData[ViewDataConstants.NeedsAADAuth] != null) { %>
        <div class="needs-aad-auth-data">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.NeedsAADAuth], new {@class = ViewDataConstants.NeedsAADAuth}) %>
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
        <div class="market-browse-url-wrapper">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.MarketplaceBrowseUrl], new { @class = ViewDataConstants.MarketplaceBrowseUrl }) %>
        </div>
    <% } %>
    <% if (this.ViewData[ViewDataConstants.PublishJobId] != null) { %>
        <div class="vss-publish-jobid-data">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.PublishJobId], new {@class = ViewDataConstants.PublishJobId}) %>
        </div>
    <% } %>
    <% if (this.ViewData[ViewDataConstants.VSSProjectCollections] != null) { %>
        <div class="VSSProjectCollections">
        <%: Html.DataContractJsonIsland(this.ViewData[ViewDataConstants.VSSProjectCollections], new {@class = ViewDataConstants.VSSProjectCollections}) %>
        </div>
    <% } %>
    <div class="vss-item-token">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtensionToken], new {@class = ViewDataConstants.VSSExtensionToken}) %>
    </div>
    <div class="vss-item-offer">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtensionOffer], new {@class = ViewDataConstants.VSSExtensionOffer}) %>
    </div>
    <div class="vss-item-overview-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemOverview], new {@class = ViewDataConstants.VSSItemOverview}) %>
    </div>
    <div class="vss-item-pricing-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemPricing], new {@class = ViewDataConstants.VSSItemPricing}) %>
    </div>
    <div class="vss-item-acquisition-customization-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VssItemAcquisitionCustomization], new {@class = ViewDataConstants.VssItemAcquisitionCustomization}) %>
    </div>
    <div class="user-login-url-data">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.UserLoginUrl], new {@class = ViewDataConstants.UserLoginUrl}) %>
    </div>
    <div class="vss-install-target-id">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSInstallTargetId], new {@class = ViewDataConstants.VSSInstallTargetId}) %>
    </div>
    <div class="msdn-browse-url">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.MsdnUrl], new { @class = ViewDataConstants.MsdnUrl }) %>
    </div>
    <div class="VSSItemScope">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemScope], new {@class = ViewDataConstants.VSSItemScope}) %>
    </div>
    <div class="VSSItemProperties">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSItemProperties], new {@class = ViewDataConstants.VSSItemProperties}) %>
    </div>
    <div class="CreateNewSubscription">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.VSSSubscriptionCreationUrl], new {@class = ViewDataConstants.VSSSubscriptionCreationUrl}) %>
    </div>
    <div class="SelectedAccount">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.SelectedAccount], new {@class = ViewDataConstants.SelectedAccount}) %>
    </div>
    <div class="AcquisitionOptions">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.DefaultOptions], new {@class = ViewDataConstants.DefaultOptions}) %>
    </div>
    <div class="Accounts">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.Accounts], new {@class = ViewDataConstants.Accounts}) %>
    </div>
    <div class="csp-user">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.IsCspUser], new { @class = ViewDataConstants.IsCspUser }) %>
    </div>
    <div class="authenticated-tenant">
        <%: Html.JsonIsland(this.ViewData[ViewDataConstants.AuthenticatedTenantId], new { @class = ViewDataConstants.AuthenticatedTenantId }) %>
    </div>
</asp:Content>

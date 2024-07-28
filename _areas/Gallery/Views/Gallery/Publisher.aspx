<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <% if (WebContext.IsHosted) { %>
        <%: string.Format("{0} | {1}", GalleryResources.PublisherTitle, GalleryResources.PageTitle) %>
    <% } else { %>
        <%: string.Format("{0} | {1}", GalleryResources.PublisherTitle, GalleryResources.OnPremGalleryPageTitle) %>
    <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Core", "Gallery", "fabric"); %>
    <% if(Html.IsReCaptchaEnabledForUpdatePublisherProfile() || Html.IsReCaptchaEnabledForCreateVisualStudioCodeExtension() || Html.IsReCaptchaEnabledForUpdateVisualStudioCodeExtension()) { %>
        <script <%= Html.GenerateNonce(true) %>>
            function onSubmit(reCaptchaToken) {
                var tokenElement = document.getElementById("tokenId")
                tokenElement.value = reCaptchaToken;
                tokenElement.click();
                grecaptcha.reset();
            }

            <% if(Html.IsReCaptchaEnabledForUpdatePublisherProfile()) { %>
                function validate(event) {
                    grecaptcha.execute();
                }
            <% } %>

            <% if(Html.IsReCaptchaEnabledForCreateVisualStudioCodeExtension() || Html.IsReCaptchaEnabledForUpdateVisualStudioCodeExtension()) { %>
                window.validate = function () {
                    grecaptcha.execute();
                }
            <% } %>
        </script>
 <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ShowFooter(false);
        Html.AddBodyClass("publisher-page"); 
        Html.UseScriptModules("Gallery/Client/Pages/PublisherRevamp/PublisherPage.View"); 
        Html.UseCSS("jQueryUI-Modified");
        Html.UseCSS("VSS.Features.Controls");
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePublisherStatVSCode);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePublisherProfilePage);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableVsForMac);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableCertifiedPublisherUIChanges);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableByolForMarketplace);
        Html.IncludeFeatureFlagState(GalleryWebConstants.WebFeatureFlags.UseNewDomainUrlInShareDropdown);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.UseIdentityDescriptorsToReadIdentities);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.MarkPublishersVerifiedByDefault);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableUrlsInPublisherProfile);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableSupportRequestFeature);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaForUpdatePublisherProfile);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaForCreateVisualStudioCodeExtension);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaForUpdateVisualStudioCodeExtension);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableVSExtensionCreation);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableVsCodeExtensionCreation); 
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisablePublisherCreation);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePlatformSpecificExtensionsUIForManagePages);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableVSConsolidationUIForManagePages);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePublisherDomainFieldUI);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.PreventDeletingVsCodeAndVsIdeExtensionsFromUI);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableUnVerifyDomainOnDisplayNameChange);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.View", "Gallery/Client/Common/Telemetry", "Gallery/Client/Controls/ErrorControl/ErrorControl.View"
             , "Gallery/Client/Service/VSSGallery/VSSGallery", "Gallery/Client/Service/VSGallery/VSGallery", "VSS/FeatureAvailability/RestClient", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.ScriptModules("VSS/Error") %>
    <% if(Html.IsReCaptchaEnabledForUpdatePublisherProfile()) { %>
        <script  <%= Html.GenerateNonce(true) %>>
        window.onload = function()
            {
                document.getElementsByClassName("ms-CommandBar vss-PivotBar--commandBar")[0].addEventListener("click", function(e) {
                    if(e && e.target && (e.target.classList.contains("bowtie-save")
                   || (e.target.childNodes[0] && e.target.childNodes[0].disabled!=true && e.target.childNodes[0].data == "Save")))
                {
                    validate(e);
                }
            })
        }
        </script>
<% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.RestApiJsonIsland(this.ViewData["GalleryPublisherViewData"], new { @class = "publisher-data" } ) %>
    <%: Html.RestApiJsonIsland(this.ViewData["ReCaptchaPublicKey"], new { @class = "reCaptchaPublicKey" } ) %>
    <%: Html.RestApiJsonIsland(Html.SignoutUrl( System.Web.HttpUtility.UrlEncode(Html.SigninUrl())), new { @class =  ViewDataConstants.PublisherClickHereUrl  } ) %>
    <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.TargetPlatforms], new { @class = ViewDataConstants.TargetPlatforms } ) %>
    <% if (!WebContext.IsHosted && this.ViewData[ViewDataConstants.ValidCategories] != null) { %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.ValidCategories], new { @class = ViewDataConstants.ValidCategories } ) %>
    <% } %>
    <% if (WebContext.IsHosted && this.ViewData[ViewDataConstants.ReservedPublisherDisplayNames] != null) { %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.ReservedPublisherDisplayNames], new { @class = ViewDataConstants.ReservedPublisherDisplayNames } ) %>
    <% } %>
    
    <% if (WebContext.IsHosted && !String.IsNullOrEmpty((string)this.ViewData[ViewDataConstants.GalleryBrowseUrl])) { %>
        <div class="market-url">
            <%: Html.JsonIsland(this.ViewData[ViewDataConstants.GalleryBrowseUrl], new { @class = ViewDataConstants.GalleryBrowseUrl }) %>
        </div>
    <% } %>
    <% if (WebContext.IsHosted && !String.IsNullOrEmpty((string)this.ViewData[ViewDataConstants.UserMailAddress])) { %>
        <div class="user-mail-data">
            <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.UserMailAddress], new {@class = ViewDataConstants.UserMailAddress}) %>
        </div>
        <div class="delete-prevent-min-acquisition-count-data">
                <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.DeletePreventMinAcquisitionCount], new {@class = ViewDataConstants.DeletePreventMinAcquisitionCount}) %>
            </div>
        <div class="delete-prevent-min-days-count-data">
                <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.DeletePreventMinDaysCount], new {@class = ViewDataConstants.DeletePreventMinDaysCount}) %>
            </div>
    <% } %>
    <div id="PublisherPageContainer" role="main"></div>
</asp:Content>

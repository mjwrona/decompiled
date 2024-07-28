<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <%: string.Format("{0} | {1}", "Publish VS Extension", GalleryResources.PageTitle) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.ShowFooter(false); %>
    <% Html.AddBodyClass("vsextensionpublish-page"); %>
    <% Html.UseScriptModules("Gallery/Client/Pages/VSExtensionPublish/VSExtensionPublish.View"); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePreviewSupportForVS); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableLinkTypeExtensions); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableLinkTypeExtensionUpdate); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaForCreateVisualStudioExtension); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableVSExtensionCreation); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableVsCodeExtensionCreation); %>
    <% Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaForEditVisualStudioExtension); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="robots" content="noindex,nofollow" />
    <% Html.UseCommonCSS("Gallery3rdParty", "Gallery", "fabric"); %>
    <% if(Html.IsReCaptchaEnabledForCreateVisualStudioExtension() || Html.IsReCaptchaEnabledForEditVisualStudioExtension()) { %>
        <script <%= Html.GenerateNonce(true) %>>
            function onSubmit(token) {
                var tokenElement = document.getElementById("tokenId")

                if(!token){
                    token = grecaptcha.getResponse();
                }

                tokenElement.value = token;
                tokenElement.click();
            }

            window.resetRecaptcha = function () {
                grecaptcha.reset();
            }
        </script>
    <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.View", "Gallery/Client/Common/Telemetry", "Gallery/Client/Controls/ErrorControl/ErrorControl.View"
           , "Gallery/Client/Service/VSSGallery/VSSGallery", "Gallery/Client/Service/VSGallery/VSGallery", "VSS/FeatureAvailability/RestClient", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.ScriptModules("VSS/Error") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.RestApiJsonIsland(this.ViewData["ReCaptchaPublicKey"], new { @class = "reCaptchaPublicKey" })%>
    <div id="VSExtensionPublishContainer" role="main"></div>
    <div class="publisher-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.Publisher], new {@class = ViewDataConstants.Publisher}) %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.IsMSPublisher], new {@class = ViewDataConstants.IsMSPublisher}) %>
    </div>
    <div class="categories-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.ValidCategories], new {@class = ViewDataConstants.ValidCategories}) %>
    </div>
    <div class="file-size-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.MaxPackageSize], new {@class = ViewDataConstants.MaxPackageSize}) %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.MaxAssetSize], new {@class = ViewDataConstants.MaxAssetSize}) %>
    </div>
    <div class="extension-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.Extension], new {@class = ViewDataConstants.Extension}) %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.ExtensionMetadata], new {@class = ViewDataConstants.ExtensionMetadata}) %>
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSItemProperties], new {@class = ViewDataConstants.VSSItemProperties}) %>
    </div>
    <div class="scenario-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSExtensionPublishScenario], new {@class = ViewDataConstants.VSExtensionPublishScenario}) %>
    </div>
</asp:Content>
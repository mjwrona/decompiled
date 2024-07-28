<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <%: string.Format("{0} | {1}", GalleryResources.CreatePublisher_Page_Title, GalleryResources.PageTitle) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Core", "Gallery", "fabric"); %>
    <% if(Html.IsReCaptchaEnabledForCreatePublisherProfile()) { %>
            <script <%= Html.GenerateNonce(true) %>>
                function onSubmit(reCaptchaToken) {
                    var tokenElement = document.getElementById("tokenId")
                    tokenElement.value = reCaptchaToken;
                    tokenElement.click();
                    grecaptcha.reset();
                }

                function validate(event) {
                    grecaptcha.execute();
                }
            </script>
     <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ShowFooter(false);
        Html.AddBodyClass("publisher-welcome-page"); 
        Html.UseCSS("jQueryUI-Modified");
        Html.UseCSS("VSS.Features.Controls");
        Html.UseScriptModules("Gallery/Client/Pages/PublisherCreate/PublisherCreate.View");
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.MarkPublishersVerifiedByDefault);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisableUrlsInPublisherProfile);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaForCreatePublisherProfile);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.DisablePublisherCreation);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnablePublisherDomainFieldUI);
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableUnVerifyDomainOnDisplayNameChange);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.Minimal.View", "Gallery/Client/Common/Telemetry", "Gallery/Client/Controls/ErrorControl/ErrorControl.View"
             , "Gallery/Client/Service/VSSGallery/VSSGallery", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.ScriptModules("VSS/Error") %>
    <% if(Html.IsReCaptchaEnabledForCreatePublisherProfile()) { %>
            <script  <%= Html.GenerateNonce(true) %>>
            window.onload = function()
            { 
                document.getElementById("submitbutton").addEventListener("click", function(e) {
                                    validate(e);
                                    });
            }
            </script>
    <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.RestApiJsonIsland(Html.SignoutUrl( System.Web.HttpUtility.UrlEncode(Html.SigninUrl())), new { @class =  ViewDataConstants.PublisherClickHereUrl  } ) %>
    <%: Html.RestApiJsonIsland(this.ViewData["ReCaptchaPublicKey"], new { @class = "reCaptchaPublicKey" })%>
    <div id="PublisherCreateContainer" role="main"></div>
    <div class="directory-info">
        <%: Html.RestApiJsonIsland(this.ViewData["directoryInfo"], new { @class = "directory-info-data" })%>
    </div>
</asp:Content>

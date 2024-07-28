<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
  <% 
    Html.AddBodyClass("customer-support-page");
    Html.UseScriptModules("Gallery/Client/Pages/CustomerSupportRequest/CustomerSupportRequest.View");
    Html.UseCSS("jQueryUI-Modified");
    Html.UseCSS("VSS.Features.Controls");
    Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableReCaptchaInCreateCSR);
  %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Core", "Gallery", "fabric"); %>
    <% if(Html.IsReCaptchaEnabledInCreateCSR()) { %>
        <script <%= Html.GenerateNonce(true) %>>
            function onSubmit(token) {
                var tokenElement = document.getElementById("tokenId")
                tokenElement.value = token;
                tokenElement.click();
                grecaptcha.reset();
            }

            function validate(event) {
                grecaptcha.execute();
            }
        </script>
     <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <% Html.UseCommonScriptModules("Gallery/Client/Pages/Common/Base.View", "Gallery/Client/Common/Telemetry", "Gallery/Client/Controls/ErrorControl/ErrorControl.View"
           , "Gallery/Client/Service/VSSGallery/VSSGallery", "Gallery/Client/Service/VSGallery/VSGallery", "VSS/FeatureAvailability/RestClient", "VSS/Error"); %>
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.ScriptModules("VSS/Error") %>
    <% if(Html.IsReCaptchaEnabledInCreateCSR()) { %>
        <script  <%= Html.GenerateNonce(true) %>>
          window.onload = function()
          { 
              document.getElementById("submitButton").addEventListener("click", function(e) {
                  validate(e);
              });
          }
        </script>
    <% } %>     
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <div id="CustomerSupportContainer"  role="main" sourceLink="<%=this.ViewData["sourceLink"] %>" <%= Html.GenerateNonce(true) %>></div>
    <%: Html.RestApiJsonIsland(this.ViewData["requestData"], new { @class = "support-request-args" } ) %>
    <%: Html.RestApiJsonIsland(this.ViewData["reCaptchaPublicKey"], new { @class = "reCaptchaPublicKey" } ) %>
  </div>
</asp:Content>
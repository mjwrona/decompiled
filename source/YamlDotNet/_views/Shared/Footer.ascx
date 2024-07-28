<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.WebApi" %>

<footer>
    <%
    if (Html.ShowDetailedFooter())
    {
    %>
    <div id="ux-footer" class="ltr detailed" role="contentinfo">
    <% } 
    else 
    {
    %>
    <div id="ux-footer" class="ltr" role="contentinfo">
    <% } %>
        <%if (Html.ShowDetailedFooter()) { %>
            <div data-bind="component: {name:'gallery-footer', params: { currentTab: currentTab }}"></div>
        <% } %>
        <div id="baseFooter">
            <div data-fragmentname="BaseFooterLinks" id="Fragment_BaseFooterLinks" xmlns="http://www.w3.org/1999/xhtml">

                <div class="linkList">
                    <ul class="links horizontal">
                        <!-- EU Cookie Consent: Do not consider user consent on clicking any of the links, by setting data-mscc-ic attribute to false -->
                        <li>
                            <%if (Html.ShowSupportRequestForm()) { %>
                                <a rel="noreferrer noopener nofollow" href="/support/ContactUs" data-mscc-ic="false"><%: GalleryResources.Contact_Us %></a>                            
                            <% } else {%>
                                <a href="https://www.visualstudio.com/support/support-overview-vs" data-mscc-ic="false"><%: GalleryResources.Contact_Us %></a>
                            <% } %>                            
                        </li>
                        <li>
                            <a href="https://careers.microsoft.com/" data-mscc-ic="false"><%: GalleryResources.Jobs %></a>
                        </li>
                        <li>
                            <a href="https://go.microsoft.com/fwlink/?LinkID=521839" data-mscc-ic="false"><%: GalleryResources.Privacy %></a>
                        </li>
                        <% if (this.ViewData.ContainsKey(GalleryCookieConsentConstants.ConsentEnabled) &&
                               (bool)(this.ViewData[GalleryCookieConsentConstants.ConsentEnabled])
                              ) %>
                        <% { %>
                            <li>
                                <a id="mgtConsentCookie" data-mscc-ic="false"><%: GalleryResources.Manage_cookies %></a>
                            </li>
                        <% } %>
                        <li>
                            <a href="https://aka.ms/vsmarketplace-ToU" data-mscc-ic="false"><%: GalleryResources.Terms_Of_Use %></a>
                        </li>
                        <li>
                            <a href="https://www.microsoft.com/en-us/legal/intellectualproperty/Trademarks/EN-US.aspx" data-mscc-ic="false"><%: GalleryResources.Trademarks %></a>
                        </li>
                    </ul>
                </div>
            </div>
            <div id="rightBaseFooter">
                &copy; <%: DateTime.Now.Year %> <%: GalleryResources.Microsoft_Text %>
                <span class="microsoftLogo" title="Microsoft"></span>
            </div>
            <div class="clear"></div>
        </div>
    </div>
</footer>
<% if (this.ViewData.ContainsKey(GalleryCookieConsentConstants.ConsentEnabled) &&
       (bool)(this.ViewData[GalleryCookieConsentConstants.ConsentEnabled])
      ) %>
<% { %>
    <script type="text/javascript" src="https://wcpstatic.microsoft.com/mscc/lib/v2/wcp-consent.js"></script>
    <% if (this.ViewData.ContainsKey(GalleryCookieConsentConstants.ConsentMarkupJavaScript) &&
           !string.IsNullOrWhiteSpace((string)(this.ViewData[GalleryCookieConsentConstants.ConsentMarkupJavaScript]))
          )
       { %>
        <script type="text/javascript" src="<%: this.ViewData[GalleryCookieConsentConstants.ConsentMarkupJavaScript] %>"></script>
        <script type="text/javascript" <%= Html.GenerateNonce(true) %>>
            (function () {
                document.getElementById("mgtConsentCookie").addEventListener("click", function (el) {
                    manageConsent();
                });
            })();
        </script>
    <% } %>
<% } %>
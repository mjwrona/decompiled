<%@ Page Async="true" Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Server" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="msvalidate.01" content="21D03D894BABE1ED475954E142FF8FD2" />
    <% Html.UseCommonCSS("Gallery3rdParty", "GalleryMinimal"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <%: Html.GetPageTitle(GalleryPages.HomePage, (PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MetaTagPlaceholder" runat="server">
    <%: Html.RenderPageMetadata(GalleryPages.HomePage, (PageMetadataInputs)this.ViewData[ViewDataConstants.PageMetaData]) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.ShowDetailedFooter(true); 
        Html.UseScriptModules("Gallery/Client/Pages/HomePageVNext/HomePageVNext.View");
        Html.IncludeFeatureFlagState(GalleryServiceFeatureFlags.EnableVerifiedPublisherDomain);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyBegin" runat="server">
    <%: Html.PageInitForModuleLoaderConfig() %>
    <%: Html.ScriptModules() %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="tab-data">
        <% foreach (var item in (this.ViewData["TabData"] as Dictionary<string, object>)) { %>
              <%: Html.RestApiJsonIsland(item.Value, new { @class = item.Key.ToLowerInvariant() + "-tab-data" })%>
        <% } %>
    </div>
    <div class="user-mail-data">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.UserMailAddress], new {@class = ViewDataConstants.UserMailAddress}) %>
    </div>
    <div class="marketplace-url">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.MarketplaceBrowseUrl], new {@class = ViewDataConstants.MarketplaceBrowseUrl}) %>
    </div>
    <% Response.WriteFile("~/_views/Shared/Image.html"); %>

    <div class="content-section-market gallerynew">
        <div class="gallery-home-page-view">
            <div class="gallery-home-page">
                <% Response.WriteFile("~/_views/Shared/VNext-TabAreaContainer.html"); %>
            </div>
        </div>
    </div>
</asp:Content>

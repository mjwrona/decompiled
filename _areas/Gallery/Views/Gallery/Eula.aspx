<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web.Utility" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    <% if (WebContext.IsHosted) { %>
        <%: string.Format("{0} | {1}", GalleryResources.EulaTitle, GalleryResources.PageTitle) %>
    <% } else { %>
        <%: string.Format("{0} | {1}", GalleryResources.EulaTitle, GalleryResources.OnPremGalleryPageTitle) %>
    <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.UseScriptModules("Gallery/Client/Pages/Eula/Eula.View"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.UseCommonCSS("Gallery3rdParty", "Gallery"); %>
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
    </div>
    <div class="vss-item-token">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSExtensionToken], new {@class = ViewDataConstants.VSSExtensionToken}) %>
    </div>
    <div class="vss-item-license-container">
        <%: Html.RestApiJsonIsland(this.ViewData[ViewDataConstants.VSSItemLicense], new {@class = ViewDataConstants.VSSItemLicense}) %>
    </div>
    <% Response.WriteFile("~/_views/Shared/BreadCrumb.html"); %>
    <div class="main-content eula-page">
        <div class="breadcrumb-container">
            <div class="breadcrumb gallery-centered-content"></div>
        </div>
        <div class="license-container">
            <div class="gallery-centered-content">
                <div class="markdown license-text" ></div>
            </div>
        </div>
    </div>
</asp:Content>
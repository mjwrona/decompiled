<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Gallery.Master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Gallery.Web" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.UseScriptModules("Gallery/Client/Pages/Search/Search.View"); %>
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
    <%-- TODO: Check if there is a better approach than WriteFile, and if we really need html, rather than ascx :
         , RenderPartial doesn't include .html files properly.
    --%>
    <% Response.WriteFile("~/_views/Shared/Image.html"); %>
    <% Response.WriteFile("~/_views/Shared/ItemTile.html"); %>
    <% Response.WriteFile("~/_views/Shared/ItemBanner.html"); %>
    <% Response.WriteFile("~/_views/Shared/ItemGrid.html"); %>
    <% Response.WriteFile("~/_views/Shared/SearchMenuBar.html"); %>
    <% Response.WriteFile("~/_views/Shared/SearchControl.html"); %>
    <% Response.WriteFile("~/_views/Shared/PaginationControl.html"); %>
    <% Response.WriteFile("~/_views/Shared/NewImage.html"); %>

    <div class="content-section-market gallerynew">
        <div class="hub-view gallery-home-page-view">
            <div class="gallery-home-page">
                <div class="search-control-container-wrapper">
                </div>
            </div>
        </div>
    </div>   
</asp:Content>

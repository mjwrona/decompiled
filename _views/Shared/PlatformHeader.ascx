<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>

<div id="header-row" class="nav-separated" role="banner">
    <a alt="Visual Studio" class="header-item logo header-logo-vsonline" href="<%: Url.ForwardLink(245131) %>" target="_blank"></a>

    <div class="right-side">
        <%: Html.RenderUserMenu() %>
    </div>
</div>

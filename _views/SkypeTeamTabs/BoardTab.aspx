<%@ Page Title="" Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="System.Web.Optimization" %>

<%
    WebContext webContext = this.TfsWebContext;
    Html.UseCommonCSS("Core", "Site");
    Html.UseScriptModules("Presentation/Scripts/TFS/SkypeTeamTabs/SkypeTeamConfigMain");
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="<%: System.Globalization.CultureInfo.CurrentUICulture.Name %>" xmlns="http://www.w3.org/1999/xhtml" dir="ltr">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=11" />
    <meta name="msapplication-config" content="none" />
    <%:Html.RenderCSSBundles() %>
</head>
<body class="tab-main">
    <%:Html.TfsAntiForgeryToken() %>
    <%:Html.PageInit() %>
    <%:Html.InjectDataProviderData() %>

    <%-- For OfficeFabric icons --%>
    <% Html.UseCommonScriptModules("VSS/Fonts/IconFonts"); %>

    <div class="tab-content">
        <div class="skype-team-tab-container"></div>
    </div>
    <%: Html.ScriptModulesWithCallback("") %>
</body>
</html>

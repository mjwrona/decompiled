<%@ Page Title="" Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Presentation.AuthenticationRedirectViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="System.Web.Optimization" %>

<%
    Html.UseScriptModules("Presentation/Scripts/TFS/IntegrationRedirect/TFS.IntegrationRedirect");
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="<%: System.Globalization.CultureInfo.CurrentUICulture.Name %>" xmlns="http://www.w3.org/1999/xhtml" dir="ltr">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=11" />
    <meta name="msapplication-config" content="none" />
    <%: Html.RenderThemedCssFile("IntegrationRedirect/MicrosoftTeams.css") %>
</head>
<body>
    <%: Html.PageInit() %>
    <%: Html.DataContractJsonIsland(Model, new { @id = "redirect-options" }) %>
    <div id="main-content"></div>
    <%: Html.ScriptModules() %>
</body>
</html>
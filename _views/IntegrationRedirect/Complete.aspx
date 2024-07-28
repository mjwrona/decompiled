<%@ Page Title="" Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Presentation.AuthenticationRedirectCompleteViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="System.Web.Optimization" %>

<%
    Html.UseScriptModules("Presentation/Scripts/TFS/IntegrationRedirect/TFS.IntegrationRedirect");
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head>
    <title><%:PresentationResources.WorkingTitle %></title>
    <meta http-equiv="X-UA-Compatible" content="IE=11; IE=10; IE=9; IE=8 width=device-width,initial-scale=1" />
</head>
<body>
    <p><%:PresentationResources.CompleteSignInToVSTS %></p>
    <%: Html.PageInit() %>
    <%: Html.DataContractJsonIsland(Model, new { @id = "complete-redirect" }) %>
    <div id="main-content"></div>
    <%: Html.ScriptModules() %>
</body>
</html>
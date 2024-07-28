<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<% Html.ContentTitle(WACommonResources.SignOut); %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%: Html.HtmlPageTitle() %></title>
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <link href="<%:Url.Themed("VSS.Icons.css") %>" type="text/css" rel="stylesheet" />
    <link href="<%:Url.Themed("Core.css") %>" type="text/css" rel="stylesheet" />
    <link href="<%:Url.Themed("Site.css") %>" type="text/css" rel="stylesheet" />
    <link href="<%:Url.Themed("jQueryUI-Modified.css") %>" type="text/css" rel="stylesheet" />
</head>
<body class="signout">
    <% LogoutMode mode = (LogoutMode)ViewData["LogOutMode"]; %>
    <% if (mode == LogoutMode.SignOut) { %>
    	<% if ((bool)ViewData["SignOutPostRedirectEnabled"]) { %>
            <form method="POST" name="hiddenform" action="<%: (string)ViewData["LocationForRealm"] %>" id="form">
                <input type="hidden" name="redirectUrl" value="<%: (string)ViewData["RedirectUrl"] %>" />
                <input type="hidden" name="forceSignout" value="<%: (bool)ViewData["ForceSignout"] %>" />
                <input type="hidden" name="mode" value="<%: (string)ViewData["Mode"] %>" />
                <input type="hidden" name="id_token" value="<%: (string)ViewData["Id_token"] %>" />
            </form>
            <noscript>
                <p>Scripting is disabled. Click Submit to continue.</p>
                <input type="submit" value="Submit" />
            </noscript>
		    <script language="javascript" type="text/javascript" nonce="<%: (string)ViewData["Nonce"]  %>">
                window.setTimeout(document.getElementById("form").submit(), 0);
            </script>

         <% } else { %>

        <div class="signout-actions">
            <ul>            
                <li><a id="A1" class="linkAction" tabIndex="0" href="<%: (string)ViewData["HomeUrl"] %>"><%: WACommonResources.Home %></a></li>
                <li><a id="A2" class="linkAction accessible" tabIndex="0" href="<%: (string)ViewData["SignInAsDifferentUserUrl"] %>"><%: WACommonResources.SignInAsDifferentUser %></a></li>
            </ul>
        </div>
        <div class="err">
            <%: WACommonResources.CloseBrowserToCompleteSignoutWarning %>
        </div>
        <div>
            <%: WACommonResources.CloseBrowserToCompleteSignoutMessage %>
        </div>
        <script language="javascript" type="text/javascript">
            window.onload = function () {
                try {
                    document.execCommand("ClearAuthenticationCache");
                }
                catch (e) { }

                window.close();
            }
        </script>
            <% } %>
    <% } else { %>
        <div class="err">
            <%: WACommonResources.SignInAsDifferentUser %>
        </div>
        <div style="display:none">
            <img src="?mode=CloseConnection&attempt=1" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=2" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=3" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=4" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=5" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=6" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=7" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=8" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=9" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=10" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=11" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=12" alt="" width="0" height="0" border="0" />
            <img src="?mode=CloseConnection&attempt=13" alt="" width="0" height="0" border="0" />
        </div>
        <script language="javascript" type="text/javascript">
            window.onload = function () {
                var _url = window.location.href;
                _url = _url.replace(/mode=SignInAsDifferentUser/i, "mode=ChangeUser");
                window.location = _url;
            }
        </script>
    <% } %>
</body>
</html>

<%@ Page Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>

<%
    Html.ViewContext.HttpContext.UseNewPlatformHost(true);
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>    
        <%: TestManagementResources.TestRunner %>              
    </title>
    <meta http-equiv="X-UA-Compatible" content="IE=11; IE=10; IE=9; IE=8" />
    <%:Html.NewPlatformHeadContent() %>

    <link rel="SHORTCUT ICON" href="<%:StaticResources.Content.GetLocation("icons/favicon.ico") %>"/>

    <% Html.UseCommonCSS("jQueryUI-Modified", "Core", "Splitter", "PivotView", "Site", "Areas"); %>
    <% Html.UseCSS("TestManagement"); %>
    <% Html.AddBodyClass("web-test-runner"); %>
    <%:Html.RenderCSSBundles() %>

    <style>
        @-ms-viewport {
            width:device-width;
        }
    </style>
</head>

<body class="<%:TfsWebContext.NavigationContext.Area %> <%: Html.BodyClasses() %>">
    <%  Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTCMEnableXtForEdge); %>
    <%  Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTestManagementEnableDesktopScreenShot); %>
    <%  Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTestManagementEnableAudioWithVideo); %>
    <%:Html.FeatureLicenseInfo() %>
    <%:Html.TfsAntiForgeryToken() %>
    <%:Html.Bootstrap() %>
    <%:Html.BuiltinPlugins() %>
    <%:Html.PageInit() %>
    <%:Html.IsAdvancedExtensionEnabled(TfsWebContext) %>
    <%: Html.ScriptModules("Presentation/Scripts/TFS/TFS.Host.UI", "TestManagement/Scripts/TFS.TestManagement.TestRun") %>
             <div class="test-run-view">
                 <div class="test-run">
                     <div class="testRun-SaveClose-toolbar">
                     </div>
                      <div class="runner-message-holder">
                     </div>
                     <div class="testRun-video-recoder testRun-recording-toolbar">
                     </div>
                     <div class="testRun-action-recoder testRun-recording-toolbar">
                     </div>
                     <div class="testRun-screenshot-window-list">
                     </div>
                     <div class="testRun-actionLog-window-list">
                     </div>
                     <div class="test-case-navigator">
                     </div>
                     <div class="testRunTitle">
                          <div class="title"></div>
                          <div class="hub-progress pageProgressIndicator"></div>
                          <div class="test-run-options">                  
                            <div class="testcase-description-toggle-btn" role = "button" tabindex ="0"> </div> 
                            <span class="testcase-title-separator-icon"> </span>          
                            <div class="test-run-mark-status-menu-items"></div> 
                          </div>                      
                     </div>
                     <div class="test-result-comment-container">
                     </div>
                     <div class="test-case-description-container" style="display: none;">
                         <div class="testcase-description-label"> </div>
                         <div class="testcase-description-text-area"> </div>
                     </div>
                     <div class="test-run-steps-list">
                     </div>
                     <div class="test-run-footer">
                     </div>
                 </div>
             </div>
          
    <%: Html.ScriptModules() %>
</body>
</html>

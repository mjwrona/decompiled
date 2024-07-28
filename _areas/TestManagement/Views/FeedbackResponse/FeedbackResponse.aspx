<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        Html.UseScriptModules("TestManagement/Scripts/TFS.TestManagement.Feedback"); 
        Html.UseAreaCSS("TestManagement");
        Html.IncludeFeatureFlagState(FeatureAvailabilityFlags.WebAccessTCMEnableXtForEdge);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubBegin" runat="server">
    <%:Html.FeatureLicenseInfo() %>
    <%:Html.TfsAntiForgeryToken() %>
    <%:Html.Bootstrap() %>
    <%:Html.BuiltinPlugins() %>
    <%:Html.IsAdvancedExtensionEnabled(TfsWebContext) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="feedback-view"></div>
</asp:Content>

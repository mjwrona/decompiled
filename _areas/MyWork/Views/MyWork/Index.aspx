<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<MyWorkViewModel>" %>

<%@ Import Namespace="System.Security.Policy" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.MyWork.ViewModels" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" media="screen" href="<%: Url.Themed("MyWork.css") %>" />
</asp:Content>

<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <%
        Html.IncludeFeatureFlagState("WebAccess.MyWork.MyCodeWidget");
        Html.IncludeFeatureFlagState("WebAccess.MyWork.BuildWidget");
        Html.AddHubViewClass("my-work-view");
    %>
</asp:content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBegin" runat="server">
    <%
        Html.UseScriptModules("MyWork/Scripts/TFS.MyWork.Controls.MyWorkView");
    %>
</asp:Content>

<asp:content contentplaceholderid="HubContent" runat="server">
    <div class="my-work-content-area" style="height:100%;" />
    <div id="container-without-scroll">
		<div id="curtain"></div>
		<div id="container-with-scroll">           
            
        </div>
    </div>
    <nav id="blade-menu"></nav>
</asp:Content>

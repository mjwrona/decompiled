<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server"%>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.UseScriptModules("TestManagement/Scripts/TestTabExtension/TestResults.ResultDetails"); %>
    <% Html.UseAreaCSS("TestManagement"); %>
    <% Html.AddHubViewClass("result-details-view"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubTitle" runat="server">
    <%: Html.HubTitle("Test Results") %>
</asp:Content>


<asp:content contentplaceholderid="HubContent" runat="server">
    <div class="test-results-grid"></div>
</asp:content>

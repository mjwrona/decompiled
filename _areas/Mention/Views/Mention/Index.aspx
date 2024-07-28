<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorer.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<object>" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" media="screen" href="<%: Url.Themed("Mention.css") %>" />
</asp:Content>

<asp:content contentplaceholderid="DocumentBegin" runat="server">
    <%
        Html.UseScriptModules("Mention/Scripts/TFS.Mention");
        Html.AddHubViewClass("mention-view");
    %>
</asp:content>

<asp:content contentplaceholderid="HubBegin" runat="server">
</asp:content>

<asp:content contentplaceholderid="LeftHubContent" runat="server">
    <div class="mention-view-selector-area">
    </div>
</asp:Content>

<asp:content contentplaceholderid="RightHubContent" runat="server">
    <div class="mention-view-content-area" style="height:100%;" >
    </div>
</asp:Content>


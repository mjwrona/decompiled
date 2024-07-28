<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master" Inherits="TfsViewPage<TestManagementErrorModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.ContentTitle(TestManagementServerResources.SharedParametersErrorTitle); %>
    <% Html.UseAreaCSS("TestManagement"); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HubPivotContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HubContent" runat="server">
    <div class="agile-error-page hub-no-content-gutter">
        <p>
            <span class="error-header"><%: Model.ErrorText %></span>
        </p>
        <% if(Model.LinkTarget != null) { %>
            <p>
                <a class="error-link" href='<%:Model.LinkTarget %>' target="_blank" rel="nofollow noopener noreferrer"><%: Model.LinkText %></a>
            </p>
        <% } %>

    </div>

</asp:Content>

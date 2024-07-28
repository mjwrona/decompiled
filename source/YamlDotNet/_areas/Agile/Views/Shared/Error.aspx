<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPagePivot.master" Inherits="TfsViewPage<SettingsErrorModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        Html.ContentTitle((string)ViewData[ViewDataConstants.ViewTitle]);
        Html.IncludeFeatureFlagState(WebAccessAgileFeatureFlags.FeatureFlagNames);
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotContent" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="agile-error-page hub-no-content-gutter" style="padding-left: 20px">
        <p>
            <span class="error-header" style="color:#C00000;font-size:16px"><%: Model.ErrorText %></span>
        </p>
        <% if(Model.LinkTarget != null) { %>
            <p>
                <a class="error-link" href='<%:Model.LinkTarget %>' target="_blank"><%: Model.LinkText %></a>
            </p>
        <% } %>
        <% if(Model.SecondaryLinkTarget != null) { %>
            <p>
                <a class="error-link" href='<%:Model.SecondaryLinkTarget %>' target="_blank"><%: Model.SecondaryLinkText %></a>
            </p>
        <% } %>

        <% if (Model.ExceptionMessages.Any()) { %>
            <p><%: AgileResources.AgileSettings_DetailsOfErrorMessages %></p>
            <ul>
            <% foreach (string message in Model.ExceptionMessages) { %>
                <li><%: message %></li>
            <% } %>
            </ul>
        <% } %>
    </div>

</asp:Content>

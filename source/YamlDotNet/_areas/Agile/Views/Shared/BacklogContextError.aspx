<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPage.master" Inherits="TfsViewPage<BacklogContextErrorModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Models" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
     <%
         Html.ContentTitle(String.Format(AgileProductBacklogServerResources.BacklogContextError_Title, Model.InvalidPluralName));
         Html.IncludeFeatureFlagState(WebAccessAgileFeatureFlags.FeatureFlagNames);
     %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubContent" runat="server">
    <div class="agile-error-page backlog-context-error hub-no-content-gutter" style="padding-left: 20px">
        <p>
            <%= AgileProductBacklogServerResources.BacklogContextError_Message1 %>
        </p>
        <p>
            <%= AgileProductBacklogServerResources.BacklogContextError_Message2 %>
        </p>
        <p>
            <%= AgileProductBacklogServerResources.BacklogContextError_Message3 %>
            <br />
            <%= AgileProductBacklogServerResources.BacklogContextError_Message4 %>
        </p>

        <ul style="list-style-type: none">
            <% foreach (string hubLink in Model.HubLinks) { %>
                <li style="padding: 2px 0;"><%= hubLink %></li>
            <% } %>
        </ul>
    </div>

</asp:Content>

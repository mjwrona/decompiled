<%@ Page Language="C#" MasterPageFile="~/_views/Shared/Account.master" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.EmailConfirmationModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<asp:Content ID="DocumentBegin" ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.PageTitle(AdminServerResources.EmailConfirmationHeader); %>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="email-confirmation-header"><%: AdminServerResources.EmailConfirmationHeader %></div>

<% if (String.IsNullOrEmpty(Model.ErrorMessage)) { %>
        <div class="email-confirmation-subheader"><%: AdminServerResources.EmailConfirmationSubHeader %></div>
        <div class="email-confirmation-address"><%: Model.EmailAddress %></div>
<% } else { %>
        <div class="email-confirmation-subheader"><%: AdminServerResources.EmailConfirmationError %></div>
        <div class="email-confirmation-error"><%: Model.ErrorMessage %></div>
<% } %>

</asp:Content>
<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.NoContentGutterModel>" %>

<div class="hub-no-content-gutter gutter-banner">
    <div class="gutter-banner-container">
        <div class="gutter-banner-text">
            <h2><%: Model.Heading %></h2>
            <p><%= Model.Message %></p>
            <ul class="gutter-banner-actions">
                <% foreach (var action in Model.Actions) { %>
                    <li><a class="gutter-banner-action" href="<%= action.Href %>"><%: action.Text %></a></li>
                <% } %>
            </ul>
        </div>

        <div class="gutter-banner-icon">
            <div class="image image-vso-cloud"></div>
        </div>
    </div>
</div>

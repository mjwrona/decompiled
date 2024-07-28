<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Account.PublicKeyEditModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Account" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<div class="key-form-wrapper">
    <form class="key-form" role="form" aria-labelledby="ssh-headertitle">
        <%:Html.TfsAntiForgeryToken() %>

        <!-- Id -->
        <input type="hidden" name="key-id" class="key-id" value="<%: Model.PublicKey != null ? Model.PublicKey.AuthorizationId.ToString() : "" %>" />
        <input type="hidden" name="key-is-valid" class="key-is-valid" value="<%:Model.PublicKey != null ? Model.PublicKey.IsValid.ToString() : "true" %>" />

        <!-- Description -->
        <div class="description-wrapper section-box">
            <label class="description-label account-form-label" id="ssh-description-id">
                <!-- fix the width of all the labels -->
                <%: AccountServerResources.TokenDescriptionLabel %>
            </label>
            <input aria-labelledby="ssh-description-id" class="input-field input-visible description" name="description" type="text" value="<%: Model.PublicKey != null ? Model.PublicKey.Description : "" %>" maxlength="256" <% if (Model.PublicKey != null)
                { %> disabled="disabled" <% } %> autofocus/>
            <label class="error description-error"></label>
        </div>

        <!-- Public Key Data -->
        <div class="data-wrapper section-box">
            <label class="description-label account-form-label" id="ssh-keydata-id">
                <%: AccountServerResources.SSH_DataLabel %>
            </label>
            <% if (Model.PublicKey != null && Model.PublicKey.Data != "") { %>
            <textarea aria-labelledby="ssh-keydata-id" rows="7" cols="20" class="input-field input-visible data" readonly><%: Model.PublicKey.Data %></textarea>
            <% } else { %>
            <textarea aria-labelledby="ssh-keydata-id" rows="7" cols="20" class="input-field input-visible data key-data"></textarea>
            <% } %>
            <label class="error description-error"></label>
        </div>

        <!-- Generate/Cancel Token Button -->
        <div class="button-container">
            <div class="button-field">
                <% if (Model.PublicKey == null) { %>
                    <button type="submit" class="key-save-button formInput" disabled="disabled"><%: AccountServerResources.DialogSave %></button>
                <% } %>
                <button type="button" class="key-cancel-button formInput"><%: AccountServerResources.TokenCancelButton %></button>
                <img class="wait" alt="<%: AccountServerResources.PleaseWait %>" src="<%= StaticResources.Versioned.Content.GetLocation("big-progress.gif") %>">
            </div>
        </div>

    </form>
</div>

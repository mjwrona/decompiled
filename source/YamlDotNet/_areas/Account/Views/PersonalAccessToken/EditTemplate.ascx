<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Account.PersonalAccessTokenModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Account" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<% if (Model.AuthorizationId == null)
    { %>
<div class="header-text-wrapper feature-header section-box">
    <label id="headertext" class="header-text"><%: AccountServerResources.TokenGenerateFormHeaderText %> </label>
    <!--<a href="#"><%: AccountResources.TokenHeaderLearnMore %></a>-->
</div>
<% } %>

<div class="token-form-wrapper">
    <form class="token-form" role="form" aria-labelledby="headertext">
        <%:Html.TfsAntiForgeryToken() %>

        <!-- Id -->
        <input type="hidden" name="token-id" class="token-id" value="<%:Model.AuthorizationId%>" />
        <input type="hidden" name="token-is-valid" class="token-is-valid" value="<%:Model.IsValid%>" />

        <!-- Description -->
        <div class="description-wrapper section-box">
            <label class="description-label account-form-label" id="pat-descriptionid" for="pat-descritionInput">
                <!-- fix the width of all the labels -->
                <%: AccountServerResources.TokenDescriptionLabel %>
            </label>
            <input id="pat-descritionInput" aria-labelledby="pat-descriptionid" class="input-field input-visible description" name="description" type="text" value="<%: Model.Description %>" maxlength="256"/>
            <label class="error description-error"></label>
        </div>

        <!-- Expiration Date -->
        <% if (!String.IsNullOrWhiteSpace(Model.ExpiresUtc))
            { %>
        <div class="existing-expiration-wrapper section-box">
            <label class="existing-expiration-date account-form-label">
                <%: AccountServerResources.TokenExpirationDateLabel %>
            </label>
            <%: Model.ExpiresUtc %>
        </div>
        <% } %>

        <!-- Expiration -->
        <div class="expiration-wrapper section-box">
            <label class="expiration-label account-form-label" id="pat-expiration-id">
                <%: AccountServerResources.TokenExpirationLabel %>
            </label>
            <select class="drpdown inputfield input-visible expiration" name="expiration" aria-labelledby="pat-expiration-id">
                <% foreach (var ExpirationValue in Model.ValidExpirationValues)
                    {
                        if (!String.IsNullOrWhiteSpace(Model.SelectedExpiration) && Model.SelectedExpiration.Equals(ExpirationValue.Key))
                        { %>
                <option selected="selected" value="<%: ExpirationValue.Key %>"><%: ExpirationValue.Value %></option>
                <% }
                else
                { %>
                <option value="<%: ExpirationValue.Key %>"><%: ExpirationValue.Value %></option>
                <% }
                } %>
            </select>
            <label class="error expiration-error"></label>
        </div>

        <!-- Account Selection for Create token -->
        <% if (Model.AllAccounts != null && Model.AuthorizationId == null)
            { %>
        <div class="account-wrapper section-box account-wrapper">
            <label class="account-label account-form-label" id="pat-accountsel-id">
                <%: AccountServerResources.TokenAccountSelectionLabel %>
            </label>
            <select class="drpdown inputfield input-visible account" name="account" aria-labelledby="pat-accountsel-id">
                <% if (Model.DisplayAllAccountsOption)
                    { %>
                <% if (String.IsNullOrWhiteSpace(Model.Tenant))
                    { %>
                <option value="all_accounts"><%: AccountServerResources.TokenAllAccountsSelection %></option>
                <% }
                else
                { %>
                <option value="all_accounts"><%: String.Format(AccountServerResources.TokenAllTenantAccounts, Model.Tenant) %></option>
                <%   } %>
                <% } %>
                <% foreach (var account in Model.AllAccounts)
                    { %>
                <option value="<%: account.Key %>"
                    <% if (Model.AccountMode == ApplicableAccountMode.SelectedAccounts && Model.SelectedAccounts != null && Model.SelectedAccounts.Any()) { if (Model.SelectedAccounts[0] == account.Key.ToString()) { Response.Write("selected=\"selected\""); } } %>>
                    <%: account.Value %></option>
                <% } %>
            </select>
        </div>
        <% } %>

        <!-- Authorized Scopes -->
        <div class="authorized-scopes section-box">
            <div class="label-row authorized-scopes-label">
                <label class="account-form-label section-box authorization-label">
                    <%: AccountServerResources.TokenAuthorizedScopesLabel %>
                    <!--<a href="#"><%: AccountResources.TokenHeaderLearnMore %></a>-->
                </label>
            </div>
            <div class="radio-field">
                <div class="all-scopes">
                    <input type="radio" id="AllScopes" name="scopeMode" class="scope-mode-all" value="AllScopes" <% if (Model.ScopeMode == AuthorizedScopeMode.AllScopes) { Response.Write("checked=\"checked\""); } %>><label for="AllScopes"><%: AccountResources.TokenAllScopesSelection %></label>
                </div>
                <div class="selected-scopes">
                    <input type="radio" id="SelectedScopes" name="scopeMode" class="scope-mode-selected" value="SelectedScopes" <% if (Model.ScopeMode == AuthorizedScopeMode.SelectedScopes) { Response.Write("checked=\"checked\""); } %>><label for="SelectedScopes"><%: AccountServerResources.TokenSelectedScopeSelection %></label>
                </div>
            </div>
            <div class="scopes check-field <% if (Model.ScopeMode == AuthorizedScopeMode.AllScopes) { Response.Write("disabled"); } %>" role="group">
                <%  var row = 0; var col = -1;
				    var isFirst = true;
                    foreach (var scope in Model.AllScopes)
                    {
                        if (scope.Key != "preview_api_all")
                        { // don't show all resources since they can click all authorized scopes
                            if (!String.IsNullOrWhiteSpace(scope.Key) && !String.IsNullOrWhiteSpace(scope.Value))
                            {
                                if (col == 2)
                                {
                                    row++;
                                    col = 0;
                                }
                                else
                                {
                                    col++;
                                }
                                // Set the checkbox state state
                                var check = false;
                                if (Model.SelectedScopes != null && Model.SelectedScopes.Contains(scope.Key))
                                {
                                    check = true;
                                }
                                var disabled = true;
                                if (Model.ScopeMode == AuthorizedScopeMode.SelectedScopes)
                                {
                                    disabled = false;
                                }
                %>
                <span class="scope-check-wrapper">
                    <input type="checkbox" class="selected-scope-checkbox" data-row="<%= row %>" data-col="<%= col %>" id="<%= scope.Key %>" name="<%= scope.Key %>" value="<%= scope.Key %>" <% if (disabled) { %> disabled="disabled" <% } %> <% if (check)
                        { %> checked="checked" <% } %>  <% if (!isFirst ) { %> tabindex="-1" <% } %> />
                        <label for="<%= scope.Key %>"><%: scope.Value %></label>
                </span>
                <%         }
                        }

                        isFirst = false;
                    } %>
            </div>
        </div>

        <!-- Generate/Cancel Token Button -->
        <div class="button-container">
            <div class="button-field">
                <% if (String.IsNullOrEmpty(Model.AuthorizationId) || Model.IsValid)
                    { %>
                <button type="submit" class="token-save-button formInput" disabled="disabled"><% Response.Write(Model.AuthorizationId == null ? AccountServerResources.TokenGenerateButton : AccountServerResources.DialogSave); %></button>
                <% } %>
                <button type="button" class="token-cancel-button formInput"><%: AccountServerResources.TokenCancelButton %></button>
                <img class="wait" alt="<%: AccountServerResources.PleaseWait %>" src="<%= StaticResources.Versioned.Content.GetLocation("big-progress.gif") %>">
            </div>
        </div>

    </form>
</div>

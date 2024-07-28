<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.TraceLicenseModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<div class="trace-permission-page">
    <div class="info-section">
        <div class="form-inline">
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.User %></div>
                <div class="form-value"><%: Model.Identity.DisplayName %></div>
            </div>
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.License %></div>
                <div class="form-value"><%: String.Join(" | ", Model.Licenses.Select(x => x.Name)) %></div>
            </div>
        </div>
    </div>
    <div class="account-section-control groups-section">
        <div class="groups-section-header">
            <p class="main-header"><%: Model.IsDefault ? AdminServerResources.DefaultLicenseHeader : AdminServerResources.InheritedLicenseHeader %></p>
            <span class="secondary-guidance"><%: Model.IsDefault ? AdminServerResources.DefaultLicenseDescription : AdminServerResources.InheritedLicenseDescription %></span>
        </div>
        <div class="content">
            <table>
            <% foreach (var v in Model.AffectingIdentities) { %>
                <tr>
                    <td class="key"><%: v.Key.DisplayName %></td>
                    <td class="value"><%: v.Value.Name %></td>
                </tr>
            <% } %>
            </table>
        </div>
    </div>
</div>
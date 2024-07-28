<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.LicenseModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<div class="display-license-control vertical-fill-layout">
    <div class="license-info fixed-header">
        <div class="form-inline">
            <div class="form-pair">
                <div class="form-key"><%: AdminResources.Name %></div>
                <div class="form-value" title="<%: Model.LicenseType.Description %>"><strong><%: Model.LicenseType.Name %></strong></div>
            </div>
            <div class="form-pair">
                <div class="form-key"><%: AdminServerResources.LicenseFeatures %></div>
                <div class="form-value">
                    <ul class="features-list">
                        <% foreach (var feature in Model.LicenseFeatures)
                           { %>
                        <li title="<%: feature.Description %>"><%: feature.Name %></li>
                        <% } %>
                    </ul>
                </div>
            </div>
        </div>
        <% if (Model.LicenseType.Name == AdminServerResources.AdvancedLicenseName)
            {%>
             <div class="message-area-control warning-message access-level-message"><% =AdminServerResources.AdvancedLicenseDepricatedMessage %></div>
        <%  }
            else { %>
             <div class="message-area-control info-message access-level-message"><% =AdminServerResources.AccessLevelsMessage %></div>
        <%  } %>
    </div>
    <div class="license-members-control fill-content">
    </div>
    <%= Html.DisplayLicenseViewOptions(Model) %>
</div>

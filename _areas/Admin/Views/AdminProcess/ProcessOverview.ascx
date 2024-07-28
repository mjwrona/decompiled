<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.ProcessViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<div class="process-content">
    <div class="process-content-overview-header" data-bind="visible: isSystem" style="display: none"><%= AdminServerResources.ProcessOverviewSystemProcess %></div>
    <div class="process-content-overview-header" data-bind="visible: !isSystem && isInherited" style="display: none"><%= AdminServerResources.ProcessOverviewCustomizableProcess %></div>
    <div class="process-content-overview-header" data-bind="visible: !isSystem && !isInherited" style="display: none"><%= AdminServerResources.ProcessOverviewImportedProcess %></div>
    <div class="process-content-info" data-bind="css: { error: name.hasError }">
        <div class="input-container">
            <span class="label"><label id="processoverview-label-name" for="processoverview-name"><%: AdminResources.Name %></label></span>
            <input aria-labelledby="processoverview-label-name" id="processoverview-name" 
                data-bind="value: currentName, disable: isSystem || !canEdit()" maxlength="256" />
            <div data-bind="visible: !currentNameValid(), text: currentNameValidationMessage" class="admin-error-message" />
        </div>
        <div class="input-container">
            <span class="label"><label id="processoverview-label-description" for="processoverview-description"><%: AdminResources.Description %></label></span>
            <textarea aria-labelledby="processoverview-label-description" id="processoverview-description" rows="3" 
                data-bind="value: currentDescription, disable: isSystem || !canEdit()" maxlength="1024"></textarea>
        </div>
    </div>
    <h3 class="section-header"><%: AdminServerResources.ProcessOverviewNewSettingsHeader %></h3>
    <p class="process-info"><%: AdminServerResources.ProcessOverviewSettingsDescription %></p>
    <div class="input-container">
        <div class="core-fields-checkbox">
            <input aria-labelledby="processoverview-label-isenabled" id="processoverview-isenabled" type="checkbox" data-bind="checked: currentIsEnabled, disable: isDefault || !canEdit()" />
            <label id="processoverview-label-isenabled" for="processoverview-isenabled"><%: AdminServerResources.ProcessOverviewIsEnabledLabel %></label>
        </div>
        <span class="label" data-bind="visible: isDefault"><%: AdminServerResources.ProcessOverviewIsDefaultLabel %></span>
    </div>
    <h3 class="section-header" data-bind="visible: isSystem || isInherited"><%: AdminServerResources.ChangeTeamProjects %></h3>
    <div><a class="change-project-link" data-bind="visible: isSystem || isInherited"><u><%: AdminServerResources.ChangeTeamProjectsToUseThisProcess %></u></a></div>
    <h3 class="section-header" data-bind="visible: !isSystem && !isInherited"><%: AdminServerResources.UpdateProcess %></h3>
    <div><a class="update-process-link" data-bind="visible: !isSystem && !isInherited"><u><%: AdminServerResources.ImportAnUpdatedVersionOfThisProcess %></u></a></div>
</div>
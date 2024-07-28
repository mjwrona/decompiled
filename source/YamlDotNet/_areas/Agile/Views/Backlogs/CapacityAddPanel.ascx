<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>

<script id="capacity-add-panel-control-template" type="text/html">
    <div class="capacity-add-panel-control-container">
        <div class="add-panel-table">
            <div class="add-panel-row">
                <label class="add-panel-label"><%:AgileControlsServerResources.Capacity_AddPanel_Title%></label>
                <button class="add-panel-close-button icon bowtie-icon bowtie-navigate-close" title="<%:AgileControlsServerResources.CAPACITY_ADD_PANEL_CLOSE_BUTTON_TEXT%>" data-bind="click: hideAddPanel, clickOnEnterKey: true"></button>
            </div>
            <div class="add-panel-row">
                <div class="capacity-add-panel-identitypicker-container"></div>
                <button type="button" class="add-panel-button capacity-add-panel-add-button" data-bind="click: addUser, enable: isAddEnabled"><%:AgileControlsServerResources.Capacity_AddPanel_Add%></button>
            </div>
            <div class="add-panel-row" data-bind="visible: isError">
                <div class="capacity-add-panel-error-message-icon bowtie-icon bowtie-status-error"></div>
                <div class="capacity-add-panel-error-message" data-bind="text: errorText"></div>
            </div>
        </div>
    </div>
</script>

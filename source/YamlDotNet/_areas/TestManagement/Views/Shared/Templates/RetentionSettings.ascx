<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.PlatformViewUserControl<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<script id="retention_settings" type="text/html">
    <table class="retention-settings-table">

        <tr>
            <td class="info-cell">
                <div class="info-header"><%: TestManagementResources.DaysToKeepAutomatedResultsText %></div>
                <div class="info-details"><%: TestManagementResources.RetainsAutomatedResultsWithRetainedBuildText %></div>
            </td>
            <td class="combo-data">
                <div class="duration-selector-automated"></div>
            </td>
        </tr>

        <tr>
            <td class="info-cell">
                <div class="info-header"><%: TestManagementResources.DaysToKeepManualResultsText %></div>
                <div class="info-details"><%: TestManagementResources.RetainsManualResultsWithRetainedBuildText %></div>
            </td>
            <td class="combo-data">
                <div class="duration-selector-manual"></div>
            </td>
        </tr>

    </table>

    <button class="cta save-changes-button" 
            data-bind="enable: canSave">
        <%: TestManagementResources.SaveChangesText %>
    </button>

    <button class="cta undo-changes-button" 
            data-bind="enable: canUndo">
        <%: TestManagementResources.UndoChangesText %>
    </button>

</script>
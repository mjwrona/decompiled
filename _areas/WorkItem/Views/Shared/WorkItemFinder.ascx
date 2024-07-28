<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models.WorkItemFinderModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking" %>
<div class="work-item-find">
    <div class="filter-container">
        <div class="project-container"></div>

        <div class="filter-type-container"></div>

    </div>

    <div class="status-container"></div>

    <div class="list-container">
        <div class="query-result-grid">
            <% =Html.WorkItemFinderOptions(Model) %>
        </div>
    </div>

    <div class="result-status-container"></div>

    <div class="buttons-container">
        <button type="button" id="select-all" class="select-all ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" role="button">
            <span class="ui-button-text"><%: WITServerResources.WorkItemFinderSelectAllButtonText() %></span>
        </button>
        <button type="button" id="unselect-all" class="unselect-all ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" role="button">
            <span class="ui-button-text"><%: WITServerResources.WorkItemFinderUnselectAllButtonText() %></span>
        </button>
        <button type="button" id="reset" class="reset ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" role="button">
            <span class="ui-button-text"><%: WITServerResources.WorkItemFinderResetButtonText() %></span>
        </button>
    </div>
</div>

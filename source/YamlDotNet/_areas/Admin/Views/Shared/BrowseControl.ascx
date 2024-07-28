<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.BrowseControlModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>

<div class="control-panel-list-view">
    <div class="splitter horizontal hub-splitter toggle-button-enabled toggle-button-hotkey-enabled">
        <div class="leftPane">
            <div class="left-hub-content vertical-fill-layout">
                <div class="control-panel-search-wrapper fixed-header">
                    <div class="sidebar-search enhance"></div>
                </div>
                <div class="jump-list-tree-control fill-content">
                    <input type="hidden" />
                    <%= Html.BrowseControlOptions(Model) %>
                </div>
            </div>
        </div>
        <div class="handleBar"></div>
        <div class="rightPane">
            <div class="control-panel-view-right-pane"></div>
        </div>
    </div>
    <%= Html.BrowseControlOptions(Model) %>
    <%= Html.TfsWebContext(TfsWebContext) %>
</div>
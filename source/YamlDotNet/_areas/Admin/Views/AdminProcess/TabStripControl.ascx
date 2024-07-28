<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<script id="tabstrip-collection-template" type="text/html">
        <div class="tabstrip-collection" data-bind="foreach: tabs, sortable: tabs">
            <div class="tabstrip" data-bind="attr: { 'aria-label': tabName }, selectTabHandler: $data, tabMenuRightClickHandler: $data ,css: { 'page-hidden': $data.isHiddenFromLayout(), 'active': $parent.activeTabIndex() == $index(), 'unsortable': !$data.isSortable() }, attr: {'tabindex': $parent.activeTabIndex() == $index() ? 0 : -1 }">
                <div class="header-gripper" data-bind="visible: $parent.tabs().length != 1 && isSortable()"></div>
                <!-- ko if: $data.iconClass -->
                    <span class="header-icon" data-bind="css: $data.iconClass"></span>
                <!-- /ko -->                                
                <div class="header" data-bind="text: tabName, titleHandler: $data"></div>
                <div class="icon bowtie-icon bowtie-ellipsis tabstrip-menu" role="button" data-bind="tabMenuClickHandler: $data,  attr: {'tabindex': $parent.activeTabIndex() == $index() ? 0 : -1 }"></div>
                <div class="icon bowtie-icon bowtie-status-error" data-bind="visible: !isValid()"></div>
            </div>
        </div>
</script>
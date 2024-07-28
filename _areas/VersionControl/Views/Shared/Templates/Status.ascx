<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.VersionControl" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>

<% Html.RenderPartial("~/_areas/Build/Views/Shared/Templates/TemplatesWizardDialog.ascx"); %>

<script id="vc-status" type="text/html" >
    <div class="status-indicator" data-bind="visible: canShowLoading()">
        <div class="status">
            <span class="icon status-progress"></span>
        </div>
    </div> 

    <img class="build-badge" data-bind="attr: { 'src': buildBadgeImgUrl }, click: onBadgeClick, visible: !canShowLoading() && canShowUpsell()" />

    <span class="vc-status-tooltip-clickable">
        <span class="bowtie-icon" data-bind="css: statusCssIconClass, visible: !canShowLoading() && !canShowUpsell()"></span>
        <span class="status-main" data-bind="text: statusMainText, css: statusCssClass, visible: !canShowLoading() && !canShowUpsell() && canShowMainText()"></span>
    </span>
</script>

<script id="vc-status-tooltip" type="text/html" >
    <div class="status-tooltip-primary" data-bind="text: statusTooltipPrimaryText, css: statusCssClass"></div>
    <div class="status-tooltip-secondary" data-bind="text: statusTooltipSecondaryText"></div>
     <div class="status-tooltip-statuses-container" data-bind="foreach: { data: _statuses, as: 'status' }">
        <div class="status-tooltip-status-container">
            <div class="status-tooltip-individual-icon bowtie-icon" data-bind="css: status.iconCssClass"></div>
            <div class="status-tooltip-individual-primary">
                <a class="status-link" data-bind="text: status.primaryText, attr: { href: status.link, target: '_blank' }"></a>
            </div>
            <div class="status-tooltip-individual-secondary" data-bind="text: status.secondaryText"></div>
        </div>
    </div>
</script>
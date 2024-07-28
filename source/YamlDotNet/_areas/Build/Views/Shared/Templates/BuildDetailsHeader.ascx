<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script type="text/html" id="buildvnext_details_header">
    <div id="build-detail-toolbar-container"></div>
    <div tabindex="0" class="buildvnext-build-details-status" data-bind="text: statusText, css: statusClass, attr: { 'aria-label': statusText }" aria-live="polite"></div>
    <div class="buildvnext-build-details-header">
        <div class="build-histogram definition-histogram"></div>
        <div class="summary">
            <div class="reason">
                <!-- ko if: currentTimelineRecordName -->
                <span data-bind="text: currentTimelineRecordName"></span>
                <!-- /ko -->
                <!-- ko ifnot: currentTimelineRecordName -->
                <span data-bind="text: buildNumber"></span>
                <!-- /ko -->
                <!-- ko if: buildReasonCss -->
                <span data-bind="css: buildReasonCss, attr: { title: buildReasonDisplayText() }"></span>
                <!-- /ko -->
            </div>
            <div class="duration" data-bind="text: durationText"></div>
        </div>
    </div>
    <!-- ko if: showRealtimeConnectionErrorMessage -->
    <div class="message-area-part message-area-control bowtie info-message"><%: BuildServerResources.RealtimeConnectionErrorMessage %></div>
    <!-- /ko -->
</script>

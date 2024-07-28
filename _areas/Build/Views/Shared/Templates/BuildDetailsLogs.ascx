<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script type="text/html" id="buildvnext_details_logs_tab">
    <!-- ko if: $data -->
    <div class="buildvnext-node-log-container">
        <!-- ko if: isSelected -->
        <div data-bind="buildLogViewer: $data.logContent"></div>
        <!-- /ko -->
    </div>
    <!-- /ko -->
</script>
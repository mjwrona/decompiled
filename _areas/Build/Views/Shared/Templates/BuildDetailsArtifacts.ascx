<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script type="text/html" id="buildvnext_details_artifacts_tab">
    <!-- ko if: $data -->
    <div class="artifacts-grid"></div>    
    <!-- /ko -->
</script>
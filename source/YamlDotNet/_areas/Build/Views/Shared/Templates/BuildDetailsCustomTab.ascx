<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script type="text/html" id="buildvnext_details_custom_tab">
    <!-- ko if: $data -->
    <!-- ko with: $data.vm -->
    <div class="build-details build-custom-tab">
        <div data-bind="applyTemplate: { templateName: 'buildvnext_details_sections', viewModel: $data }"></div>
        <div data-bind="attr: { id: contribution.id }, enhanceResultsViewContributions: { contribution: contribution, selected: $parent.isSelected }" class="hub-external"></div>
    </div>
    <!-- /ko -->
    <!-- /ko -->
</script>

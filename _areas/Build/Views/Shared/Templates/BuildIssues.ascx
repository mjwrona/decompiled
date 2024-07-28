<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script type="text/html" id="buildvnext_issue">
    <div class="icon build-issue-icon" data-bind="attr: { 'aria-label': iconAriaLabel }, css: iconCssClass" />

    <!-- ko foreach: messageLines -->
    <div class="build-issue-detail" data-bind="text: $data"></div>
    <!-- /ko -->
</script>

<script type="text/html" id="buildvnext_code_issue">
    <div class="icon build-issue-icon" data-bind="css: iconCssClass" />
    <div class="build-issue-detail">
        <!-- ko if: contentUrl -->
        <a data-bind="text: locationText, attr: { 'href': contentUrl }" target="_blank"></a>
        <!-- /ko -->
        <!-- ko ifnot: contentUrl -->
        <span data-bind="text: locationText" />
        <!-- /ko -->
    </div>
    
    <!-- ko foreach: messageLines -->
    <div class="build-issue-detail" data-bind="text: $data"></div>
    <!-- /ko -->
</script>
<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>

<script id="buildvnext_pivot_filter" type="text/html">
    <div class="dropdown pivot-filter" data-bind="css: { 'active': active }">
        <span class="title" data-bind="text: name"></span>
        <a href="#" class="selected" data-bind="text: selectedText, attr: { title: selectedText }, click: onDropdownClick "></a>
    </div>
</script>
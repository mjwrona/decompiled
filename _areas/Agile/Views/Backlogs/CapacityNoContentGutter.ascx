<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>

<script id="capacity-nocontent-gutter-control-template" type="text/html">
    <div class="hub-no-content-gutter gutter-banner" data-bind="visible: isVisible">
    <div class="gutter-banner-container">
        <div class="gutter-banner-text">
            <h2 data-bind="text: heading"></h2>
            <p data-bind="html: message"></p>
            <ul data-bind="foreach: bullets">
                <li>
                    <span class="gutter-banner-bullet" data-bind="text: name"></span>
                    <span data-bind="css: iconClass"></span>
                </li>
            </ul>
        </div>
        <div class="gutter-banner-icon">
            <div class="image image-vso-cloud"></div>
        </div>
</script>

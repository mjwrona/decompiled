<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.VersionControl" %>

<script id="vc-pullrequest-notification-ko" type="text/html" >
    <div class="vc-pullrequest-notification-area" data-bind="visible: notificationViewModel.hasError() || notificationViewModel.hasNotification() ">
        <div class="vc-pullrequest-information" data-bind="visible: notificationViewModel.hasNotification">
            <span data-bind="text: notificationViewModel.notificationMessages"></span>
            <a data-bind="text: notificationViewModel.notificationLink, click: notificationViewModel.notificationLinkAction"></a>
        </div>
        <div class="vc-pullrequest-error" data-bind="visible: notificationViewModel.hasError">
            <span data-bind="text: notificationViewModel.errorMessages"></span>
        </div>
    </div>
</script>
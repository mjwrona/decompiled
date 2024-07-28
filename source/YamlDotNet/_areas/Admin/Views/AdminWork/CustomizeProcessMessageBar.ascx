<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Agile" %>

<div class="customize-process-message" hidden>
    <div class="message-area-control info-message">
        <div class="message-icon">
            <span class="bowtie bowtie-icon bowtie-status-info " />
        </div>
        <div class="message-header">
            <span><%:String.Format(System.Globalization.CultureInfo.CurrentCulture, AgileAdminResources.CustomizeProcessMessageBarText, Model) %></span>
            <a class="customize-process-link"><%: AgileAdminResources.CustomizeProcessMessageBarLinkText %></a>
        </div>
    </div>
</div>

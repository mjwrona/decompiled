<%@ Page Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.ProcessViewModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<div class="inherited-process-dialog bowtie">
    <div id="warning-wrapper" />
    <div>
        <span><%: AdminServerResources.InheritedProcessExplanation %></span>
    </div>
    <div class="create-inherited-input-section">
        <fieldset class="process-name-block">
            <div class="parent-process-box">
                <label>
                    <span id="parent-name-label" />
                    <%: AdminResources.SystemProcess %>
                </label>
            </div>
            <div class="name-field-wrapper">
                <input class="inherited-process-name-field" id="inheritedProcessName" type="text" maxlength="128" />
                <div class="error-message" id="error-message" />
            </div>
        </fieldset>
        <fieldset>
            <label for="inheritedProcessDescription"><%: AdminServerResources.ProcessDescription %></label>
            <textarea class="inherited-process-description-field process-common-textarea"
                maxlength="1024" id="inheritedProcessDescription" rows="4"></textarea>
        </fieldset>
    </div>
</div>

<%@ Page Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.Models.SelectMigrateTargetModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<div class="admin-dialog-pad-sides bowtie" id="select-migrate-target-dialog">
    <div id="warning-box-wrapper"/>
    <%
    if (Model.CanChangeProcess)
    {
        if (Model.ToSystem)
        {
    %>
            <div class="migration-content">
                <span><%: string.Format(AdminServerResources.ChangeProcessOfProject, Model.ProjectName) %><b><%: Model.SystemProcessName %></b><%: "."%></span>
            </div>
            <div class="migration-content">
                <span><br/><%: AdminServerResources.MigrationWarning %></span>
            </div>
        <%
            }
            else
            {
        %>
            <div class="migration-content">
                <span><%: AdminServerResources.ChooseMigrateTarget %></span>
            </div>
        <%
            }
        %>
        <div class="select-target-content">
        <% 
            if (Model.ToSystem)
            {
            %>
                <div class="change-to-system">
            <%  
            } else
            {
            %>
                <div class="change-to-inherited">
            <%
            } 
            %>
                <div>
                    <span id="parent-name" />
                </div>
                <div class="select-child-input-section">
                    <%
                        if (!Model.ToSystem)
                        {
                    %>
                        <div id="project-selector"/>
                    <%
                        }
                        else
                        {
                    %>
                        <div class="current-process-name">
                            <span><%: Model.CurrentProcessName %> </span>
                        </div>
                    <%
                        }
                    %>
                </div>
            </div>
        </div>
    <%
    } else
    {
    %>
        <span class="icon icon-warning margin3" />
        <span>
        <%
        if (Model.ToSystem)
        {
            if (string.IsNullOrWhiteSpace(Model.SystemProcessName))
            {
        %>
                <%:string.Format(AdminServerResources.ProcessChangeUnsupported, Model.ProjectName) %>
        <%
            }
            else
            {
        %>
                <%:string.Format(AdminServerResources.ParentProcessNotEnabled, Model.SystemProcessName, Model.ProjectName) %>
        <%
            }
        }
        else
        {
        %>
            <%: string.Format(AdminServerResources.NoProcessesAvailableToChangeTo, Model.CurrentProcessName, Model.ProjectName) %>
        <%
        }
        %>
        </span> 
    <%
    }
    %>
</div>

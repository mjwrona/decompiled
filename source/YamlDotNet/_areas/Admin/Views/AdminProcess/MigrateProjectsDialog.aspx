<%@ Page Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.Models.MigrateProjectsModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<div class="admin-dialog-pad-sides bowtie">
    <div id="warning-box-wrapper" />
    <div class="project-migration-content">
        <%
            if (Model.Projects.Count() == 0)
            {
        %>
        <span class="icon icon-warning margin3" />
        <span>
            <%: string.Format(AdminServerResources.NoProjectsAvailableToChange, Model.TargetName) %>
        </span>
        <%
            }
            else
            {
        %>
        <div class="migration-content">
            <!-- Change team projects to use the <b>"Process Name"</b> process. -->
            <span><%: AdminServerResources.ChangeTeamProjectProcessStart %><b><%: Model.TargetName %></b><%: " " + AdminServerResources.ChangeTeamProjectProcessEnd %></span>
        </div>
        <%
            if (Model.ToSystem)
            {
        %>
        <div class="migration-content">
            <span>
                <br />
                <%: AdminServerResources.MigrationWarning %></span>
        </div>
        <%
            }
        %>
    </div>
    <div class="available-projects">
        <label for="available-list">
            <%: AdminServerResources.AvailableProjects %>
        </label>
        <select class="available-project-list" multiple id="available-list">
            <% foreach (var proj in Model.Projects)
                { %>
            <option value="<%: proj.Id %>"><%: proj.Name + " [" + Model.ProjectIdToProcessNameMap[proj.Id] + "]" %></option>
            <% } %>
        </select>
        <div class="migration-buttons-wrapper">
            <div class="migration-buttons">
                <button id="left-button" class="project-button">
                    <span class="bowtie-icon bowtie-chevron-right" />
                </button>
                <button id="right-button" class="project-button">
                    <span class="bowtie-icon bowtie-chevron-left" />
                </button>
            </div>
        </div>
    </div>
    <div class="selected-projects">
        <label for="selected-list">
            <%: AdminServerResources.ProjectsThatWillChangeProcess %>
        </label>
        <select class="selected-project-list" multiple id="selected-list" />
        <%
            }
        %>
    </div>
</div>

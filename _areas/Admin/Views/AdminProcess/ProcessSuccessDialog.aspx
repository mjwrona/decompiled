<%@ Page Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<Microsoft.TeamFoundation.Server.WebAccess.Admin.Models.ProcessSuccessModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>

<div id="admin-process-sucess-dialog" class="admin-dialog-pad-sides admin-dialog-pad-bottom">
	<h2 class="inherited-process-header" id="dialog-header"/>
	<div class="dialog-description" id="dialog-description"/>
	<div class="dialog-next-step dialog-next-step-label" id="next-step-header">
        <%: AdminServerResources.NextSteps %>
	</div>
	<div class="dialog-next-step">
		<ul>
	    <% if (Model.CanCreateProjects) { %>
			<li>
				<a id="create-project" href="#">
		            <%: string.Format(AdminServerResources.CreateProjectWithProcess, Model.ProcessName) %>
				</a>
			</li>
	    <% } %>
            <li>
				<a id="change-projects" href="#">
		            <%: string.Format(AdminServerResources.ChangeTeamProjectsToUse, Model.ProcessName) %>
				</a>
			</li>
			<li>
				<a id="customize-process" href="#">
		            <%: string.Format(AdminServerResources.Customize, Model.ProcessName) %>
				</a>
			</li>
		</ul>
	</div>
</div>

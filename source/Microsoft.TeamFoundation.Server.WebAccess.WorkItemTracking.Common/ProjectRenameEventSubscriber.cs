// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectRenameEventSubscriber
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Team;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class ProjectRenameEventSubscriber : ISubscriber
  {
    public string Name => "DevOps TeamConfigurationService Project Rename Event Subscriber";

    public SubscriberPriority Priority => SubscriberPriority.High;

    public Type[] SubscribedTypes() => new Type[1]
    {
      typeof (ProjectUpdatedEvent)
    };

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      statusMessage = string.Empty;
      properties = (ExceptionPropertyCollection) null;
      ProjectUpdatedEvent projectRenameEvent = (ProjectUpdatedEvent) notificationEventArgs;
      if (projectRenameEvent.GetOperationProperty<bool>("IsRename", false))
        this.RenameDefaultTeam(requestContext, projectRenameEvent);
      return EventNotificationStatus.ActionPermitted;
    }

    private void RenameDefaultTeam(
      IVssRequestContext requestContext,
      ProjectUpdatedEvent projectRenameEvent)
    {
      try
      {
        requestContext.Trace(102140, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Entering renaming team '" + projectRenameEvent.Name + "'");
        ITeamService service1 = requestContext.GetService<ITeamService>();
        Guid defaultTeamId = service1.GetDefaultTeamId(requestContext, ProjectInfo.GetProjectId(projectRenameEvent.Uri));
        if (!(defaultTeamId != Guid.Empty))
          return;
        IProjectService service2 = requestContext.GetService<IProjectService>();
        Guid projectId1 = ProjectInfo.GetProjectId(projectRenameEvent.Uri);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId2 = projectId1;
        ProjectInfo project = service2.GetProject(requestContext1, projectId2);
        WebApiTeam teamInProject1 = service1.GetTeamInProject(requestContext, projectId1, defaultTeamId.ToString());
        if (teamInProject1 != null && this.IsDefaultTeamNameMatched(teamInProject1.Name, (IEnumerable<string>) project.KnownNames))
        {
          string defaultTeamName = this.CreateDefaultTeamName(projectRenameEvent.Name);
          WebApiTeam teamInProject2 = service1.GetTeamInProject(requestContext, projectId1, defaultTeamName);
          if (teamInProject2 != null && teamInProject2.Id != defaultTeamId || StringComparer.Ordinal.Equals(defaultTeamName, teamInProject1.Name))
            return;
          UpdateTeam newTeamProperties = new UpdateTeam()
          {
            Name = defaultTeamName,
            Description = teamInProject1.Description
          };
          service1.UpdateTeam(requestContext, teamInProject1.ProjectId, teamInProject1.Id, newTeamProperties);
          requestContext.Trace(102142, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Team name updated from '" + teamInProject1.Name + "' to '" + newTeamProperties.Name + "' " + string.Format("for project GUID '{0}'", (object) teamInProject1.ProjectId));
        }
        else
          requestContext.Trace(102143, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Default team not found or no matching team found for '" + projectRenameEvent.Name + "'");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(102936, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, ex);
      }
      finally
      {
        requestContext.Trace(102141, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Exiting renaming team '" + projectRenameEvent.Name + "'");
      }
    }

    private bool IsDefaultTeamNameMatched(
      string teamNameToCompare,
      IEnumerable<string> projectKnownNames)
    {
      foreach (object projectKnownName in projectKnownNames)
      {
        if (TFStringComparer.TeamProjectName.Equals(FrameworkResources.ProjectDefaultTeam(projectKnownName), teamNameToCompare))
          return true;
      }
      return false;
    }

    private string CreateDefaultTeamName(string projectName)
    {
      string name = FrameworkResources.ProjectDefaultTeam((object) projectName);
      if (!TeamsUtility.IsValidTeamName(name))
        name = projectName;
      return name;
    }
  }
}

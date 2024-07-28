// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.AgileSettings
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public class AgileSettings : IAgileSettings
  {
    internal AgileSettings()
    {
    }

    public AgileSettings(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      bool bypassCache = false,
      bool bypassValidation = false)
    {
      AgileSettings agileSettings = this;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      requestContext.TraceBlock(6000101, 6000102, "AgileService", "AgileService", nameof (AgileSettings), (Action) (() =>
      {
        agileSettings.ProjectName = project.Name;
        agileSettings.Team = new Team()
        {
          Id = team.Id,
          Name = team.Name
        };
        agileSettings.Process = requestContext.GetProjectProcessSettings(project.ToProjectInfo(), true);
        requestContext.Trace(6000103, TraceLevel.Verbose, "AgileService", "AgileService", "Reading Agile Settings for Team " + team.Name + " in Project " + project.Name);
        agileSettings.TeamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, !bypassValidation, bypassCache);
        agileSettings.BacklogConfiguration = agileSettings.GetTeamBacklogConfiguration(requestContext, project.GetId(), team);
      }));
    }

    internal void UpdateForTeamSettings(
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      Guid teamId,
      HashSet<string> hiddenBacklogs,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior bugsBehavior,
      IReadOnlyCollection<string> bugWorkItems)
    {
      backlogConfiguration.TeamId = new Guid?(teamId);
      backlogConfiguration.HiddenBacklogs = (IReadOnlyCollection<string>) hiddenBacklogs;
      backlogConfiguration.BugsBehavior = AgileSettingsUtils.Convert(bugsBehavior);
      if (backlogConfiguration.BugsBehavior == Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.Off)
        return;
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = backlogConfiguration.BugsBehavior == Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.AsTasks ? backlogConfiguration.TaskBacklog : backlogConfiguration.RequirementBacklog;
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) levelConfiguration.WorkItemTypes);
      stringList.AddRange((IEnumerable<string>) bugWorkItems);
      levelConfiguration.WorkItemTypes = (IReadOnlyCollection<string>) stringList;
    }

    private Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration GetTeamBacklogConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      WebApiTeam team)
    {
      return requestContext.TraceBlock<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>(15165002, 15165003, nameof (AgileSettings), nameof (AgileSettings), nameof (GetTeamBacklogConfiguration), (Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>) (() =>
      {
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, projectId);
        IReadOnlyCollection<string> bugWorkItems = (IReadOnlyCollection<string>) new List<string>(0);
        if (this.Process.BugWorkItems != null)
          bugWorkItems = backlogConfiguration.BugWorkItemTypes;
        // ISSUE: explicit non-virtual call
        HashSet<string> hiddenBacklogs = new HashSet<string>(this.TeamSettings.BacklogVisibilities.Keys.Where<string>((Func<string, bool>) (backlogLevelId => !__nonvirtual (this.TeamSettings).BacklogVisibilities[backlogLevelId])), (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
        this.UpdateForTeamSettings(backlogConfiguration, team.Id, hiddenBacklogs, this.TeamSettings.BugsBehavior, bugWorkItems);
        return backlogConfiguration;
      }));
    }

    public string ProjectName { get; private set; }

    public Team Team { get; private set; }

    public ProjectProcessConfiguration Process { get; private set; }

    public ITeamSettings TeamSettings { get; private set; }

    public Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration BacklogConfiguration { get; private set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugBehaviorFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class BugBehaviorFeature : ProjectFeatureBase, INotifyProjectFeatureProvisioned
  {
    private BugsOnBacklogFeature m_bugsOnBacklogFeature;
    private BugsOnTaskBoardFeature m_bugsOnTaskBoardFeature;
    private BugsBehavior m_oldBugsBehavior;
    private BugsBehavior m_newBugsBehavior;

    public BugBehaviorFeature()
      : base(Resources.BugBehaviorFeatureName)
    {
      this.m_bugsOnBacklogFeature = new BugsOnBacklogFeature();
      this.m_bugsOnTaskBoardFeature = new BugsOnTaskBoardFeature();
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      ProjectFeatureState state1 = this.m_bugsOnBacklogFeature.GetState(projectMetadata);
      ProjectFeatureState state2 = this.m_bugsOnTaskBoardFeature.GetState(projectMetadata);
      if (state1 == ProjectFeatureState.PartiallyConfigured || state2 == ProjectFeatureState.PartiallyConfigured)
        return ProjectFeatureState.PartiallyConfigured;
      return state1 == ProjectFeatureState.NotConfigured || state2 == ProjectFeatureState.NotConfigured ? ProjectFeatureState.NotConfigured : ProjectFeatureState.FullyConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      this.m_oldBugsBehavior = processConfiguration == null ? BugsBehavior.Off : processConfiguration.BugsBehavior;
      this.m_bugsOnBacklogFeature.Process(context);
      this.m_bugsOnTaskBoardFeature.Process(context);
      this.m_newBugsBehavior = processConfiguration.BugsBehavior;
    }

    void INotifyProjectFeatureProvisioned.OnProvisioned(
      IVssRequestContext requestContext,
      string projectUri)
    {
      requestContext.TraceEnter(1004054, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, "BugBehaviorFeature.OnProvisioned");
      try
      {
        if (this.m_newBugsBehavior != this.m_oldBugsBehavior)
        {
          if (this.m_newBugsBehavior != BugsBehavior.Off)
          {
            CommonStructureProjectInfo projectFromUri = CssUtils.GetProjectFromUri(requestContext, projectUri);
            IReadOnlyCollection<WebApiTeam> webApiTeams = requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext, projectFromUri.GetId());
            ITeamConfigurationService service = requestContext.GetService<ITeamConfigurationService>();
            foreach (WebApiTeam team in (IEnumerable<WebApiTeam>) webApiTeams)
            {
              try
              {
                if (service.GetTeamSettings(requestContext, team, true, false).BugsBehavior == BugsBehavior.Off)
                  service.SetBugsBehavior(requestContext, team, this.m_newBugsBehavior);
              }
              catch (Exception ex)
              {
                requestContext.Trace(1004056, TraceLevel.Error, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, "Team " + team.Identity.Id.ToString() + ": Failed to set BugsBehavior team setting during BugBehavior Feature Enablement");
                requestContext.TraceException(1004057, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, ex);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException("BugBehavior feature threw exception while post-configuration custom actions", ex);
        requestContext.TraceException(1004058, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, ex);
      }
      requestContext.TraceLeave(1004055, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, "BugBehaviorFeature.OnProvisioned");
    }
  }
}

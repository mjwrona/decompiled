// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class TeamConfiguration : ITeamSettings
  {
    public TeamConfiguration()
    {
      this.TeamFieldConfig = (ITeamFieldSettings) new TeamFieldSettings();
      this.Iterations = (ITeamIterationsCollection) new TeamIterationsCollection();
      this.Weekends = (ITeamWeekends) new TeamWeekends();
      this.BugsBehavior = BugsBehavior.Off;
      this.BacklogVisibilities = (IDictionary<string, bool>) new Dictionary<string, bool>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
      this.DefaultedBacklogVisibilities = (IList<string>) new List<string>();
    }

    public Guid BacklogIterationId { get; set; }

    public Guid DefaultIterationId { get; set; }

    public string DefaultIterationMacro { get; set; }

    public ITeamFieldSettings TeamFieldConfig { get; private set; }

    public ITeamIterationsCollection Iterations { get; set; }

    public ITeamWeekends Weekends { get; set; }

    public BugsBehavior BugsBehavior { get; set; }

    public IDictionary<string, bool> BacklogVisibilities { get; set; }

    public IList<string> DefaultedBacklogVisibilities { get; set; }

    public IDictionary<string, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CumulativeFlowDiagramSettings> CumulativeFlowDiagramSettings { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetBacklogIterationNode(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNode(requestContext, this.BacklogIterationId);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetDefaultIterationNode(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNode(requestContext, this.DefaultIterationId);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetCurrentIterationNode(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      SortedIterationSubscriptions sortedTeamIterations = requestContext.GetService<ITeamIterationsService>().GetSortedTeamIterations(requestContext, projectId, this.Iterations);
      return sortedTeamIterations.IsValidSubscription ? sortedTeamIterations.CurrentIteration : throw new InvalidTeamSettingsException(Resources.Settings_ErrorMissingSprintIterations, TeamSettingsFields.TeamIteration);
    }

    public IReadOnlyList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> GetSortedNodes(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.GetService<ITeamIterationsService>().GetSortedTeamIterations(requestContext, projectId, this.Iterations).Iterations;
    }

    public SortedIterationSubscriptions GetIterationTimeline(
      IVssRequestContext requestContext,
      string projectUri)
    {
      return this.GetIterationTimeline(requestContext, ProjectInfo.GetProjectId(projectUri));
    }

    public SortedIterationSubscriptions GetIterationTimeline(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.GetService<ITeamIterationsService>().GetSortedTeamIterations(requestContext, projectId, this.Iterations);
    }

    public IterationsInDateRange GetIterationNodesInRange(
      IVssRequestContext requestContext,
      Guid projectId,
      DateTime startDate,
      DateTime endDate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return new IterationsInDateRange(this.GetIterationTimeline(requestContext, projectId).Iterations, startDate, endDate);
    }
  }
}

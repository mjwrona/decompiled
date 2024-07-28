// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ITeamSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public interface ITeamSettings
  {
    Guid DefaultIterationId { get; set; }

    string DefaultIterationMacro { get; set; }

    Guid BacklogIterationId { get; set; }

    ITeamFieldSettings TeamFieldConfig { get; }

    ITeamIterationsCollection Iterations { get; set; }

    ITeamWeekends Weekends { get; }

    BugsBehavior BugsBehavior { get; set; }

    IDictionary<string, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CumulativeFlowDiagramSettings> CumulativeFlowDiagramSettings { get; set; }

    IDictionary<string, bool> BacklogVisibilities { get; set; }

    IList<string> DefaultedBacklogVisibilities { get; set; }

    Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetBacklogIterationNode(
      IVssRequestContext requestContext);

    Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetDefaultIterationNode(
      IVssRequestContext requestContext);

    Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetCurrentIterationNode(
      IVssRequestContext requestContext,
      Guid projectId);

    SortedIterationSubscriptions GetIterationTimeline(
      IVssRequestContext requestContext,
      Guid projectId);

    SortedIterationSubscriptions GetIterationTimeline(
      IVssRequestContext requestContext,
      string projectUri);

    IterationsInDateRange GetIterationNodesInRange(
      IVssRequestContext requestContext,
      Guid projectId,
      DateTime startDate,
      DateTime endDate);
  }
}

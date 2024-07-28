// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IterationsInDateRange
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class IterationsInDateRange
  {
    public IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> IterationsInRange;
    public IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> IterationsMissingDates;
    public IDictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> OverlappedIterationsMap;

    public IterationsInDateRange(
      IReadOnlyList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> sortedIterations,
      DateTime startDate,
      DateTime endDate)
    {
      IterationsInDateRange iterationsInDateRange = this;
      this.IterationsInRange = (IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
      this.IterationsMissingDates = (IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
      for (int index = 0; index < sortedIterations.Count; ++index)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode sortedIteration = sortedIterations[index];
        if (this.IsInDateRange(sortedIteration, startDate, endDate))
          this.IterationsInRange.Add(sortedIterations[index]);
        else if (!sortedIteration.StartDate.HasValue || !sortedIteration.FinishDate.HasValue)
          this.IterationsMissingDates.Add(sortedIterations[index]);
      }
      this.OverlappedIterationsMap = (IDictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) IterationsInDateRange.GetOverlapIterations(sortedIterations).Where<KeyValuePair<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>>((Func<KeyValuePair<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>, bool>) (pair => iterationsInDateRange.IsInDateRange(pair.Value, startDate, endDate))).ToDictionary<KeyValuePair<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>, Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<KeyValuePair<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) (x => x.Value));
    }

    private static IDictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> GetOverlapIterations(
      IReadOnlyList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> sortedIterations)
    {
      Dictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> overlapIterations = new Dictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
      if (sortedIterations.Count == 0)
        return (IDictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) overlapIterations;
      for (int index = 1; index < sortedIterations.Count; ++index)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode sortedIteration1 = sortedIterations[index - 1];
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode sortedIteration2 = sortedIterations[index];
        DateTime? nullable = sortedIteration2.StartDate;
        if (nullable.HasValue)
        {
          nullable = sortedIteration1.FinishDate;
          if (nullable.HasValue)
          {
            nullable = sortedIteration2.StartDate;
            DateTime dateTime1 = nullable.Value;
            nullable = sortedIteration1.FinishDate;
            DateTime dateTime2 = nullable.Value;
            if (dateTime1 <= dateTime2)
            {
              if (!overlapIterations.ContainsKey(sortedIteration1.CssNodeId))
                overlapIterations.Add(sortedIteration1.CssNodeId, sortedIteration1);
              if (!overlapIterations.ContainsKey(sortedIteration2.CssNodeId))
                overlapIterations.Add(sortedIteration2.CssNodeId, sortedIteration2);
            }
          }
        }
      }
      return (IDictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) overlapIterations;
    }

    private bool IsInDateRange(Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iteration, DateTime startDate, DateTime endDate)
    {
      if (iteration.StartDate.HasValue && iteration.FinishDate.HasValue)
      {
        DateTime? finishDate = iteration.FinishDate;
        DateTime? nullable = iteration.StartDate;
        if ((finishDate.HasValue & nullable.HasValue ? (finishDate.GetValueOrDefault() >= nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          nullable = iteration.StartDate;
          if (nullable.Value.Date <= endDate.Date)
          {
            nullable = iteration.FinishDate;
            return nullable.Value.Date >= startDate.Date;
          }
        }
      }
      return false;
    }
  }
}

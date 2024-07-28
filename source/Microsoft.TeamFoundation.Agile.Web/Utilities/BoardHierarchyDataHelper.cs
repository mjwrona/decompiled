// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.BoardHierarchyDataHelper
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  internal static class BoardHierarchyDataHelper
  {
    public static List<int> GetSubTreeIdsByLeafNodes(
      IEnumerable<LinkQueryResultEntry> allLinks,
      IEnumerable<int> incomingLeafIds,
      IEnumerable<int> outgoingLeafIds,
      int incomingOutgoingLimit,
      IEnumerable<int> inProgressLeafIds,
      int inProgressLimit)
    {
      List<int> workItemIds = new List<int>();
      IEnumerable<int> collection1 = incomingLeafIds.Take<int>(incomingOutgoingLimit).Union<int>(inProgressLeafIds.Take<int>(inProgressLimit));
      IEnumerable<int> collection2 = outgoingLeafIds.Take<int>(incomingOutgoingLimit);
      workItemIds.AddRange(collection1);
      workItemIds.AddRange(collection2);
      IEnumerable<int> ancestorIds = BoardHierarchyDataHelper.FindAncestorIds(allLinks, (IEnumerable<int>) workItemIds);
      workItemIds.AddRange(ancestorIds);
      return workItemIds;
    }

    public static void GetOrderedLeafIds(
      IEnumerable<LinkQueryResultEntry> allLinks,
      IEnumerable<int> incomingWorkItemIds,
      IEnumerable<int> outgoingWorkItemIds,
      out IEnumerable<int> orderedIncomingLeafIds,
      out IEnumerable<int> orderedInProgressLeafIds,
      out IEnumerable<int> orderedOutgoingLeafIds)
    {
      IEnumerable<int> ints = allLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (l => l.TargetId)).Except<int>(allLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (l => l.SourceId)));
      orderedInProgressLeafIds = ints.Except<int>(incomingWorkItemIds.Union<int>(outgoingWorkItemIds));
      orderedIncomingLeafIds = ints.Except<int>(orderedInProgressLeafIds.Union<int>(outgoingWorkItemIds));
      orderedOutgoingLeafIds = outgoingWorkItemIds.Intersect<int>(ints);
    }

    public static IEnumerable<int> FindAncestorIds(
      IEnumerable<LinkQueryResultEntry> allLinks,
      IEnumerable<int> workItemIds)
    {
      List<int> source = new List<int>();
      Dictionary<int, LinkQueryResultEntry> dictionary = allLinks.ToDictionary<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (l => l.TargetId));
      foreach (int workItemId in workItemIds)
      {
        int key = workItemId;
        LinkQueryResultEntry queryResultEntry = (LinkQueryResultEntry) null;
        int sourceId;
        for (; dictionary.TryGetValue(key, out queryResultEntry); key = sourceId)
        {
          sourceId = queryResultEntry.SourceId;
          if (sourceId != 0)
            source.Add(sourceId);
          else
            break;
        }
      }
      return source.Distinct<int>();
    }

    public static ICollection<Tuple<int, int>> GetHierarchyByIds(
      IEnumerable<LinkQueryResultEntry> allLinks,
      IEnumerable<int> workItemIds)
    {
      ICollection<Tuple<int, int>> hierarchyByIds = (ICollection<Tuple<int, int>>) new LinkedList<Tuple<int, int>>();
      Dictionary<int, LinkQueryResultEntry> dictionary = allLinks.Where<LinkQueryResultEntry>((Func<LinkQueryResultEntry, bool>) (l => l.SourceId != 0)).ToDictionary<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (l => l.TargetId));
      foreach (int workItemId in workItemIds)
      {
        LinkQueryResultEntry queryResultEntry = (LinkQueryResultEntry) null;
        if (dictionary.TryGetValue(workItemId, out queryResultEntry))
          hierarchyByIds.Add(new Tuple<int, int>(queryResultEntry.TargetId, queryResultEntry.SourceId));
      }
      return hierarchyByIds;
    }
  }
}

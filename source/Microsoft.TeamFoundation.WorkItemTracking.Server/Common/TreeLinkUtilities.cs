// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.TreeLinkUtilities
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  internal class TreeLinkUtilities
  {
    public static ICollection<Microsoft.TeamFoundation.WorkItemTracking.Server.LinkQueryResultEntry> FilterTreeLinkResult(
      IVssRequestContext requestContext,
      IPermissionCheckHelper helper,
      QueryRecursionOption option,
      short linkTypeId,
      bool hasRhsFilter,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry> rawResult)
    {
      return (ICollection<Microsoft.TeamFoundation.WorkItemTracking.Server.LinkQueryResultEntry>) requestContext.TraceBlock<Microsoft.TeamFoundation.WorkItemTracking.Server.LinkQueryResultEntry[]>(906005, 906006, "Query", "WorkItemQueryService", nameof (FilterTreeLinkResult), (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.LinkQueryResultEntry[]>) (() =>
      {
        Dictionary<int, bool> roots = new Dictionary<int, bool>();
        Dictionary<int, List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>> sortedAdjacencyLists = new Dictionary<int, List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>>()
        {
          {
            0,
            new List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>(rawResult.Count)
          }
        };
        CommonWITUtils.TraceRawResultCount(requestContext, rawResult.Count);
        List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry> queryResultEntryList;
        for (int index = 0; index < rawResult.Count; ++index)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry queryResultEntry = rawResult[index];
          if (!sortedAdjacencyLists.TryGetValue(queryResultEntry.SourceId, out queryResultEntryList))
          {
            queryResultEntryList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>(4);
            sortedAdjacencyLists.Add(queryResultEntry.SourceId, queryResultEntryList);
          }
          queryResultEntryList.Add(queryResultEntry);
          roots[queryResultEntry.TargetId] = queryResultEntry.SourceId == 0;
          if (queryResultEntry.SourceId != 0)
            sortedAdjacencyLists[0].Add(queryResultEntry);
        }
        requestContext.TraceBlock(906014, 906015, "Query", "WorkItemQueryService", "PreprocessTreeLinkResult", (Action) (() => TreeLinkUtilities.PreprocessTreeLinkResult(helper, option, sortedAdjacencyLists, roots, 0)));
        int nextPos = 0;
        if (sortedAdjacencyLists.TryGetValue(0, out queryResultEntryList))
        {
          for (int index = 0; index < queryResultEntryList.Count; ++index)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry queryResultEntry = queryResultEntryList[index];
            if (roots[queryResultEntry.TargetId])
              TreeLinkUtilities.FilterTreeLinkResult(helper, option, hasRhsFilter, rawResult, sortedAdjacencyLists, queryResultEntryList[index], ref nextPos, true);
          }
        }
        CommonWITUtils.TraceRawAndFilteredResultCount(requestContext, rawResult.Count, nextPos);
        Dictionary<int, int> workitemIdToAreaIdCache = new Dictionary<int, int>();
        return rawResult.Take<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>(nextPos).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, Microsoft.TeamFoundation.WorkItemTracking.Server.LinkQueryResultEntry>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry, Microsoft.TeamFoundation.WorkItemTracking.Server.LinkQueryResultEntry>) (x =>
        {
          workitemIdToAreaIdCache[x.TargetId] = x.TargetAreaId;
          return new Microsoft.TeamFoundation.WorkItemTracking.Server.LinkQueryResultEntry()
          {
            SourceId = x.SourceId,
            TargetId = x.TargetId,
            LinkTypeId = x.SourceId != 0 ? linkTypeId : (short) 0,
            IsLocked = x.SourceId != 0 && x.IsLocked,
            SourceToken = x.SourceId > 0 ? helper.GetWorkItemSecurityToken(workitemIdToAreaIdCache[x.SourceId]) : string.Empty,
            TargetToken = helper.GetWorkItemSecurityToken(x.TargetAreaId)
          };
        })).ToArray<Microsoft.TeamFoundation.WorkItemTracking.Server.LinkQueryResultEntry>();
      }));
    }

    private static void PreprocessTreeLinkResult(
      IPermissionCheckHelper helper,
      QueryRecursionOption option,
      Dictionary<int, List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>> sortedAdjacencyLists,
      Dictionary<int, bool> roots,
      int sourceId)
    {
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry> queryResultEntryList;
      if (!sortedAdjacencyLists.TryGetValue(sourceId, out queryResultEntryList))
        return;
      for (int index = 0; index < queryResultEntryList.Count; ++index)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry queryResultEntry = queryResultEntryList[index];
        if (sourceId != 0 || roots[queryResultEntry.TargetId])
        {
          if ((queryResultEntry.MeetsParentCriteria || option == QueryRecursionOption.ChildFirst && queryResultEntry.MeetsChildCriteria && !sortedAdjacencyLists.ContainsKey(queryResultEntry.TargetId)) && helper.HasWorkItemPermission(queryResultEntry.TargetAreaId, 16))
          {
            if (!roots[queryResultEntry.TargetId])
            {
              roots[queryResultEntry.TargetId] = true;
              queryResultEntry.SourceId = 0;
            }
          }
          else
          {
            roots[queryResultEntry.TargetId] = false;
            TreeLinkUtilities.PreprocessTreeLinkResult(helper, option, sortedAdjacencyLists, roots, queryResultEntry.TargetId);
          }
        }
      }
    }

    private static void FilterTreeLinkResult(
      IPermissionCheckHelper helper,
      QueryRecursionOption option,
      bool hasRhsFilter,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry> rawResult,
      Dictionary<int, List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>> sortedAdjacencyLists,
      Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry current,
      ref int nextPos,
      bool parentMeetsCriteria)
    {
      rawResult[nextPos++] = current;
      bool flag = current.SourceId == 0 || helper.HasWorkItemPermission(current.TargetAreaId, 16);
      bool parentMeetsCriteria1 = option == QueryRecursionOption.ParentFirst && current.SourceId == 0 && current.MeetsParentCriteria || flag && current.MeetsChildCriteria;
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry> queryResultEntryList;
      if (sortedAdjacencyLists.TryGetValue(current.TargetId, out queryResultEntryList))
      {
        sortedAdjacencyLists[current.TargetId] = (List<Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.LinkQueryResultEntry>) null;
        sortedAdjacencyLists.Remove(current.TargetId);
        int num = nextPos;
        for (int index = 0; index < queryResultEntryList.Count; ++index)
          TreeLinkUtilities.FilterTreeLinkResult(helper, option, hasRhsFilter, rawResult, sortedAdjacencyLists, queryResultEntryList[index], ref nextPos, parentMeetsCriteria1);
        if (num != nextPos || parentMeetsCriteria1 || !hasRhsFilter && option == QueryRecursionOption.ParentFirst & parentMeetsCriteria)
          return;
        --nextPos;
      }
      else
      {
        if (parentMeetsCriteria1 || !hasRhsFilter && option == QueryRecursionOption.ParentFirst & parentMeetsCriteria)
          return;
        --nextPos;
      }
    }
  }
}

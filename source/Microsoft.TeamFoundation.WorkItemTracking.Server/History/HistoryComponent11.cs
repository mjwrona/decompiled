// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.HistoryComponent11
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  internal class HistoryComponent11 : HistoryComponent10
  {
    internal override void PrepareGetChangedWorkItemsPageable(
      WorkItemIdRevisionPair workItemIdRevPair,
      int batchSize,
      Guid? projectId,
      IEnumerable<string> types = null,
      bool includeLatestOnly = false,
      bool includeDiscussionChangesOnly = false,
      bool useLegacyDiscussionChanges = false,
      bool includeDiscussionHistory = false)
    {
      base.PrepareGetChangedWorkItemsPageable(workItemIdRevPair, batchSize, projectId, types, includeLatestOnly, includeDiscussionChangesOnly, useLegacyDiscussionChanges, includeDiscussionHistory);
      this.BindBoolean("@includeDiscussionHistory", includeDiscussionHistory);
    }

    public override IEnumerable<WorkItemIdRevisionPair> GetChangedWorkItemsPageable(
      WorkItemIdRevisionPair workItemIdRevPair,
      int batchSize,
      Guid? projectId,
      IEnumerable<string> types = null,
      bool includeLatestOnly = false,
      bool includeDiscussionChangesOnly = false,
      bool useLegacyDiscussionChanges = false,
      bool includeDiscussionHistory = false)
    {
      this.PrepareGetChangedWorkItemsPageable(workItemIdRevPair, batchSize, projectId, types, includeLatestOnly, includeDiscussionChangesOnly, useLegacyDiscussionChanges, includeDiscussionHistory);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), "prc_GetChangedWorkItemsPageable", this.RequestContext))
      {
        resultCollection.AddBinder<WorkItemIdRevisionPair>((ObjectBinder<WorkItemIdRevisionPair>) new HistoryComponent.WorkItemIdRevisionPairBinder());
        List<WorkItemIdRevisionPair> workItemsPageable = new List<WorkItemIdRevisionPair>();
        foreach (WorkItemIdRevisionPair itemIdRevisionPair in resultCollection.GetCurrent<WorkItemIdRevisionPair>())
          workItemsPageable.Add(itemIdRevisionPair);
        return (IEnumerable<WorkItemIdRevisionPair>) workItemsPageable;
      }
    }

    public override WorkItemIdRevisionPair GetWorkItemWatermarkForDate(DateTime date)
    {
      WorkItemIdRevisionPair watermarkForDate = new WorkItemIdRevisionPair();
      this.PrepareStoredProcedure("prc_GetWorkItemWatermarkForDate");
      this.BindDateTime("@dateTime", date.ToUniversalTime());
      watermarkForDate.Watermark = (int) this.ExecuteScalar();
      return watermarkForDate;
    }
  }
}

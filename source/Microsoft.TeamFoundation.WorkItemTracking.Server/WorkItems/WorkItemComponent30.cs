// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent30
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Social.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent30 : WorkItemComponent29
  {
    protected override WorkItemComponent.WorkItemRemoteLinkRecordBinder GetWorkItemRemoteLinkRecordBinder() => (WorkItemComponent.WorkItemRemoteLinkRecordBinder) new WorkItemComponent.WorkItemRemoteLinkRecordBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier));

    public override void ResetCommentCount(int batchSize)
    {
      this.PrepareStoredProcedure("prc_ResetCommentCountOnWorkItems", 3600);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
    }

    public override void CreateOrUpdateWorkItemReactionsAggregateCount(
      int workItemId,
      SocialEngagementType socialEngagementType,
      int incrementCounterValue)
    {
      this.PrepareStoredProcedure("prc_CreateOrUpdateWorkItemReactionsAggregateCount");
      this.BindInt("@workItemId", workItemId);
      this.BindInt("@socialEngagementType", (int) (byte) socialEngagementType);
      this.BindInt("@incrementCounterValue", incrementCounterValue);
      this.ExecuteNonQuery();
    }

    public override IList<WorkItemReactionsCount> GetSortedWorkItemReactionsCount(
      IEnumerable<int> workItemIds,
      SocialEngagementType socialEngagementType)
    {
      this.PrepareStoredProcedure("prc_GetSortedWorkItemReactionsCount");
      this.BindInt32Table("@workItemIds", workItemIds);
      this.BindInt("@socialEngagementType", (int) (byte) socialEngagementType);
      return (IList<WorkItemReactionsCount>) this.ExecuteUnknown<List<WorkItemReactionsCount>>((System.Func<IDataReader, List<WorkItemReactionsCount>>) (reader => new WorkItemComponent.WorkItemReactionsCountBinder().BindAll(reader).ToList<WorkItemReactionsCount>()));
    }
  }
}

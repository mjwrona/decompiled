// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.HistoryComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  internal class HistoryComponent2 : HistoryComponent
  {
    public override IEnumerable<WorkItemIdRevisionPair> GetChangedWorkItems(
      int watermark,
      int batchSize,
      DateTime? startDate = null)
    {
      return this.GetChangedWorkItems(watermark, batchSize, new Guid?(), (IEnumerable<string>) null, startDate);
    }

    public override IEnumerable<WorkItemIdRevisionPair> GetChangedWorkItems(
      int watermark,
      int batchSize,
      Guid? projectId,
      IEnumerable<string> types = null,
      DateTime? startDate = null)
    {
      this.PrepareGetChangedWorkItems(watermark, batchSize, projectId, startDate);
      return this.ExecuteUnknown<IEnumerable<WorkItemIdRevisionPair>>((System.Func<IDataReader, IEnumerable<WorkItemIdRevisionPair>>) (reader => new HistoryComponent.WorkItemIdRevisionPairBinder().BindAll(reader)));
    }

    protected void PrepareGetChangedWorkItems(
      int watermark,
      int batchSize,
      Guid? projectId,
      DateTime? startDate = null)
    {
      int dataspaceId = !projectId.HasValue || !(projectId.Value != Guid.Empty) ? 0 : this.GetDataspaceId(projectId.Value);
      this.PrepareStoredProcedure("prc_GetChangedWorkItems");
      this.BindNullableInt("@dataspaceId", dataspaceId, 0);
      this.BindInt("@watermark", watermark);
      this.BindInt("@batchSize", batchSize);
      if (!startDate.HasValue)
        return;
      this.BindDateTime("@startDate", startDate.Value);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.HistoryComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  internal class HistoryComponent3 : HistoryComponent2
  {
    public override IEnumerable<WorkItemIdRevisionPair> GetChangedWorkItems(
      int watermark,
      int batchSize,
      Guid? projectId,
      IEnumerable<string> types = null,
      DateTime? startDate = null)
    {
      this.PrepareGetChangedWorkItems(watermark, batchSize, projectId, startDate);
      this.BindStringTable("@types", types);
      return this.ExecuteUnknown<IEnumerable<WorkItemIdRevisionPair>>((System.Func<IDataReader, IEnumerable<WorkItemIdRevisionPair>>) (reader => new HistoryComponent.WorkItemIdRevisionPairBinder().BindAll(reader)));
    }
  }
}

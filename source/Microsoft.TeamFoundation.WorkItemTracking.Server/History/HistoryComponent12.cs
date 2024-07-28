// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.HistoryComponent12
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  internal class HistoryComponent12 : HistoryComponent11
  {
    public override WorkItemIdRevisionPair GetWorkItemWatermarkForDate(DateTime date)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemWatermarkForDate");
      this.BindDateTime("@dateTime", date.ToUniversalTime());
      return this.ExecuteUnknown<WorkItemIdRevisionPair>((System.Func<IDataReader, WorkItemIdRevisionPair>) (reader => new HistoryComponent.WorkItemIdRevisionPairBinder().BindAll(reader).FirstOrDefault<WorkItemIdRevisionPair>()));
    }
  }
}

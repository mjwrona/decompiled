// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1150
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1150 : EventNotificationComponent1130
  {
    public override List<TeamFoundationEvent> GetUnprocessedEvents(
      int eventBatchSize,
      IEnumerable<string> processQueues)
    {
      this.PrepareStoredProcedure("prc_GetUnprocessedEvents2");
      this.BindInt("@batchSize", eventBatchSize);
      this.BindStringTable("@processQueues", processQueues, true, 105, false);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationEvent>(this.GetTeamFoundationEventVersion());
        return resultCollection.GetCurrent<TeamFoundationEvent>().Items;
      }
    }

    protected override void BindEventEntryRow(
      SqlDataRecord record,
      SerializedNotificationEvent notifEvent)
    {
      base.BindEventEntryRow(record, notifEvent);
      record.SetString(6, notifEvent.ProcessQueue ?? string.Empty);
    }

    protected override SqlDataRecord NewEventEntryRecord() => new SqlDataRecord(NotificationEventTypes.typ_EventEntryTable6);

    protected override string EventEntryTableName => "typ_EventEntryTable6";
  }
}

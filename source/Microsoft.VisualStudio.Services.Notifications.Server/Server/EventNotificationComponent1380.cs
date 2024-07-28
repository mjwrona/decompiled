// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1380
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1380 : EventNotificationComponent1360
  {
    public override NotificationEventBacklogStatus QueryNotificationBacklogStatus(
      int maxAllowedDelayDays,
      HashSet<Tuple<string, string>> processQueueChannels)
    {
      NotificationEventBacklogStatus eventBacklogStatus = new NotificationEventBacklogStatus();
      this.PrepareStoredProcedure("prc_QueryNotificationBacklogStatus");
      this.BindInt("@maxAllowedDelayDays", maxAllowedDelayDays);
      this.BindProcessQueueAndProcessQueueChannelsTable("@processQueues", "@processQueueChannels", processQueueChannels);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<EventBacklogStatus>((ObjectBinder<EventBacklogStatus>) this.GetEventBacklogStatusVersion());
      resultCollection.AddBinder<NotificationBacklogStatus>((ObjectBinder<NotificationBacklogStatus>) this.GetNotificationBacklogStatusVersion());
      eventBacklogStatus.EventBacklogStatus = resultCollection.GetCurrent<EventBacklogStatus>().Items;
      resultCollection.NextResult();
      eventBacklogStatus.NotificationBacklogStatus = resultCollection.GetCurrent<NotificationBacklogStatus>().Items;
      return eventBacklogStatus;
    }

    public override DateTime GetNextEventProcessTime(IEnumerable<string> processQueues)
    {
      this.PrepareStoredProcedure("prc_GetNextEventProcessTime");
      this.BindProcessQueueTable("@processQueues", processQueues.ToHashSet());
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetNextProcessedTime(rc);
    }

    public override List<TeamFoundationEvent> GetUnprocessedEvents(
      int eventBatchSize,
      IEnumerable<string> processQueues)
    {
      this.PrepareStoredProcedure("prc_GetUnprocessedEvents2");
      this.BindInt("@batchSize", eventBatchSize);
      this.BindProcessQueueTable("@processQueues", processQueues.ToHashSet());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationEvent>(this.GetTeamFoundationEventVersion());
        return resultCollection.GetCurrent<TeamFoundationEvent>().Items;
      }
    }

    protected virtual SqlParameter BindProcessQueueTable(
      string fieldName,
      HashSet<string> processQueues)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (string processQueue in processQueues)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_ProcessQueueTable);
        sqlDataRecord.SetString(0, processQueue);
        rows.Add(sqlDataRecord);
      }
      return this.BindTable(fieldName, "typ_ProcessQueueTable", (IEnumerable<SqlDataRecord>) rows);
    }

    protected virtual SqlParameter BindProcessQueueChannelsTable(
      string fieldName,
      HashSet<Tuple<string, string>> processQueueChannels,
      HashSet<string> processQueues = null)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (Tuple<string, string> processQueueChannel in processQueueChannels)
      {
        processQueues?.Add(processQueueChannel.Item1);
        if (!string.IsNullOrEmpty(processQueueChannel.Item2))
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_ProcessQueueChannelTable);
          sqlDataRecord.SetString(0, processQueueChannel.Item1);
          sqlDataRecord.SetString(1, processQueueChannel.Item2);
          rows.Add(sqlDataRecord);
        }
      }
      return this.BindTable(fieldName, "typ_ProcessQueueChannelTable", (IEnumerable<SqlDataRecord>) rows);
    }

    protected virtual void BindProcessQueueAndProcessQueueChannelsTable(
      string pqFieldName,
      string pqChannelFieldName,
      HashSet<Tuple<string, string>> processQueueChannels)
    {
      HashSet<string> processQueues = new HashSet<string>();
      this.BindProcessQueueChannelsTable(pqChannelFieldName, processQueueChannels, processQueues);
      this.BindProcessQueueTable(pqFieldName, processQueues);
    }

    internal override EventBacklogStatusBinder GetEventBacklogStatusVersion() => (EventBacklogStatusBinder) new EventBacklogStatusBinder1380();

    internal override NotificationBacklogStatusBinder GetNotificationBacklogStatusVersion() => (NotificationBacklogStatusBinder) new NotificationBacklogStatusBinder1380();
  }
}

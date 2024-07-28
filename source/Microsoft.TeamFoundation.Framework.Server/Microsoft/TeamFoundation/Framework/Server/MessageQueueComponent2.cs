// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageQueueComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MessageQueueComponent2 : MessageQueueComponent
  {
    private static readonly SqlMetaData[] typ_MessageQueueAcknowledgementTable = new SqlMetaData[3]
    {
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("LowerMessageId", SqlDbType.BigInt),
      new SqlMetaData("UpperMessageId", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] typ_MessageQueueDispatchTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("SequenceId", SqlDbType.Int),
      new SqlMetaData("SessionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("LastMessageId", SqlDbType.BigInt)
    };

    protected SqlParameter BindMessageQueueAcknowledgementTable(
      string parameterName,
      IEnumerable<AcknowledgementRange> rows)
    {
      rows = rows ?? Enumerable.Empty<AcknowledgementRange>();
      System.Func<AcknowledgementRange, SqlDataRecord> selector = (System.Func<AcknowledgementRange, SqlDataRecord>) (acknowledgementRange =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(MessageQueueComponent2.typ_MessageQueueAcknowledgementTable);
        sqlDataRecord.SetInt32(0, acknowledgementRange.QueueId);
        sqlDataRecord.SetInt64(1, acknowledgementRange.Lower);
        sqlDataRecord.SetInt64(2, acknowledgementRange.Upper);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_MessageQueueAcknowledgementTable", rows.Select<AcknowledgementRange, SqlDataRecord>(selector));
    }

    protected override SqlParameter BindMessageQueueDispatchTable(
      string parameterName,
      IEnumerable<DequeueOperation> rows)
    {
      rows = rows ?? Enumerable.Empty<DequeueOperation>();
      System.Func<DequeueOperation, SqlDataRecord> selector = (System.Func<DequeueOperation, SqlDataRecord>) (operation =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(MessageQueueComponent2.typ_MessageQueueDispatchTable2);
        sqlDataRecord.SetInt32(0, operation.Queue.Id);
        sqlDataRecord.SetInt32(1, operation.SequenceId);
        sqlDataRecord.SetGuid(2, operation.SessionId);
        if (operation.LastMessageId <= 0L)
          sqlDataRecord.SetDBNull(3);
        else
          sqlDataRecord.SetInt64(3, operation.LastMessageId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_MessageQueueDispatchTable2", rows.Select<DequeueOperation, SqlDataRecord>(selector));
    }

    public override ResultCollection LoadMessageQueues()
    {
      this.TraceEnter(0, nameof (LoadMessageQueues));
      this.PrepareStoredProcedure("prc_LoadMessageQueues");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationMessageQueue>((ObjectBinder<TeamFoundationMessageQueue>) new MessageQueueBinder2());
      this.TraceLeave(0, nameof (LoadMessageQueues));
      return resultCollection;
    }

    public override TeamFoundationMessageQueue RegisterMessageQueue(string name, string description)
    {
      this.TraceEnter(0, nameof (RegisterMessageQueue));
      this.PrepareStoredProcedure("prc_RegisterMessageQueue");
      this.BindString("@queueName", name, 128, true, SqlDbType.NVarChar);
      this.BindString("@description", description, 2048, true, SqlDbType.NVarChar);
      this.BindGuid("@eventAuthor", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationMessageQueue>((ObjectBinder<TeamFoundationMessageQueue>) new MessageQueueBinder2());
        ObjectBinder<TeamFoundationMessageQueue> current = resultCollection.GetCurrent<TeamFoundationMessageQueue>();
        current.MoveNext();
        this.TraceLeave(0, nameof (RegisterMessageQueue));
        return current.Current;
      }
    }

    public override ResultCollection DispatchMessageQueues(
      IEnumerable<DequeueOperation> operations,
      IEnumerable<AcknowledgementRange> acknowledgements)
    {
      this.TraceEnter(0, nameof (DispatchMessageQueues));
      this.PrepareStoredProcedure("prc_DispatchMessageQueues");
      this.BindMessageQueueDispatchTable("@dispatchTable", operations);
      this.BindMessageQueueAcknowledgementTable("@acknowledgementTable", acknowledgements);
      this.BindGuid("@eventAuthor", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsmqDequeueData>((ObjectBinder<TfsmqDequeueData>) new DequeueMessageDataBinder());
      resultCollection.AddBinder<TfsmqConnectionNotification>((ObjectBinder<TfsmqConnectionNotification>) new ConnectionNotificationBinder2());
      this.TraceLeave(0, nameof (DispatchMessageQueues));
      return resultCollection;
    }

    public override ResultCollection SetMessageQueuesOffline(
      IEnumerable<KeyValuePair<int, DateTime>> pendingDisconnections,
      bool force = false)
    {
      this.TraceEnter(0, nameof (SetMessageQueuesOffline));
      this.PrepareStoredProcedure("prc_SetMessageQueuesOffline", 600);
      this.BindKeyValuePairInt32DateTimeTable("@disconnectionTable", pendingDisconnections);
      this.BindBoolean("@force", force);
      this.BindGuid("@eventAuthor", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsmqConnectionNotification>((ObjectBinder<TfsmqConnectionNotification>) new ConnectionNotificationBinder2());
      this.TraceLeave(0, nameof (SetMessageQueuesOffline));
      return resultCollection;
    }
  }
}

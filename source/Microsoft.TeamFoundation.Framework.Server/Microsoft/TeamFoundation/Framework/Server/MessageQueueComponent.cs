// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageQueueComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MessageQueueComponent : TeamFoundationSqlResourceComponent
  {
    public const string MessageQueueDataspaceCategory = "MessageQueue";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<MessageQueueComponent>(1, true),
      (IComponentCreator) new ComponentCreator<MessageQueueComponent2>(2),
      (IComponentCreator) new ComponentCreator<MessageQueueComponent3>(3),
      (IComponentCreator) new ComponentCreator<MessageQueueComponent4>(4),
      (IComponentCreator) new ComponentCreator<MessageQueueComponent5>(5),
      (IComponentCreator) new ComponentCreator<MessageQueueComponent6>(6)
    }, "MessageQueue", "MessageQueue", 6);
    private static readonly SqlMetaData[] typ_MessageQueueDispatchTable = new SqlMetaData[2]
    {
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("LastMessageId", SqlDbType.BigInt)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static MessageQueueComponent()
    {
      MessageQueueComponent.s_sqlExceptionFactories.Add(800048, new SqlExceptionFactory(typeof (MessageQueueAlreadyExistsException)));
      MessageQueueComponent.s_sqlExceptionFactories.Add(800049, new SqlExceptionFactory(typeof (MessageQueueNotFoundException)));
    }

    protected override string TraceArea => "MessageQueue";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) MessageQueueComponent.s_sqlExceptionFactories;

    protected virtual SqlParameter BindMessageQueueDispatchTable(
      string parameterName,
      IEnumerable<DequeueOperation> rows)
    {
      rows = rows ?? Enumerable.Empty<DequeueOperation>();
      System.Func<DequeueOperation, SqlDataRecord> selector = (System.Func<DequeueOperation, SqlDataRecord>) (operation =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(MessageQueueComponent.typ_MessageQueueDispatchTable);
        sqlDataRecord.SetInt32(0, operation.QueueId);
        if (operation.LastMessageId <= 0L)
          sqlDataRecord.SetDBNull(1);
        else
          sqlDataRecord.SetInt64(1, operation.LastMessageId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_MessageQueueDispatchTable", rows.Select<DequeueOperation, SqlDataRecord>(selector));
    }

    public virtual ResultCollection DispatchMessageQueues(
      IEnumerable<DequeueOperation> operations,
      IEnumerable<AcknowledgementRange> acknowledgements)
    {
      this.TraceEnter(0, nameof (DispatchMessageQueues));
      this.PrepareStoredProcedure("prc_DispatchMessageQueues");
      this.BindMessageQueueDispatchTable("@dispatchTable", operations);
      this.BindGuid("@eventAuthor", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsmqDequeueData>((ObjectBinder<TfsmqDequeueData>) new DequeueMessageDataBinder());
      resultCollection.AddBinder<TfsmqConnectionNotification>((ObjectBinder<TfsmqConnectionNotification>) new ConnectionNotificationBinder());
      this.TraceLeave(0, nameof (DispatchMessageQueues));
      return resultCollection;
    }

    public virtual void EmptyMessageQueue(string queueName) => throw new NotImplementedException();

    public virtual long EnqueueMessage(string queueName, string contentType, string body)
    {
      this.TraceEnter(0, nameof (EnqueueMessage));
      this.PrepareStoredProcedure("prc_EnqueueMessage");
      this.BindString("@queueName", queueName, 128, false, SqlDbType.NVarChar);
      this.BindString("@message", body, int.MaxValue, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (EnqueueMessage));
      return 0;
    }

    public virtual Task<long> EnqueueMessageAsync(
      string queueName,
      string contentType,
      string body)
    {
      return Task.FromResult<long>(this.EnqueueMessage(queueName, contentType, body));
    }

    public virtual TeamFoundationMessageQueue GetMessageQueue(TeamFoundationMessageQueue queue) => queue;

    public virtual ResultCollection LoadMessageQueues()
    {
      this.TraceEnter(0, nameof (LoadMessageQueues));
      this.PrepareStoredProcedure("prc_LoadMessageQueues");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationMessageQueue>((ObjectBinder<TeamFoundationMessageQueue>) new MessageQueueBinder());
      this.TraceLeave(0, nameof (LoadMessageQueues));
      return resultCollection;
    }

    public virtual TeamFoundationMessageQueue RegisterMessageQueue(string name, string description)
    {
      this.TraceEnter(0, nameof (RegisterMessageQueue));
      this.PrepareStoredProcedure("prc_RegisterMessageQueue");
      this.BindString("@queueName", name, 128, true, SqlDbType.NVarChar);
      this.BindString("@description", description, 2048, true, SqlDbType.NVarChar);
      this.BindGuid("@eventAuthor", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationMessageQueue>((ObjectBinder<TeamFoundationMessageQueue>) new MessageQueueBinder());
        ObjectBinder<TeamFoundationMessageQueue> current = resultCollection.GetCurrent<TeamFoundationMessageQueue>();
        current.MoveNext();
        this.TraceLeave(0, nameof (RegisterMessageQueue));
        return current.Current;
      }
    }

    public virtual ResultCollection SetMessageQueuesOffline(
      IEnumerable<KeyValuePair<int, DateTime>> pendingDisconnections,
      bool force = false)
    {
      this.TraceEnter(0, nameof (SetMessageQueuesOffline));
      this.PrepareStoredProcedure("prc_SetMessageQueuesOffline", 600);
      this.BindKeyValuePairInt32DateTimeTable("@disconnectionTable", pendingDisconnections);
      this.BindGuid("@eventAuthor", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsmqConnectionNotification>((ObjectBinder<TfsmqConnectionNotification>) new ConnectionNotificationBinder());
      this.TraceLeave(0, nameof (SetMessageQueuesOffline));
      return resultCollection;
    }

    public void UnregisterMessageQueue(string queueName)
    {
      this.TraceEnter(0, "SetMessageQueuesOffline");
      this.PrepareStoredProcedure("prc_UnregisterMessageQueue");
      this.BindString("@queueName", queueName, 128, true, SqlDbType.NVarChar);
      this.BindGuid("@eventAuthor", this.Author);
      this.TraceLeave(0, "SetMessageQueuesOffline");
      this.ExecuteNonQuery();
    }
  }
}

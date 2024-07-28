// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageQueueComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MessageQueueComponent5 : MessageQueueComponent4
  {
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
      resultCollection.AddBinder<TfsmqDequeueData>((ObjectBinder<TfsmqDequeueData>) new DequeueMessageDataBinder2());
      resultCollection.AddBinder<TfsmqConnectionNotification>((ObjectBinder<TfsmqConnectionNotification>) new ConnectionNotificationBinder2());
      this.TraceLeave(0, nameof (DispatchMessageQueues));
      return resultCollection;
    }

    public override long EnqueueMessage(string queueName, string messageType, string message)
    {
      this.TraceEnter(0, nameof (EnqueueMessage));
      this.PrepareStoredProcedure("prc_EnqueueMessage");
      this.BindString("@queueName", queueName, 128, false, SqlDbType.NVarChar);
      this.BindString("@messageType", messageType, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@message", message, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindGuid("@eventAuthor", this.Author);
      long num = (long) this.ExecuteScalar();
      this.TraceLeave(0, nameof (EnqueueMessage));
      return num;
    }

    public override async Task<long> EnqueueMessageAsync(
      string queueName,
      string messageType,
      string message)
    {
      MessageQueueComponent5 messageQueueComponent5 = this;
      messageQueueComponent5.TraceEnter(0, "EnqueueMessage");
      messageQueueComponent5.PrepareStoredProcedure("prc_EnqueueMessage");
      messageQueueComponent5.BindString("@queueName", queueName, 128, false, SqlDbType.NVarChar);
      messageQueueComponent5.BindString("@messageType", messageType, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      messageQueueComponent5.BindString("@message", message, int.MaxValue, false, SqlDbType.NVarChar);
      messageQueueComponent5.BindGuid("@eventAuthor", messageQueueComponent5.Author);
      long num = (long) await messageQueueComponent5.ExecuteScalarAsync();
      messageQueueComponent5.TraceLeave(0, "EnqueueMessage");
      return num;
    }

    public override TeamFoundationMessageQueue GetMessageQueue(TeamFoundationMessageQueue queue)
    {
      this.TraceEnter(0, nameof (GetMessageQueue));
      this.PrepareStoredProcedure("prc_GetMessageQueue");
      this.BindInt("@queueId", queue.Id);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationMessageQueue>((ObjectBinder<TeamFoundationMessageQueue>) new MessageQueueBinder2());
        queue = resultCollection.GetCurrent<TeamFoundationMessageQueue>().FirstOrDefault<TeamFoundationMessageQueue>();
      }
      this.TraceLeave(0, nameof (GetMessageQueue));
      return queue;
    }
  }
}

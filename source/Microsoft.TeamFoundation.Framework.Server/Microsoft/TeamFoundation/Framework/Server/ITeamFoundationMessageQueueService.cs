// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationMessageQueueService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationMessageQueueService))]
  public interface ITeamFoundationMessageQueueService : IVssFrameworkService
  {
    TimeSpan IdleTimeout { get; }

    TimeSpan OfflineTimeout { get; }

    void CreateQueue(IVssRequestContext requestContext, string queueName, string description);

    bool QueueExists(IVssRequestContext requestContext, string queueName);

    void DeleteQueue(IVssRequestContext requestContext, string queueName);

    void EmptyQueue(IVssRequestContext requestContext, string queueName);

    IAsyncResult BeginAcknowledge(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      IList<AcknowledgementRange> ranges,
      TimeSpan timeout,
      AsyncCallback callback,
      object state);

    IAsyncResult BeginAcknowledge(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      IList<AcknowledgementRange> ranges,
      MessageHeaders headers,
      TimeSpan timeout,
      AsyncCallback callback,
      object state);

    void EndAcknowledge(IAsyncResult result);

    IAsyncResult BeginDequeue(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long lastMessageId,
      IList<AcknowledgementRange> ranges,
      TimeSpan timeout,
      AsyncCallback callback,
      object state);

    IAsyncResult BeginDequeue(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long lastMessageId,
      IList<AcknowledgementRange> ranges,
      MessageHeaders headers,
      TimeSpan timeout,
      AsyncCallback callback,
      object state);

    Message EndDequeue(IAsyncResult result);

    void EnqueueMessage(IVssRequestContext requestContext, string queueName, Message message);

    long EnqueueMessage(
      IVssRequestContext requestContext,
      string queueName,
      string messageType,
      string message);

    Task<long> EnqueueMessageAsync(
      IVssRequestContext requestContext,
      string queueName,
      string messageType,
      string message);

    Task<MessageContainer> GetMessageAsync(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      TimeSpan timeout,
      long? lastMessageId = null);

    Task DeleteMessagesAsync(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long[] messageIds,
      TimeSpan timeout);

    Task DeleteMessagesAsync(
      IVssRequestContext requestContext,
      string queueName,
      Guid sessionId,
      long messageId,
      TimeSpan timeout);

    MessageQueueStatus GetQueueConnectionStatus(
      IVssRequestContext requestContext,
      string queueName,
      out DateTime lastConnectedOn);

    void SetQueueOffline(IVssRequestContext requestContext, string queueName);
  }
}

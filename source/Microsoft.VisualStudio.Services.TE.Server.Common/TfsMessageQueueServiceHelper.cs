// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TfsMessageQueueServiceHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [ExcludeFromCodeCoverage]
  internal class TfsMessageQueueServiceHelper : ITfsMessageQueueServiceHelper
  {
    public void CreateQueue(
      TestExecutionRequestContext requestContext,
      string queueName,
      string description)
    {
      requestContext.RequestContext.GetService<TeamFoundationMessageQueueService>().CreateQueue(requestContext.RequestContext, queueName, description);
    }

    public void DeleteQueue(TestExecutionRequestContext requestContext, string queueName) => requestContext.RequestContext.GetService<TeamFoundationMessageQueueService>().DeleteQueue(requestContext.RequestContext, queueName);

    public void EnqueueMessage(
      TestExecutionRequestContext requestContext,
      string queueName,
      string messageType,
      string message)
    {
      requestContext.RequestContext.GetService<TeamFoundationMessageQueueService>().EnqueueMessage(requestContext.RequestContext, queueName, messageType, message);
    }

    public Task<MessageContainer> GetMessageAsync(
      TestExecutionRequestContext requestContext,
      string queueName,
      Guid sessionId,
      TimeSpan timeout,
      long? lastMessageId = null)
    {
      return requestContext.RequestContext.GetService<TeamFoundationMessageQueueService>().GetMessageAsync(requestContext.RequestContext, queueName, sessionId, timeout, lastMessageId);
    }

    public bool QueueExists(TestExecutionRequestContext requestContext, string queueName) => requestContext.RequestContext.GetService<TeamFoundationMessageQueueService>().QueueExists(requestContext.RequestContext, queueName);

    public void EmptyQueue(TestExecutionRequestContext requestContext, string queueName) => requestContext.RequestContext.GetService<TeamFoundationMessageQueueService>().EmptyQueue(requestContext.RequestContext, queueName);
  }
}

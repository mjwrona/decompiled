// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedObserverContextCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedObserverContextCore
  {
    private readonly PartitionCheckpointer checkpointer;
    private readonly ResponseMessage responseMessage;

    internal ChangeFeedObserverContextCore(
      string leaseToken,
      ResponseMessage feedResponse,
      PartitionCheckpointer checkpointer)
    {
      this.LeaseToken = leaseToken;
      this.responseMessage = feedResponse;
      this.checkpointer = checkpointer;
    }

    public string LeaseToken { get; }

    public CosmosDiagnostics Diagnostics => this.responseMessage.Diagnostics;

    public Microsoft.Azure.Cosmos.Headers Headers => this.responseMessage.Headers;

    public async Task CheckpointAsync()
    {
      try
      {
        await this.checkpointer.CheckpointPartitionAsync(this.responseMessage.Headers.ContinuationToken);
      }
      catch (LeaseLostException ex)
      {
        throw CosmosExceptionFactory.Create(HttpStatusCode.PreconditionFailed, "Lease was lost due to load balancing and will be processed by another instance", ex.StackTrace, new Microsoft.Azure.Cosmos.Headers(), (ITrace) NoOpTrace.Singleton, (Error) null, (Exception) ex);
      }
    }

    public static implicit operator ChangeFeedProcessorContext(
      ChangeFeedObserverContextCore contextCore)
    {
      return (ChangeFeedProcessorContext) new ChangeFeedProcessorContextCore(contextCore);
    }
  }
}

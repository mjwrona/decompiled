// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing.FeedProcessorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.DocDBErrors;
using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing
{
  internal sealed class FeedProcessorCore : FeedProcessor
  {
    private readonly ProcessorOptions options;
    private readonly PartitionCheckpointer checkpointer;
    private readonly ChangeFeedObserver observer;
    private readonly FeedIterator resultSetIterator;

    public FeedProcessorCore(
      ChangeFeedObserver observer,
      FeedIterator resultSetIterator,
      ProcessorOptions options,
      PartitionCheckpointer checkpointer)
    {
      this.observer = observer ?? throw new ArgumentNullException(nameof (observer));
      this.options = options ?? throw new ArgumentNullException(nameof (options));
      this.checkpointer = checkpointer ?? throw new ArgumentNullException(nameof (checkpointer));
      this.resultSetIterator = resultSetIterator ?? throw new ArgumentNullException(nameof (resultSetIterator));
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
      string lastContinuation = this.options.StartContinuation;
      while (!cancellationToken.IsCancellationRequested)
      {
        TimeSpan delay = this.options.FeedPollDelay;
        try
        {
          do
          {
            Task<ResponseMessage> task1 = this.resultSetIterator.ReadNextAsync(cancellationToken);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
              if (await Task.WhenAny((Task) task1, Task.Delay(this.options.RequestTimeout, cts.Token)) != task1)
              {
                task1.ContinueWith((Action<Task<ResponseMessage>>) (task => DefaultTrace.TraceInformation("Timed out Change Feed request failed with exception: {2}", (object) task.Exception.InnerException)), TaskContinuationOptions.OnlyOnFaulted);
                throw CosmosExceptionFactory.CreateRequestTimeoutException("Change Feed request timed out", new Headers());
              }
              cts.Cancel();
            }
            ResponseMessage responseMessage = await task1;
            if (responseMessage.StatusCode != HttpStatusCode.NotModified && !responseMessage.IsSuccessStatusCode)
            {
              DefaultTrace.TraceWarning("unsuccessful feed read: lease token '{0}' status code {1}. substatuscode {2}", (object) this.options.LeaseToken, (object) responseMessage.StatusCode, (object) responseMessage.Headers.SubStatusCode);
              this.HandleFailedRequest(responseMessage, lastContinuation);
              TimeSpan? retryAfter = responseMessage.Headers.RetryAfter;
              if (retryAfter.HasValue)
              {
                retryAfter = responseMessage.Headers.RetryAfter;
                delay = retryAfter.Value;
                break;
              }
              break;
            }
            lastContinuation = responseMessage.Headers.ContinuationToken;
            if (this.resultSetIterator.HasMoreResults)
              await this.DispatchChangesAsync(responseMessage, cancellationToken).ConfigureAwait(false);
            task1 = (Task<ResponseMessage>) null;
            if (!this.resultSetIterator.HasMoreResults)
              break;
          }
          while (!cancellationToken.IsCancellationRequested);
        }
        catch (OperationCanceledException ex)
        {
          if (cancellationToken.IsCancellationRequested)
          {
            throw;
          }
          else
          {
            Extensions.TraceException((Exception) ex);
            DefaultTrace.TraceWarning("exception: lease token '{0}'", (object) this.options.LeaseToken);
          }
        }
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        delay = new TimeSpan();
      }
      lastContinuation = (string) null;
    }

    private void HandleFailedRequest(ResponseMessage responseMessage, string lastContinuation)
    {
      DocDbError docDbError = ExceptionClassifier.ClassifyStatusCodes(responseMessage.StatusCode, (int) responseMessage.Headers.SubStatusCode);
      switch (docDbError)
      {
        case DocDbError.Undefined:
          throw CosmosExceptionFactory.Create(responseMessage);
        case DocDbError.PartitionSplit:
          throw new FeedRangeGoneException("Partition split.", lastContinuation);
        default:
          DefaultTrace.TraceCritical(string.Format("Unrecognized DocDbError enum value {0}", (object) docDbError));
          throw new InvalidOperationException(string.Format("Unrecognized DocDbError enum value {0} for status code {1} and substatus code {2}", (object) docDbError, (object) responseMessage.StatusCode, (object) responseMessage.Headers.SubStatusCode));
      }
    }

    private Task DispatchChangesAsync(ResponseMessage response, CancellationToken cancellationToken) => this.observer.ProcessChangesAsync(new ChangeFeedObserverContextCore(this.options.LeaseToken, response, this.checkpointer), response.Content, cancellationToken);
  }
}

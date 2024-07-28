// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement.PartitionSupervisorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement
{
  internal sealed class PartitionSupervisorCore : PartitionSupervisor
  {
    private readonly DocumentServiceLease lease;
    private readonly ChangeFeedObserver observer;
    private readonly FeedProcessor processor;
    private readonly LeaseRenewer renewer;
    private readonly CancellationTokenSource renewerCancellation = new CancellationTokenSource();
    private CancellationTokenSource processorCancellation;

    public PartitionSupervisorCore(
      DocumentServiceLease lease,
      ChangeFeedObserver observer,
      FeedProcessor processor,
      LeaseRenewer renewer)
    {
      this.lease = lease;
      this.observer = observer;
      this.processor = processor;
      this.renewer = renewer;
    }

    public override async Task RunAsync(CancellationToken shutdownToken)
    {
      PartitionSupervisorCore partitionSupervisorCore = this;
      ConfiguredTaskAwaitable configuredTaskAwaitable = partitionSupervisorCore.observer.OpenAsync(partitionSupervisorCore.lease.CurrentLeaseToken).ConfigureAwait(false);
      await configuredTaskAwaitable;
      partitionSupervisorCore.processorCancellation = CancellationTokenSource.CreateLinkedTokenSource(shutdownToken);
      Task processorTask = partitionSupervisorCore.processor.RunAsync(partitionSupervisorCore.processorCancellation.Token);
      // ISSUE: reference to a compiler-generated method
      processorTask.ContinueWith(new Action<Task>(partitionSupervisorCore.\u003CRunAsync\u003Eb__7_0)).LogException();
      Task task = partitionSupervisorCore.renewer.RunAsync(partitionSupervisorCore.renewerCancellation.Token);
      // ISSUE: reference to a compiler-generated method
      task.ContinueWith(new Action<Task>(partitionSupervisorCore.\u003CRunAsync\u003Eb__7_1)).LogException();
      ChangeFeedObserverCloseReason closeReason = shutdownToken.IsCancellationRequested ? ChangeFeedObserverCloseReason.Shutdown : ChangeFeedObserverCloseReason.Unknown;
      try
      {
        configuredTaskAwaitable = Task.WhenAll(processorTask, task).ConfigureAwait(false);
        await configuredTaskAwaitable;
      }
      catch (LeaseLostException ex)
      {
        closeReason = ChangeFeedObserverCloseReason.LeaseLost;
        throw;
      }
      catch (FeedRangeGoneException ex)
      {
        closeReason = ChangeFeedObserverCloseReason.LeaseGone;
        throw;
      }
      catch (CosmosException ex)
      {
        closeReason = ChangeFeedObserverCloseReason.CosmosException;
        throw;
      }
      catch (OperationCanceledException ex) when (shutdownToken.IsCancellationRequested)
      {
        closeReason = ChangeFeedObserverCloseReason.Shutdown;
      }
      catch (ChangeFeedProcessorUserException ex)
      {
        closeReason = ChangeFeedObserverCloseReason.ObserverError;
        throw;
      }
      catch (Exception ex) when (processorTask.IsFaulted)
      {
        closeReason = ChangeFeedObserverCloseReason.Unknown;
        throw;
      }
      finally
      {
        await partitionSupervisorCore.observer.CloseAsync(partitionSupervisorCore.lease.CurrentLeaseToken, closeReason).ConfigureAwait(false);
      }
      processorTask = (Task) null;
    }

    public override void Dispose()
    {
      this.processorCancellation?.Dispose();
      this.renewerCancellation.Dispose();
    }
  }
}

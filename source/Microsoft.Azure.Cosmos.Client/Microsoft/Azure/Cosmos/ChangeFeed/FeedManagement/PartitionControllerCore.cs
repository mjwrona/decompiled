// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement.PartitionControllerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement
{
  internal sealed class PartitionControllerCore : PartitionController
  {
    private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> currentlyOwnedPartitions = new ConcurrentDictionary<string, TaskCompletionSource<bool>>();
    private readonly DocumentServiceLeaseContainer leaseContainer;
    private readonly DocumentServiceLeaseManager leaseManager;
    private readonly PartitionSupervisorFactory partitionSupervisorFactory;
    private readonly PartitionSynchronizer synchronizer;
    private readonly ChangeFeedProcessorHealthMonitor monitor;
    private CancellationTokenSource shutdownCts;

    public PartitionControllerCore(
      DocumentServiceLeaseContainer leaseContainer,
      DocumentServiceLeaseManager leaseManager,
      PartitionSupervisorFactory partitionSupervisorFactory,
      PartitionSynchronizer synchronizer,
      ChangeFeedProcessorHealthMonitor monitor)
    {
      this.leaseContainer = leaseContainer;
      this.leaseManager = leaseManager;
      this.partitionSupervisorFactory = partitionSupervisorFactory;
      this.synchronizer = synchronizer;
      this.monitor = monitor;
    }

    public override async Task InitializeAsync()
    {
      this.shutdownCts = new CancellationTokenSource();
      await this.LoadLeasesAsync().ConfigureAwait(false);
    }

    public override async Task AddOrUpdateLeaseAsync(DocumentServiceLease lease)
    {
      TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
      if (!this.currentlyOwnedPartitions.TryAdd(lease.CurrentLeaseToken, completionSource))
      {
        DocumentServiceLease documentServiceLease = await this.leaseManager.UpdatePropertiesAsync(lease).ConfigureAwait(false);
        DefaultTrace.TraceVerbose("Lease with token {0}: updated", (object) lease.CurrentLeaseToken);
      }
      else
      {
        try
        {
          DocumentServiceLease documentServiceLease = await this.leaseManager.AcquireAsync(lease).ConfigureAwait(false);
          if (documentServiceLease != null)
            lease = documentServiceLease;
          await this.monitor.NotifyLeaseAcquireAsync(lease.CurrentLeaseToken);
        }
        catch (Exception ex)
        {
          await this.RemoveLeaseAsync(lease, false).ConfigureAwait(false);
          if (ex is LeaseLostException leaseLostException)
          {
            if (leaseLostException.InnerException != null)
              await this.monitor.NotifyErrorAsync(lease.CurrentLeaseToken, leaseLostException.InnerException);
          }
          else
            await this.monitor.NotifyErrorAsync(lease.CurrentLeaseToken, ex);
          throw;
        }
        this.ProcessPartitionAsync(this.partitionSupervisorFactory.Create(lease), lease).LogException();
      }
    }

    public override async Task ShutdownAsync()
    {
      this.shutdownCts.Cancel();
      await Task.WhenAll((IEnumerable<Task>) this.currentlyOwnedPartitions.Select<KeyValuePair<string, TaskCompletionSource<bool>>, Task<bool>>((Func<KeyValuePair<string, TaskCompletionSource<bool>>, Task<bool>>) (pair => pair.Value.Task)).ToList<Task<bool>>()).ConfigureAwait(false);
    }

    private async Task LoadLeasesAsync()
    {
      PartitionControllerCore partitionControllerCore = this;
      DefaultTrace.TraceVerbose("Starting renew leases assigned to this host on initialize.");
      List<Task> addLeaseTasks = new List<Task>();
      foreach (DocumentServiceLease lease in await partitionControllerCore.leaseContainer.GetOwnedLeasesAsync().ConfigureAwait(false))
      {
        DefaultTrace.TraceInformation("Acquired lease with token '{0}' on startup.", (object) lease.CurrentLeaseToken);
        addLeaseTasks.Add(partitionControllerCore.AddOrUpdateLeaseAsync(lease));
      }
      await Task.WhenAll(addLeaseTasks.ToArray()).ConfigureAwait(false);
      addLeaseTasks = (List<Task>) null;
    }

    private async Task RemoveLeaseAsync(DocumentServiceLease lease, bool wasAcquired)
    {
      TaskCompletionSource<bool> worker;
      if (!this.currentlyOwnedPartitions.TryRemove(lease.CurrentLeaseToken, out worker))
      {
        worker = (TaskCompletionSource<bool>) null;
      }
      else
      {
        try
        {
          try
          {
            await this.leaseManager.ReleaseAsync(lease).ConfigureAwait(false);
            await this.monitor.NotifyLeaseReleaseAsync(lease.CurrentLeaseToken);
          }
          catch (LeaseLostException ex)
          {
            if (wasAcquired)
              await this.monitor.NotifyLeaseReleaseAsync(lease.CurrentLeaseToken);
            DefaultTrace.TraceVerbose("Lease with token {0}: taken by another host during release", (object) lease.CurrentLeaseToken);
          }
          catch (Exception ex)
          {
            await this.monitor.NotifyErrorAsync(lease.CurrentLeaseToken, ex);
            DefaultTrace.TraceWarning("Lease with token {0}: failed to remove lease", (object) lease.CurrentLeaseToken);
          }
          worker = (TaskCompletionSource<bool>) null;
        }
        finally
        {
          worker.SetResult(false);
        }
      }
    }

    private async Task ProcessPartitionAsync(
      PartitionSupervisor partitionSupervisor,
      DocumentServiceLease lease)
    {
      ConfiguredTaskAwaitable configuredTaskAwaitable;
      try
      {
        await partitionSupervisor.RunAsync(this.shutdownCts.Token).ConfigureAwait(false);
      }
      catch (FeedRangeGoneException ex)
      {
        configuredTaskAwaitable = this.HandlePartitionGoneAsync(lease, ex.LastContinuation).ConfigureAwait(false);
        await configuredTaskAwaitable;
      }
      catch (OperationCanceledException ex) when (this.shutdownCts.IsCancellationRequested)
      {
        DefaultTrace.TraceVerbose("Lease with token {0}: processing canceled", (object) lease.CurrentLeaseToken);
      }
      catch (Exception ex)
      {
        await this.monitor.NotifyErrorAsync(lease.CurrentLeaseToken, ex);
        DefaultTrace.TraceWarning("Lease with token {0}: processing failed", (object) lease.CurrentLeaseToken);
      }
      configuredTaskAwaitable = this.RemoveLeaseAsync(lease, true).ConfigureAwait(false);
      await configuredTaskAwaitable;
    }

    private async Task HandlePartitionGoneAsync(
      DocumentServiceLease lease,
      string lastContinuationToken)
    {
      try
      {
        lease.ContinuationToken = lastContinuationToken;
        (IEnumerable<DocumentServiceLease>, bool) valueTuple = await this.synchronizer.HandlePartitionGoneAsync(lease).ConfigureAwait(false);
        IEnumerable<DocumentServiceLease> source = valueTuple.Item1;
        int num = valueTuple.Item2 ? 1 : 0;
        Task[] addLeaseTasks = source.Select<DocumentServiceLease, Task>((Func<DocumentServiceLease, Task>) (l =>
        {
          l.Properties = lease.Properties;
          return this.AddOrUpdateLeaseAsync(l);
        })).ToArray<Task>();
        if (num != 0)
          await this.leaseManager.DeleteAsync(lease).ConfigureAwait(false);
        await Task.WhenAll(addLeaseTasks).ConfigureAwait(false);
        addLeaseTasks = (Task[]) null;
      }
      catch (Exception ex)
      {
        await this.monitor.NotifyErrorAsync(lease.CurrentLeaseToken, ex);
        DefaultTrace.TraceWarning("Lease with token {0}: failed to handle gone", (object) ex, (object) lease.CurrentLeaseToken);
      }
    }
  }
}

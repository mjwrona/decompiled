// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement.PartitionLoadBalancerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement
{
  internal sealed class PartitionLoadBalancerCore : PartitionLoadBalancer
  {
    private readonly PartitionController partitionController;
    private readonly DocumentServiceLeaseContainer leaseContainer;
    private readonly LoadBalancingStrategy partitionLoadBalancingStrategy;
    private readonly TimeSpan leaseAcquireInterval;
    private CancellationTokenSource cancellationTokenSource;
    private Task runTask;

    public PartitionLoadBalancerCore(
      PartitionController partitionController,
      DocumentServiceLeaseContainer leaseContainer,
      LoadBalancingStrategy partitionLoadBalancingStrategy,
      TimeSpan leaseAcquireInterval)
    {
      if (partitionController == null)
        throw new ArgumentNullException(nameof (partitionController));
      if (leaseContainer == null)
        throw new ArgumentNullException(nameof (leaseContainer));
      if (partitionLoadBalancingStrategy == null)
        throw new ArgumentNullException(nameof (partitionLoadBalancingStrategy));
      this.partitionController = partitionController;
      this.leaseContainer = leaseContainer;
      this.partitionLoadBalancingStrategy = partitionLoadBalancingStrategy;
      this.leaseAcquireInterval = leaseAcquireInterval;
    }

    public override void Start()
    {
      if (this.runTask != null && !this.runTask.IsCompleted)
        throw new InvalidOperationException("Already started");
      this.cancellationTokenSource = new CancellationTokenSource();
      this.runTask = this.RunAsync();
    }

    public override async Task StopAsync()
    {
      if (this.runTask == null)
        throw new InvalidOperationException("Start has to be called before stop");
      this.cancellationTokenSource.Cancel();
      await this.runTask.ConfigureAwait(false);
    }

    private async Task RunAsync()
    {
      try
      {
        while (true)
        {
          ConfiguredTaskAwaitable configuredTaskAwaitable;
          try
          {
            foreach (DocumentServiceLease lease in this.partitionLoadBalancingStrategy.SelectLeasesToTake((IEnumerable<DocumentServiceLease>) await this.leaseContainer.GetAllLeasesAsync().ConfigureAwait(false)))
            {
              try
              {
                configuredTaskAwaitable = this.partitionController.AddOrUpdateLeaseAsync(lease).ConfigureAwait(false);
                await configuredTaskAwaitable;
              }
              catch (Exception ex)
              {
                Extensions.TraceException(ex);
                DefaultTrace.TraceError("Partition load balancer lease add/update iteration failed");
              }
            }
          }
          catch (Exception ex)
          {
            Extensions.TraceException(ex);
            DefaultTrace.TraceError("Partition load balancer iteration failed");
          }
          configuredTaskAwaitable = Task.Delay(this.leaseAcquireInterval, this.cancellationTokenSource.Token).ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
      }
      catch (OperationCanceledException ex)
      {
        DefaultTrace.TraceInformation("Partition load balancer task stopped.");
      }
    }
  }
}

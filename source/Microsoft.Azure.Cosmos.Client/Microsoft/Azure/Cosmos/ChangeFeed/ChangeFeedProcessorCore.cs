// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedProcessorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Bootstrapping;
using Microsoft.Azure.Cosmos.ChangeFeed.Configuration;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.Utils;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedProcessorCore : ChangeFeedProcessor
  {
    private readonly ChangeFeedObserverFactory observerFactory;
    private ContainerInternal leaseContainer;
    private string instanceName;
    private ContainerInternal monitoredContainer;
    private PartitionManager partitionManager;
    private ChangeFeedLeaseOptions changeFeedLeaseOptions;
    private ChangeFeedProcessorOptions changeFeedProcessorOptions;
    private DocumentServiceLeaseStoreManager documentServiceLeaseStoreManager;
    private bool initialized;

    public ChangeFeedProcessorCore(ChangeFeedObserverFactory observerFactory) => this.observerFactory = observerFactory ?? throw new ArgumentNullException(nameof (observerFactory));

    public void ApplyBuildConfiguration(
      DocumentServiceLeaseStoreManager customDocumentServiceLeaseStoreManager,
      ContainerInternal leaseContainer,
      string instanceName,
      ChangeFeedLeaseOptions changeFeedLeaseOptions,
      ChangeFeedProcessorOptions changeFeedProcessorOptions,
      ContainerInternal monitoredContainer)
    {
      this.documentServiceLeaseStoreManager = customDocumentServiceLeaseStoreManager != null || leaseContainer != null ? customDocumentServiceLeaseStoreManager : throw new ArgumentNullException(nameof (leaseContainer));
      this.leaseContainer = leaseContainer;
      this.instanceName = instanceName ?? throw new ArgumentNullException("InstanceName is required for the processor to initialize.");
      this.changeFeedProcessorOptions = changeFeedProcessorOptions;
      this.changeFeedLeaseOptions = changeFeedLeaseOptions;
      this.monitoredContainer = monitoredContainer ?? throw new ArgumentNullException(nameof (monitoredContainer));
    }

    public override async Task StartAsync()
    {
      ConfiguredTaskAwaitable configuredTaskAwaitable;
      if (!this.initialized)
      {
        configuredTaskAwaitable = this.InitializeAsync().ConfigureAwait(false);
        await configuredTaskAwaitable;
      }
      DefaultTrace.TraceInformation("Starting processor...");
      configuredTaskAwaitable = this.partitionManager.StartAsync().ConfigureAwait(false);
      await configuredTaskAwaitable;
      DefaultTrace.TraceInformation("Processor started.");
    }

    public override async Task StopAsync()
    {
      DefaultTrace.TraceInformation("Stopping processor...");
      await this.partitionManager.StopAsync().ConfigureAwait(false);
      DefaultTrace.TraceInformation("Processor stopped.");
    }

    private async Task InitializeAsync()
    {
      string containerRid = await this.monitoredContainer.GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, new CancellationToken());
      string leaseContainerPrefix = this.monitoredContainer.GetLeasePrefix(this.changeFeedLeaseOptions.LeasePrefix, await this.monitoredContainer.GetMonitoredDatabaseAndContainerRidAsync());
      PartitionKeyRangeCache partitionKeyRangeCache = await this.monitoredContainer.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);
      if (this.documentServiceLeaseStoreManager == null)
        this.documentServiceLeaseStoreManager = await DocumentServiceLeaseStoreManagerBuilder.InitializeAsync(this.monitoredContainer, this.leaseContainer, leaseContainerPrefix, this.instanceName).ConfigureAwait(false);
      this.partitionManager = this.BuildPartitionManager(containerRid, partitionKeyRangeCache);
      this.initialized = true;
      containerRid = (string) null;
      leaseContainerPrefix = (string) null;
      partitionKeyRangeCache = (PartitionKeyRangeCache) null;
    }

    private PartitionManager BuildPartitionManager(
      string containerRid,
      PartitionKeyRangeCache partitionKeyRangeCache)
    {
      PartitionSynchronizerCore synchronizer = new PartitionSynchronizerCore(this.monitoredContainer, this.documentServiceLeaseStoreManager.LeaseContainer, this.documentServiceLeaseStoreManager.LeaseManager, PartitionSynchronizerCore.DefaultDegreeOfParallelism, partitionKeyRangeCache, containerRid);
      BootstrapperCore bootstrapperCore = new BootstrapperCore((PartitionSynchronizer) synchronizer, this.documentServiceLeaseStoreManager.LeaseStore, BootstrapperCore.DefaultLockTime, BootstrapperCore.DefaultSleepTime);
      PartitionSupervisorFactoryCore supervisorFactoryCore = new PartitionSupervisorFactoryCore(this.observerFactory, this.documentServiceLeaseStoreManager.LeaseManager, (FeedProcessorFactory) new FeedProcessorFactoryCore(this.monitoredContainer, this.changeFeedProcessorOptions, this.documentServiceLeaseStoreManager.LeaseCheckpointer), this.changeFeedLeaseOptions);
      EqualPartitionsBalancingStrategy partitionLoadBalancingStrategy = new EqualPartitionsBalancingStrategy(this.instanceName, EqualPartitionsBalancingStrategy.DefaultMinLeaseCount, EqualPartitionsBalancingStrategy.DefaultMaxLeaseCount, this.changeFeedLeaseOptions.LeaseExpirationInterval);
      PartitionController partitionController1 = (PartitionController) new PartitionControllerCore(this.documentServiceLeaseStoreManager.LeaseContainer, this.documentServiceLeaseStoreManager.LeaseManager, (PartitionSupervisorFactory) supervisorFactoryCore, (PartitionSynchronizer) synchronizer, (ChangeFeedProcessorHealthMonitor) this.changeFeedProcessorOptions.HealthMonitor);
      PartitionLoadBalancerCore loadBalancerCore1 = new PartitionLoadBalancerCore(partitionController1, this.documentServiceLeaseStoreManager.LeaseContainer, (LoadBalancingStrategy) partitionLoadBalancingStrategy, this.changeFeedLeaseOptions.LeaseAcquireInterval);
      PartitionController partitionController2 = partitionController1;
      PartitionLoadBalancerCore loadBalancerCore2 = loadBalancerCore1;
      return (PartitionManager) new PartitionManagerCore((Bootstrapper) bootstrapperCore, partitionController2, (PartitionLoadBalancer) loadBalancerCore2);
    }
  }
}

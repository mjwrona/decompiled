// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedEstimatorRunner
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Configuration;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.Utils;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedEstimatorRunner : ChangeFeedProcessor
  {
    private const string EstimatorDefaultHostName = "Estimator";
    private readonly Container.ChangesEstimationHandler initialEstimateDelegate;
    private readonly TimeSpan? estimatorPeriod;
    private ChangeFeedProcessorHealthMonitor healthMonitor;
    private CancellationTokenSource shutdownCts;
    private ContainerInternal leaseContainer;
    private ContainerInternal monitoredContainer;
    private FeedEstimatorRunner feedEstimatorRunner;
    private ChangeFeedEstimator remainingWorkEstimator;
    private ChangeFeedLeaseOptions changeFeedLeaseOptions;
    private DocumentServiceLeaseContainer documentServiceLeaseContainer;
    private bool initialized;
    private bool running;
    private Task runAsync;

    public ChangeFeedEstimatorRunner(
      Container.ChangesEstimationHandler initialEstimateDelegate,
      TimeSpan? estimatorPeriod)
      : this(estimatorPeriod)
    {
      this.initialEstimateDelegate = initialEstimateDelegate ?? throw new ArgumentNullException(nameof (initialEstimateDelegate));
    }

    internal ChangeFeedEstimatorRunner(
      Container.ChangesEstimationHandler initialEstimateDelegate,
      TimeSpan? estimatorPeriod,
      ChangeFeedEstimator remainingWorkEstimator)
      : this(initialEstimateDelegate, estimatorPeriod)
    {
      this.remainingWorkEstimator = remainingWorkEstimator;
    }

    private ChangeFeedEstimatorRunner(TimeSpan? estimatorPeriod)
    {
      if (estimatorPeriod.HasValue && estimatorPeriod.Value <= TimeSpan.Zero)
        throw new ArgumentOutOfRangeException(nameof (estimatorPeriod));
      this.estimatorPeriod = estimatorPeriod;
    }

    public void ApplyBuildConfiguration(
      DocumentServiceLeaseStoreManager customDocumentServiceLeaseStoreManager,
      ContainerInternal leaseContainer,
      string instanceName,
      ChangeFeedLeaseOptions changeFeedLeaseOptions,
      ChangeFeedProcessorOptions changeFeedProcessorOptions,
      ContainerInternal monitoredContainer)
    {
      this.leaseContainer = leaseContainer != null || customDocumentServiceLeaseStoreManager != null ? leaseContainer : throw new ArgumentNullException(nameof (leaseContainer));
      this.monitoredContainer = monitoredContainer ?? throw new ArgumentNullException(nameof (monitoredContainer));
      this.changeFeedLeaseOptions = changeFeedLeaseOptions;
      this.documentServiceLeaseContainer = customDocumentServiceLeaseStoreManager?.LeaseContainer;
      this.healthMonitor = (ChangeFeedProcessorHealthMonitor) changeFeedProcessorOptions.HealthMonitor;
    }

    public override async Task StartAsync()
    {
      if (!this.initialized)
      {
        await this.InitializeLeaseStoreAsync();
        this.feedEstimatorRunner = this.BuildFeedEstimatorRunner();
        this.initialized = true;
      }
      if (this.running)
        throw new InvalidOperationException("Change Feed Estimator for container " + this.monitoredContainer.Id + " with lease container " + this.leaseContainer.Id + " already started.");
      this.shutdownCts = new CancellationTokenSource();
      DefaultTrace.TraceInformation("Starting estimator...");
      this.runAsync = this.feedEstimatorRunner.RunAsync(this.shutdownCts.Token);
      this.running = true;
    }

    public override async Task StopAsync()
    {
      DefaultTrace.TraceInformation("Stopping estimator...");
      if (!this.running)
        return;
      this.shutdownCts.Cancel();
      try
      {
        await this.runAsync.ConfigureAwait(false);
      }
      catch (OperationCanceledException ex)
      {
        Extensions.TraceException((Exception) ex);
      }
      this.running = false;
    }

    private FeedEstimatorRunner BuildFeedEstimatorRunner()
    {
      if (this.remainingWorkEstimator == null)
        this.remainingWorkEstimator = (ChangeFeedEstimator) new ChangeFeedEstimatorCore(this.changeFeedLeaseOptions.LeasePrefix, this.monitoredContainer, this.leaseContainer, this.documentServiceLeaseContainer);
      return new FeedEstimatorRunner(this.initialEstimateDelegate, this.remainingWorkEstimator, this.healthMonitor, this.estimatorPeriod);
    }

    private async Task InitializeLeaseStoreAsync()
    {
      if (this.documentServiceLeaseContainer != null)
        return;
      this.documentServiceLeaseContainer = (await DocumentServiceLeaseStoreManagerBuilder.InitializeAsync(this.monitoredContainer, this.leaseContainer, this.monitoredContainer.GetLeasePrefix(this.changeFeedLeaseOptions.LeasePrefix, await this.monitoredContainer.GetMonitoredDatabaseAndContainerRidAsync()), "Estimator")).LeaseContainer;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeedProcessorBuilder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Configuration;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using System;

namespace Microsoft.Azure.Cosmos
{
  public class ChangeFeedProcessorBuilder
  {
    private const string InMemoryDefaultHostName = "InMemory";
    private readonly ContainerInternal monitoredContainer;
    private readonly ChangeFeedProcessor changeFeedProcessor;
    private readonly ChangeFeedLeaseOptions changeFeedLeaseOptions;
    private readonly Action<DocumentServiceLeaseStoreManager, ContainerInternal, string, ChangeFeedLeaseOptions, ChangeFeedProcessorOptions, ContainerInternal> applyBuilderConfiguration;
    private readonly ChangeFeedProcessorOptions changeFeedProcessorOptions = new ChangeFeedProcessorOptions();
    private ContainerInternal leaseContainer;
    private string InstanceName;
    private DocumentServiceLeaseStoreManager LeaseStoreManager;
    private bool isBuilt;

    internal ChangeFeedProcessorBuilder(
      string processorName,
      ContainerInternal container,
      ChangeFeedProcessor changeFeedProcessor,
      Action<DocumentServiceLeaseStoreManager, ContainerInternal, string, ChangeFeedLeaseOptions, ChangeFeedProcessorOptions, ContainerInternal> applyBuilderConfiguration)
    {
      this.changeFeedLeaseOptions = new ChangeFeedLeaseOptions()
      {
        LeasePrefix = processorName
      };
      this.monitoredContainer = container;
      this.changeFeedProcessor = changeFeedProcessor;
      this.applyBuilderConfiguration = applyBuilderConfiguration;
    }

    public ChangeFeedProcessorBuilder WithInstanceName(string instanceName)
    {
      this.InstanceName = instanceName;
      return this;
    }

    public ChangeFeedProcessorBuilder WithLeaseConfiguration(
      TimeSpan? acquireInterval = null,
      TimeSpan? expirationInterval = null,
      TimeSpan? renewInterval = null)
    {
      ChangeFeedLeaseOptions feedLeaseOptions1 = this.changeFeedLeaseOptions;
      TimeSpan? nullable = renewInterval;
      TimeSpan timeSpan1 = nullable ?? ChangeFeedLeaseOptions.DefaultRenewInterval;
      feedLeaseOptions1.LeaseRenewInterval = timeSpan1;
      ChangeFeedLeaseOptions feedLeaseOptions2 = this.changeFeedLeaseOptions;
      nullable = acquireInterval;
      TimeSpan timeSpan2 = nullable ?? ChangeFeedLeaseOptions.DefaultAcquireInterval;
      feedLeaseOptions2.LeaseAcquireInterval = timeSpan2;
      ChangeFeedLeaseOptions feedLeaseOptions3 = this.changeFeedLeaseOptions;
      nullable = expirationInterval;
      TimeSpan timeSpan3 = nullable ?? ChangeFeedLeaseOptions.DefaultExpirationInterval;
      feedLeaseOptions3.LeaseExpirationInterval = timeSpan3;
      return this;
    }

    public ChangeFeedProcessorBuilder WithPollInterval(TimeSpan pollInterval)
    {
      this.changeFeedProcessorOptions.FeedPollDelay = pollInterval;
      return this;
    }

    internal virtual ChangeFeedProcessorBuilder WithStartFromBeginning()
    {
      this.changeFeedProcessorOptions.StartFromBeginning = true;
      return this;
    }

    public ChangeFeedProcessorBuilder WithStartTime(DateTime startTime)
    {
      this.changeFeedProcessorOptions.StartTime = new DateTime?(startTime);
      return this;
    }

    public ChangeFeedProcessorBuilder WithMaxItems(int maxItemCount)
    {
      this.changeFeedProcessorOptions.MaxItemCount = maxItemCount > 0 ? new int?(maxItemCount) : throw new ArgumentOutOfRangeException(nameof (maxItemCount));
      return this;
    }

    public ChangeFeedProcessorBuilder WithLeaseContainer(Container leaseContainer)
    {
      if (leaseContainer == null)
        throw new ArgumentNullException(nameof (leaseContainer));
      if (this.leaseContainer != null)
        throw new InvalidOperationException("The builder already defined a lease container.");
      if (this.LeaseStoreManager != null)
        throw new InvalidOperationException("The builder already defined an in-memory lease container instance.");
      this.leaseContainer = (ContainerInternal) leaseContainer;
      return this;
    }

    internal virtual ChangeFeedProcessorBuilder WithInMemoryLeaseContainer()
    {
      if (this.leaseContainer != null)
        throw new InvalidOperationException("The builder already defined a lease container.");
      if (this.LeaseStoreManager != null)
        throw new InvalidOperationException("The builder already defined an in-memory lease container instance.");
      if (string.IsNullOrEmpty(this.InstanceName))
        this.InstanceName = "InMemory";
      this.LeaseStoreManager = (DocumentServiceLeaseStoreManager) new DocumentServiceLeaseStoreManagerInMemory();
      return this;
    }

    public ChangeFeedProcessorBuilder WithErrorNotification(
      Container.ChangeFeedMonitorErrorDelegate errorDelegate)
    {
      if (errorDelegate == null)
        throw new ArgumentNullException(nameof (errorDelegate));
      this.changeFeedProcessorOptions.HealthMonitor.SetErrorDelegate(errorDelegate);
      return this;
    }

    public ChangeFeedProcessorBuilder WithLeaseAcquireNotification(
      Container.ChangeFeedMonitorLeaseAcquireDelegate acquireDelegate)
    {
      if (acquireDelegate == null)
        throw new ArgumentNullException(nameof (acquireDelegate));
      this.changeFeedProcessorOptions.HealthMonitor.SetLeaseAcquireDelegate(acquireDelegate);
      return this;
    }

    public ChangeFeedProcessorBuilder WithLeaseReleaseNotification(
      Container.ChangeFeedMonitorLeaseReleaseDelegate releaseDelegate)
    {
      if (releaseDelegate == null)
        throw new ArgumentNullException(nameof (releaseDelegate));
      this.changeFeedProcessorOptions.HealthMonitor.SetLeaseReleaseDelegate(releaseDelegate);
      return this;
    }

    public ChangeFeedProcessor Build()
    {
      if (this.isBuilt)
        throw new InvalidOperationException("This builder instance has already been used to build a processor. Create a new instance to build another.");
      if (this.monitoredContainer == null)
        throw new InvalidOperationException("monitoredContainer was not specified");
      if (this.leaseContainer == null && this.LeaseStoreManager == null)
        throw new InvalidOperationException("Defining the lease store by WithLeaseContainer or WithInMemoryLeaseContainer is required.");
      if (this.changeFeedLeaseOptions.LeasePrefix == null)
        throw new InvalidOperationException("Processor name not specified during creation.");
      this.applyBuilderConfiguration(this.LeaseStoreManager, this.leaseContainer, this.InstanceName, this.changeFeedLeaseOptions, this.changeFeedProcessorOptions, this.monitoredContainer);
      this.isBuilt = true;
      return this.changeFeedProcessor;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement.PartitionSupervisorFactoryCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Configuration;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement
{
  internal sealed class PartitionSupervisorFactoryCore : PartitionSupervisorFactory
  {
    private readonly ChangeFeedObserverFactory observerFactory;
    private readonly DocumentServiceLeaseManager leaseManager;
    private readonly ChangeFeedLeaseOptions changeFeedLeaseOptions;
    private readonly FeedProcessorFactory partitionProcessorFactory;

    public PartitionSupervisorFactoryCore(
      ChangeFeedObserverFactory observerFactory,
      DocumentServiceLeaseManager leaseManager,
      FeedProcessorFactory partitionProcessorFactory,
      ChangeFeedLeaseOptions options)
    {
      this.observerFactory = observerFactory ?? throw new ArgumentNullException(nameof (observerFactory));
      this.leaseManager = leaseManager ?? throw new ArgumentNullException(nameof (leaseManager));
      this.changeFeedLeaseOptions = options ?? throw new ArgumentNullException(nameof (options));
      this.partitionProcessorFactory = partitionProcessorFactory ?? throw new ArgumentNullException(nameof (partitionProcessorFactory));
    }

    public override PartitionSupervisor Create(DocumentServiceLease lease)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      ChangeFeedObserver observer = this.observerFactory.CreateObserver();
      FeedProcessor processor = this.partitionProcessorFactory.Create(lease, observer);
      LeaseRenewerCore renewer = new LeaseRenewerCore(lease, this.leaseManager, this.changeFeedLeaseOptions.LeaseRenewInterval);
      return (PartitionSupervisor) new PartitionSupervisorCore(lease, observer, processor, (LeaseRenewer) renewer);
    }
  }
}

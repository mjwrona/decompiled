// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing.FeedProcessorFactoryCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Configuration;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing
{
  internal class FeedProcessorFactoryCore : FeedProcessorFactory
  {
    private readonly ContainerInternal container;
    private readonly ChangeFeedProcessorOptions changeFeedProcessorOptions;
    private readonly DocumentServiceLeaseCheckpointer leaseCheckpointer;

    public FeedProcessorFactoryCore(
      ContainerInternal container,
      ChangeFeedProcessorOptions changeFeedProcessorOptions,
      DocumentServiceLeaseCheckpointer leaseCheckpointer)
    {
      this.container = container ?? throw new ArgumentNullException(nameof (container));
      this.changeFeedProcessorOptions = changeFeedProcessorOptions ?? throw new ArgumentNullException(nameof (changeFeedProcessorOptions));
      this.leaseCheckpointer = leaseCheckpointer ?? throw new ArgumentNullException(nameof (leaseCheckpointer));
    }

    public override FeedProcessor Create(DocumentServiceLease lease, ChangeFeedObserver observer)
    {
      if (observer == null)
        throw new ArgumentNullException(nameof (observer));
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      ProcessorOptions options = new ProcessorOptions()
      {
        StartContinuation = !string.IsNullOrEmpty(lease.ContinuationToken) ? lease.ContinuationToken : this.changeFeedProcessorOptions.StartContinuation,
        LeaseToken = lease.CurrentLeaseToken,
        FeedPollDelay = this.changeFeedProcessorOptions.FeedPollDelay,
        MaxItemCount = this.changeFeedProcessorOptions.MaxItemCount,
        StartFromBeginning = this.changeFeedProcessorOptions.StartFromBeginning,
        StartTime = this.changeFeedProcessorOptions.StartTime
      };
      PartitionCheckpointerCore checkpointer = new PartitionCheckpointerCore(this.leaseCheckpointer, lease);
      ChangeFeedPartitionKeyResultSetIteratorCore resultSetIterator = ChangeFeedPartitionKeyResultSetIteratorCore.Create(lease, options.StartContinuation, options.MaxItemCount, this.container, options.StartTime, options.StartFromBeginning);
      return (FeedProcessor) new FeedProcessorCore(observer, (FeedIterator) resultSetIterator, options, (PartitionCheckpointer) checkpointer);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeedEstimatorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ChangeFeedEstimatorCore : ChangeFeedEstimator
  {
    private readonly string processorName;
    private readonly ContainerInternal monitoredContainer;
    private readonly ContainerInternal leaseContainer;
    private readonly DocumentServiceLeaseContainer documentServiceLeaseContainer;

    public ChangeFeedEstimatorCore(
      string processorName,
      ContainerInternal monitoredContainer,
      ContainerInternal leaseContainer,
      DocumentServiceLeaseContainer documentServiceLeaseContainer)
    {
      this.processorName = processorName ?? throw new ArgumentNullException(nameof (processorName));
      this.leaseContainer = leaseContainer ?? throw new ArgumentNullException(nameof (leaseContainer));
      this.monitoredContainer = monitoredContainer ?? throw new ArgumentNullException(nameof (monitoredContainer));
      this.documentServiceLeaseContainer = documentServiceLeaseContainer;
    }

    public override FeedIterator<ChangeFeedProcessorState> GetCurrentStateIterator(
      ChangeFeedEstimatorRequestOptions changeFeedEstimatorRequestOptions = null)
    {
      return (FeedIterator<ChangeFeedProcessorState>) new ChangeFeedEstimatorIterator(this.processorName, this.monitoredContainer, this.leaseContainer, this.documentServiceLeaseContainer, changeFeedEstimatorRequestOptions);
    }
  }
}

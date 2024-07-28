// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement.PartitionManagerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Bootstrapping;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement
{
  internal sealed class PartitionManagerCore : PartitionManager
  {
    private readonly Bootstrapper bootstrapper;
    private readonly PartitionController partitionController;
    private readonly PartitionLoadBalancer partitionLoadBalancer;

    public PartitionManagerCore(
      Bootstrapper bootstrapper,
      PartitionController partitionController,
      PartitionLoadBalancer partitionLoadBalancer)
    {
      this.bootstrapper = bootstrapper;
      this.partitionController = partitionController;
      this.partitionLoadBalancer = partitionLoadBalancer;
    }

    public override async Task StartAsync()
    {
      await this.bootstrapper.InitializeAsync().ConfigureAwait(false);
      await this.partitionController.InitializeAsync().ConfigureAwait(false);
      this.partitionLoadBalancer.Start();
    }

    public override async Task StopAsync()
    {
      await this.partitionLoadBalancer.StopAsync().ConfigureAwait(false);
      await this.partitionController.ShutdownAsync().ConfigureAwait(false);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement.PartitionCheckpointerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.Core.Trace;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement
{
  internal sealed class PartitionCheckpointerCore : PartitionCheckpointer
  {
    private readonly DocumentServiceLeaseCheckpointer leaseCheckpointer;
    private DocumentServiceLease lease;

    public PartitionCheckpointerCore(
      DocumentServiceLeaseCheckpointer leaseCheckpointer,
      DocumentServiceLease lease)
    {
      this.leaseCheckpointer = leaseCheckpointer;
      this.lease = lease;
    }

    public override async Task CheckpointPartitionAsync(string сontinuationToken)
    {
      this.lease = await this.leaseCheckpointer.CheckpointAsync(this.lease, сontinuationToken).ConfigureAwait(false);
      DefaultTrace.TraceInformation("Checkpoint: lease token {0}, new continuation {1}", (object) this.lease.CurrentLeaseToken, (object) this.lease.ContinuationToken);
    }
  }
}

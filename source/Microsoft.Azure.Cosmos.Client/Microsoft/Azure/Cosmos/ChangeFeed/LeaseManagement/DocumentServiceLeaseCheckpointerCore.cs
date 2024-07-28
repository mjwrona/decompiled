// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseCheckpointerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseCheckpointerCore : DocumentServiceLeaseCheckpointer
  {
    private readonly DocumentServiceLeaseUpdater leaseUpdater;
    private readonly RequestOptionsFactory requestOptionsFactory;

    public DocumentServiceLeaseCheckpointerCore(
      DocumentServiceLeaseUpdater leaseUpdater,
      RequestOptionsFactory requestOptionsFactory)
    {
      this.leaseUpdater = leaseUpdater;
      this.requestOptionsFactory = requestOptionsFactory;
    }

    public override async Task<DocumentServiceLease> CheckpointAsync(
      DocumentServiceLease lease,
      string continuationToken)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      if (string.IsNullOrEmpty(continuationToken))
        throw new ArgumentException("continuationToken must be a non-empty string", nameof (continuationToken));
      return await this.leaseUpdater.UpdateLeaseAsync(lease, lease.Id, this.requestOptionsFactory.GetPartitionKey(lease.Id, lease.PartitionKey), (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease =>
      {
        if (serverLease.Owner != lease.Owner)
        {
          DefaultTrace.TraceInformation("{0} lease token was taken over by owner '{1}'", (object) lease.CurrentLeaseToken, (object) serverLease.Owner);
          throw new LeaseLostException(lease);
        }
        serverLease.ContinuationToken = continuationToken;
        return serverLease;
      })).ConfigureAwait(false);
    }
  }
}

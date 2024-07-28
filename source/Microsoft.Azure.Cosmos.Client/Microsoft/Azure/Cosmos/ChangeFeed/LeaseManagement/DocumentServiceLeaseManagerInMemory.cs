// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseManagerInMemory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseManagerInMemory : DocumentServiceLeaseManager
  {
    private readonly DocumentServiceLeaseUpdater leaseUpdater;
    private readonly ConcurrentDictionary<string, DocumentServiceLease> container;

    public DocumentServiceLeaseManagerInMemory(
      DocumentServiceLeaseUpdater leaseUpdater,
      ConcurrentDictionary<string, DocumentServiceLease> container)
    {
      this.leaseUpdater = leaseUpdater;
      this.container = container;
    }

    public override Task<DocumentServiceLease> AcquireAsync(DocumentServiceLease lease)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      return this.leaseUpdater.UpdateLeaseAsync(lease, lease.Id, Microsoft.Azure.Cosmos.PartitionKey.Null, (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease =>
      {
        serverLease.Properties = lease.Properties;
        return serverLease;
      }));
    }

    public override Task<DocumentServiceLease> CreateLeaseIfNotExistAsync(
      PartitionKeyRange partitionKeyRange,
      string continuationToken)
    {
      string str = partitionKeyRange != null ? partitionKeyRange.Id : throw new ArgumentNullException(nameof (partitionKeyRange));
      DocumentServiceLeaseCore serviceLeaseCore = new DocumentServiceLeaseCore();
      serviceLeaseCore.LeaseId = str;
      serviceLeaseCore.LeaseToken = str;
      serviceLeaseCore.ContinuationToken = continuationToken;
      serviceLeaseCore.FeedRange = (FeedRangeInternal) new FeedRangeEpk(partitionKeyRange.ToRange());
      return this.TryCreateDocumentServiceLeaseAsync((DocumentServiceLease) serviceLeaseCore);
    }

    public override Task<DocumentServiceLease> CreateLeaseIfNotExistAsync(
      FeedRangeEpk feedRange,
      string continuationToken)
    {
      if (feedRange == null)
        throw new ArgumentNullException(nameof (feedRange));
      string str = feedRange.Range.Min + "-" + feedRange.Range.Max;
      DocumentServiceLeaseCoreEpk serviceLeaseCoreEpk = new DocumentServiceLeaseCoreEpk();
      serviceLeaseCoreEpk.LeaseId = str;
      serviceLeaseCoreEpk.LeaseToken = str;
      serviceLeaseCoreEpk.ContinuationToken = continuationToken;
      serviceLeaseCoreEpk.FeedRange = (FeedRangeInternal) feedRange;
      return this.TryCreateDocumentServiceLeaseAsync((DocumentServiceLease) serviceLeaseCoreEpk);
    }

    public override Task ReleaseAsync(DocumentServiceLease lease)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      DocumentServiceLease cachedLease;
      if (!this.container.TryGetValue(lease.CurrentLeaseToken, out cachedLease))
      {
        DefaultTrace.TraceInformation("Lease with token {0} failed to release lease. The lease is gone already.", (object) lease.CurrentLeaseToken);
        throw new LeaseLostException(lease);
      }
      return (Task) this.leaseUpdater.UpdateLeaseAsync(cachedLease, cachedLease.Id, Microsoft.Azure.Cosmos.PartitionKey.Null, (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease =>
      {
        serverLease.Owner = (string) null;
        return serverLease;
      }));
    }

    public override Task DeleteAsync(DocumentServiceLease lease)
    {
      if (lease == null || lease.Id == null)
        throw new ArgumentNullException(nameof (lease));
      this.container.TryRemove(lease.CurrentLeaseToken, out DocumentServiceLease _);
      return Task.CompletedTask;
    }

    public override Task<DocumentServiceLease> RenewAsync(DocumentServiceLease lease)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      DocumentServiceLease cachedLease;
      if (!this.container.TryGetValue(lease.CurrentLeaseToken, out cachedLease))
      {
        DefaultTrace.TraceInformation("Lease with token {0} failed to renew lease. The lease is gone already.", (object) lease.CurrentLeaseToken);
        throw new LeaseLostException(lease);
      }
      return this.leaseUpdater.UpdateLeaseAsync(cachedLease, cachedLease.Id, Microsoft.Azure.Cosmos.PartitionKey.Null, (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease => serverLease));
    }

    public override Task<DocumentServiceLease> UpdatePropertiesAsync(DocumentServiceLease lease)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      return this.leaseUpdater.UpdateLeaseAsync(lease, lease.Id, Microsoft.Azure.Cosmos.PartitionKey.Null, (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease =>
      {
        serverLease.Properties = lease.Properties;
        return serverLease;
      }));
    }

    private Task<DocumentServiceLease> TryCreateDocumentServiceLeaseAsync(
      DocumentServiceLease documentServiceLease)
    {
      if (this.container.TryAdd(documentServiceLease.CurrentLeaseToken, documentServiceLease))
      {
        DefaultTrace.TraceInformation("Created lease with lease token {0}.", (object) documentServiceLease.CurrentLeaseToken);
        return Task.FromResult<DocumentServiceLease>(documentServiceLease);
      }
      DefaultTrace.TraceInformation("Some other host created lease for {0}.", (object) documentServiceLease.CurrentLeaseToken);
      return Task.FromResult<DocumentServiceLease>((DocumentServiceLease) null);
    }
  }
}

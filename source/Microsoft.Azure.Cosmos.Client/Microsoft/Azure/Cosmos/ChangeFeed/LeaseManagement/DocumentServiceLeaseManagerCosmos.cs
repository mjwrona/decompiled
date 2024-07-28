// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseManagerCosmos
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.ChangeFeed.Utils;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseManagerCosmos : DocumentServiceLeaseManager
  {
    private readonly ContainerInternal monitoredContainer;
    private readonly ContainerInternal leaseContainer;
    private readonly DocumentServiceLeaseUpdater leaseUpdater;
    private readonly DocumentServiceLeaseStoreManagerOptions options;
    private readonly RequestOptionsFactory requestOptionsFactory;
    private readonly AsyncLazy<TryCatch<string>> lazyContainerRid;
    private PartitionKeyRangeCache partitionKeyRangeCache;

    public DocumentServiceLeaseManagerCosmos(
      ContainerInternal monitoredContainer,
      ContainerInternal leaseContainer,
      DocumentServiceLeaseUpdater leaseUpdater,
      DocumentServiceLeaseStoreManagerOptions options,
      RequestOptionsFactory requestOptionsFactory)
    {
      this.monitoredContainer = monitoredContainer;
      this.leaseContainer = leaseContainer;
      this.leaseUpdater = leaseUpdater;
      this.options = options;
      this.requestOptionsFactory = requestOptionsFactory;
      this.lazyContainerRid = new AsyncLazy<TryCatch<string>>((Func<ITrace, CancellationToken, Task<TryCatch<string>>>) ((trace, innerCancellationToken) => this.TryInitializeContainerRIdAsync(innerCancellationToken)));
    }

    public override async Task<DocumentServiceLease> AcquireAsync(DocumentServiceLease lease)
    {
      string oldOwner = lease != null ? lease.Owner : throw new ArgumentNullException(nameof (lease));
      if (lease.FeedRange == null)
      {
        if (!this.lazyContainerRid.ValueInitialized)
        {
          TryCatch<string> valueAsync = await this.lazyContainerRid.GetValueAsync((ITrace) NoOpTrace.Singleton, new CancellationToken());
          if (!valueAsync.Succeeded)
            throw valueAsync.Exception.InnerException;
          this.partitionKeyRangeCache = await this.monitoredContainer.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);
        }
        PartitionKeyRange keyRangeByIdAsync = await this.partitionKeyRangeCache.TryGetPartitionKeyRangeByIdAsync(this.lazyContainerRid.Result.Result, lease.CurrentLeaseToken, (ITrace) NoOpTrace.Singleton, false);
        if (keyRangeByIdAsync != null)
          lease.FeedRange = (FeedRangeInternal) new FeedRangeEpk(keyRangeByIdAsync.ToRange());
      }
      return await this.leaseUpdater.UpdateLeaseAsync(lease, lease.Id, this.requestOptionsFactory.GetPartitionKey(lease.Id, lease.PartitionKey), (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease =>
      {
        if (serverLease.Owner != oldOwner)
        {
          DefaultTrace.TraceInformation("{0} lease token was taken over by owner '{1}'", (object) lease.CurrentLeaseToken, (object) serverLease.Owner);
          throw new LeaseLostException(lease, (Exception) CosmosExceptionFactory.Create(HttpStatusCode.PreconditionFailed, lease.CurrentLeaseToken + " lease token was taken over by owner '" + serverLease.Owner + "'", (string) null, new Microsoft.Azure.Cosmos.Headers(), (ITrace) NoOpTrace.Singleton, (Error) null, (Exception) null), false);
        }
        serverLease.Owner = this.options.HostName;
        serverLease.Properties = lease.Properties;
        return serverLease;
      })).ConfigureAwait(false);
    }

    public override Task<DocumentServiceLease> CreateLeaseIfNotExistAsync(
      PartitionKeyRange partitionKeyRange,
      string continuationToken)
    {
      string partitionId = partitionKeyRange != null ? partitionKeyRange.Id : throw new ArgumentNullException(nameof (partitionKeyRange));
      string documentId = this.GetDocumentId(partitionId);
      DocumentServiceLeaseCore serviceLeaseCore = new DocumentServiceLeaseCore();
      serviceLeaseCore.LeaseId = documentId;
      serviceLeaseCore.LeaseToken = partitionId;
      serviceLeaseCore.ContinuationToken = continuationToken;
      serviceLeaseCore.FeedRange = (FeedRangeInternal) new FeedRangeEpk(partitionKeyRange.ToRange());
      DocumentServiceLeaseCore documentServiceLease = serviceLeaseCore;
      this.requestOptionsFactory.AddPartitionKeyIfNeeded((Action<string>) (pk => documentServiceLease.LeasePartitionKey = pk), Guid.NewGuid().ToString());
      return this.TryCreateDocumentServiceLeaseAsync((DocumentServiceLease) documentServiceLease);
    }

    public override Task<DocumentServiceLease> CreateLeaseIfNotExistAsync(
      FeedRangeEpk feedRange,
      string continuationToken)
    {
      if (feedRange == null)
        throw new ArgumentNullException(nameof (feedRange));
      string partitionId = feedRange.Range.Min + "-" + feedRange.Range.Max;
      string documentId = this.GetDocumentId(partitionId);
      DocumentServiceLeaseCoreEpk serviceLeaseCoreEpk = new DocumentServiceLeaseCoreEpk();
      serviceLeaseCoreEpk.LeaseId = documentId;
      serviceLeaseCoreEpk.LeaseToken = partitionId;
      serviceLeaseCoreEpk.ContinuationToken = continuationToken;
      serviceLeaseCoreEpk.FeedRange = (FeedRangeInternal) feedRange;
      DocumentServiceLeaseCoreEpk documentServiceLease = serviceLeaseCoreEpk;
      this.requestOptionsFactory.AddPartitionKeyIfNeeded((Action<string>) (pk => documentServiceLease.LeasePartitionKey = pk), Guid.NewGuid().ToString());
      return this.TryCreateDocumentServiceLeaseAsync((DocumentServiceLease) documentServiceLease);
    }

    public override async Task ReleaseAsync(DocumentServiceLease lease)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      DocumentServiceLease cachedLease;
      try
      {
        cachedLease = await this.TryGetLeaseAsync(lease).ConfigureAwait(false);
      }
      catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound && ex.SubStatusCode == 0)
      {
        return;
      }
      if (cachedLease == null)
      {
        DefaultTrace.TraceInformation("Lease with token {0} failed to release lease. The lease is gone already.", (object) lease.CurrentLeaseToken);
        throw new LeaseLostException(lease);
      }
      DocumentServiceLease documentServiceLease = await this.leaseUpdater.UpdateLeaseAsync(cachedLease, cachedLease.Id, this.requestOptionsFactory.GetPartitionKey(lease.Id, lease.PartitionKey), (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease =>
      {
        if (serverLease.Owner != lease.Owner)
        {
          DefaultTrace.TraceInformation("Lease with token {0} no need to release lease. The lease was already taken by another host '{1}'.", (object) lease.CurrentLeaseToken, (object) serverLease.Owner);
          throw new LeaseLostException(lease);
        }
        serverLease.Owner = (string) null;
        return serverLease;
      })).ConfigureAwait(false);
    }

    public override async Task DeleteAsync(DocumentServiceLease lease)
    {
      if (lease?.Id == null)
        throw new ArgumentNullException(nameof (lease));
      int num = await this.leaseContainer.TryDeleteItemAsync<DocumentServiceLeaseCore>(this.requestOptionsFactory.GetPartitionKey(lease.Id, lease.PartitionKey), lease.Id).ConfigureAwait(false) ? 1 : 0;
    }

    public override async Task<DocumentServiceLease> RenewAsync(DocumentServiceLease lease)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      DocumentServiceLease cachedLease = await this.TryGetLeaseAsync(lease).ConfigureAwait(false);
      if (cachedLease == null)
      {
        DefaultTrace.TraceInformation("Lease with token {0} failed to renew lease. The lease is gone already.", (object) lease.CurrentLeaseToken);
        throw new LeaseLostException(lease);
      }
      return await this.leaseUpdater.UpdateLeaseAsync(cachedLease, cachedLease.Id, this.requestOptionsFactory.GetPartitionKey(lease.Id, lease.PartitionKey), (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease =>
      {
        if (serverLease.Owner != lease.Owner)
        {
          DefaultTrace.TraceInformation("Lease with token {0} was taken over by owner '{1}'", (object) lease.CurrentLeaseToken, (object) serverLease.Owner);
          throw new LeaseLostException(lease);
        }
        return serverLease;
      })).ConfigureAwait(false);
    }

    public override async Task<DocumentServiceLease> UpdatePropertiesAsync(
      DocumentServiceLease lease)
    {
      if (lease == null)
        throw new ArgumentNullException(nameof (lease));
      if (lease.Owner != this.options.HostName)
      {
        DefaultTrace.TraceInformation("Lease with token '{0}' was taken over by owner '{1}' before lease properties update", (object) lease.CurrentLeaseToken, (object) lease.Owner);
        throw new LeaseLostException(lease);
      }
      return await this.leaseUpdater.UpdateLeaseAsync(lease, lease.Id, this.requestOptionsFactory.GetPartitionKey(lease.Id, lease.PartitionKey), (Func<DocumentServiceLease, DocumentServiceLease>) (serverLease =>
      {
        if (serverLease.Owner != lease.Owner)
        {
          DefaultTrace.TraceInformation("Lease with token '{0}' was taken over by owner '{1}'", (object) lease.CurrentLeaseToken, (object) serverLease.Owner);
          throw new LeaseLostException(lease);
        }
        serverLease.Properties = lease.Properties;
        return serverLease;
      })).ConfigureAwait(false);
    }

    private async Task<DocumentServiceLease> TryCreateDocumentServiceLeaseAsync(
      DocumentServiceLease documentServiceLease)
    {
      if (await this.leaseContainer.TryCreateItemAsync<DocumentServiceLease>(this.requestOptionsFactory.GetPartitionKey(documentServiceLease.Id, documentServiceLease.PartitionKey), documentServiceLease).ConfigureAwait(false) != null)
      {
        DefaultTrace.TraceInformation("Created lease with lease token {0}.", (object) documentServiceLease.CurrentLeaseToken);
        return documentServiceLease;
      }
      DefaultTrace.TraceInformation("Some other host created lease for {0}.", (object) documentServiceLease.CurrentLeaseToken);
      return (DocumentServiceLease) null;
    }

    private async Task<DocumentServiceLease> TryGetLeaseAsync(DocumentServiceLease lease) => await this.leaseContainer.TryGetItemAsync<DocumentServiceLease>(this.requestOptionsFactory.GetPartitionKey(lease.Id, lease.PartitionKey), lease.Id).ConfigureAwait(false);

    private string GetDocumentId(string partitionId) => this.options.GetPartitionLeasePrefix() + partitionId;

    private async Task<TryCatch<string>> TryInitializeContainerRIdAsync(
      CancellationToken cancellationToken)
    {
      try
      {
        return TryCatch<string>.FromResult(await this.monitoredContainer.GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken));
      }
      catch (CosmosException ex)
      {
        return TryCatch<string>.FromException((Exception) ex);
      }
    }
  }
}

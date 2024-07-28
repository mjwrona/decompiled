// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseUpdaterCosmos
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.ChangeFeed.Utils;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseUpdaterCosmos : DocumentServiceLeaseUpdater
  {
    private const int RetryCountOnConflict = 5;
    private readonly Container container;

    public DocumentServiceLeaseUpdaterCosmos(Container container) => this.container = container ?? throw new ArgumentNullException(nameof (container));

    public override async Task<DocumentServiceLease> UpdateLeaseAsync(
      DocumentServiceLease cachedLease,
      string itemId,
      PartitionKey partitionKey,
      Func<DocumentServiceLease, DocumentServiceLease> updateLease)
    {
      DocumentServiceLease lease = cachedLease;
      for (int retryCount = 5; retryCount >= 0; --retryCount)
      {
        lease = updateLease(lease);
        if (lease == null)
          return (DocumentServiceLease) null;
        lease.Timestamp = DateTime.UtcNow;
        DocumentServiceLease documentServiceLease = await this.TryReplaceLeaseAsync(lease, partitionKey, itemId).ConfigureAwait(false);
        if (documentServiceLease != null)
          return documentServiceLease;
        DefaultTrace.TraceInformation("Lease with token {0} update conflict. Reading the current version of lease.", (object) lease.CurrentLeaseToken);
        try
        {
          DocumentServiceLease itemAsync = await this.container.TryGetItemAsync<DocumentServiceLease>(partitionKey, itemId);
          DefaultTrace.TraceInformation("Lease with token {0} update failed because the lease with concurrency token '{1}' was updated by host '{2}' with concurrency token '{3}'. Will retry, {4} retry(s) left.", (object) lease.CurrentLeaseToken, (object) lease.ConcurrencyToken, (object) itemAsync.Owner, (object) itemAsync.ConcurrencyToken, (object) retryCount);
          lease = itemAsync;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
          DefaultTrace.TraceInformation("Lease with token {0} no longer exists", (object) lease.CurrentLeaseToken);
          throw new LeaseLostException(lease, (Exception) ex, true);
        }
      }
      throw new LeaseLostException(lease);
    }

    private async Task<DocumentServiceLease> TryReplaceLeaseAsync(
      DocumentServiceLease lease,
      PartitionKey partitionKey,
      string itemId)
    {
      try
      {
        ItemRequestOptions ifMatchOptions = this.CreateIfMatchOptions(lease);
        return (await this.container.TryReplaceItemAsync<DocumentServiceLease>(itemId, lease, partitionKey, ifMatchOptions).ConfigureAwait(false)).Resource;
      }
      catch (CosmosException ex)
      {
        DefaultTrace.TraceWarning("Lease operation exception, status code: {0}", (object) ex.StatusCode);
        if (ex.StatusCode == HttpStatusCode.NotFound)
          throw new LeaseLostException(lease, (Exception) ex, true);
        if (ex.StatusCode == HttpStatusCode.PreconditionFailed)
          return (DocumentServiceLease) null;
        if (ex.StatusCode == HttpStatusCode.Conflict)
          throw new LeaseLostException(lease, (Exception) ex, false);
        throw;
      }
    }

    private ItemRequestOptions CreateIfMatchOptions(DocumentServiceLease lease)
    {
      ItemRequestOptions ifMatchOptions = new ItemRequestOptions();
      ifMatchOptions.IfMatchEtag = lease.ConcurrencyToken;
      return ifMatchOptions;
    }
  }
}

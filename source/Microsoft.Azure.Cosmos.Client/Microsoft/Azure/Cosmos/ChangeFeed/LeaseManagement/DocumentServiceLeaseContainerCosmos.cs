// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseContainerCosmos
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseContainerCosmos : DocumentServiceLeaseContainer
  {
    private readonly Container container;
    private readonly DocumentServiceLeaseStoreManagerOptions options;
    private static readonly QueryRequestOptions queryRequestOptions = new QueryRequestOptions()
    {
      MaxConcurrency = new int?(0)
    };

    public DocumentServiceLeaseContainerCosmos(
      Container container,
      DocumentServiceLeaseStoreManagerOptions options)
    {
      this.container = container;
      this.options = options;
    }

    public override async Task<IReadOnlyList<DocumentServiceLease>> GetAllLeasesAsync() => await this.ListDocumentsAsync(this.options.GetPartitionLeasePrefix()).ConfigureAwait(false);

    public override async Task<IEnumerable<DocumentServiceLease>> GetOwnedLeasesAsync()
    {
      DocumentServiceLeaseContainerCosmos leaseContainerCosmos = this;
      List<DocumentServiceLease> ownedLeases = new List<DocumentServiceLease>();
      foreach (DocumentServiceLease documentServiceLease in (IEnumerable<DocumentServiceLease>) await leaseContainerCosmos.GetAllLeasesAsync().ConfigureAwait(false))
      {
        if (string.Compare(documentServiceLease.Owner, leaseContainerCosmos.options.HostName, StringComparison.OrdinalIgnoreCase) == 0)
          ownedLeases.Add(documentServiceLease);
      }
      IEnumerable<DocumentServiceLease> ownedLeasesAsync = (IEnumerable<DocumentServiceLease>) ownedLeases;
      ownedLeases = (List<DocumentServiceLease>) null;
      return ownedLeasesAsync;
    }

    private async Task<IReadOnlyList<DocumentServiceLease>> ListDocumentsAsync(string prefix)
    {
      if (string.IsNullOrEmpty(prefix))
        throw new ArgumentException("Prefix must be non-empty string", nameof (prefix));
      List<DocumentServiceLease> leases;
      IReadOnlyList<DocumentServiceLease> documentServiceLeaseList;
      using (FeedIterator iterator = this.container.GetItemQueryStreamIterator("SELECT * FROM c WHERE STARTSWITH(c.id, '" + prefix + "')", requestOptions: DocumentServiceLeaseContainerCosmos.queryRequestOptions))
      {
        leases = new List<DocumentServiceLease>();
        while (iterator.HasMoreResults)
        {
          using (ResponseMessage responseMessage = await iterator.ReadNextAsync().ConfigureAwait(false))
          {
            responseMessage.EnsureSuccessStatusCode();
            leases.AddRange((IEnumerable<DocumentServiceLease>) CosmosFeedResponseSerializer.FromFeedResponseStream<DocumentServiceLease>(CosmosContainerExtensions.DefaultJsonSerializer, responseMessage.Content));
          }
        }
        documentServiceLeaseList = (IReadOnlyList<DocumentServiceLease>) leases;
      }
      leases = (List<DocumentServiceLease>) null;
      return documentServiceLeaseList;
    }
  }
}

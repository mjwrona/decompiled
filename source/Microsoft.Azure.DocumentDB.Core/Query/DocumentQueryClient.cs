// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.DocumentQueryClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Common;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class DocumentQueryClient : IDocumentQueryClient, IDisposable
  {
    private readonly DocumentClient innerClient;
    private QueryPartitionProvider queryPartitionProvider;
    private readonly SemaphoreSlim semaphore;

    public DocumentQueryClient(DocumentClient innerClient)
    {
      this.innerClient = innerClient != null ? innerClient : throw new ArgumentNullException(nameof (innerClient));
      this.semaphore = new SemaphoreSlim(1, 1);
    }

    public void Dispose()
    {
      this.innerClient.Dispose();
      if (this.queryPartitionProvider == null)
        return;
      this.queryPartitionProvider.Dispose();
    }

    QueryCompatibilityMode IDocumentQueryClient.QueryCompatibilityMode
    {
      get => this.innerClient.QueryCompatibilityMode;
      set => this.innerClient.QueryCompatibilityMode = value;
    }

    IRetryPolicyFactory IDocumentQueryClient.ResetSessionTokenRetryPolicy => this.innerClient.ResetSessionTokenRetryPolicy;

    Uri IDocumentQueryClient.ServiceEndpoint => this.innerClient.ReadEndpoint;

    [Obsolete("Support for IPartitionResolver is now obsolete.")]
    IDictionary<string, IPartitionResolver> IDocumentQueryClient.PartitionResolvers => this.innerClient.PartitionResolvers;

    ConnectionMode IDocumentQueryClient.ConnectionMode => this.innerClient.ConnectionPolicy.ConnectionMode;

    Action<IQueryable> IDocumentQueryClient.OnExecuteScalarQueryCallback => this.innerClient.OnExecuteScalarQueryCallback;

    async Task<CollectionCache> IDocumentQueryClient.GetCollectionCacheAsync() => (CollectionCache) await this.innerClient.GetCollectionCacheAsync();

    async Task<IRoutingMapProvider> IDocumentQueryClient.GetRoutingMapProviderAsync() => (IRoutingMapProvider) await this.innerClient.GetPartitionKeyRangeCacheAsync();

    public async Task<QueryPartitionProvider> GetQueryPartitionProviderAsync(
      CancellationToken cancellationToken)
    {
      if (this.queryPartitionProvider == null)
      {
        await this.semaphore.WaitAsync(cancellationToken);
        if (this.queryPartitionProvider == null)
        {
          cancellationToken.ThrowIfCancellationRequested();
          this.queryPartitionProvider = new QueryPartitionProvider(await this.innerClient.GetQueryEngineConfiguration());
        }
        this.semaphore.Release();
      }
      return this.queryPartitionProvider;
    }

    public Task<DocumentServiceResponse> ExecuteQueryAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.innerClient.ExecuteQueryAsync(request, retryPolicyInstance, cancellationToken);
    }

    public Task<DocumentServiceResponse> ReadFeedAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.innerClient.ReadFeedAsync(request, retryPolicyInstance, cancellationToken);
    }

    public Task<ConsistencyLevel> GetDefaultConsistencyLevelAsync() => this.innerClient.GetDefaultConsistencyLevelAsync();

    public Task<ConsistencyLevel?> GetDesiredConsistencyLevelAsync() => this.innerClient.GetDesiredConsistencyLevelAsync();

    public Task EnsureValidOverwrite(ConsistencyLevel requestedConsistencyLevel)
    {
      this.innerClient.EnsureValidOverwrite(requestedConsistencyLevel);
      return CompletedTask.Instance;
    }

    public Task<PartitionKeyRangeCache> GetPartitionKeyRangeCache() => this.innerClient.GetPartitionKeyRangeCacheAsync();
  }
}

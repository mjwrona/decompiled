// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.DocumentQueryClient
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query
{
  internal sealed class DocumentQueryClient : IDocumentQueryClient, IDisposable
  {
    private readonly DocumentClient innerClient;

    public DocumentQueryClient(DocumentClient innerClient) => this.innerClient = innerClient != null ? innerClient : throw new ArgumentNullException(nameof (innerClient));

    public void Dispose() => this.innerClient.Dispose();

    QueryCompatibilityMode IDocumentQueryClient.QueryCompatibilityMode
    {
      get => this.innerClient.QueryCompatibilityMode;
      set => this.innerClient.QueryCompatibilityMode = value;
    }

    IRetryPolicyFactory IDocumentQueryClient.ResetSessionTokenRetryPolicy => this.innerClient.ResetSessionTokenRetryPolicy;

    Uri IDocumentQueryClient.ServiceEndpoint => this.innerClient.ReadEndpoint;

    ConnectionMode IDocumentQueryClient.ConnectionMode => this.innerClient.ConnectionPolicy.ConnectionMode;

    Action<IQueryable> IDocumentQueryClient.OnExecuteScalarQueryCallback => this.innerClient.OnExecuteScalarQueryCallback;

    async Task<CollectionCache> IDocumentQueryClient.GetCollectionCacheAsync() => (CollectionCache) await this.innerClient.GetCollectionCacheAsync((ITrace) NoOpTrace.Singleton);

    async Task<IRoutingMapProvider> IDocumentQueryClient.GetRoutingMapProviderAsync() => (IRoutingMapProvider) await this.innerClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);

    public Task<QueryPartitionProvider> GetQueryPartitionProviderAsync() => this.innerClient.QueryPartitionProvider;

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

    public async Task<Microsoft.Azure.Documents.ConsistencyLevel> GetDefaultConsistencyLevelAsync() => (Microsoft.Azure.Documents.ConsistencyLevel) await this.innerClient.GetDefaultConsistencyLevelAsync();

    public Task<Microsoft.Azure.Documents.ConsistencyLevel?> GetDesiredConsistencyLevelAsync() => this.innerClient.GetDesiredConsistencyLevelAsync();

    public Task EnsureValidOverwriteAsync(
      Microsoft.Azure.Documents.ConsistencyLevel requestedConsistencyLevel,
      Microsoft.Azure.Documents.OperationType operationType,
      ResourceType resourceType)
    {
      this.innerClient.EnsureValidOverwrite(requestedConsistencyLevel, new Microsoft.Azure.Documents.OperationType?(operationType), new ResourceType?(resourceType));
      return Task.CompletedTask;
    }

    public Task<PartitionKeyRangeCache> GetPartitionKeyRangeCacheAsync() => this.innerClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);
  }
}

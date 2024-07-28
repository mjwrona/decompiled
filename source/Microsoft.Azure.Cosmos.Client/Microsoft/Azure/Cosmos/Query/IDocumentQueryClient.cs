// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.IDocumentQueryClient
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Documents;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query
{
  internal interface IDocumentQueryClient : IDisposable
  {
    QueryCompatibilityMode QueryCompatibilityMode { get; set; }

    IRetryPolicyFactory ResetSessionTokenRetryPolicy { get; }

    Uri ServiceEndpoint { get; }

    ConnectionMode ConnectionMode { get; }

    Action<IQueryable> OnExecuteScalarQueryCallback { get; }

    Task<CollectionCache> GetCollectionCacheAsync();

    Task<IRoutingMapProvider> GetRoutingMapProviderAsync();

    Task<QueryPartitionProvider> GetQueryPartitionProviderAsync();

    Task<DocumentServiceResponse> ExecuteQueryAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken);

    Task<DocumentServiceResponse> ReadFeedAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken);

    Task<Microsoft.Azure.Documents.ConsistencyLevel> GetDefaultConsistencyLevelAsync();

    Task<Microsoft.Azure.Documents.ConsistencyLevel?> GetDesiredConsistencyLevelAsync();

    Task EnsureValidOverwriteAsync(
      Microsoft.Azure.Documents.ConsistencyLevel desiredConsistencyLevel,
      Microsoft.Azure.Documents.OperationType operationType,
      ResourceType resourceType);

    Task<PartitionKeyRangeCache> GetPartitionKeyRangeCacheAsync();
  }
}

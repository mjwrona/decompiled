// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.IDocumentQueryClient
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
  internal interface IDocumentQueryClient : IDisposable
  {
    QueryCompatibilityMode QueryCompatibilityMode { get; set; }

    IRetryPolicyFactory ResetSessionTokenRetryPolicy { get; }

    Uri ServiceEndpoint { get; }

    [Obsolete("Support for IPartitionResolver is now obsolete.")]
    IDictionary<string, IPartitionResolver> PartitionResolvers { get; }

    ConnectionMode ConnectionMode { get; }

    Action<IQueryable> OnExecuteScalarQueryCallback { get; }

    Task<CollectionCache> GetCollectionCacheAsync();

    Task<IRoutingMapProvider> GetRoutingMapProviderAsync();

    Task<QueryPartitionProvider> GetQueryPartitionProviderAsync(CancellationToken cancellationToken);

    Task<DocumentServiceResponse> ExecuteQueryAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken);

    Task<DocumentServiceResponse> ReadFeedAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken);

    Task<ConsistencyLevel> GetDefaultConsistencyLevelAsync();

    Task<ConsistencyLevel?> GetDesiredConsistencyLevelAsync();

    Task EnsureValidOverwrite(ConsistencyLevel desiredConsistencyLevel);

    Task<PartitionKeyRangeCache> GetPartitionKeyRangeCache();
  }
}

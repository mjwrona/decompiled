// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.CosmosQueryContextCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query
{
  internal class CosmosQueryContextCore : CosmosQueryContext
  {
    public CosmosQueryContextCore(
      CosmosQueryClient client,
      Microsoft.Azure.Documents.ResourceType resourceTypeEnum,
      OperationType operationType,
      Type resourceType,
      string resourceLink,
      Guid correlatedActivityId,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool useSystemPrefix,
      string containerResourceId = null)
      : base(client, resourceTypeEnum, operationType, resourceType, resourceLink, correlatedActivityId, isContinuationExpected, allowNonValueAggregateQuery, useSystemPrefix, containerResourceId)
    {
    }

    internal override Task<TryCatch<QueryPage>> ExecuteQueryAsync(
      SqlQuerySpec querySpecForInit,
      QueryRequestOptions queryRequestOptions,
      string continuationToken,
      FeedRange feedRange,
      bool isContinuationExpected,
      int pageSize,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosQueryClient queryClient = this.QueryClient;
      string resourceLink = this.ResourceLink;
      int resourceTypeEnum = (int) this.ResourceTypeEnum;
      int operationTypeEnum = (int) this.OperationTypeEnum;
      Guid correlatedActivityId = this.CorrelatedActivityId;
      QueryRequestOptions queryRequestOptions1 = queryRequestOptions;
      SqlQuerySpec sqlQuerySpec1 = querySpecForInit;
      string str = continuationToken;
      FeedRange feedRange1 = feedRange;
      QueryRequestOptions requestOptions = queryRequestOptions1;
      SqlQuerySpec sqlQuerySpec2 = sqlQuerySpec1;
      string continuationToken1 = str;
      int num = isContinuationExpected ? 1 : 0;
      int pageSize1 = pageSize;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return queryClient.ExecuteItemQueryAsync(resourceLink, (Microsoft.Azure.Documents.ResourceType) resourceTypeEnum, (OperationType) operationTypeEnum, correlatedActivityId, feedRange1, requestOptions, sqlQuerySpec2, continuationToken1, num != 0, pageSize1, trace1, cancellationToken1);
    }

    internal override Task<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo> ExecuteQueryPlanRequestAsync(
      string resourceUri,
      Microsoft.Azure.Documents.ResourceType resourceType,
      OperationType operationType,
      SqlQuerySpec sqlQuerySpec,
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      string supportedQueryFeatures,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.QueryClient.ExecuteQueryPlanRequestAsync(resourceUri, resourceType, operationType, sqlQuerySpec, partitionKey, supportedQueryFeatures, this.CorrelatedActivityId, trace, cancellationToken);
    }
  }
}

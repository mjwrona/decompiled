// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryClient.CosmosQueryContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryClient
{
  internal abstract class CosmosQueryContext
  {
    public virtual CosmosQueryClient QueryClient { get; }

    public virtual Microsoft.Azure.Documents.ResourceType ResourceTypeEnum { get; }

    public virtual OperationType OperationTypeEnum { get; }

    public virtual Type ResourceType { get; }

    public virtual bool IsContinuationExpected { get; }

    public virtual bool AllowNonValueAggregateQuery { get; }

    public virtual string ResourceLink { get; }

    public virtual string ContainerResourceId { get; set; }

    public virtual Guid CorrelatedActivityId { get; }

    public virtual bool UseSystemPrefix { get; }

    internal CosmosQueryContext()
    {
    }

    public CosmosQueryContext(
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
    {
      this.OperationTypeEnum = operationType;
      this.QueryClient = client ?? throw new ArgumentNullException(nameof (client));
      this.ResourceTypeEnum = resourceTypeEnum;
      this.ResourceType = resourceType ?? throw new ArgumentNullException(nameof (resourceType));
      this.ResourceLink = resourceLink;
      this.ContainerResourceId = containerResourceId;
      this.IsContinuationExpected = isContinuationExpected;
      this.AllowNonValueAggregateQuery = allowNonValueAggregateQuery;
      this.UseSystemPrefix = useSystemPrefix;
      this.CorrelatedActivityId = !(correlatedActivityId == Guid.Empty) ? correlatedActivityId : throw new ArgumentOutOfRangeException(nameof (correlatedActivityId));
    }

    internal abstract Task<TryCatch<QueryPage>> ExecuteQueryAsync(
      SqlQuerySpec querySpecForInit,
      QueryRequestOptions queryRequestOptions,
      string continuationToken,
      FeedRange feedRange,
      bool isContinuationExpected,
      int pageSize,
      ITrace trace,
      CancellationToken cancellationToken);

    internal abstract Task<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo> ExecuteQueryPlanRequestAsync(
      string resourceUri,
      Microsoft.Azure.Documents.ResourceType resourceType,
      OperationType operationType,
      SqlQuerySpec sqlQuerySpec,
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      string supportedQueryFeatures,
      ITrace trace,
      CancellationToken cancellationToken);
  }
}

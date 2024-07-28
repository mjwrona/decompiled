// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.ExecutionContext.CosmosQueryExecutionContextFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Parser;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.OptimisticDirectExecutionQuery;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.SqlObjects;
using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.ExecutionContext
{
  internal static class CosmosQueryExecutionContextFactory
  {
    private const string InternalPartitionKeyDefinitionProperty = "x-ms-query-partitionkey-definition";
    private const string OptimisticDirectExecution = "OptimisticDirectExecution";
    private const string Passthrough = "Passthrough";
    private const string Specialized = "Specialized";
    private const int PageSizeFactorForTop = 5;

    public static IQueryPipelineStage Create(
      DocumentContainer documentContainer,
      CosmosQueryContext cosmosQueryContext,
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      ITrace trace1)
    {
      if (cosmosQueryContext == null)
        throw new ArgumentNullException(nameof (cosmosQueryContext));
      if (inputParameters == null)
        throw new ArgumentNullException(nameof (inputParameters));
      if (trace1 == null)
        throw new ArgumentNullException(nameof (trace));
      return (IQueryPipelineStage) new CatchAllQueryPipelineStage((IQueryPipelineStage) new NameCacheStaleRetryQueryPipelineStage(cosmosQueryContext, (Func<IQueryPipelineStage>) (() => (IQueryPipelineStage) new LazyQueryPipelineStage(new AsyncLazy<TryCatch<IQueryPipelineStage>>((Func<ITrace, CancellationToken, Task<TryCatch<IQueryPipelineStage>>>) ((trace2, innerCancellationToken) => CosmosQueryExecutionContextFactory.TryCreateCoreContextAsync(documentContainer, cosmosQueryContext, inputParameters, trace2, innerCancellationToken))), new CancellationToken()))), new CancellationToken());
    }

    private static async Task<TryCatch<IQueryPipelineStage>> TryCreateCoreContextAsync(
      DocumentContainer documentContainer,
      CosmosQueryContext cosmosQueryContext,
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      using (ITrace createQueryPipelineTrace = trace.StartChild("Create Query Pipeline", TraceComponent.Query, TraceLevel.Info))
      {
        CosmosElement continuationToken1 = inputParameters.InitialUserContinuationToken;
        Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo queryPlanFromContinuationToken = inputParameters.PartitionedQueryExecutionInfo;
        if (continuationToken1 != (CosmosElement) null)
        {
          PipelineContinuationToken pipelineContinuationToken;
          if (!PipelineContinuationToken.TryCreateFromCosmosElement(continuationToken1, out pipelineContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0}: {1}.", (object) "PipelineContinuationToken", (object) continuationToken1)));
          if (PipelineContinuationToken.IsTokenFromTheFuture(pipelineContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException("PipelineContinuationToken Continuation token is from a newer version of the SDK. Upgrade the SDK to avoid this issue." + string.Format("{0}.", (object) continuationToken1)));
          PipelineContinuationTokenV1_1 pipelineContinuationTokenV1_1;
          if (!PipelineContinuationToken.TryConvertToLatest(pipelineContinuationToken, out pipelineContinuationTokenV1_1))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0}: '{1}' is no longer supported.", (object) "PipelineContinuationToken", (object) continuationToken1)));
          CosmosElement continuationToken2 = pipelineContinuationTokenV1_1.SourceContinuationToken;
          if (pipelineContinuationTokenV1_1.QueryPlan != null)
            queryPlanFromContinuationToken = pipelineContinuationTokenV1_1.QueryPlan;
        }
        ContainerQueryProperties containerQueryProperties = await cosmosQueryContext.QueryClient.GetCachedContainerQueryPropertiesAsync(cosmosQueryContext.ResourceLink, inputParameters.PartitionKey, createQueryPipelineTrace, cancellationToken);
        cosmosQueryContext.ContainerResourceId = containerQueryProperties.ResourceId;
        PartitionKeyRange directExecutionAsync = await CosmosQueryExecutionContextFactory.GetTargetRangeOptimisticDirectExecutionAsync(inputParameters, queryPlanFromContinuationToken, cosmosQueryContext, containerQueryProperties, trace);
        if (directExecutionAsync != null)
        {
          CosmosQueryExecutionContextFactory.SetTestInjectionPipelineType(inputParameters, "OptimisticDirectExecution");
          return CosmosQueryExecutionContextFactory.OptimisticDirectExecutionContext(documentContainer, inputParameters, directExecutionAsync, cancellationToken);
        }
        Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo;
        if (inputParameters.ForcePassthrough)
          partitionedQueryExecutionInfo = new Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo()
          {
            QueryInfo = new QueryInfo()
            {
              Aggregates = (IReadOnlyList<AggregateOperator>) null,
              DistinctType = DistinctQueryType.None,
              GroupByAliases = (IReadOnlyList<string>) null,
              GroupByAliasToAggregateType = (IReadOnlyDictionary<string, AggregateOperator?>) null,
              GroupByExpressions = (IReadOnlyList<string>) null,
              HasSelectValue = false,
              Limit = new int?(),
              Offset = new int?(),
              OrderBy = (IReadOnlyList<SortOrder>) null,
              OrderByExpressions = (IReadOnlyList<string>) null,
              RewrittenQuery = (string) null,
              Top = new int?()
            },
            QueryRanges = new List<Range<string>>()
          };
        else if (queryPlanFromContinuationToken != null)
        {
          partitionedQueryExecutionInfo = queryPlanFromContinuationToken;
        }
        else
        {
          if (cosmosQueryContext.QueryClient.ByPassQueryParsing() && inputParameters.PartitionKey.HasValue)
          {
            SqlQuery sqlQuery;
            bool flag1;
            using (createQueryPipelineTrace.StartChild("Parse Query", TraceComponent.Query, TraceLevel.Info))
              flag1 = SqlQueryParser.TryParse(inputParameters.SqlQuerySpec.QueryText, out sqlQuery);
            if (flag1)
            {
              bool hasDistinct = sqlQuery.SelectClause.HasDistinct;
              bool flag2 = (SqlObject) sqlQuery.GroupByClause != (SqlObject) null;
              if ((CosmosQueryExecutionContextFactory.AggregateProjectionDetector.HasAggregate(sqlQuery.SelectClause.SelectSpec) || hasDistinct ? 0 : (!flag2 ? 1 : 0)) != 0)
              {
                CosmosQueryExecutionContextFactory.SetTestInjectionPipelineType(inputParameters, "Passthrough");
                return CosmosQueryExecutionContextFactory.TryCreatePassthroughQueryExecutionContext(documentContainer, inputParameters, await cosmosQueryContext.QueryClient.GetTargetPartitionKeyRangesByEpkStringAsync(cosmosQueryContext.ResourceLink, containerQueryProperties.ResourceId, inputParameters.PartitionKey.Value.InternalKey.GetEffectivePartitionKeyString(CosmosQueryExecutionContextFactory.GetPartitionKeyDefinition(inputParameters, containerQueryProperties)), false, createQueryPipelineTrace), cancellationToken);
              }
            }
          }
          if (cosmosQueryContext.QueryClient.ByPassQueryParsing())
            partitionedQueryExecutionInfo = await QueryPlanRetriever.GetQueryPlanThroughGatewayAsync(cosmosQueryContext, inputParameters.SqlQuerySpec, cosmosQueryContext.ResourceLink, inputParameters.PartitionKey, createQueryPipelineTrace, cancellationToken);
          else
            partitionedQueryExecutionInfo = await QueryPlanRetriever.GetQueryPlanWithServiceInteropAsync(cosmosQueryContext.QueryClient, inputParameters.SqlQuerySpec, cosmosQueryContext.ResourceTypeEnum, CosmosQueryExecutionContextFactory.GetPartitionKeyDefinition(inputParameters, containerQueryProperties), inputParameters.PartitionKey.HasValue, cosmosQueryContext.UseSystemPrefix, createQueryPipelineTrace, cancellationToken);
        }
        return await CosmosQueryExecutionContextFactory.TryCreateFromPartitionedQueryExecutionInfoAsync(documentContainer, partitionedQueryExecutionInfo, containerQueryProperties, cosmosQueryContext, inputParameters, createQueryPipelineTrace, cancellationToken);
      }
    }

    public static async Task<TryCatch<IQueryPipelineStage>> TryCreateFromPartitionedQueryExecutionInfoAsync(
      DocumentContainer documentContainer,
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      ContainerQueryProperties containerQueryProperties,
      CosmosQueryContext cosmosQueryContext,
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      List<PartitionKeyRange> targetRanges = await CosmosQueryExecutionContextFactory.GetTargetPartitionKeyRangesAsync(cosmosQueryContext.QueryClient, cosmosQueryContext.ResourceLink, partitionedQueryExecutionInfo, containerQueryProperties, inputParameters.Properties, inputParameters.InitialFeedRange, trace);
      int num = inputParameters.PartitionKey.HasValue ? 1 : (partitionedQueryExecutionInfo.QueryRanges.Count != 1 ? 0 : (partitionedQueryExecutionInfo.QueryRanges[0].IsSingleValue ? 1 : 0));
      bool flag = !partitionedQueryExecutionInfo.QueryInfo.HasAggregates && !partitionedQueryExecutionInfo.QueryInfo.HasDistinct && !partitionedQueryExecutionInfo.QueryInfo.HasGroupBy;
      bool createPassthroughQuery = (num & (flag ? 1 : 0)) != 0 | num == 0 & (flag && !partitionedQueryExecutionInfo.QueryInfo.HasOrderBy && !partitionedQueryExecutionInfo.QueryInfo.HasTop && !partitionedQueryExecutionInfo.QueryInfo.HasLimit && !partitionedQueryExecutionInfo.QueryInfo.HasOffset);
      PartitionKeyRange directExecutionAsync = await CosmosQueryExecutionContextFactory.GetTargetRangeOptimisticDirectExecutionAsync(inputParameters, partitionedQueryExecutionInfo, cosmosQueryContext, containerQueryProperties, trace);
      if (directExecutionAsync != null)
      {
        CosmosQueryExecutionContextFactory.SetTestInjectionPipelineType(inputParameters, "OptimisticDirectExecution");
        return CosmosQueryExecutionContextFactory.OptimisticDirectExecutionContext(documentContainer, inputParameters, directExecutionAsync, cancellationToken);
      }
      TryCatch<IQueryPipelineStage> executionContext;
      if (createPassthroughQuery)
      {
        CosmosQueryExecutionContextFactory.SetTestInjectionPipelineType(inputParameters, "Passthrough");
        executionContext = CosmosQueryExecutionContextFactory.TryCreatePassthroughQueryExecutionContext(documentContainer, inputParameters, targetRanges, cancellationToken);
      }
      else
      {
        CosmosQueryExecutionContextFactory.SetTestInjectionPipelineType(inputParameters, "Specialized");
        if (!string.IsNullOrEmpty(partitionedQueryExecutionInfo.QueryInfo.RewrittenQuery))
          inputParameters = new CosmosQueryExecutionContextFactory.InputParameters(new SqlQuerySpec()
          {
            QueryText = partitionedQueryExecutionInfo.QueryInfo.RewrittenQuery,
            Parameters = inputParameters.SqlQuerySpec.Parameters
          }, inputParameters.InitialUserContinuationToken, inputParameters.InitialFeedRange, new int?(inputParameters.MaxConcurrency), new int?(inputParameters.MaxItemCount), new int?(inputParameters.MaxBufferedItemCount), inputParameters.PartitionKey, inputParameters.Properties, inputParameters.PartitionedQueryExecutionInfo, new ExecutionEnvironment?(inputParameters.ExecutionEnvironment), new bool?(inputParameters.ReturnResultsInDeterministicOrder), inputParameters.ForcePassthrough, inputParameters.TestInjections);
        executionContext = CosmosQueryExecutionContextFactory.TryCreateSpecializedDocumentQueryExecutionContext(documentContainer, cosmosQueryContext, inputParameters, partitionedQueryExecutionInfo, targetRanges, cancellationToken);
      }
      return executionContext;
    }

    private static TryCatch<IQueryPipelineStage> OptimisticDirectExecutionContext(
      DocumentContainer documentContainer,
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      PartitionKeyRange targetRange,
      CancellationToken cancellationToken)
    {
      DocumentContainer documentContainer1 = documentContainer;
      SqlQuerySpec sqlQuerySpec = inputParameters.SqlQuerySpec;
      FeedRangeEpk targetRange1 = new FeedRangeEpk(targetRange.ToRange());
      QueryPaginationOptions paginationOptions = new QueryPaginationOptions(new int?(inputParameters.MaxItemCount));
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey = inputParameters.PartitionKey;
      QueryPaginationOptions queryPaginationOptions = paginationOptions;
      CosmosElement continuationToken = inputParameters.InitialUserContinuationToken;
      CancellationToken cancellationToken1 = cancellationToken;
      return OptimisticDirectExecutionQueryPipelineStage.MonadicCreate((IDocumentContainer) documentContainer1, sqlQuerySpec, targetRange1, partitionKey, queryPaginationOptions, continuationToken, cancellationToken1);
    }

    private static TryCatch<IQueryPipelineStage> TryCreatePassthroughQueryExecutionContext(
      DocumentContainer documentContainer,
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      List<PartitionKeyRange> targetRanges,
      CancellationToken cancellationToken)
    {
      DocumentContainer documentContainer1 = documentContainer;
      SqlQuerySpec sqlQuerySpec = inputParameters.SqlQuerySpec;
      List<FeedRangeEpk> list = targetRanges.Select<PartitionKeyRange, FeedRangeEpk>((Func<PartitionKeyRange, FeedRangeEpk>) (range => new FeedRangeEpk(new Range<string>(range.MinInclusive, range.MaxExclusive, true, false)))).ToList<FeedRangeEpk>();
      QueryPaginationOptions paginationOptions = new QueryPaginationOptions(new int?(inputParameters.MaxItemCount));
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey = inputParameters.PartitionKey;
      QueryPaginationOptions queryPaginationOptions = paginationOptions;
      int maxConcurrency = inputParameters.MaxConcurrency;
      CancellationToken cancellationToken1 = cancellationToken;
      CosmosElement continuationToken = inputParameters.InitialUserContinuationToken;
      CancellationToken cancellationToken2 = cancellationToken1;
      return ParallelCrossPartitionQueryPipelineStage.MonadicCreate((IDocumentContainer) documentContainer1, sqlQuerySpec, (IReadOnlyList<FeedRangeEpk>) list, partitionKey, queryPaginationOptions, maxConcurrency, PrefetchPolicy.PrefetchSinglePage, continuationToken, cancellationToken2);
    }

    private static TryCatch<IQueryPipelineStage> TryCreateSpecializedDocumentQueryExecutionContext(
      DocumentContainer documentContainer,
      CosmosQueryContext cosmosQueryContext,
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      List<PartitionKeyRange> targetRanges,
      CancellationToken cancellationToken)
    {
      QueryInfo queryInfo = partitionedQueryExecutionInfo.QueryInfo;
      long val2_1 = (long) inputParameters.MaxItemCount;
      if (queryInfo.HasOrderBy)
      {
        if (queryInfo.HasTop)
        {
          int? top = partitionedQueryExecutionInfo.QueryInfo.Top;
          int val2_2;
          if ((val2_2 = top.Value) > 0)
          {
            val2_1 = Math.Min((long) Math.Min(Math.Ceiling((double) val2_2 / (double) targetRanges.Count) * 5.0, (double) val2_2), val2_1);
            goto label_6;
          }
        }
        if (cosmosQueryContext.IsContinuationExpected)
          val2_1 = (long) Math.Min(Math.Ceiling((double) val2_1 / (double) targetRanges.Count) * 5.0, (double) val2_1);
      }
label_6:
      return PipelineFactory.MonadicCreate(inputParameters.ExecutionEnvironment, (IDocumentContainer) documentContainer, inputParameters.SqlQuerySpec, (IReadOnlyList<FeedRangeEpk>) targetRanges.Select<PartitionKeyRange, FeedRangeEpk>((Func<PartitionKeyRange, FeedRangeEpk>) (range => new FeedRangeEpk(new Range<string>(range.MinInclusive, range.MaxExclusive, true, false)))).ToList<FeedRangeEpk>(), inputParameters.PartitionKey, partitionedQueryExecutionInfo.QueryInfo, new QueryPaginationOptions(new int?((int) val2_1)), inputParameters.MaxConcurrency, inputParameters.InitialUserContinuationToken, cancellationToken);
    }

    internal static async Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangesAsync(
      CosmosQueryClient queryClient,
      string resourceLink,
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      ContainerQueryProperties containerQueryProperties,
      IReadOnlyDictionary<string, object> properties,
      FeedRangeInternal feedRangeInternal,
      ITrace trace)
    {
      List<PartitionKeyRange> partitionKeyRangesAsync;
      if (containerQueryProperties.EffectivePartitionKeyString != null)
      {
        partitionKeyRangesAsync = await queryClient.GetTargetPartitionKeyRangesByEpkStringAsync(resourceLink, containerQueryProperties.ResourceId, containerQueryProperties.EffectivePartitionKeyString, false, trace);
      }
      else
      {
        string effectivePartitionKeyString;
        if (CosmosQueryExecutionContextFactory.TryGetEpkProperty(properties, out effectivePartitionKeyString))
          partitionKeyRangesAsync = await queryClient.GetTargetPartitionKeyRangesByEpkStringAsync(resourceLink, containerQueryProperties.ResourceId, effectivePartitionKeyString, false, trace);
        else if (feedRangeInternal != null)
          partitionKeyRangesAsync = await queryClient.GetTargetPartitionKeyRangeByFeedRangeAsync(resourceLink, containerQueryProperties.ResourceId, containerQueryProperties.PartitionKeyDefinition, feedRangeInternal, false, trace);
        else
          partitionKeyRangesAsync = await queryClient.GetTargetPartitionKeyRangesAsync(resourceLink, containerQueryProperties.ResourceId, partitionedQueryExecutionInfo.QueryRanges, false, trace);
      }
      return partitionKeyRangesAsync;
    }

    public static void SetTestInjectionPipelineType(
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      string pipelineType)
    {
      TestInjections.ResponseStats stats = inputParameters?.TestInjections?.Stats;
      if (stats == null)
        return;
      switch (pipelineType)
      {
        case "OptimisticDirectExecution":
          stats.PipelineType = new TestInjections.PipelineType?(TestInjections.PipelineType.OptimisticDirectExecution);
          break;
        case "Specialized":
          stats.PipelineType = new TestInjections.PipelineType?(TestInjections.PipelineType.Specialized);
          break;
        default:
          stats.PipelineType = new TestInjections.PipelineType?(TestInjections.PipelineType.Passthrough);
          break;
      }
    }

    private static bool TryGetEpkProperty(
      IReadOnlyDictionary<string, object> properties,
      out string effectivePartitionKeyString)
    {
      object obj;
      if (properties != null && properties.TryGetValue("x-ms-effective-partition-key-string", out obj))
      {
        effectivePartitionKeyString = obj as string;
        if (string.IsNullOrEmpty(effectivePartitionKeyString))
          throw new ArgumentOutOfRangeException(nameof (effectivePartitionKeyString));
        return true;
      }
      effectivePartitionKeyString = (string) null;
      return false;
    }

    private static PartitionKeyDefinition GetPartitionKeyDefinition(
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      ContainerQueryProperties containerQueryProperties)
    {
      object obj;
      PartitionKeyDefinition partitionKeyDefinition1;
      if (inputParameters.Properties != null && inputParameters.Properties.TryGetValue("x-ms-query-partitionkey-definition", out obj))
        partitionKeyDefinition1 = obj is PartitionKeyDefinition partitionKeyDefinition2 ? partitionKeyDefinition2 : throw new ArgumentException("partitionkeydefinition has invalid type", "partitionKeyDefinitionObject");
      else
        partitionKeyDefinition1 = containerQueryProperties.PartitionKeyDefinition;
      return partitionKeyDefinition1;
    }

    private static async Task<PartitionKeyRange> GetTargetRangeOptimisticDirectExecutionAsync(
      CosmosQueryExecutionContextFactory.InputParameters inputParameters,
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      CosmosQueryContext cosmosQueryContext,
      ContainerQueryProperties containerQueryProperties,
      ITrace trace)
    {
      if (inputParameters.TestInjections == null || !inputParameters.TestInjections.EnableOptimisticDirectExecution)
        return (PartitionKeyRange) null;
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey1 = inputParameters.PartitionKey;
      int num;
      if (partitionKey1.HasValue)
      {
        partitionKey1 = inputParameters.PartitionKey;
        Microsoft.Azure.Cosmos.PartitionKey partitionKey2 = Microsoft.Azure.Cosmos.PartitionKey.Null;
        if ((partitionKey1.HasValue ? (partitionKey1.HasValue ? (partitionKey1.GetValueOrDefault() != partitionKey2 ? 1 : 0) : 0) : 1) != 0)
        {
          partitionKey1 = inputParameters.PartitionKey;
          Microsoft.Azure.Cosmos.PartitionKey none = Microsoft.Azure.Cosmos.PartitionKey.None;
          num = partitionKey1.HasValue ? (partitionKey1.HasValue ? (partitionKey1.GetValueOrDefault() != none ? 1 : 0) : 0) : 1;
          goto label_6;
        }
      }
      num = 0;
label_6:
      bool flag = partitionedQueryExecutionInfo != null && partitionedQueryExecutionInfo.QueryRanges.Count == 1 && partitionedQueryExecutionInfo.QueryRanges[0].IsSingleValue;
      if (num == 0 && !flag)
        return (PartitionKeyRange) null;
      List<PartitionKeyRange> source = new List<PartitionKeyRange>();
      if (partitionedQueryExecutionInfo != null)
      {
        source = await CosmosQueryExecutionContextFactory.GetTargetPartitionKeyRangesAsync(cosmosQueryContext.QueryClient, cosmosQueryContext.ResourceLink, partitionedQueryExecutionInfo, containerQueryProperties, inputParameters.Properties, inputParameters.InitialFeedRange, trace);
      }
      else
      {
        PartitionKeyDefinition partitionKeyDefinition = CosmosQueryExecutionContextFactory.GetPartitionKeyDefinition(inputParameters, containerQueryProperties);
        if (partitionKeyDefinition != null && containerQueryProperties.ResourceId != null)
        {
          partitionKey1 = inputParameters.PartitionKey;
          if (partitionKey1.HasValue)
          {
            CosmosQueryClient queryClient = cosmosQueryContext.QueryClient;
            string resourceLink = cosmosQueryContext.ResourceLink;
            string resourceId = containerQueryProperties.ResourceId;
            partitionKey1 = inputParameters.PartitionKey;
            string partitionKeyString = partitionKey1.Value.InternalKey.GetEffectivePartitionKeyString(partitionKeyDefinition);
            ITrace trace1 = trace;
            source = await queryClient.GetTargetPartitionKeyRangesByEpkStringAsync(resourceLink, resourceId, partitionKeyString, false, trace1);
          }
        }
      }
      return source.Count != 1 ? (PartitionKeyRange) null : source.Single<PartitionKeyRange>();
    }

    public sealed class InputParameters
    {
      private const int DefaultMaxConcurrency = 0;
      private const int DefaultMaxItemCount = 1000;
      private const int DefaultMaxBufferedItemCount = 1000;
      private const bool DefaultReturnResultsInDeterministicOrder = true;
      private const ExecutionEnvironment DefaultExecutionEnvironment = ExecutionEnvironment.Client;

      public InputParameters(
        SqlQuerySpec sqlQuerySpec,
        CosmosElement initialUserContinuationToken,
        FeedRangeInternal initialFeedRange,
        int? maxConcurrency,
        int? maxItemCount,
        int? maxBufferedItemCount,
        Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
        IReadOnlyDictionary<string, object> properties,
        Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
        ExecutionEnvironment? executionEnvironment,
        bool? returnResultsInDeterministicOrder,
        bool forcePassthrough,
        TestInjections testInjections)
      {
        this.SqlQuerySpec = sqlQuerySpec ?? throw new ArgumentNullException(nameof (sqlQuerySpec));
        this.InitialUserContinuationToken = initialUserContinuationToken;
        this.InitialFeedRange = initialFeedRange;
        int num1 = maxConcurrency.GetValueOrDefault(0);
        if (num1 < 0)
          num1 = int.MaxValue;
        this.MaxConcurrency = num1;
        int num2 = maxItemCount.GetValueOrDefault(1000);
        if (num2 < 0)
          num2 = int.MaxValue;
        this.MaxItemCount = num2;
        int num3 = maxBufferedItemCount.GetValueOrDefault(1000);
        if (num3 < 0)
          num3 = int.MaxValue;
        this.MaxBufferedItemCount = num3;
        this.PartitionKey = partitionKey;
        this.Properties = properties;
        this.PartitionedQueryExecutionInfo = partitionedQueryExecutionInfo;
        this.ExecutionEnvironment = executionEnvironment.GetValueOrDefault(ExecutionEnvironment.Client);
        this.ReturnResultsInDeterministicOrder = returnResultsInDeterministicOrder.GetValueOrDefault(true);
        this.ForcePassthrough = forcePassthrough;
        this.TestInjections = testInjections;
      }

      public SqlQuerySpec SqlQuerySpec { get; }

      public CosmosElement InitialUserContinuationToken { get; }

      public FeedRangeInternal InitialFeedRange { get; }

      public int MaxConcurrency { get; }

      public int MaxItemCount { get; }

      public int MaxBufferedItemCount { get; }

      public Microsoft.Azure.Cosmos.PartitionKey? PartitionKey { get; }

      public IReadOnlyDictionary<string, object> Properties { get; }

      public Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo PartitionedQueryExecutionInfo { get; }

      public ExecutionEnvironment ExecutionEnvironment { get; }

      public bool ReturnResultsInDeterministicOrder { get; }

      public TestInjections TestInjections { get; }

      public bool ForcePassthrough { get; }
    }

    internal sealed class AggregateProjectionDetector
    {
      public static bool HasAggregate(SqlSelectSpec selectSpec) => selectSpec.Accept<bool>((SqlSelectSpecVisitor<bool>) CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor.Singleton);

      private sealed class AggregateProjectionDectorVisitor : SqlSelectSpecVisitor<bool>
      {
        public static readonly CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor Singleton = new CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor();

        public override bool Visit(SqlSelectListSpec selectSpec)
        {
          bool flag = false;
          foreach (SqlSelectItem sqlSelectItem in selectSpec.Items)
            flag |= sqlSelectItem.Expression.Accept<bool>((SqlScalarExpressionVisitor<bool>) CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor.AggregateScalarExpressionDetector.Singleton);
          return flag;
        }

        public override bool Visit(SqlSelectValueSpec selectSpec) => selectSpec.Expression.Accept<bool>((SqlScalarExpressionVisitor<bool>) CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor.AggregateScalarExpressionDetector.Singleton);

        public override bool Visit(SqlSelectStarSpec selectSpec) => false;

        private sealed class AggregateScalarExpressionDetector : SqlScalarExpressionVisitor<bool>
        {
          public static readonly CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor.AggregateScalarExpressionDetector Singleton = new CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor.AggregateScalarExpressionDetector();

          public override bool Visit(
            SqlArrayCreateScalarExpression sqlArrayCreateScalarExpression)
          {
            bool flag = false;
            foreach (SqlScalarExpression scalarExpression in sqlArrayCreateScalarExpression.Items)
              flag |= scalarExpression.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);
            return flag;
          }

          public override bool Visit(SqlArrayScalarExpression sqlArrayScalarExpression) => false;

          public override bool Visit(
            SqlBetweenScalarExpression sqlBetweenScalarExpression)
          {
            return sqlBetweenScalarExpression.Expression.Accept<bool>((SqlScalarExpressionVisitor<bool>) this) || sqlBetweenScalarExpression.StartInclusive.Accept<bool>((SqlScalarExpressionVisitor<bool>) this) || sqlBetweenScalarExpression.EndInclusive.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);
          }

          public override bool Visit(
            SqlBinaryScalarExpression sqlBinaryScalarExpression)
          {
            return sqlBinaryScalarExpression.LeftExpression.Accept<bool>((SqlScalarExpressionVisitor<bool>) this) || sqlBinaryScalarExpression.RightExpression.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);
          }

          public override bool Visit(
            SqlCoalesceScalarExpression sqlCoalesceScalarExpression)
          {
            return sqlCoalesceScalarExpression.Left.Accept<bool>((SqlScalarExpressionVisitor<bool>) this) || sqlCoalesceScalarExpression.Right.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);
          }

          public override bool Visit(
            SqlConditionalScalarExpression sqlConditionalScalarExpression)
          {
            return sqlConditionalScalarExpression.Condition.Accept<bool>((SqlScalarExpressionVisitor<bool>) this) || sqlConditionalScalarExpression.Consequent.Accept<bool>((SqlScalarExpressionVisitor<bool>) this) || sqlConditionalScalarExpression.Alternative.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);
          }

          public override bool Visit(
            SqlExistsScalarExpression sqlExistsScalarExpression)
          {
            return false;
          }

          public override bool Visit(
            SqlFunctionCallScalarExpression sqlFunctionCallScalarExpression)
          {
            return !sqlFunctionCallScalarExpression.IsUdf && Enum.TryParse<CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor.AggregateScalarExpressionDetector.Aggregate>(sqlFunctionCallScalarExpression.Name.Value, true, out CosmosQueryExecutionContextFactory.AggregateProjectionDetector.AggregateProjectionDectorVisitor.AggregateScalarExpressionDetector.Aggregate _);
          }

          public override bool Visit(SqlInScalarExpression sqlInScalarExpression)
          {
            bool flag = false;
            int index = 0;
            while (true)
            {
              int num1 = index;
              ImmutableArray<SqlScalarExpression> haystack = sqlInScalarExpression.Haystack;
              int length = haystack.Length;
              if (num1 < length)
              {
                int num2 = flag ? 1 : 0;
                haystack = sqlInScalarExpression.Haystack;
                int num3 = haystack[index].Accept<bool>((SqlScalarExpressionVisitor<bool>) this) ? 1 : 0;
                flag = (num2 | num3) != 0;
                ++index;
              }
              else
                break;
            }
            return flag;
          }

          public override bool Visit(
            SqlLiteralScalarExpression sqlLiteralScalarExpression)
          {
            return false;
          }

          public override bool Visit(
            SqlMemberIndexerScalarExpression sqlMemberIndexerScalarExpression)
          {
            return sqlMemberIndexerScalarExpression.Member.Accept<bool>((SqlScalarExpressionVisitor<bool>) this) || sqlMemberIndexerScalarExpression.Indexer.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);
          }

          public override bool Visit(
            SqlObjectCreateScalarExpression sqlObjectCreateScalarExpression)
          {
            bool flag = false;
            foreach (SqlObjectProperty property in sqlObjectCreateScalarExpression.Properties)
              flag |= property.Value.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);
            return flag;
          }

          public override bool Visit(
            SqlPropertyRefScalarExpression sqlPropertyRefScalarExpression)
          {
            bool flag = false;
            if ((SqlObject) sqlPropertyRefScalarExpression.Member != (SqlObject) null)
              flag = sqlPropertyRefScalarExpression.Member.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);
            return flag;
          }

          public override bool Visit(
            SqlSubqueryScalarExpression sqlSubqueryScalarExpression)
          {
            return false;
          }

          public override bool Visit(SqlUnaryScalarExpression sqlUnaryScalarExpression) => sqlUnaryScalarExpression.Expression.Accept<bool>((SqlScalarExpressionVisitor<bool>) this);

          public override bool Visit(SqlParameterRefScalarExpression scalarExpression) => false;

          public override bool Visit(SqlLikeScalarExpression scalarExpression) => false;

          private enum Aggregate
          {
            Min,
            Max,
            Sum,
            Count,
            Avg,
          }
        }
      }
    }
  }
}

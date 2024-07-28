// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryPlanRetriever
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryPlan
{
  internal static class QueryPlanRetriever
  {
    private static readonly QueryFeatures SupportedQueryFeatures = QueryFeatures.Aggregate | QueryFeatures.Distinct | QueryFeatures.GroupBy | QueryFeatures.MultipleAggregates | QueryFeatures.MultipleOrderBy | QueryFeatures.OffsetAndLimit | QueryFeatures.OrderBy | QueryFeatures.Top | QueryFeatures.NonValueAggregate | QueryFeatures.DCount;
    private static readonly string SupportedQueryFeaturesString = QueryPlanRetriever.SupportedQueryFeatures.ToString();

    public static async Task<PartitionedQueryExecutionInfo> GetQueryPlanWithServiceInteropAsync(
      CosmosQueryClient queryClient,
      SqlQuerySpec sqlQuerySpec,
      ResourceType resourceType,
      PartitionKeyDefinition partitionKeyDefinition,
      bool hasLogicalPartitionKey,
      bool useSystemPrefix,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (queryClient == null)
        throw new ArgumentNullException(nameof (queryClient));
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (partitionKeyDefinition == null)
        throw new ArgumentNullException(nameof (partitionKeyDefinition));
      cancellationToken.ThrowIfCancellationRequested();
      PartitionedQueryExecutionInfo result;
      using (ITrace serviceInteropTrace = trace.StartChild("Service Interop Query Plan", TraceComponent.Query, TraceLevel.Info))
      {
        TryCatch<PartitionedQueryExecutionInfo> queryPlanAsync = await new QueryPlanHandler(queryClient).TryGetQueryPlanAsync(sqlQuerySpec, resourceType, partitionKeyDefinition, QueryPlanRetriever.SupportedQueryFeatures, hasLogicalPartitionKey, useSystemPrefix, cancellationToken);
        if (!queryPlanAsync.Succeeded)
        {
          Exception exception1 = ExceptionWithStackTraceException.UnWrapMonadExcepion(queryPlanAsync.Exception, serviceInteropTrace);
          string message = !(exception1 is CosmosException) ? exception1.Message : throw exception1;
          Microsoft.Azure.Cosmos.Headers headers = new Microsoft.Azure.Cosmos.Headers();
          string stackTrace = queryPlanAsync.Exception.StackTrace;
          Exception exception2 = exception1;
          ITrace trace1 = trace;
          Exception innerException = exception2;
          throw CosmosExceptionFactory.CreateBadRequestException(message, headers, stackTrace, trace1, innerException: innerException);
        }
        result = queryPlanAsync.Result;
      }
      return result;
    }

    public static Task<PartitionedQueryExecutionInfo> GetQueryPlanThroughGatewayAsync(
      CosmosQueryContext queryContext,
      SqlQuerySpec sqlQuerySpec,
      string resourceLink,
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (queryContext == null)
        throw new ArgumentNullException(nameof (queryContext));
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (resourceLink == null)
        throw new ArgumentNullException(nameof (resourceLink));
      cancellationToken.ThrowIfCancellationRequested();
      using (ITrace trace1 = trace.StartChild("Gateway QueryPlan", TraceComponent.Query, TraceLevel.Info))
      {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && ServiceInteropWrapper.Is64BitProcess)
          trace1.AddDatum("ServiceInterop unavailable", (object) true);
        return queryContext.ExecuteQueryPlanRequestAsync(resourceLink, ResourceType.Document, OperationType.QueryPlan, sqlQuerySpec, partitionKey, QueryPlanRetriever.SupportedQueryFeaturesString, trace, cancellationToken);
      }
    }
  }
}

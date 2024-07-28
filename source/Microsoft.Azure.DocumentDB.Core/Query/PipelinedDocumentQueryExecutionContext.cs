// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.PipelinedDocumentQueryExecutionContext
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Query.ExecutionComponent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class PipelinedDocumentQueryExecutionContext : 
    IDocumentQueryExecutionContext,
    IDisposable
  {
    private readonly IDocumentQueryExecutionComponent component;
    private readonly int actualPageSize;

    private PipelinedDocumentQueryExecutionContext(
      IDocumentQueryExecutionComponent component,
      int actualPageSize)
    {
      if (component == null)
        throw new ArgumentNullException("component can not be null.");
      if (actualPageSize < 0)
        throw new ArgumentException("actualPageSize can not be negative.");
      this.component = component;
      this.actualPageSize = actualPageSize;
    }

    public bool IsDone => this.component.IsDone;

    public static async Task<IDocumentQueryExecutionContext> CreateAsync(
      DocumentQueryExecutionContextBase.InitParams constructorParams,
      string collectionRid,
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      List<PartitionKeyRange> partitionKeyRanges,
      int initialPageSize,
      string requestContinuation,
      CancellationToken cancellationToken)
    {
      DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, CorrelatedActivityId: {1} | Pipelined~Context.CreateAsync", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) constructorParams.CorrelatedActivityId));
      int actualPageSize = initialPageSize;
      if ((partitionedQueryExecutionInfo.QueryInfo.HasAggregates || partitionedQueryExecutionInfo.QueryInfo.HasDistinct ? 1 : (partitionedQueryExecutionInfo.QueryInfo.HasGroupBy ? 1 : 0)) != 0)
      {
        initialPageSize = int.MaxValue;
        constructorParams.FeedOptions.MaxItemCount = new int?(int.MaxValue);
      }
      QueryInfo queryInfo = partitionedQueryExecutionInfo.QueryInfo;
      Func<string, Task<IDocumentQueryExecutionComponent>> func = !queryInfo.HasOrderBy ? (Func<string, Task<IDocumentQueryExecutionComponent>>) (async continuationToken =>
      {
        CrossPartitionQueryExecutionContext<object>.CrossPartitionInitParams initParams = new CrossPartitionQueryExecutionContext<object>.CrossPartitionInitParams(collectionRid, partitionedQueryExecutionInfo, partitionKeyRanges, initialPageSize, continuationToken);
        return (IDocumentQueryExecutionComponent) await ParallelDocumentQueryExecutionContext.CreateAsync(constructorParams, initParams, cancellationToken);
      }) : (Func<string, Task<IDocumentQueryExecutionComponent>>) (async continuationToken =>
      {
        CrossPartitionQueryExecutionContext<object>.CrossPartitionInitParams initParams = new CrossPartitionQueryExecutionContext<object>.CrossPartitionInitParams(collectionRid, partitionedQueryExecutionInfo, partitionKeyRanges, initialPageSize, continuationToken);
        return (IDocumentQueryExecutionComponent) await OrderByDocumentQueryExecutionContext.CreateAsync(constructorParams, initParams, cancellationToken);
      });
      if (queryInfo.HasAggregates && !queryInfo.HasGroupBy)
      {
        Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback = func;
        func = (Func<string, Task<IDocumentQueryExecutionComponent>>) (async continuationToken => (IDocumentQueryExecutionComponent) await AggregateDocumentQueryExecutionComponent.CreateAsync(queryInfo.Aggregates, (IReadOnlyDictionary<string, AggregateOperator?>) queryInfo.GroupByAliasToAggregateType, (IReadOnlyList<string>) queryInfo.GroupByAliases, queryInfo.HasSelectValue, continuationToken, createSourceCallback));
      }
      if (queryInfo.HasDistinct)
      {
        Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback = func;
        func = (Func<string, Task<IDocumentQueryExecutionComponent>>) (async continuationToken => await DistinctDocumentQueryExecutionComponent.CreateAsync(continuationToken, createSourceCallback, queryInfo.DistinctType));
      }
      if (queryInfo.HasGroupBy)
      {
        Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback = func;
        func = (Func<string, Task<IDocumentQueryExecutionComponent>>) (async continuationToken => await GroupByDocumentQueryExecutionComponent.CreateAsync(continuationToken, createSourceCallback, (IReadOnlyDictionary<string, AggregateOperator?>) queryInfo.GroupByAliasToAggregateType, (IReadOnlyList<string>) queryInfo.GroupByAliases, queryInfo.HasSelectValue));
      }
      if (queryInfo.HasOffset)
      {
        Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback = func;
        func = (Func<string, Task<IDocumentQueryExecutionComponent>>) (async continuationToken => (IDocumentQueryExecutionComponent) await SkipDocumentQueryExecutionComponent.CreateAsync(queryInfo.Offset.Value, continuationToken, createSourceCallback));
      }
      if (queryInfo.HasLimit)
      {
        Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback = func;
        func = (Func<string, Task<IDocumentQueryExecutionComponent>>) (async continuationToken => (IDocumentQueryExecutionComponent) await TakeDocumentQueryExecutionComponent.CreateLimitDocumentQueryExecutionComponentAsync(queryInfo.Limit.Value, continuationToken, createSourceCallback));
      }
      if (queryInfo.HasTop)
      {
        Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback = func;
        func = (Func<string, Task<IDocumentQueryExecutionComponent>>) (async continuationToken => (IDocumentQueryExecutionComponent) await TakeDocumentQueryExecutionComponent.CreateTopDocumentQueryExecutionComponentAsync(queryInfo.Top.Value, continuationToken, createSourceCallback));
      }
      return (IDocumentQueryExecutionContext) new PipelinedDocumentQueryExecutionContext(await func(requestContinuation), actualPageSize);
    }

    public void Dispose() => this.component.Dispose();

    public async Task<FeedResponse<object>> ExecuteNextAsync(CancellationToken token)
    {
      FeedResponse<object> feedResponse;
      try
      {
        feedResponse = await this.component.DrainAsync(this.actualPageSize, token);
      }
      catch (Exception ex)
      {
        this.component.Stop();
        throw;
      }
      return feedResponse;
    }
  }
}

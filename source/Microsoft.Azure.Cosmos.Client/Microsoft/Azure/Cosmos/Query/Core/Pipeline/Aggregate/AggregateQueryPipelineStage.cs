// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.AggregateQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate
{
  internal abstract class AggregateQueryPipelineStage : QueryPipelineStageBase
  {
    private static readonly IReadOnlyList<CosmosElement> EmptyResults = (IReadOnlyList<CosmosElement>) new List<CosmosElement>().AsReadOnly();
    private readonly SingleGroupAggregator singleGroupAggregator;
    private readonly bool isValueQuery;
    protected bool returnedFinalPage;

    public AggregateQueryPipelineStage(
      IQueryPipelineStage source,
      SingleGroupAggregator singleGroupAggregator,
      bool isValueQuery,
      CancellationToken cancellationToken)
      : base(source, cancellationToken)
    {
      this.singleGroupAggregator = singleGroupAggregator ?? throw new ArgumentNullException(nameof (singleGroupAggregator));
      this.isValueQuery = isValueQuery;
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      ExecutionEnvironment executionEnvironment,
      IReadOnlyList<AggregateOperator> aggregates,
      IReadOnlyDictionary<string, AggregateOperator?> aliasToAggregateType,
      IReadOnlyList<string> orderedAliases,
      bool hasSelectValue,
      CosmosElement continuationToken,
      CancellationToken cancellationToken,
      MonadicCreatePipelineStage monadicCreatePipelineStage)
    {
      if (executionEnvironment == ExecutionEnvironment.Client)
        return AggregateQueryPipelineStage.ClientAggregateQueryPipelineStage.MonadicCreate(aggregates, aliasToAggregateType, orderedAliases, hasSelectValue, continuationToken, cancellationToken, monadicCreatePipelineStage);
      if (executionEnvironment == ExecutionEnvironment.Compute)
        return AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.MonadicCreate(aggregates, aliasToAggregateType, orderedAliases, hasSelectValue, continuationToken, cancellationToken, monadicCreatePipelineStage);
      throw new ArgumentException(string.Format("Unknown {0}: {1}.", (object) "ExecutionEnvironment", (object) executionEnvironment));
    }

    private sealed class ClientAggregateQueryPipelineStage : AggregateQueryPipelineStage
    {
      private ClientAggregateQueryPipelineStage(
        IQueryPipelineStage source,
        SingleGroupAggregator singleGroupAggregator,
        bool isValueAggregateQuery,
        CancellationToken cancellationToken)
        : base(source, singleGroupAggregator, isValueAggregateQuery, cancellationToken)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        IReadOnlyList<AggregateOperator> aggregates,
        IReadOnlyDictionary<string, AggregateOperator?> aliasToAggregateType,
        IReadOnlyList<string> orderedAliases,
        bool hasSelectValue,
        CosmosElement continuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        TryCatch<SingleGroupAggregator> tryCatch1 = SingleGroupAggregator.TryCreate(aggregates, aliasToAggregateType, orderedAliases, hasSelectValue, (CosmosElement) null);
        if (tryCatch1.Failed)
          return TryCatch<IQueryPipelineStage>.FromException(tryCatch1.Exception);
        TryCatch<IQueryPipelineStage> tryCatch2 = monadicCreatePipelineStage(continuationToken, cancellationToken);
        return tryCatch2.Failed ? tryCatch2 : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new AggregateQueryPipelineStage.ClientAggregateQueryPipelineStage(tryCatch2.Result, tryCatch1.Result, hasSelectValue, cancellationToken));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        AggregateQueryPipelineStage.ClientAggregateQueryPipelineStage queryPipelineStage = this;
        queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        if (queryPipelineStage.returnedFinalPage)
          return false;
        double requestCharge = 0.0;
        long responseLengthBytes = 0;
label_14:
        if (await queryPipelineStage.inputStage.MoveNextAsync(trace))
        {
          TryCatch<QueryPage> current1 = queryPipelineStage.inputStage.Current;
          if (current1.Failed)
          {
            queryPipelineStage.Current = current1;
            return true;
          }
          QueryPage result = current1.Result;
          requestCharge += result.RequestCharge;
          responseLengthBytes += result.ResponseLengthInBytes;
          using (IEnumerator<CosmosElement> enumerator = result.Documents.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              CosmosElement current2 = enumerator.Current;
              queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
              AggregateQueryPipelineStage.RewrittenAggregateProjections aggregateProjections = new AggregateQueryPipelineStage.RewrittenAggregateProjections(queryPipelineStage.isValueQuery, current2);
              queryPipelineStage.singleGroupAggregator.AddValues(aggregateProjections.Payload);
            }
            goto label_14;
          }
        }
        else
        {
          List<CosmosElement> documents = new List<CosmosElement>();
          CosmosElement result1 = queryPipelineStage.singleGroupAggregator.GetResult();
          if (result1 != (CosmosElement) null)
            documents.Add(result1);
          QueryPage result2 = new QueryPage((IReadOnlyList<CosmosElement>) documents, requestCharge, (string) null, responseLengthBytes, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, (IReadOnlyDictionary<string, string>) null, (QueryState) null);
          queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result2);
          queryPipelineStage.returnedFinalPage = true;
          return true;
        }
      }
    }

    private sealed class ComputeAggregateQueryPipelineStage : AggregateQueryPipelineStage
    {
      private static readonly CosmosString DoneSourceToken = CosmosString.Create("DONE");

      private ComputeAggregateQueryPipelineStage(
        IQueryPipelineStage source,
        SingleGroupAggregator singleGroupAggregator,
        bool isValueAggregateQuery,
        CancellationToken cancellationToken)
        : base(source, singleGroupAggregator, isValueAggregateQuery, cancellationToken)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        IReadOnlyList<AggregateOperator> aggregates,
        IReadOnlyDictionary<string, AggregateOperator?> aliasToAggregateType,
        IReadOnlyList<string> orderedAliases,
        bool hasSelectValue,
        CosmosElement continuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        cancellationToken.ThrowIfCancellationRequested();
        AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken aggregateContinuationToken;
        if (continuationToken != (CosmosElement) null)
        {
          if (!AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken.TryCreateFromCosmosElement(continuationToken, out aggregateContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malfomed {0}: '{1}'", (object) "AggregateContinuationToken", (object) continuationToken)));
        }
        else
          aggregateContinuationToken = new AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken((CosmosElement) null, (CosmosElement) null);
        TryCatch<SingleGroupAggregator> tryCatch1 = SingleGroupAggregator.TryCreate(aggregates, aliasToAggregateType, orderedAliases, hasSelectValue, aggregateContinuationToken.SingleGroupAggregatorContinuationToken);
        if (tryCatch1.Failed)
          return TryCatch<IQueryPipelineStage>.FromException(tryCatch1.Exception);
        TryCatch<IQueryPipelineStage> tryCatch2 = !(aggregateContinuationToken.SourceContinuationToken is CosmosString continuationToken1) || !UtfAnyString.op_Equality(continuationToken1.Value, AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.DoneSourceToken.Value) ? monadicCreatePipelineStage(aggregateContinuationToken.SourceContinuationToken, cancellationToken) : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) EmptyQueryPipelineStage.Singleton);
        return tryCatch2.Failed ? tryCatch2 : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage(tryCatch2.Result, tryCatch1.Result, hasSelectValue, cancellationToken));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage queryPipelineStage = this;
        queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        if (queryPipelineStage.returnedFinalPage)
        {
          queryPipelineStage.Current = new TryCatch<QueryPage>();
          return false;
        }
        QueryPage result1;
        if (await queryPipelineStage.inputStage.MoveNextAsync(trace))
        {
          TryCatch<QueryPage> current = queryPipelineStage.inputStage.Current;
          if (current.Failed)
          {
            queryPipelineStage.Current = current;
            return true;
          }
          QueryPage result2 = current.Result;
          foreach (CosmosElement document in (IEnumerable<CosmosElement>) result2.Documents)
          {
            queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
            AggregateQueryPipelineStage.RewrittenAggregateProjections aggregateProjections = new AggregateQueryPipelineStage.RewrittenAggregateProjections(queryPipelineStage.isValueQuery, document);
            queryPipelineStage.singleGroupAggregator.AddValues(aggregateProjections.Payload);
          }
          QueryState state = new QueryState(AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken.ToCosmosElement(new AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken(queryPipelineStage.singleGroupAggregator.GetCosmosElementContinuationToken(), result2.State != null ? result2.State.Value : (CosmosElement) AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.DoneSourceToken)));
          result1 = new QueryPage(AggregateQueryPipelineStage.EmptyResults, result2.RequestCharge, result2.ActivityId, result2.ResponseLengthInBytes, result2.CosmosQueryExecutionInfo, result2.DisallowContinuationTokenMessage, result2.AdditionalHeaders, state);
        }
        else
        {
          List<CosmosElement> documents = new List<CosmosElement>();
          CosmosElement result3 = queryPipelineStage.singleGroupAggregator.GetResult();
          if (result3 != (CosmosElement) null)
            documents.Add(result3);
          result1 = new QueryPage((IReadOnlyList<CosmosElement>) documents, 0.0, (string) null, 0L, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, (IReadOnlyDictionary<string, string>) null, (QueryState) null);
          queryPipelineStage.returnedFinalPage = true;
        }
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result1);
        return true;
      }

      private sealed class AggregateContinuationToken
      {
        private const string SourceTokenName = "SourceToken";
        private const string AggregationTokenName = "AggregationToken";

        public AggregateContinuationToken(
          CosmosElement singleGroupAggregatorContinuationToken,
          CosmosElement sourceContinuationToken)
        {
          this.SingleGroupAggregatorContinuationToken = singleGroupAggregatorContinuationToken;
          this.SourceContinuationToken = sourceContinuationToken;
        }

        public CosmosElement SingleGroupAggregatorContinuationToken { get; }

        public CosmosElement SourceContinuationToken { get; }

        public static CosmosElement ToCosmosElement(
          AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken aggregateContinuationToken)
        {
          return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
          {
            {
              "SourceToken",
              aggregateContinuationToken.SourceContinuationToken
            },
            {
              "AggregationToken",
              aggregateContinuationToken.SingleGroupAggregatorContinuationToken
            }
          });
        }

        public static bool TryCreateFromCosmosElement(
          CosmosElement continuationToken,
          out AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken aggregateContinuationToken)
        {
          if (continuationToken == (CosmosElement) null)
            throw new ArgumentNullException(nameof (continuationToken));
          if (!(continuationToken is CosmosObject cosmosObject))
          {
            aggregateContinuationToken = (AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken) null;
            return false;
          }
          CosmosElement singleGroupAggregatorContinuationToken;
          if (!cosmosObject.TryGetValue("AggregationToken", out singleGroupAggregatorContinuationToken))
          {
            aggregateContinuationToken = (AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken) null;
            return false;
          }
          CosmosElement sourceContinuationToken;
          if (!cosmosObject.TryGetValue("SourceToken", out sourceContinuationToken))
          {
            aggregateContinuationToken = (AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken) null;
            return false;
          }
          aggregateContinuationToken = new AggregateQueryPipelineStage.ComputeAggregateQueryPipelineStage.AggregateContinuationToken(singleGroupAggregatorContinuationToken, sourceContinuationToken);
          return true;
        }
      }
    }

    private readonly struct RewrittenAggregateProjections
    {
      public RewrittenAggregateProjections(bool isValueAggregateQuery, CosmosElement raw)
      {
        if (raw == (CosmosElement) null)
          throw new ArgumentNullException(nameof (raw));
        if (isValueAggregateQuery)
        {
          this.Payload = raw is CosmosArray cosmosArray ? cosmosArray[0] : throw new ArgumentException(string.Format("{0} was not an array for a value aggregate query. Type is: {1}", (object) nameof (RewrittenAggregateProjections), (object) raw.GetType()));
        }
        else
        {
          if (!(raw is CosmosObject cosmosObject))
            throw new ArgumentException("raw must not be an object.");
          CosmosElement cosmosElement;
          if (!cosmosObject.TryGetValue("payload", out cosmosElement))
            throw new InvalidOperationException("Underlying object does not have an 'payload' field.");
          this.Payload = cosmosElement ?? throw new ArgumentException("RewrittenAggregateProjections does not have a 'payload' property.");
        }
      }

      public CosmosElement Payload { get; }
    }
  }
}

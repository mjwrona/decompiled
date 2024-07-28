// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct.DistinctQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct
{
  internal abstract class DistinctQueryPipelineStage : QueryPipelineStageBase
  {
    private readonly DistinctMap distinctMap;

    protected DistinctQueryPipelineStage(
      DistinctMap distinctMap,
      IQueryPipelineStage source,
      CancellationToken cancellationToken)
      : base(source, cancellationToken)
    {
      this.distinctMap = distinctMap ?? throw new ArgumentNullException(nameof (distinctMap));
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      ExecutionEnvironment executionEnvironment,
      CosmosElement requestContinuation,
      CancellationToken cancellationToken,
      MonadicCreatePipelineStage monadicCreatePipelineStage,
      DistinctQueryType distinctQueryType)
    {
      if (executionEnvironment == ExecutionEnvironment.Client)
        return DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.MonadicCreate(requestContinuation, cancellationToken, monadicCreatePipelineStage, distinctQueryType);
      if (executionEnvironment == ExecutionEnvironment.Compute)
        return DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.MonadicCreate(requestContinuation, cancellationToken, monadicCreatePipelineStage, distinctQueryType);
      throw new ArgumentException(string.Format("Unknown {0}: {1}.", (object) "ExecutionEnvironment", (object) executionEnvironment));
    }

    private sealed class ClientDistinctQueryPipelineStage : DistinctQueryPipelineStage
    {
      private static readonly string DisallowContinuationTokenMessage = "DISTINCT queries only return continuation tokens when there is a matching ORDER BY clause.For example if your query is 'SELECT DISTINCT VALUE c.name FROM c', then rewrite it as 'SELECT DISTINCT VALUE c.name FROM c ORDER BY c.name'.";
      private readonly DistinctQueryType distinctQueryType;

      private ClientDistinctQueryPipelineStage(
        DistinctQueryType distinctQueryType,
        DistinctMap distinctMap,
        IQueryPipelineStage source,
        CancellationToken cancellationToken)
        : base(distinctMap, source, cancellationToken)
      {
        this.distinctQueryType = distinctQueryType == DistinctQueryType.Unordered || distinctQueryType == DistinctQueryType.Ordered ? distinctQueryType : throw new ArgumentException(string.Format("Unknown {0}: {1}.", (object) "DistinctQueryType", (object) distinctQueryType));
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        CosmosElement requestContinuation,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage,
        DistinctQueryType distinctQueryType)
      {
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken distinctContinuationToken;
        if (requestContinuation != (CosmosElement) null)
        {
          if (!DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken.TryParse(requestContinuation, out distinctContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid {0}: {1}", (object) "DistinctContinuationToken", (object) requestContinuation)));
        }
        else
          distinctContinuationToken = new DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken((string) null, (string) null);
        CosmosElement distinctMapContinuationToken = distinctContinuationToken.DistinctMapToken != null ? (CosmosElement) CosmosString.Create(distinctContinuationToken.DistinctMapToken) : (CosmosElement) null;
        TryCatch<DistinctMap> tryCatch1 = DistinctMap.TryCreate(distinctQueryType, distinctMapContinuationToken);
        if (!tryCatch1.Succeeded)
          return TryCatch<IQueryPipelineStage>.FromException(tryCatch1.Exception);
        CosmosElement continuationToken;
        if (distinctContinuationToken.SourceToken != null)
        {
          TryCatch<CosmosElement> tryCatch2 = CosmosElement.Monadic.Parse(distinctContinuationToken.SourceToken);
          if (tryCatch2.Failed)
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException("Invalid Source Token: " + distinctContinuationToken.SourceToken, tryCatch2.Exception));
          continuationToken = tryCatch2.Result;
        }
        else
          continuationToken = (CosmosElement) null;
        TryCatch<IQueryPipelineStage> tryCatch3 = monadicCreatePipelineStage(continuationToken, cancellationToken);
        return !tryCatch3.Succeeded ? TryCatch<IQueryPipelineStage>.FromException(tryCatch3.Exception) : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage(distinctQueryType, tryCatch1.Result, tryCatch3.Result, cancellationToken));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage queryPipelineStage = this;
        queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        if (!await queryPipelineStage.inputStage.MoveNextAsync(trace))
        {
          queryPipelineStage.Current = new TryCatch<QueryPage>();
          return false;
        }
        TryCatch<QueryPage> current = queryPipelineStage.inputStage.Current;
        if (current.Failed)
        {
          queryPipelineStage.Current = current;
          return true;
        }
        QueryPage result1 = current.Result;
        List<CosmosElement> documents = new List<CosmosElement>();
        foreach (CosmosElement document in (IEnumerable<CosmosElement>) result1.Documents)
        {
          queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
          if (queryPipelineStage.distinctMap.Add(document, out UInt128 _))
            documents.Add(document);
        }
        QueryPage result2;
        if (queryPipelineStage.distinctQueryType == DistinctQueryType.Ordered)
        {
          QueryState state = result1.State == null ? (QueryState) null : new QueryState(CosmosElement.Parse(new DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken(result1.State.Value.ToString(), queryPipelineStage.distinctMap.GetContinuationToken()).ToString()));
          result2 = new QueryPage((IReadOnlyList<CosmosElement>) documents, result1.RequestCharge, result1.ActivityId, result1.ResponseLengthInBytes, result1.CosmosQueryExecutionInfo, result1.DisallowContinuationTokenMessage, result1.AdditionalHeaders, state);
        }
        else
          result2 = new QueryPage((IReadOnlyList<CosmosElement>) documents, result1.RequestCharge, result1.ActivityId, result1.ResponseLengthInBytes, result1.CosmosQueryExecutionInfo, DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DisallowContinuationTokenMessage, result1.AdditionalHeaders, (QueryState) null);
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result2);
        return true;
      }

      private sealed class DistinctContinuationToken
      {
        public DistinctContinuationToken(string sourceToken, string distinctMapToken)
        {
          this.SourceToken = sourceToken;
          this.DistinctMapToken = distinctMapToken;
        }

        public string SourceToken { get; }

        public string DistinctMapToken { get; }

        public static bool TryParse(
          CosmosElement cosmosElement,
          out DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken distinctContinuationToken)
        {
          if (!(cosmosElement is CosmosObject cosmosObject))
          {
            distinctContinuationToken = (DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken) null;
            return false;
          }
          CosmosString typedCosmosElement1;
          if (!cosmosObject.TryGetValue<CosmosString>("SourceToken", out typedCosmosElement1))
          {
            distinctContinuationToken = (DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken) null;
            return false;
          }
          CosmosString typedCosmosElement2;
          if (!cosmosObject.TryGetValue<CosmosString>("DistinctMapToken", out typedCosmosElement2))
          {
            distinctContinuationToken = (DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken) null;
            return false;
          }
          distinctContinuationToken = new DistinctQueryPipelineStage.ClientDistinctQueryPipelineStage.DistinctContinuationToken(UtfAnyString.op_Implicit(typedCosmosElement1.Value), UtfAnyString.op_Implicit(typedCosmosElement2.Value));
          return true;
        }

        public override string ToString() => JsonConvert.SerializeObject((object) this);

        private static class PropertyNames
        {
          public const string SourceToken = "SourceToken";
          public const string DistinctMapToken = "DistinctMapToken";
        }
      }
    }

    private sealed class ComputeDistinctQueryPipelineStage : DistinctQueryPipelineStage
    {
      private static readonly string UseTryGetContinuationTokenMessage = "Use TryGetContinuationToken";

      private ComputeDistinctQueryPipelineStage(
        DistinctMap distinctMap,
        IQueryPipelineStage source,
        CancellationToken cancellationToken)
        : base(distinctMap, source, cancellationToken)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        CosmosElement requestContinuation,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage,
        DistinctQueryType distinctQueryType)
      {
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken distinctContinuationToken;
        if (requestContinuation != (CosmosElement) null)
        {
          if (!DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken.TryParse(requestContinuation, out distinctContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid {0}: {1}", (object) "DistinctContinuationToken", (object) requestContinuation)));
        }
        else
          distinctContinuationToken = new DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken((CosmosElement) null, (CosmosElement) null);
        TryCatch<DistinctMap> tryCatch1 = DistinctMap.TryCreate(distinctQueryType, distinctContinuationToken.DistinctMapToken);
        if (!tryCatch1.Succeeded)
          return TryCatch<IQueryPipelineStage>.FromException(tryCatch1.Exception);
        TryCatch<IQueryPipelineStage> tryCatch2 = monadicCreatePipelineStage(distinctContinuationToken.SourceToken, cancellationToken);
        return !tryCatch2.Succeeded ? TryCatch<IQueryPipelineStage>.FromException(tryCatch2.Exception) : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage(tryCatch1.Result, tryCatch2.Result, cancellationToken));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage queryPipelineStage = this;
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        if (!await queryPipelineStage.inputStage.MoveNextAsync(trace))
        {
          queryPipelineStage.Current = new TryCatch<QueryPage>();
          return false;
        }
        TryCatch<QueryPage> current = queryPipelineStage.inputStage.Current;
        if (current.Failed)
        {
          queryPipelineStage.Current = current;
          return true;
        }
        QueryPage result1 = current.Result;
        List<CosmosElement> documents = new List<CosmosElement>();
        foreach (CosmosElement document in (IEnumerable<CosmosElement>) result1.Documents)
        {
          if (queryPipelineStage.distinctMap.Add(document, out UInt128 _))
            documents.Add(document);
        }
        QueryState state = result1.State == null ? (QueryState) null : new QueryState(DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken.ToCosmosElement(new DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken(result1.State.Value, queryPipelineStage.distinctMap.GetCosmosElementContinuationToken())));
        QueryPage result2 = new QueryPage((IReadOnlyList<CosmosElement>) documents, result1.RequestCharge, result1.ActivityId, result1.ResponseLengthInBytes, result1.CosmosQueryExecutionInfo, DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.UseTryGetContinuationTokenMessage, result1.AdditionalHeaders, state);
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result2);
        return true;
      }

      private readonly struct DistinctContinuationToken
      {
        private const string SourceTokenName = "SourceToken";
        private const string DistinctMapTokenName = "DistinctMapToken";

        public DistinctContinuationToken(CosmosElement sourceToken, CosmosElement distinctMapToken)
        {
          this.SourceToken = sourceToken;
          this.DistinctMapToken = distinctMapToken;
        }

        public CosmosElement SourceToken { get; }

        public CosmosElement DistinctMapToken { get; }

        public static CosmosElement ToCosmosElement(
          DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken distinctContinuationToken)
        {
          return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
          {
            {
              "SourceToken",
              distinctContinuationToken.SourceToken
            },
            {
              "DistinctMapToken",
              distinctContinuationToken.DistinctMapToken
            }
          });
        }

        public static bool TryParse(
          CosmosElement requestContinuationToken,
          out DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken distinctContinuationToken)
        {
          if (requestContinuationToken == (CosmosElement) null)
          {
            distinctContinuationToken = new DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken();
            return false;
          }
          if (!(requestContinuationToken is CosmosObject cosmosObject))
          {
            distinctContinuationToken = new DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken();
            return false;
          }
          CosmosElement sourceToken;
          if (!cosmosObject.TryGetValue("SourceToken", out sourceToken))
          {
            distinctContinuationToken = new DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken();
            return false;
          }
          CosmosElement distinctMapToken;
          if (!cosmosObject.TryGetValue("DistinctMapToken", out distinctMapToken))
          {
            distinctContinuationToken = new DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken();
            return false;
          }
          distinctContinuationToken = new DistinctQueryPipelineStage.ComputeDistinctQueryPipelineStage.DistinctContinuationToken(sourceToken, distinctMapToken);
          return true;
        }
      }
    }
  }
}

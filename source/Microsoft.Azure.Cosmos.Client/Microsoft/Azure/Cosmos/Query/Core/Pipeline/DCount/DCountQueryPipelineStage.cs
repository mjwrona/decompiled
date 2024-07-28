// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.DCount.DCountQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.DCount
{
  internal abstract class DCountQueryPipelineStage : QueryPipelineStageBase
  {
    private static readonly IReadOnlyList<CosmosElement> EmptyResults = (IReadOnlyList<CosmosElement>) new List<CosmosElement>().AsReadOnly();
    private readonly DCountInfo info;
    private long count;
    protected bool returnedFinalPage;

    public DCountQueryPipelineStage(
      IQueryPipelineStage source,
      long count,
      DCountInfo info,
      CancellationToken cancellationToken)
      : base(source, cancellationToken)
    {
      this.count = count;
      this.info = info;
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      ExecutionEnvironment executionEnvironment,
      DCountInfo info,
      CosmosElement continuationToken,
      CancellationToken cancellationToken,
      MonadicCreatePipelineStage monadicCreatePipelineStage)
    {
      if (executionEnvironment == ExecutionEnvironment.Client)
        return DCountQueryPipelineStage.ClientDCountQueryPipelineStage.MonadicCreate(info, continuationToken, cancellationToken, monadicCreatePipelineStage);
      if (executionEnvironment == ExecutionEnvironment.Compute)
        return DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.MonadicCreate(info, continuationToken, cancellationToken, monadicCreatePipelineStage);
      throw new ArgumentException(string.Format("Unknown {0}: {1}.", (object) "ExecutionEnvironment", (object) executionEnvironment));
    }

    protected CosmosElement GetFinalResult()
    {
      if (this.info.IsValueAggregate)
        return (CosmosElement) CosmosNumber64.Create((Number64) this.count);
      return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          this.info.DCountAlias,
          (CosmosElement) CosmosNumber64.Create((Number64) this.count)
        }
      });
    }

    private sealed class ClientDCountQueryPipelineStage : DCountQueryPipelineStage
    {
      private ClientDCountQueryPipelineStage(
        IQueryPipelineStage source,
        long count,
        DCountInfo info,
        CancellationToken cancellationToken)
        : base(source, count, info, cancellationToken)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        DCountInfo info,
        CosmosElement continuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        TryCatch<IQueryPipelineStage> tryCatch = monadicCreatePipelineStage(continuationToken, cancellationToken);
        return tryCatch.Failed ? tryCatch : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new DCountQueryPipelineStage.ClientDCountQueryPipelineStage(tryCatch.Result, 0L, info, cancellationToken));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        DCountQueryPipelineStage.ClientDCountQueryPipelineStage queryPipelineStage = this;
        queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        if (queryPipelineStage.returnedFinalPage)
          return false;
        double requestCharge = 0.0;
        long responseLengthBytes = 0;
        IReadOnlyDictionary<string, string> additionalHeaders = (IReadOnlyDictionary<string, string>) null;
        TryCatch<QueryPage> current;
        while (true)
        {
          if (await queryPipelineStage.inputStage.MoveNextAsync(trace))
          {
            current = queryPipelineStage.inputStage.Current;
            if (!current.Failed)
            {
              QueryPage result = current.Result;
              requestCharge += result.RequestCharge;
              responseLengthBytes += result.ResponseLengthInBytes;
              additionalHeaders = result.AdditionalHeaders;
              queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
              queryPipelineStage.count += (long) result.Documents.Count;
            }
            else
              break;
          }
          else
            goto label_10;
        }
        queryPipelineStage.Current = current;
        return true;
label_10:
        List<CosmosElement> documents = new List<CosmosElement>();
        CosmosElement finalResult = queryPipelineStage.GetFinalResult();
        if (finalResult != (CosmosElement) null)
          documents.Add(finalResult);
        QueryPage result1 = new QueryPage((IReadOnlyList<CosmosElement>) documents, requestCharge, (string) null, responseLengthBytes, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, additionalHeaders, (QueryState) null);
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result1);
        queryPipelineStage.returnedFinalPage = true;
        return true;
      }
    }

    private sealed class ComputeDCountQueryPipelineStage : DCountQueryPipelineStage
    {
      private static readonly CosmosString DoneSourceToken = CosmosString.Create("DONE");

      private ComputeDCountQueryPipelineStage(
        IQueryPipelineStage source,
        long count,
        DCountInfo info,
        CancellationToken cancellationToken)
        : base(source, count, info, cancellationToken)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        DCountInfo info,
        CosmosElement continuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        cancellationToken.ThrowIfCancellationRequested();
        DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken dContinuationToken;
        if (continuationToken != (CosmosElement) null)
        {
          if (!DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken.TryCreateFromCosmosElement(continuationToken, out dContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malfomed {0}: '{1}'", (object) "DCountContinuationToken", (object) continuationToken)));
        }
        else
          dContinuationToken = new DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken(0L, (CosmosElement) null);
        TryCatch<IQueryPipelineStage> tryCatch = !(dContinuationToken.SourceContinuationToken is CosmosString continuationToken1) || !UtfAnyString.op_Equality(continuationToken1.Value, DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DoneSourceToken.Value) ? monadicCreatePipelineStage(dContinuationToken.SourceContinuationToken, cancellationToken) : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) EmptyQueryPipelineStage.Singleton);
        return tryCatch.Failed ? tryCatch : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new DCountQueryPipelineStage.ComputeDCountQueryPipelineStage(tryCatch.Result, dContinuationToken.Count, info, cancellationToken));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        DCountQueryPipelineStage.ComputeDCountQueryPipelineStage queryPipelineStage = this;
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
          queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
          queryPipelineStage.count += (long) result2.Documents.Count;
          QueryState state = new QueryState(DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken.ToCosmosElement(new DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken(queryPipelineStage.count, result2.State != null ? result2.State.Value : (CosmosElement) DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DoneSourceToken)));
          result1 = new QueryPage(DCountQueryPipelineStage.EmptyResults, result2.RequestCharge, result2.ActivityId, result2.ResponseLengthInBytes, result2.CosmosQueryExecutionInfo, result2.DisallowContinuationTokenMessage, result2.AdditionalHeaders, state);
        }
        else
        {
          List<CosmosElement> documents = new List<CosmosElement>();
          CosmosElement finalResult = queryPipelineStage.GetFinalResult();
          if (finalResult != (CosmosElement) null)
            documents.Add(finalResult);
          result1 = new QueryPage((IReadOnlyList<CosmosElement>) documents, 0.0, (string) null, 0L, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, (IReadOnlyDictionary<string, string>) null, (QueryState) null);
          queryPipelineStage.returnedFinalPage = true;
        }
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result1);
        return true;
      }

      private readonly struct DCountContinuationToken
      {
        private const string SourceTokenName = "SourceToken";
        private const string DCountTokenName = "DCountToken";

        public DCountContinuationToken(long count, CosmosElement sourceContinuationToken)
        {
          this.Count = count;
          this.SourceContinuationToken = sourceContinuationToken;
        }

        public long Count { get; }

        public CosmosElement CountToken => (CosmosElement) CosmosNumber64.Create((Number64) this.Count);

        public CosmosElement SourceContinuationToken { get; }

        public static CosmosElement ToCosmosElement(
          DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken dcountContinuationToken)
        {
          return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
          {
            {
              "SourceToken",
              dcountContinuationToken.SourceContinuationToken
            },
            {
              "DCountToken",
              dcountContinuationToken.CountToken
            }
          });
        }

        public static bool TryCreateFromCosmosElement(
          CosmosElement continuationToken,
          out DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken dContinuationToken)
        {
          if (continuationToken == (CosmosElement) null)
            throw new ArgumentNullException(nameof (continuationToken));
          if (!(continuationToken is CosmosObject cosmosObject))
          {
            dContinuationToken = new DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken();
            return false;
          }
          CosmosElement cosmosElement;
          if (!cosmosObject.TryGetValue("DCountToken", out cosmosElement))
          {
            dContinuationToken = new DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken();
            return false;
          }
          if (!(cosmosElement is CosmosNumber cosmosNumber))
          {
            dContinuationToken = new DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken();
            return false;
          }
          CosmosElement sourceContinuationToken;
          if (!cosmosObject.TryGetValue("SourceToken", out sourceContinuationToken))
          {
            dContinuationToken = new DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken();
            return false;
          }
          dContinuationToken = new DCountQueryPipelineStage.ComputeDCountQueryPipelineStage.DCountContinuationToken(Number64.ToLong(cosmosNumber.Value), sourceContinuationToken);
          return true;
        }
      }
    }
  }
}

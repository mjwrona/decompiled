// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Skip.SkipQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Skip
{
  internal abstract class SkipQueryPipelineStage : QueryPipelineStageBase
  {
    private int skipCount;

    protected SkipQueryPipelineStage(
      IQueryPipelineStage source,
      CancellationToken cancellationToken,
      long skipCount)
      : base(source, cancellationToken)
    {
      this.skipCount = skipCount <= (long) int.MaxValue ? (int) skipCount : throw new ArgumentOutOfRangeException(nameof (skipCount));
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      ExecutionEnvironment executionEnvironment,
      int offsetCount,
      CosmosElement continuationToken,
      CancellationToken cancellationToken,
      MonadicCreatePipelineStage monadicCreatePipelineStage)
    {
      if (executionEnvironment == ExecutionEnvironment.Client)
        return SkipQueryPipelineStage.ClientSkipQueryPipelineStage.MonadicCreate(offsetCount, continuationToken, cancellationToken, monadicCreatePipelineStage);
      if (executionEnvironment == ExecutionEnvironment.Compute)
        return SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.MonadicCreate(offsetCount, continuationToken, cancellationToken, monadicCreatePipelineStage);
      throw new ArgumentException(string.Format("Unknown {0}: {1}", (object) "ExecutionEnvironment", (object) executionEnvironment));
    }

    private sealed class ClientSkipQueryPipelineStage : SkipQueryPipelineStage
    {
      private ClientSkipQueryPipelineStage(
        IQueryPipelineStage source,
        CancellationToken cancellationToken,
        long skipCount)
        : base(source, cancellationToken, skipCount)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        int offsetCount,
        CosmosElement continuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        SkipQueryPipelineStage.ClientSkipQueryPipelineStage.OffsetContinuationToken offsetContinuationToken;
        if (continuationToken != (CosmosElement) null)
        {
          if (!SkipQueryPipelineStage.ClientSkipQueryPipelineStage.OffsetContinuationToken.TryParse(continuationToken.ToString(), out offsetContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid {0}: {1}.", (object) nameof (SkipQueryPipelineStage), (object) continuationToken)));
        }
        else
          offsetContinuationToken = new SkipQueryPipelineStage.ClientSkipQueryPipelineStage.OffsetContinuationToken(offsetCount, (string) null);
        if (offsetContinuationToken.Offset > offsetCount)
          return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException("offset count in continuation token can not be greater than the offsetcount in the query."));
        CosmosElement continuationToken1;
        if (offsetContinuationToken.SourceToken != null)
        {
          TryCatch<CosmosElement> tryCatch = CosmosElement.Monadic.Parse(offsetContinuationToken.SourceToken);
          if (tryCatch.Failed)
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException("source token: '" + (offsetContinuationToken.SourceToken ?? "<null>") + "' is not valid.", tryCatch.Exception));
          continuationToken1 = tryCatch.Result;
        }
        else
          continuationToken1 = (CosmosElement) null;
        TryCatch<IQueryPipelineStage> tryCatch1 = monadicCreatePipelineStage(continuationToken1, cancellationToken);
        return tryCatch1.Failed ? tryCatch1 : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new SkipQueryPipelineStage.ClientSkipQueryPipelineStage(tryCatch1.Result, cancellationToken, (long) offsetContinuationToken.Offset));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        SkipQueryPipelineStage.ClientSkipQueryPipelineStage queryPipelineStage = this;
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
        IReadOnlyList<CosmosElement> list = (IReadOnlyList<CosmosElement>) result1.Documents.Skip<CosmosElement>(queryPipelineStage.skipCount).ToList<CosmosElement>();
        int num = result1.Documents.Count - list.Count;
        queryPipelineStage.skipCount -= num;
        QueryState state = result1.State == null || result1.DisallowContinuationTokenMessage != null ? (QueryState) null : new QueryState(CosmosElement.Parse(new SkipQueryPipelineStage.ClientSkipQueryPipelineStage.OffsetContinuationToken(queryPipelineStage.skipCount, result1.State?.Value.ToString()).ToString()));
        QueryPage result2 = new QueryPage(list, result1.RequestCharge, result1.ActivityId, result1.ResponseLengthInBytes, result1.CosmosQueryExecutionInfo, result1.DisallowContinuationTokenMessage, result1.AdditionalHeaders, state);
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result2);
        return true;
      }

      private readonly struct OffsetContinuationToken
      {
        public OffsetContinuationToken(int offset, string sourceToken)
        {
          this.Offset = offset >= 0 ? offset : throw new ArgumentException("offset must be a non negative number.");
          this.SourceToken = sourceToken;
        }

        [JsonProperty("offset")]
        public int Offset { get; }

        [JsonProperty("sourceToken")]
        public string SourceToken { get; }

        public static bool TryParse(
          string value,
          out SkipQueryPipelineStage.ClientSkipQueryPipelineStage.OffsetContinuationToken offsetContinuationToken)
        {
          offsetContinuationToken = new SkipQueryPipelineStage.ClientSkipQueryPipelineStage.OffsetContinuationToken();
          if (string.IsNullOrWhiteSpace(value))
            return false;
          try
          {
            offsetContinuationToken = JsonConvert.DeserializeObject<SkipQueryPipelineStage.ClientSkipQueryPipelineStage.OffsetContinuationToken>(value);
            return true;
          }
          catch (JsonException ex)
          {
            return false;
          }
        }

        public override string ToString() => JsonConvert.SerializeObject((object) this);
      }
    }

    private sealed class ComputeSkipQueryPipelineStage : SkipQueryPipelineStage
    {
      private ComputeSkipQueryPipelineStage(
        IQueryPipelineStage source,
        CancellationToken cancellationToken,
        long skipCount)
        : base(source, cancellationToken, skipCount)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreate(
        int offsetCount,
        CosmosElement continuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken continuationToken1;
        if (continuationToken != (CosmosElement) null)
        {
          (bool parsed, SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken offsetContinuationToken) = SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken.TryParse(continuationToken);
          if (!parsed)
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid {0}: {1}.", (object) nameof (SkipQueryPipelineStage), (object) continuationToken)));
          continuationToken1 = offsetContinuationToken;
        }
        else
          continuationToken1 = new SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken((long) offsetCount, (CosmosElement) null);
        if (continuationToken1.Offset > offsetCount)
          return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException("offset count in continuation token can not be greater than the offsetcount in the query."));
        TryCatch<IQueryPipelineStage> tryCatch = monadicCreatePipelineStage(continuationToken1.SourceToken, cancellationToken);
        return tryCatch.Failed ? tryCatch : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new SkipQueryPipelineStage.ComputeSkipQueryPipelineStage(tryCatch.Result, cancellationToken, (long) continuationToken1.Offset));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        SkipQueryPipelineStage.ComputeSkipQueryPipelineStage queryPipelineStage = this;
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
        IReadOnlyList<CosmosElement> list = (IReadOnlyList<CosmosElement>) result1.Documents.Skip<CosmosElement>(queryPipelineStage.skipCount).ToList<CosmosElement>();
        int num = result1.Documents.Count<CosmosElement>() - list.Count<CosmosElement>();
        queryPipelineStage.skipCount -= num;
        QueryState state = result1.State != null ? new QueryState(SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken.ToCosmosElement(new SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken((long) queryPipelineStage.skipCount, result1.State.Value))) : (QueryState) null;
        QueryPage result2 = new QueryPage(list, result1.RequestCharge, result1.ActivityId, result1.ResponseLengthInBytes, result1.CosmosQueryExecutionInfo, result1.DisallowContinuationTokenMessage, result1.AdditionalHeaders, state);
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result2);
        return true;
      }

      private readonly struct OffsetContinuationToken
      {
        public OffsetContinuationToken(long offset, CosmosElement sourceToken)
        {
          this.Offset = offset >= 0L && offset <= (long) int.MaxValue ? (int) offset : throw new ArgumentOutOfRangeException(nameof (offset));
          this.SourceToken = sourceToken;
        }

        public int Offset { get; }

        public CosmosElement SourceToken { get; }

        public static CosmosElement ToCosmosElement(
          SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken offsetContinuationToken)
        {
          return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
          {
            {
              "SkipCount",
              (CosmosElement) CosmosNumber64.Create((Number64) (long) offsetContinuationToken.Offset)
            },
            {
              "SourceToken",
              offsetContinuationToken.SourceToken
            }
          });
        }

        public static (bool parsed, SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken offsetContinuationToken) TryParse(
          CosmosElement value)
        {
          if (value == (CosmosElement) null)
            return (false, new SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken());
          if (!(value is CosmosObject cosmosObject))
            return (false, new SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken());
          CosmosNumber typedCosmosElement;
          if (!cosmosObject.TryGetValue<CosmosNumber>("SkipCount", out typedCosmosElement))
            return (false, new SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken());
          CosmosElement sourceToken;
          return !cosmosObject.TryGetValue("SourceToken", out sourceToken) ? (false, new SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken()) : (true, new SkipQueryPipelineStage.ComputeSkipQueryPipelineStage.OffsetContinuationToken(Number64.ToLong(typedCosmosElement.Value), sourceToken));
        }

        private static class ProperytNames
        {
          public const string SkipCountProperty = "SkipCount";
          public const string SourceTokenProperty = "SourceToken";
        }
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Take.TakeQueryPipelineStage
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

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Take
{
  internal abstract class TakeQueryPipelineStage : QueryPipelineStageBase
  {
    private int takeCount;

    private bool ReturnedFinalPage => this.takeCount <= 0;

    protected TakeQueryPipelineStage(
      IQueryPipelineStage source,
      CancellationToken cancellationToken,
      int takeCount)
      : base(source, cancellationToken)
    {
      this.takeCount = takeCount;
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreateLimitStage(
      ExecutionEnvironment executionEnvironment,
      int limitCount,
      CosmosElement requestContinuationToken,
      CancellationToken cancellationToken,
      MonadicCreatePipelineStage monadicCreatePipelineStage)
    {
      if (executionEnvironment == ExecutionEnvironment.Client)
        return TakeQueryPipelineStage.ClientTakeQueryPipelineStage.MonadicCreateLimitStage(limitCount, requestContinuationToken, cancellationToken, monadicCreatePipelineStage);
      if (executionEnvironment == ExecutionEnvironment.Compute)
        return TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.MonadicCreateLimitStage(limitCount, requestContinuationToken, cancellationToken, monadicCreatePipelineStage);
      throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "ExecutionEnvironment", (object) executionEnvironment));
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreateTopStage(
      ExecutionEnvironment executionEnvironment,
      int limitCount,
      CosmosElement requestContinuationToken,
      CancellationToken cancellationToken,
      MonadicCreatePipelineStage monadicCreatePipelineStage)
    {
      if (executionEnvironment == ExecutionEnvironment.Client)
        return TakeQueryPipelineStage.ClientTakeQueryPipelineStage.MonadicCreateTopStage(limitCount, requestContinuationToken, cancellationToken, monadicCreatePipelineStage);
      if (executionEnvironment == ExecutionEnvironment.Compute)
        return TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.MonadicCreateTopStage(limitCount, requestContinuationToken, cancellationToken, monadicCreatePipelineStage);
      throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "ExecutionEnvironment", (object) executionEnvironment));
    }

    private sealed class ClientTakeQueryPipelineStage : TakeQueryPipelineStage
    {
      private readonly TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TakeEnum takeEnum;

      private ClientTakeQueryPipelineStage(
        IQueryPipelineStage source,
        CancellationToken cancellationToken,
        int takeCount,
        TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TakeEnum takeEnum)
        : base(source, cancellationToken, takeCount)
      {
        this.takeEnum = takeEnum;
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreateLimitStage(
        int limitCount,
        CosmosElement requestContinuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        if (limitCount < 0)
          throw new ArgumentException(string.Format("{0}: {1} must be a non negative number.", (object) nameof (limitCount), (object) limitCount));
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        TakeQueryPipelineStage.ClientTakeQueryPipelineStage.LimitContinuationToken limitContinuationToken;
        if (requestContinuationToken != (CosmosElement) null)
        {
          if (!TakeQueryPipelineStage.ClientTakeQueryPipelineStage.LimitContinuationToken.TryParse(requestContinuationToken.ToString(), out limitContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0}: {1}.", (object) "LimitContinuationToken", (object) requestContinuationToken)));
        }
        else
          limitContinuationToken = new TakeQueryPipelineStage.ClientTakeQueryPipelineStage.LimitContinuationToken(limitCount, (string) null);
        if (limitContinuationToken.Limit > limitCount)
          return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} in {1}: {2}: {3} can not be greater than the limit count in the query: {4}.", (object) "Limit", (object) "LimitContinuationToken", (object) requestContinuationToken, (object) limitContinuationToken.Limit, (object) limitCount)));
        CosmosElement continuationToken;
        if (limitContinuationToken.SourceToken != null)
        {
          TryCatch<CosmosElement> tryCatch = CosmosElement.Monadic.Parse(limitContinuationToken.SourceToken);
          if (tryCatch.Failed)
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0}: {1}.", (object) "LimitContinuationToken", (object) requestContinuationToken), tryCatch.Exception));
          continuationToken = tryCatch.Result;
        }
        else
          continuationToken = (CosmosElement) null;
        TryCatch<IQueryPipelineStage> tryCatch1 = monadicCreatePipelineStage(continuationToken, cancellationToken);
        return tryCatch1.Failed ? tryCatch1 : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new TakeQueryPipelineStage.ClientTakeQueryPipelineStage(tryCatch1.Result, cancellationToken, limitContinuationToken.Limit, TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TakeEnum.Limit));
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreateTopStage(
        int topCount,
        CosmosElement requestContinuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        if (topCount < 0)
          throw new ArgumentException(string.Format("{0}: {1} must be a non negative number.", (object) nameof (topCount), (object) topCount));
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TopContinuationToken topContinuationToken;
        if (requestContinuationToken != (CosmosElement) null)
        {
          if (!TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TopContinuationToken.TryParse(requestContinuationToken.ToString(), out topContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0}: {1}.", (object) "LimitContinuationToken", (object) requestContinuationToken)));
        }
        else
          topContinuationToken = new TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TopContinuationToken(topCount, (string) null);
        if (topContinuationToken.Top > topCount)
          return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} in {1}: {2}: {3} can not be greater than the top count in the query: {4}.", (object) "Top", (object) "TopContinuationToken", (object) requestContinuationToken, (object) topContinuationToken.Top, (object) topCount)));
        CosmosElement continuationToken;
        if (topContinuationToken.SourceToken != null)
        {
          TryCatch<CosmosElement> tryCatch = CosmosElement.Monadic.Parse(topContinuationToken.SourceToken);
          if (tryCatch.Failed)
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} in {1}: {2}: {3} was malformed.", (object) "SourceToken", (object) "TopContinuationToken", (object) requestContinuationToken, (object) (topContinuationToken.SourceToken ?? "<null>")), tryCatch.Exception));
          continuationToken = tryCatch.Result;
        }
        else
          continuationToken = (CosmosElement) null;
        TryCatch<IQueryPipelineStage> tryCatch1 = monadicCreatePipelineStage(continuationToken, cancellationToken);
        return tryCatch1.Failed ? tryCatch1 : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new TakeQueryPipelineStage.ClientTakeQueryPipelineStage(tryCatch1.Result, cancellationToken, topContinuationToken.Top, TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TakeEnum.Top));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        TakeQueryPipelineStage.ClientTakeQueryPipelineStage queryPipelineStage = this;
        queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        bool flag = queryPipelineStage.ReturnedFinalPage;
        if (!flag)
          flag = !await queryPipelineStage.inputStage.MoveNextAsync(trace);
        if (flag)
        {
          queryPipelineStage.Current = new TryCatch<QueryPage>();
          queryPipelineStage.takeCount = 0;
          return false;
        }
        TryCatch<QueryPage> current = queryPipelineStage.inputStage.Current;
        if (current.Failed)
        {
          queryPipelineStage.Current = current;
          return true;
        }
        QueryPage result1 = current.Result;
        List<CosmosElement> list = result1.Documents.Take<CosmosElement>(queryPipelineStage.takeCount).ToList<CosmosElement>();
        queryPipelineStage.takeCount -= list.Count;
        QueryState state;
        if (result1.State != null && result1.DisallowContinuationTokenMessage == null)
        {
          string json;
          switch (queryPipelineStage.takeEnum)
          {
            case TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TakeEnum.Limit:
              json = new TakeQueryPipelineStage.ClientTakeQueryPipelineStage.LimitContinuationToken(queryPipelineStage.takeCount, result1.State?.Value.ToString()).ToString();
              break;
            case TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TakeEnum.Top:
              json = new TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TopContinuationToken(queryPipelineStage.takeCount, result1.State?.Value.ToString()).ToString();
              break;
            default:
              throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "TakeEnum", (object) queryPipelineStage.takeEnum));
          }
          state = new QueryState(CosmosElement.Parse(json));
        }
        else
          state = (QueryState) null;
        QueryPage result2 = new QueryPage((IReadOnlyList<CosmosElement>) list, result1.RequestCharge, result1.ActivityId, result1.ResponseLengthInBytes, result1.CosmosQueryExecutionInfo, result1.DisallowContinuationTokenMessage, result1.AdditionalHeaders, state);
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result2);
        return true;
      }

      private enum TakeEnum
      {
        Limit,
        Top,
      }

      private abstract class TakeContinuationToken
      {
      }

      private sealed class LimitContinuationToken : 
        TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TakeContinuationToken
      {
        public LimitContinuationToken(int limit, string sourceToken)
        {
          this.Limit = limit >= 0 ? limit : throw new ArgumentException("limit must be a non negative number.");
          this.SourceToken = sourceToken;
        }

        [JsonProperty("limit")]
        public int Limit { get; }

        [JsonProperty("sourceToken")]
        public string SourceToken { get; }

        public static bool TryParse(
          string value,
          out TakeQueryPipelineStage.ClientTakeQueryPipelineStage.LimitContinuationToken limitContinuationToken)
        {
          limitContinuationToken = (TakeQueryPipelineStage.ClientTakeQueryPipelineStage.LimitContinuationToken) null;
          if (string.IsNullOrWhiteSpace(value))
            return false;
          try
          {
            limitContinuationToken = JsonConvert.DeserializeObject<TakeQueryPipelineStage.ClientTakeQueryPipelineStage.LimitContinuationToken>(value);
            return true;
          }
          catch (JsonException ex)
          {
            return false;
          }
        }

        public override string ToString() => JsonConvert.SerializeObject((object) this);
      }

      private sealed class TopContinuationToken : 
        TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TakeContinuationToken
      {
        public TopContinuationToken(int top, string sourceToken)
        {
          this.Top = top;
          this.SourceToken = sourceToken;
        }

        [JsonProperty("top")]
        public int Top { get; }

        [JsonProperty("sourceToken")]
        public string SourceToken { get; }

        public static bool TryParse(
          string value,
          out TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TopContinuationToken topContinuationToken)
        {
          topContinuationToken = (TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TopContinuationToken) null;
          if (string.IsNullOrWhiteSpace(value))
            return false;
          try
          {
            topContinuationToken = JsonConvert.DeserializeObject<TakeQueryPipelineStage.ClientTakeQueryPipelineStage.TopContinuationToken>(value);
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

    private sealed class ComputeTakeQueryPipelineStage : TakeQueryPipelineStage
    {
      private ComputeTakeQueryPipelineStage(
        IQueryPipelineStage source,
        CancellationToken cancellationToken,
        int takeCount)
        : base(source, cancellationToken, takeCount)
      {
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreateLimitStage(
        int takeCount,
        CosmosElement requestContinuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        return TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.MonadicCreate(takeCount, requestContinuationToken, cancellationToken, monadicCreatePipelineStage);
      }

      public static TryCatch<IQueryPipelineStage> MonadicCreateTopStage(
        int takeCount,
        CosmosElement requestContinuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        return TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.MonadicCreate(takeCount, requestContinuationToken, cancellationToken, monadicCreatePipelineStage);
      }

      private static TryCatch<IQueryPipelineStage> MonadicCreate(
        int takeCount,
        CosmosElement requestContinuationToken,
        CancellationToken cancellationToken,
        MonadicCreatePipelineStage monadicCreatePipelineStage)
      {
        if (takeCount < 0)
          throw new ArgumentException(string.Format("{0}: {1} must be a non negative number.", (object) nameof (takeCount), (object) takeCount));
        if (monadicCreatePipelineStage == null)
          throw new ArgumentNullException(nameof (monadicCreatePipelineStage));
        TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken takeContinuationToken;
        if (requestContinuationToken != (CosmosElement) null)
        {
          if (!TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken.TryParse(requestContinuationToken, out takeContinuationToken))
            return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0}: {1}.", (object) "TakeContinuationToken", (object) requestContinuationToken)));
        }
        else
          takeContinuationToken = new TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken((long) takeCount, (CosmosElement) null);
        if (takeContinuationToken.TakeCount > takeCount)
          return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} in {1}: {2}: {3} can not be greater than the limit count in the query: {4}.", (object) "TakeCount", (object) "TakeContinuationToken", (object) requestContinuationToken, (object) takeContinuationToken.TakeCount, (object) takeCount)));
        TryCatch<IQueryPipelineStage> tryCatch = monadicCreatePipelineStage(takeContinuationToken.SourceToken, cancellationToken);
        return tryCatch.Failed ? tryCatch : TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new TakeQueryPipelineStage.ComputeTakeQueryPipelineStage(tryCatch.Result, cancellationToken, takeContinuationToken.TakeCount));
      }

      public override async ValueTask<bool> MoveNextAsync(ITrace trace)
      {
        TakeQueryPipelineStage.ComputeTakeQueryPipelineStage queryPipelineStage = this;
        queryPipelineStage.cancellationToken.ThrowIfCancellationRequested();
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        bool flag = queryPipelineStage.ReturnedFinalPage;
        if (!flag)
          flag = !await queryPipelineStage.inputStage.MoveNextAsync(trace);
        if (flag)
        {
          queryPipelineStage.Current = new TryCatch<QueryPage>();
          queryPipelineStage.takeCount = 0;
          return false;
        }
        TryCatch<QueryPage> current = queryPipelineStage.inputStage.Current;
        if (current.Failed)
        {
          queryPipelineStage.Current = current;
          return true;
        }
        QueryPage result1 = current.Result;
        List<CosmosElement> list = result1.Documents.Take<CosmosElement>(queryPipelineStage.takeCount).ToList<CosmosElement>();
        queryPipelineStage.takeCount -= list.Count;
        QueryState state = result1.State == null ? (QueryState) null : new QueryState(TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken.ToCosmosElement(new TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken((long) queryPipelineStage.takeCount, result1.State.Value)));
        QueryPage result2 = new QueryPage((IReadOnlyList<CosmosElement>) list, result1.RequestCharge, result1.ActivityId, result1.ResponseLengthInBytes, result1.CosmosQueryExecutionInfo, result1.DisallowContinuationTokenMessage, result1.AdditionalHeaders, state);
        queryPipelineStage.Current = TryCatch<QueryPage>.FromResult(result2);
        return true;
      }

      private readonly struct TakeContinuationToken
      {
        public TakeContinuationToken(long takeCount, CosmosElement sourceToken)
        {
          this.TakeCount = takeCount >= 0L && takeCount <= (long) int.MaxValue ? (int) takeCount : throw new ArgumentException("takeCount must be a non negative number.");
          this.SourceToken = sourceToken;
        }

        public int TakeCount { get; }

        public CosmosElement SourceToken { get; }

        public static CosmosElement ToCosmosElement(
          TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken takeContinuationToken)
        {
          return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
          {
            {
              "SourceToken",
              takeContinuationToken.SourceToken
            },
            {
              "TakeCount",
              (CosmosElement) CosmosNumber64.Create((Number64) (long) takeContinuationToken.TakeCount)
            }
          });
        }

        public static bool TryParse(
          CosmosElement value,
          out TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken takeContinuationToken)
        {
          if (value == (CosmosElement) null)
            throw new ArgumentNullException(nameof (value));
          if (!(value is CosmosObject cosmosObject))
          {
            takeContinuationToken = new TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken();
            return false;
          }
          CosmosNumber typedCosmosElement;
          if (!cosmosObject.TryGetValue<CosmosNumber>("TakeCount", out typedCosmosElement))
          {
            takeContinuationToken = new TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken();
            return false;
          }
          CosmosElement sourceToken;
          if (!cosmosObject.TryGetValue("SourceToken", out sourceToken))
          {
            takeContinuationToken = new TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken();
            return false;
          }
          takeContinuationToken = new TakeQueryPipelineStage.ComputeTakeQueryPipelineStage.TakeContinuationToken(Number64.ToLong(typedCosmosElement.Value), sourceToken);
          return true;
        }

        public static class PropertyNames
        {
          public const string SourceToken = "SourceToken";
          public const string TakeCount = "TakeCount";
        }
      }
    }
  }
}

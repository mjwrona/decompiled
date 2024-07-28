// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ExecutionComponent.TakeDocumentQueryExecutionComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query.ExecutionComponent
{
  internal sealed class TakeDocumentQueryExecutionComponent : DocumentQueryExecutionComponentBase
  {
    private readonly TakeDocumentQueryExecutionComponent.TakeEnum takeEnum;
    private int takeCount;

    private TakeDocumentQueryExecutionComponent(
      IDocumentQueryExecutionComponent source,
      int takeCount,
      TakeDocumentQueryExecutionComponent.TakeEnum takeEnum)
      : base(source)
    {
      this.takeCount = takeCount >= 0 ? takeCount : throw new ArgumentException("takeCount must be a non negative number.");
      this.takeEnum = takeEnum;
    }

    public static async Task<TakeDocumentQueryExecutionComponent> CreateLimitDocumentQueryExecutionComponentAsync(
      int limitCount,
      string continuationToken,
      Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback)
    {
      TakeDocumentQueryExecutionComponent.LimitContinuationToken limitContinuationToken = continuationToken == null ? new TakeDocumentQueryExecutionComponent.LimitContinuationToken(limitCount, (string) null) : TakeDocumentQueryExecutionComponent.LimitContinuationToken.Parse(continuationToken);
      if (limitContinuationToken.Limit > limitCount)
        throw new BadRequestException(string.Format("limit count in continuation token: {0} can not be greater than the limit count in the query: {1}.", (object) limitContinuationToken.Limit, (object) limitCount));
      return new TakeDocumentQueryExecutionComponent(await createSourceCallback(limitContinuationToken.SourceToken), limitContinuationToken.Limit, TakeDocumentQueryExecutionComponent.TakeEnum.Limit);
    }

    public static async Task<TakeDocumentQueryExecutionComponent> CreateTopDocumentQueryExecutionComponentAsync(
      int topCount,
      string continuationToken,
      Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback)
    {
      TakeDocumentQueryExecutionComponent.TopContinuationToken topContinuationToken = continuationToken == null ? new TakeDocumentQueryExecutionComponent.TopContinuationToken(topCount, (string) null) : TakeDocumentQueryExecutionComponent.TopContinuationToken.Parse(continuationToken);
      if (topContinuationToken.Top > topCount)
        throw new BadRequestException(string.Format("top count in continuation token: {0} can not be greater than the top count in the query: {1}.", (object) topContinuationToken.Top, (object) topCount));
      return new TakeDocumentQueryExecutionComponent(await createSourceCallback(topContinuationToken.SourceToken), topContinuationToken.Top, TakeDocumentQueryExecutionComponent.TakeEnum.Top);
    }

    public override bool IsDone => this.Source.IsDone || this.takeCount <= 0;

    public override async Task<FeedResponse<object>> DrainAsync(
      int maxElements,
      CancellationToken token)
    {
      TakeDocumentQueryExecutionComponent executionComponent = this;
      // ISSUE: reference to a compiler-generated method
      FeedResponse<object> source = await executionComponent.\u003C\u003En__0(maxElements, token);
      List<object> list = source.Take<object>(executionComponent.takeCount).ToList<object>();
      FeedResponse<object> feedResponse = new FeedResponse<object>((IEnumerable<object>) list, list.Count, source.Headers, source.UseETagAsContinuation, source.QueryMetrics, source.PartitionedClientSideRequestStatistics, source.DisallowContinuationTokenMessage, source.ResponseLengthBytes);
      executionComponent.takeCount -= list.Count;
      if (feedResponse.DisallowContinuationTokenMessage == null)
      {
        if (!executionComponent.IsDone)
        {
          string responseContinuation = feedResponse.ResponseContinuation;
          TakeDocumentQueryExecutionComponent.TakeContinuationToken continuationToken;
          switch (executionComponent.takeEnum)
          {
            case TakeDocumentQueryExecutionComponent.TakeEnum.Limit:
              continuationToken = (TakeDocumentQueryExecutionComponent.TakeContinuationToken) new TakeDocumentQueryExecutionComponent.LimitContinuationToken(executionComponent.takeCount, responseContinuation);
              break;
            case TakeDocumentQueryExecutionComponent.TakeEnum.Top:
              continuationToken = (TakeDocumentQueryExecutionComponent.TakeContinuationToken) new TakeDocumentQueryExecutionComponent.TopContinuationToken(executionComponent.takeCount, responseContinuation);
              break;
            default:
              throw new ArgumentException(string.Format("Unknown {0}: {1}", (object) "TakeEnum", (object) executionComponent.takeEnum));
          }
          feedResponse.ResponseContinuation = continuationToken.ToString();
        }
        else
          feedResponse.ResponseContinuation = (string) null;
      }
      return feedResponse;
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
      TakeDocumentQueryExecutionComponent.TakeContinuationToken
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

      public static TakeDocumentQueryExecutionComponent.LimitContinuationToken Parse(string value)
      {
        TakeDocumentQueryExecutionComponent.LimitContinuationToken LimitContinuationToken;
        if (!TakeDocumentQueryExecutionComponent.LimitContinuationToken.TryParse(value, out LimitContinuationToken))
          throw new BadRequestException("Invalid LimitContinuationToken: " + value);
        return LimitContinuationToken;
      }

      public static bool TryParse(
        string value,
        out TakeDocumentQueryExecutionComponent.LimitContinuationToken LimitContinuationToken)
      {
        LimitContinuationToken = (TakeDocumentQueryExecutionComponent.LimitContinuationToken) null;
        if (string.IsNullOrWhiteSpace(value))
          return false;
        try
        {
          LimitContinuationToken = JsonConvert.DeserializeObject<TakeDocumentQueryExecutionComponent.LimitContinuationToken>(value);
          return true;
        }
        catch (JsonException ex)
        {
          DefaultTrace.TraceWarning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} Invalid continuation token {1} for limit~Component, exception: {2}", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) value, (object) ex.Message));
          return false;
        }
      }

      public override string ToString() => JsonConvert.SerializeObject((object) this);
    }

    private sealed class TopContinuationToken : 
      TakeDocumentQueryExecutionComponent.TakeContinuationToken
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

      public static TakeDocumentQueryExecutionComponent.TopContinuationToken Parse(string value)
      {
        TakeDocumentQueryExecutionComponent.TopContinuationToken topContinuationToken;
        if (!TakeDocumentQueryExecutionComponent.TopContinuationToken.TryParse(value, out topContinuationToken))
          throw new BadRequestException("Invalid TopContinuationToken: " + value);
        return topContinuationToken;
      }

      public static bool TryParse(
        string value,
        out TakeDocumentQueryExecutionComponent.TopContinuationToken topContinuationToken)
      {
        topContinuationToken = (TakeDocumentQueryExecutionComponent.TopContinuationToken) null;
        if (string.IsNullOrWhiteSpace(value))
          return false;
        try
        {
          topContinuationToken = JsonConvert.DeserializeObject<TakeDocumentQueryExecutionComponent.TopContinuationToken>(value);
          return true;
        }
        catch (JsonException ex)
        {
          DefaultTrace.TraceWarning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} Invalid continuation token {1} for Top~Component, exception: {2}", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) value, (object) ex.Message));
          return false;
        }
      }

      public override string ToString() => JsonConvert.SerializeObject((object) this);
    }
  }
}

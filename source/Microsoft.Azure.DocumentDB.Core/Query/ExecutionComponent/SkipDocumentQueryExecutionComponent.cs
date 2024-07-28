// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ExecutionComponent.SkipDocumentQueryExecutionComponent
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
  internal sealed class SkipDocumentQueryExecutionComponent : DocumentQueryExecutionComponentBase
  {
    private int skipCount;

    private SkipDocumentQueryExecutionComponent(
      IDocumentQueryExecutionComponent source,
      int skipCount)
      : base(source)
    {
      this.skipCount = skipCount;
    }

    public static async Task<SkipDocumentQueryExecutionComponent> CreateAsync(
      int offsetCount,
      string continuationToken,
      Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback)
    {
      SkipDocumentQueryExecutionComponent.OffsetContinuationToken offsetContinuationToken = continuationToken == null ? new SkipDocumentQueryExecutionComponent.OffsetContinuationToken(offsetCount, (string) null) : SkipDocumentQueryExecutionComponent.OffsetContinuationToken.Parse(continuationToken);
      if (offsetContinuationToken.Offset > offsetCount)
        throw new BadRequestException("offset count in continuation token can not be greater than the offsetcount in the query.");
      return new SkipDocumentQueryExecutionComponent(await createSourceCallback(offsetContinuationToken.SourceToken), offsetContinuationToken.Offset);
    }

    public override bool IsDone => this.Source.IsDone;

    public override async Task<FeedResponse<object>> DrainAsync(
      int maxElements,
      CancellationToken token)
    {
      SkipDocumentQueryExecutionComponent executionComponent = this;
      // ISSUE: reference to a compiler-generated method
      FeedResponse<object> source = await executionComponent.\u003C\u003En__0(maxElements, token);
      List<object> list = source.Skip<object>(executionComponent.skipCount).ToList<object>();
      FeedResponse<object> feedResponse = new FeedResponse<object>((IEnumerable<object>) list, list.Count<object>(), source.Headers, source.UseETagAsContinuation, source.QueryMetrics, source.PartitionedClientSideRequestStatistics, source.DisallowContinuationTokenMessage, source.ResponseLengthBytes);
      int num = source.Count - list.Count;
      executionComponent.skipCount -= num;
      if (source.DisallowContinuationTokenMessage == null)
      {
        if (!executionComponent.IsDone)
        {
          string responseContinuation = source.ResponseContinuation;
          feedResponse.ResponseContinuation = new SkipDocumentQueryExecutionComponent.OffsetContinuationToken(executionComponent.skipCount, responseContinuation).ToString();
        }
        else
          feedResponse.ResponseContinuation = (string) null;
      }
      return feedResponse;
    }

    private struct OffsetContinuationToken
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

      public static SkipDocumentQueryExecutionComponent.OffsetContinuationToken Parse(string value)
      {
        SkipDocumentQueryExecutionComponent.OffsetContinuationToken offsetContinuationToken;
        if (!SkipDocumentQueryExecutionComponent.OffsetContinuationToken.TryParse(value, out offsetContinuationToken))
          throw new BadRequestException("Invalid OffsetContinuationToken: " + value);
        return offsetContinuationToken;
      }

      public static bool TryParse(
        string value,
        out SkipDocumentQueryExecutionComponent.OffsetContinuationToken offsetContinuationToken)
      {
        offsetContinuationToken = new SkipDocumentQueryExecutionComponent.OffsetContinuationToken();
        if (string.IsNullOrWhiteSpace(value))
          return false;
        try
        {
          offsetContinuationToken = JsonConvert.DeserializeObject<SkipDocumentQueryExecutionComponent.OffsetContinuationToken>(value);
          return true;
        }
        catch (JsonException ex)
        {
          DefaultTrace.TraceWarning(string.Format("{0} Invalid continuation token {1} for offset~Component, exception: {2}", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) value, (object) ex));
          return false;
        }
      }

      public override string ToString() => JsonConvert.SerializeObject((object) this);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ExecutionComponent.DistinctDocumentQueryExecutionComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query.ExecutionComponent
{
  internal sealed class DistinctDocumentQueryExecutionComponent : DocumentQueryExecutionComponentBase
  {
    private readonly DistinctMap distinctMap;
    private readonly DistinctQueryType distinctQueryType;
    private UInt192? lastHash;

    private DistinctDocumentQueryExecutionComponent(
      DistinctQueryType distinctQueryType,
      UInt192? previousHash,
      IDocumentQueryExecutionComponent source)
      : base(source)
    {
      this.distinctQueryType = distinctQueryType != DistinctQueryType.None ? distinctQueryType : throw new ArgumentException("It doesn't make sense to create a distinct component of type None.");
      this.distinctMap = DistinctMap.Create(distinctQueryType, previousHash);
    }

    public static async Task<IDocumentQueryExecutionComponent> CreateAsync(
      string requestContinuation,
      Func<string, Task<IDocumentQueryExecutionComponent>> createSourceCallback,
      DistinctQueryType distinctQueryType)
    {
      DistinctDocumentQueryExecutionComponent.DistinctContinuationToken continuationToken = new DistinctDocumentQueryExecutionComponent.DistinctContinuationToken(new UInt192?(), (string) null);
      if (requestContinuation != null)
      {
        continuationToken = DistinctDocumentQueryExecutionComponent.DistinctContinuationToken.Parse(requestContinuation);
        if (distinctQueryType != DistinctQueryType.Ordered && continuationToken.LastHash.HasValue)
          throw new BadRequestException(string.Format("DistinctContinuationToken is malformed: {0}. DistinctContinuationToken can not have a 'lastHash', when the query type is not ordered (ex SELECT DISTINCT VALUE c.blah FROM c ORDER BY c.blah).", (object) continuationToken));
      }
      DistinctQueryType distinctQueryType1 = distinctQueryType;
      UInt192? lastHash = continuationToken.LastHash;
      return (IDocumentQueryExecutionComponent) new DistinctDocumentQueryExecutionComponent(distinctQueryType1, lastHash, await createSourceCallback(continuationToken.SourceToken));
    }

    public override async Task<FeedResponse<object>> DrainAsync(
      int maxElements,
      CancellationToken cancellationToken)
    {
      DistinctDocumentQueryExecutionComponent executionComponent = this;
      List<object> distinctResults = new List<object>();
      // ISSUE: reference to a compiler-generated method
      FeedResponse<object> feedResponse = await executionComponent.\u003C\u003En__0(maxElements, cancellationToken);
      foreach (object document in feedResponse)
      {
        JToken jtokenFromObject = JTokenAndQueryResultConversionUtils.GetJTokenFromObject(document, out JsonSerializer _, out string _);
        if (executionComponent.distinctMap.Add(jtokenFromObject, out executionComponent.lastHash))
          distinctResults.Add(document);
      }
      if (!executionComponent.IsDone)
      {
        string responseContinuation = feedResponse.ResponseContinuation;
        feedResponse.ResponseContinuation = new DistinctDocumentQueryExecutionComponent.DistinctContinuationToken(executionComponent.lastHash, responseContinuation).ToString();
      }
      else
      {
        executionComponent.Source.Stop();
        feedResponse.ResponseContinuation = (string) null;
      }
      return new FeedResponse<object>((IEnumerable<object>) distinctResults, distinctResults.Count, feedResponse.Headers, feedResponse.UseETagAsContinuation, feedResponse.QueryMetrics, feedResponse.PartitionedClientSideRequestStatistics, executionComponent.distinctQueryType == DistinctQueryType.Ordered ? (string) null : RMResources.UnorderedDistinctQueryContinuationToken, feedResponse.ResponseLengthBytes);
    }

    private struct DistinctContinuationToken
    {
      public DistinctContinuationToken(UInt192? lastHash, string sourceToken)
      {
        this.LastHash = lastHash;
        this.SourceToken = sourceToken;
      }

      [JsonProperty("lastHash")]
      [JsonConverter(typeof (DistinctDocumentQueryExecutionComponent.DistinctContinuationToken.NullableUInt192Serializer))]
      public UInt192? LastHash { get; }

      [JsonProperty("sourceToken")]
      public string SourceToken { get; }

      public static DistinctDocumentQueryExecutionComponent.DistinctContinuationToken Parse(
        string value)
      {
        DistinctDocumentQueryExecutionComponent.DistinctContinuationToken distinctContinuationToken;
        if (!DistinctDocumentQueryExecutionComponent.DistinctContinuationToken.TryParse(value, out distinctContinuationToken))
          throw new BadRequestException("Invalid DistinctContinuationToken: " + value);
        return distinctContinuationToken;
      }

      public static bool TryParse(
        string value,
        out DistinctDocumentQueryExecutionComponent.DistinctContinuationToken distinctContinuationToken)
      {
        distinctContinuationToken = new DistinctDocumentQueryExecutionComponent.DistinctContinuationToken();
        if (string.IsNullOrWhiteSpace(value))
          return false;
        try
        {
          distinctContinuationToken = JsonConvert.DeserializeObject<DistinctDocumentQueryExecutionComponent.DistinctContinuationToken>(value);
          return true;
        }
        catch (JsonException ex)
        {
          DefaultTrace.TraceWarning(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + " Invalid continuation token " + value + " for Distinct~Component, exception: " + ex.Message);
          return false;
        }
      }

      public override string ToString() => JsonConvert.SerializeObject((object) this);

      private class NullableUInt192Serializer : JsonConverter
      {
        public override bool CanConvert(Type objectType) => (object) objectType == (object) typeof (UInt192) || (object) objectType == (object) typeof (object);

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
          return reader.Value == null ? (object) null : (object) UInt192.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
          if (value == null)
            writer.WriteNull();
          else
            JToken.FromObject((object) ((UInt192) value).ToString()).WriteTo(writer);
        }
      }
    }
  }
}

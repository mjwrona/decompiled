// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.QueryResponse`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Serializer;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.Cosmos
{
  internal class QueryResponse<T> : FeedResponse<T>
  {
    private readonly CosmosSerializerCore serializerCore;
    private readonly CosmosSerializationFormatOptions serializationOptions;

    private QueryResponse(
      HttpStatusCode httpStatusCode,
      IReadOnlyList<CosmosElement> cosmosElements,
      CosmosQueryResponseMessageHeaders responseMessageHeaders,
      CosmosDiagnostics diagnostics,
      CosmosSerializerCore serializerCore,
      CosmosSerializationFormatOptions serializationOptions,
      RequestMessage requestMessage)
    {
      this.QueryHeaders = responseMessageHeaders;
      this.Diagnostics = diagnostics;
      this.serializerCore = serializerCore;
      this.serializationOptions = serializationOptions;
      this.StatusCode = httpStatusCode;
      this.Count = cosmosElements.Count;
      this.Resource = (IEnumerable<T>) CosmosElementSerializer.GetResources<T>(cosmosElements, serializerCore);
      this.IndexUtilizationText = ResponseMessage.DecodeIndexMetrics((Headers) responseMessageHeaders, true);
      this.RequestMessage = requestMessage;
    }

    public override string ContinuationToken => this.Headers.ContinuationToken;

    public override double RequestCharge => this.Headers.RequestCharge;

    public override Headers Headers => (Headers) this.QueryHeaders;

    public override HttpStatusCode StatusCode { get; }

    public override CosmosDiagnostics Diagnostics { get; }

    public override int Count { get; }

    internal CosmosQueryResponseMessageHeaders QueryHeaders { get; }

    private Lazy<string> IndexUtilizationText { get; }

    public override string IndexMetrics => this.IndexUtilizationText?.Value;

    public override IEnumerator<T> GetEnumerator() => this.Resource.GetEnumerator();

    public override IEnumerable<T> Resource { get; }

    internal override RequestMessage RequestMessage { get; }

    internal static QueryResponse<TInput> CreateResponse<TInput>(
      QueryResponse cosmosQueryResponse,
      CosmosSerializerCore serializerCore)
    {
      using (cosmosQueryResponse)
      {
        cosmosQueryResponse.EnsureSuccessStatusCode();
        return new QueryResponse<TInput>(cosmosQueryResponse.StatusCode, cosmosQueryResponse.CosmosElements, cosmosQueryResponse.QueryHeaders, cosmosQueryResponse.Diagnostics, serializerCore, cosmosQueryResponse.CosmosSerializationOptions, cosmosQueryResponse.RequestMessage);
      }
    }
  }
}

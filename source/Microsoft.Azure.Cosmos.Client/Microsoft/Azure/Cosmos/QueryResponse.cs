// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.QueryResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Serializer;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Cosmos
{
  internal class QueryResponse : ResponseMessage
  {
    private readonly Lazy<MemoryStream> memoryStream;

    internal QueryResponse()
    {
    }

    private QueryResponse(
      IReadOnlyList<CosmosElement> result,
      int count,
      long responseLengthBytes,
      CosmosQueryResponseMessageHeaders responseHeaders,
      HttpStatusCode statusCode,
      RequestMessage requestMessage,
      CosmosException cosmosException,
      Lazy<MemoryStream> memoryStream,
      CosmosSerializationFormatOptions serializationOptions,
      ITrace trace)
    {
      int statusCode1 = (int) statusCode;
      RequestMessage requestMessage1 = requestMessage;
      CosmosException cosmosException1 = cosmosException;
      CosmosQueryResponseMessageHeaders responseMessageHeaders = responseHeaders;
      CosmosException cosmosException2 = cosmosException1;
      ITrace trace1 = trace;
      // ISSUE: explicit constructor call
      base.\u002Ector((HttpStatusCode) statusCode1, requestMessage1, (Headers) responseMessageHeaders, cosmosException2, trace1);
      this.CosmosElements = result;
      this.Count = count;
      this.ResponseLengthBytes = responseLengthBytes;
      this.memoryStream = memoryStream;
      this.CosmosSerializationOptions = serializationOptions;
    }

    public int Count { get; }

    public override Stream Content
    {
      get
      {
        Lazy<MemoryStream> memoryStream = this.memoryStream;
        return memoryStream == null ? (Stream) null : (Stream) memoryStream.Value;
      }
    }

    internal virtual IReadOnlyList<CosmosElement> CosmosElements { get; }

    internal virtual CosmosQueryResponseMessageHeaders QueryHeaders => (CosmosQueryResponseMessageHeaders) this.Headers;

    internal long ResponseLengthBytes { get; }

    internal virtual CosmosSerializationFormatOptions CosmosSerializationOptions { get; }

    internal bool GetHasMoreResults() => !string.IsNullOrEmpty(this.Headers.ContinuationToken);

    internal static QueryResponse CreateSuccess(
      IReadOnlyList<CosmosElement> result,
      int count,
      long responseLengthBytes,
      CosmosQueryResponseMessageHeaders responseHeaders,
      CosmosSerializationFormatOptions serializationOptions,
      ITrace trace)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count must be positive");
      if (responseLengthBytes < 0L)
        throw new ArgumentOutOfRangeException("responseLengthBytes must be positive");
      Lazy<MemoryStream> memoryStream = new Lazy<MemoryStream>((Func<MemoryStream>) (() => CosmosElementSerializer.ToStream(responseHeaders.ContainerRid, (IEnumerable<CosmosElement>) result, responseHeaders.ResourceType, serializationOptions)));
      return new QueryResponse(result, count, responseLengthBytes, responseHeaders, HttpStatusCode.OK, (RequestMessage) null, (CosmosException) null, memoryStream, serializationOptions, trace);
    }

    internal static QueryResponse CreateFailure(
      CosmosQueryResponseMessageHeaders responseHeaders,
      HttpStatusCode statusCode,
      RequestMessage requestMessage,
      CosmosException cosmosException,
      ITrace trace)
    {
      List<CosmosElement> result = new List<CosmosElement>();
      CosmosQueryResponseMessageHeaders responseHeaders1 = responseHeaders;
      int statusCode1 = (int) statusCode;
      CosmosException cosmosException1 = cosmosException;
      RequestMessage requestMessage1 = requestMessage;
      CosmosException cosmosException2 = cosmosException1;
      ITrace trace1 = trace;
      return new QueryResponse((IReadOnlyList<CosmosElement>) result, 0, 0L, responseHeaders1, (HttpStatusCode) statusCode1, requestMessage1, cosmosException2, (Lazy<MemoryStream>) null, (CosmosSerializationFormatOptions) null, trace1);
    }
  }
}

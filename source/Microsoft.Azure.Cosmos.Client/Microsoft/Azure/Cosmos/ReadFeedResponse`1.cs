// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadFeedResponse`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.Cosmos
{
  internal class ReadFeedResponse<T> : FeedResponse<T>
  {
    internal ReadFeedResponse(
      HttpStatusCode httpStatusCode,
      IEnumerable<T> resources,
      int resourceCount,
      Headers responseMessageHeaders,
      CosmosDiagnostics diagnostics,
      RequestMessage requestMessage)
    {
      this.Count = resourceCount;
      this.Headers = responseMessageHeaders;
      this.StatusCode = httpStatusCode;
      this.Diagnostics = diagnostics;
      this.Resource = resources;
      this.RequestMessage = requestMessage;
    }

    public override int Count { get; }

    public override string ContinuationToken => this.Headers?.ContinuationToken;

    public override Headers Headers { get; }

    public override IEnumerable<T> Resource { get; }

    public override HttpStatusCode StatusCode { get; }

    public override CosmosDiagnostics Diagnostics { get; }

    public override string IndexMetrics { get; }

    internal override RequestMessage RequestMessage { get; }

    public override IEnumerator<T> GetEnumerator() => this.Resource.GetEnumerator();

    internal static ReadFeedResponse<TInput> CreateResponse<TInput>(
      ResponseMessage responseMessage,
      CosmosSerializerCore serializerCore)
    {
      using (responseMessage)
      {
        if (responseMessage.StatusCode != HttpStatusCode.NotModified)
          responseMessage.EnsureSuccessStatusCode();
        IReadOnlyCollection<TInput> resources = CosmosFeedResponseSerializer.FromFeedResponseStream<TInput>(serializerCore, responseMessage.Content);
        return new ReadFeedResponse<TInput>(responseMessage.StatusCode, (IEnumerable<TInput>) resources, resources.Count, responseMessage.Headers, responseMessage.Diagnostics, responseMessage.RequestMessage);
      }
    }
  }
}

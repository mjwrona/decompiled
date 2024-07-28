// Decompiled with JetBrains decompiler
// Type: Nest.PreviewDatafeedResponseBuilder`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal class PreviewDatafeedResponseBuilder<TDocument> : CustomResponseBuilderBase where TDocument : class
  {
    public static PreviewDatafeedResponseBuilder<TDocument> Instance { get; } = new PreviewDatafeedResponseBuilder<TDocument>();

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      if (!response.Success)
        return (object) new PreviewDatafeedResponse<TDocument>();
      return (object) new PreviewDatafeedResponse<TDocument>()
      {
        Data = builtInSerializer.Deserialize<IReadOnlyCollection<TDocument>>(stream)
      };
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      PreviewDatafeedResponse<TDocument> datafeedResponse;
      if (response.Success)
      {
        PreviewDatafeedResponse<TDocument> datafeedResponse1 = new PreviewDatafeedResponse<TDocument>();
        PreviewDatafeedResponse<TDocument> datafeedResponse2 = datafeedResponse1;
        datafeedResponse2.Data = await builtInSerializer.DeserializeAsync<IReadOnlyCollection<TDocument>>(stream, ctx).ConfigureAwait(false);
        datafeedResponse = datafeedResponse1;
        datafeedResponse2 = (PreviewDatafeedResponse<TDocument>) null;
        datafeedResponse1 = (PreviewDatafeedResponse<TDocument>) null;
      }
      else
        datafeedResponse = new PreviewDatafeedResponse<TDocument>();
      return (object) datafeedResponse;
    }
  }
}

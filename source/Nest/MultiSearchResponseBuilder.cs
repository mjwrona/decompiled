// Decompiled with JetBrains decompiler
// Type: Nest.MultiSearchResponseBuilder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal class MultiSearchResponseBuilder : CustomResponseBuilderBase
  {
    public MultiSearchResponseBuilder(IRequest request) => this.Formatter = new MultiSearchResponseFormatter(request);

    private MultiSearchResponseFormatter Formatter { get; }

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      return !response.Success ? (object) new MultiSearchResponse() : (object) builtInSerializer.CreateStateful<MultiSearchResponse>((IJsonFormatter<MultiSearchResponse>) this.Formatter).Deserialize<MultiSearchResponse>(stream);
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      MultiSearchResponse multiSearchResponse;
      if (response.Success)
        multiSearchResponse = await builtInSerializer.CreateStateful<MultiSearchResponse>((IJsonFormatter<MultiSearchResponse>) this.Formatter).DeserializeAsync<MultiSearchResponse>(stream, ctx).ConfigureAwait(false);
      else
        multiSearchResponse = new MultiSearchResponse();
      return (object) multiSearchResponse;
    }
  }
}

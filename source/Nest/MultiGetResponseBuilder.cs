// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetResponseBuilder
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
  internal class MultiGetResponseBuilder : CustomResponseBuilderBase
  {
    public MultiGetResponseBuilder(IMultiGetRequest request) => this.Formatter = new MultiGetResponseFormatter(request);

    private MultiGetResponseFormatter Formatter { get; }

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      return !response.Success ? (object) new MultiGetResponse() : (object) builtInSerializer.CreateStateful<MultiGetResponse>((IJsonFormatter<MultiGetResponse>) this.Formatter).Deserialize<MultiGetResponse>(stream);
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      MultiGetResponse multiGetResponse;
      if (response.Success)
        multiGetResponse = await builtInSerializer.CreateStateful<MultiGetResponse>((IJsonFormatter<MultiGetResponse>) this.Formatter).DeserializeAsync<MultiGetResponse>(stream, ctx).ConfigureAwait(false);
      else
        multiGetResponse = new MultiGetResponse();
      return (object) multiGetResponse;
    }
  }
}

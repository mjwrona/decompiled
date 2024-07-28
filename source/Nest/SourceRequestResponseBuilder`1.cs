// Decompiled with JetBrains decompiler
// Type: Nest.SourceRequestResponseBuilder`1
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
  public class SourceRequestResponseBuilder<TDocument> : CustomResponseBuilderBase
  {
    public static SourceRequestResponseBuilder<TDocument> Instance { get; } = new SourceRequestResponseBuilder<TDocument>();

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      if (!response.Success)
        return (object) new SourceResponse<TDocument>();
      IJsonFormatterResolver formatterResolver;
      if (builtInSerializer is IInternalSerializer internalSerializer && internalSerializer.TryGetJsonFormatter(out formatterResolver))
      {
        IElasticsearchSerializer sourceSerializer = formatterResolver.GetConnectionSettings().SourceSerializer;
        return (object) new SourceResponse<TDocument>()
        {
          Body = sourceSerializer.Deserialize<TDocument>(stream)
        };
      }
      return (object) new SourceResponse<TDocument>()
      {
        Body = builtInSerializer.Deserialize<TDocument>(stream)
      };
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      if (!response.Success)
        return (object) new SourceResponse<TDocument>();
      SourceResponse<TDocument> sourceResponse1;
      SourceResponse<TDocument> sourceResponse2;
      IJsonFormatterResolver formatterResolver;
      if (builtInSerializer is IInternalSerializer internalSerializer && internalSerializer.TryGetJsonFormatter(out formatterResolver))
      {
        IElasticsearchSerializer sourceSerializer = formatterResolver.GetConnectionSettings().SourceSerializer;
        sourceResponse2 = new SourceResponse<TDocument>();
        sourceResponse1 = sourceResponse2;
        sourceResponse1.Body = await sourceSerializer.DeserializeAsync<TDocument>(stream, ctx).ConfigureAwait(false);
        return (object) sourceResponse2;
      }
      sourceResponse1 = new SourceResponse<TDocument>();
      sourceResponse2 = sourceResponse1;
      sourceResponse2.Body = await builtInSerializer.DeserializeAsync<TDocument>(stream, ctx).ConfigureAwait(false);
      return (object) sourceResponse1;
    }
  }
}

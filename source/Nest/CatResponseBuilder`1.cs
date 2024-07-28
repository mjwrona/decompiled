// Decompiled with JetBrains decompiler
// Type: Nest.CatResponseBuilder`1
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
  internal class CatResponseBuilder<TCatRecord> : CustomResponseBuilderBase where TCatRecord : ICatRecord
  {
    public static CatResponseBuilder<TCatRecord> Instance { get; } = new CatResponseBuilder<TCatRecord>();

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      if (response.Success)
      {
        int? httpStatusCode = response.HttpStatusCode;
        int num = 404;
        if (!(httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue))
          return (object) new CatResponse<TCatRecord>()
          {
            Records = builtInSerializer.Deserialize<IReadOnlyCollection<TCatRecord>>(stream)
          };
      }
      return (object) builtInSerializer.Deserialize<CatResponse<TCatRecord>>(stream);
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      if (response.Success)
      {
        int? httpStatusCode = response.HttpStatusCode;
        int num = 404;
        if (!(httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue))
          return (object) new CatResponse<TCatRecord>()
          {
            Records = await builtInSerializer.DeserializeAsync<IReadOnlyCollection<TCatRecord>>(stream, ctx).ConfigureAwait(false)
          };
      }
      return (object) await builtInSerializer.DeserializeAsync<CatResponse<TCatRecord>>(stream, ctx).ConfigureAwait(false);
    }
  }
}

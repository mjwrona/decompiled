// Decompiled with JetBrains decompiler
// Type: Nest.TranslateSqlResponseBuilder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal class TranslateSqlResponseBuilder : CustomResponseBuilderBase
  {
    public static TranslateSqlResponseBuilder Instance { get; } = new TranslateSqlResponseBuilder();

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      if (!response.Success)
        return (object) new TranslateSqlResponse();
      return (object) new TranslateSqlResponse()
      {
        Result = builtInSerializer.Deserialize<ISearchRequest>(stream)
      };
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      TranslateSqlResponse translateSqlResponse;
      if (response.Success)
      {
        TranslateSqlResponse translateSqlResponse1 = new TranslateSqlResponse();
        TranslateSqlResponse translateSqlResponse2 = translateSqlResponse1;
        translateSqlResponse2.Result = await builtInSerializer.DeserializeAsync<ISearchRequest>(stream, ctx).ConfigureAwait(false);
        translateSqlResponse = translateSqlResponse1;
        translateSqlResponse2 = (TranslateSqlResponse) null;
        translateSqlResponse1 = (TranslateSqlResponse) null;
      }
      else
        translateSqlResponse = new TranslateSqlResponse();
      return (object) translateSqlResponse;
    }
  }
}

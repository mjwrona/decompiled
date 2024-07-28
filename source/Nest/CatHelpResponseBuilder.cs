// Decompiled with JetBrains decompiler
// Type: Nest.CatHelpResponseBuilder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal class CatHelpResponseBuilder : CustomResponseBuilderBase
  {
    public static CatHelpResponseBuilder Instance { get; } = new CatHelpResponseBuilder();

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      CatResponse<CatHelpRecord> catResponse = new CatResponse<CatHelpRecord>();
      if (!response.Success)
        return (object) catResponse;
      using (stream)
      {
        using (MemoryStream destination = response.ConnectionConfiguration.MemoryStreamFactory.Create())
        {
          stream.CopyTo((Stream) destination);
          string body = destination.ToArray().Utf8String();
          CatHelpResponseBuilder.Parse(catResponse, body);
        }
      }
      return (object) catResponse;
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      CatResponse<CatHelpRecord> catResponse = new CatResponse<CatHelpRecord>();
      if (!response.Success)
        return (object) catResponse;
      using (stream)
      {
        using (MemoryStream ms = response.ConnectionConfiguration.MemoryStreamFactory.Create())
        {
          await stream.CopyToAsync((Stream) ms, 81920, ctx).ConfigureAwait(false);
          CatHelpResponseBuilder.Parse(catResponse, ms.ToArray().Utf8String());
        }
      }
      return (object) catResponse;
    }

    private static void Parse(CatResponse<CatHelpRecord> catResponse, string body) => catResponse.Records = (IReadOnlyCollection<CatHelpRecord>) ((IEnumerable<string>) body.Split('\n')).Skip<string>(1).Select<string, CatHelpRecord>((Func<string, CatHelpRecord>) (f => new CatHelpRecord()
    {
      Endpoint = f.Trim()
    })).ToList<CatHelpRecord>();
  }
}

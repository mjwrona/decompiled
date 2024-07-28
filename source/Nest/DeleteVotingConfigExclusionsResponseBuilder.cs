// Decompiled with JetBrains decompiler
// Type: Nest.DeleteVotingConfigExclusionsResponseBuilder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal class DeleteVotingConfigExclusionsResponseBuilder : CustomResponseBuilderBase
  {
    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      return (object) new DeleteVotingConfigExclusionsResponse();
    }

    public override Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      return Task.FromResult<object>((object) new DeleteVotingConfigExclusionsResponse());
    }
  }
}

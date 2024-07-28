// Decompiled with JetBrains decompiler
// Type: Nest.EnrichStatsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.EnrichApi;

namespace Nest
{
  public class EnrichStatsRequest : 
    PlainRequestBase<EnrichStatsRequestParameters>,
    IEnrichStatsRequest,
    IRequest<EnrichStatsRequestParameters>,
    IRequest
  {
    protected IEnrichStatsRequest Self => (IEnrichStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.EnrichStats;
  }
}

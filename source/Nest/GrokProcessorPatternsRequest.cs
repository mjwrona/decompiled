// Decompiled with JetBrains decompiler
// Type: Nest.GrokProcessorPatternsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IngestApi;

namespace Nest
{
  public class GrokProcessorPatternsRequest : 
    PlainRequestBase<GrokProcessorPatternsRequestParameters>,
    IGrokProcessorPatternsRequest,
    IRequest<GrokProcessorPatternsRequestParameters>,
    IRequest
  {
    protected IGrokProcessorPatternsRequest Self => (IGrokProcessorPatternsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IngestGrokProcessorPatterns;
  }
}

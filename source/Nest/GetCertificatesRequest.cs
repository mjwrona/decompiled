// Decompiled with JetBrains decompiler
// Type: Nest.GetCertificatesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;

namespace Nest
{
  public class GetCertificatesRequest : 
    PlainRequestBase<GetCertificatesRequestParameters>,
    IGetCertificatesRequest,
    IRequest<GetCertificatesRequestParameters>,
    IRequest
  {
    protected IGetCertificatesRequest Self => (IGetCertificatesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetCertificates;

    protected override sealed void RequestDefaults(GetCertificatesRequestParameters parameters) => parameters.CustomResponseBuilder = (CustomResponseBuilderBase) GetCertificatesResponseBuilder.Instance;
  }
}

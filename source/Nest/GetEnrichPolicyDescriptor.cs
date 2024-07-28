// Decompiled with JetBrains decompiler
// Type: Nest.GetEnrichPolicyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.EnrichApi;
using System;

namespace Nest
{
  public class GetEnrichPolicyDescriptor : 
    RequestDescriptorBase<GetEnrichPolicyDescriptor, GetEnrichPolicyRequestParameters, IGetEnrichPolicyRequest>,
    IGetEnrichPolicyRequest,
    IRequest<GetEnrichPolicyRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.EnrichGetPolicy;

    public GetEnrichPolicyDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    public GetEnrichPolicyDescriptor()
    {
    }

    Names IGetEnrichPolicyRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public GetEnrichPolicyDescriptor Name(Names name) => this.Assign<Names>(name, (Action<IGetEnrichPolicyRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.GetComponentTemplateDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using System;

namespace Nest
{
  public class GetComponentTemplateDescriptor : 
    RequestDescriptorBase<GetComponentTemplateDescriptor, GetComponentTemplateRequestParameters, IGetComponentTemplateRequest>,
    IGetComponentTemplateRequest,
    IRequest<GetComponentTemplateRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterGetComponentTemplate;

    public GetComponentTemplateDescriptor()
    {
    }

    public GetComponentTemplateDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Names IGetComponentTemplateRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public GetComponentTemplateDescriptor Name(Names name) => this.Assign<Names>(name, (Action<IGetComponentTemplateRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));

    public GetComponentTemplateDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public GetComponentTemplateDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.GetComponentTemplateRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetComponentTemplateRequest : 
    PlainRequestBase<GetComponentTemplateRequestParameters>,
    IGetComponentTemplateRequest,
    IRequest<GetComponentTemplateRequestParameters>,
    IRequest
  {
    protected IGetComponentTemplateRequest Self => (IGetComponentTemplateRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterGetComponentTemplate;

    public GetComponentTemplateRequest()
    {
    }

    public GetComponentTemplateRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    [IgnoreDataMember]
    Names IGetComponentTemplateRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}

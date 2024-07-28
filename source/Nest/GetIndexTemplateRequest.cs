// Decompiled with JetBrains decompiler
// Type: Nest.GetIndexTemplateRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetIndexTemplateRequest : 
    PlainRequestBase<GetIndexTemplateRequestParameters>,
    IGetIndexTemplateRequest,
    IRequest<GetIndexTemplateRequestParameters>,
    IRequest
  {
    protected IGetIndexTemplateRequest Self => (IGetIndexTemplateRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetTemplate;

    public GetIndexTemplateRequest()
    {
    }

    public GetIndexTemplateRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    [IgnoreDataMember]
    Names IGetIndexTemplateRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public bool? FlatSettings
    {
      get => this.Q<bool?>("flat_settings");
      set => this.Q("flat_settings", (object) value);
    }

    public bool? IncludeTypeName
    {
      get => this.Q<bool?>("include_type_name");
      set => this.Q("include_type_name", (object) value);
    }

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

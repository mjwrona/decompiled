// Decompiled with JetBrains decompiler
// Type: Nest.IndexTemplateExistsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class IndexTemplateExistsRequest : 
    PlainRequestBase<IndexTemplateExistsRequestParameters>,
    IIndexTemplateExistsRequest,
    IRequest<IndexTemplateExistsRequestParameters>,
    IRequest
  {
    protected IIndexTemplateExistsRequest Self => (IIndexTemplateExistsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesTemplateExists;

    public IndexTemplateExistsRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected IndexTemplateExistsRequest()
    {
    }

    [IgnoreDataMember]
    Names IIndexTemplateExistsRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public bool? FlatSettings
    {
      get => this.Q<bool?>("flat_settings");
      set => this.Q("flat_settings", (object) value);
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

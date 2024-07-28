// Decompiled with JetBrains decompiler
// Type: Nest.IndexTemplateV2ExistsRequest
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
  public class IndexTemplateV2ExistsRequest : 
    PlainRequestBase<IndexTemplateV2ExistsRequestParameters>,
    IIndexTemplateV2ExistsRequest,
    IRequest<IndexTemplateV2ExistsRequestParameters>,
    IRequest
  {
    protected IIndexTemplateV2ExistsRequest Self => (IIndexTemplateV2ExistsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesTemplateV2Exists;

    public IndexTemplateV2ExistsRequest(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected IndexTemplateV2ExistsRequest()
    {
    }

    [IgnoreDataMember]
    Name IIndexTemplateV2ExistsRequest.Name => this.Self.RouteValues.Get<Name>("name");

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

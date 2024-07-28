// Decompiled with JetBrains decompiler
// Type: Nest.PutIndexTemplateV2Request
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutIndexTemplateV2Request : 
    PlainRequestBase<PutIndexTemplateV2RequestParameters>,
    IPutIndexTemplateV2Request,
    IRequest<PutIndexTemplateV2RequestParameters>,
    IRequest
  {
    public IEnumerable<string> IndexPatterns { get; set; }

    public IEnumerable<string> ComposedOf { get; set; }

    public ITemplate Template { get; set; }

    public DataStream DataStream { get; set; }

    public int? Priority { get; set; }

    public long? Version { get; set; }

    public IDictionary<string, object> Meta { get; set; }

    protected IPutIndexTemplateV2Request Self => (IPutIndexTemplateV2Request) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesPutTemplateV2;

    public PutIndexTemplateV2Request(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutIndexTemplateV2Request()
    {
    }

    [IgnoreDataMember]
    Name IPutIndexTemplateV2Request.Name => this.Self.RouteValues.Get<Name>("name");

    public string Cause
    {
      get => this.Q<string>("cause");
      set => this.Q("cause", (object) value);
    }

    public bool? Create
    {
      get => this.Q<bool?>("create");
      set => this.Q("create", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.PutAliasRequest
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
  public class PutAliasRequest : 
    PlainRequestBase<PutAliasRequestParameters>,
    IPutAliasRequest,
    IRequest<PutAliasRequestParameters>,
    IRequest
  {
    public QueryContainer Filter { get; set; }

    public Routing IndexRouting { get; set; }

    public bool? IsWriteIndex { get; set; }

    public Routing Routing { get; set; }

    public Routing SearchRouting { get; set; }

    protected IPutAliasRequest Self => (IPutAliasRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesPutAlias;

    public PutAliasRequest(Indices index, Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutAliasRequest()
    {
    }

    [IgnoreDataMember]
    Indices IPutAliasRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    [IgnoreDataMember]
    Name IPutAliasRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}

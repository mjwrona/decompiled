// Decompiled with JetBrains decompiler
// Type: Nest.FollowInfoRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class FollowInfoRequest : 
    PlainRequestBase<FollowInfoRequestParameters>,
    IFollowInfoRequest,
    IRequest<FollowInfoRequestParameters>,
    IRequest
  {
    protected IFollowInfoRequest Self => (IFollowInfoRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationFollowInfo;

    public FollowInfoRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected FollowInfoRequest()
    {
    }

    [IgnoreDataMember]
    Indices IFollowInfoRequest.Index => this.Self.RouteValues.Get<Indices>("index");
  }
}

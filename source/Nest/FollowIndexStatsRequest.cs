// Decompiled with JetBrains decompiler
// Type: Nest.FollowIndexStatsRequest
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
  public class FollowIndexStatsRequest : 
    PlainRequestBase<FollowIndexStatsRequestParameters>,
    IFollowIndexStatsRequest,
    IRequest<FollowIndexStatsRequestParameters>,
    IRequest
  {
    protected IFollowIndexStatsRequest Self => (IFollowIndexStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationFollowIndexStats;

    public FollowIndexStatsRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected FollowIndexStatsRequest()
    {
    }

    [IgnoreDataMember]
    Indices IFollowIndexStatsRequest.Index => this.Self.RouteValues.Get<Indices>("index");
  }
}

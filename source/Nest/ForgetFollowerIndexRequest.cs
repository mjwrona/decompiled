// Decompiled with JetBrains decompiler
// Type: Nest.ForgetFollowerIndexRequest
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
  public class ForgetFollowerIndexRequest : 
    PlainRequestBase<ForgetFollowerIndexRequestParameters>,
    IForgetFollowerIndexRequest,
    IRequest<ForgetFollowerIndexRequestParameters>,
    IRequest
  {
    protected IForgetFollowerIndexRequest Self => (IForgetFollowerIndexRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationForgetFollowerIndex;

    public ForgetFollowerIndexRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected ForgetFollowerIndexRequest()
    {
    }

    [IgnoreDataMember]
    IndexName IForgetFollowerIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public string FollowerCluster { get; set; }

    public IndexName FollowerIndex { get; set; }

    public string FollowerIndexUUID { get; set; }

    public string LeaderRemoteCluster { get; set; }
  }
}

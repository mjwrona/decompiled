// Decompiled with JetBrains decompiler
// Type: Nest.CreateAutoFollowPatternRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class CreateAutoFollowPatternRequest : 
    PlainRequestBase<CreateAutoFollowPatternRequestParameters>,
    ICreateAutoFollowPatternRequest,
    IRequest<CreateAutoFollowPatternRequestParameters>,
    IRequest,
    IAutoFollowPattern
  {
    protected ICreateAutoFollowPatternRequest Self => (ICreateAutoFollowPatternRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationCreateAutoFollowPattern;

    public CreateAutoFollowPatternRequest(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected CreateAutoFollowPatternRequest()
    {
    }

    [IgnoreDataMember]
    Name ICreateAutoFollowPatternRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public string RemoteCluster { get; set; }

    public IEnumerable<string> LeaderIndexPatterns { get; set; }

    public IEnumerable<string> LeaderIndexExclusionPatterns { get; set; }

    public IIndexSettings Settings { get; set; }

    public string FollowIndexPattern { get; set; }

    public int? MaxReadRequestOperationCount { get; set; }

    public long? MaxOutstandingReadRequests { get; set; }

    public string MaxReadRequestSize { get; set; }

    public int? MaxWriteRequestOperationCount { get; set; }

    public string MaxWriteRequestSize { get; set; }

    public int? MaxOutstandingWriteRequests { get; set; }

    public int? MaxWriteBufferCount { get; set; }

    public string MaxWriteBufferSize { get; set; }

    public Time MaxRetryDelay { get; set; }

    public Time MaxPollTimeout { get; set; }
  }
}

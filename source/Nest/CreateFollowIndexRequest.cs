// Decompiled with JetBrains decompiler
// Type: Nest.CreateFollowIndexRequest
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
  public class CreateFollowIndexRequest : 
    PlainRequestBase<CreateFollowIndexRequestParameters>,
    ICreateFollowIndexRequest,
    IRequest<CreateFollowIndexRequestParameters>,
    IRequest
  {
    protected ICreateFollowIndexRequest Self => (ICreateFollowIndexRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationCreateFollowIndex;

    public CreateFollowIndexRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected CreateFollowIndexRequest()
    {
    }

    [IgnoreDataMember]
    IndexName ICreateFollowIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }

    public string RemoteCluster { get; set; }

    public IndexName LeaderIndex { get; set; }

    public IIndexSettings Settings { get; set; }

    public long? MaxReadRequestOperationCount { get; set; }

    public long? MaxOutstandingReadRequests { get; set; }

    public string MaxRequestSize { get; set; }

    public long? MaxWriteRequestOperationCount { get; set; }

    public string MaxWriteRequestSize { get; set; }

    public long? MaxOutstandingWriteRequests { get; set; }

    public long? MaxWriteBufferCount { get; set; }

    public string MaxWriteBufferSize { get; set; }

    public Time MaxRetryDelay { get; set; }

    public Time ReadPollTimeout { get; set; }
  }
}

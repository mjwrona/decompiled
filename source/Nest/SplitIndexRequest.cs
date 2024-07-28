// Decompiled with JetBrains decompiler
// Type: Nest.SplitIndexRequest
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
  public class SplitIndexRequest : 
    PlainRequestBase<SplitIndexRequestParameters>,
    ISplitIndexRequest,
    IRequest<SplitIndexRequestParameters>,
    IRequest
  {
    public IAliases Aliases { get; set; }

    public IIndexSettings Settings { get; set; }

    protected ISplitIndexRequest Self => (ISplitIndexRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesSplit;

    public SplitIndexRequest(IndexName index, IndexName target)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (target), (IUrlParameter) target)))
    {
    }

    [SerializationConstructor]
    protected SplitIndexRequest()
    {
    }

    [IgnoreDataMember]
    IndexName ISplitIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    [IgnoreDataMember]
    IndexName ISplitIndexRequest.Target => this.Self.RouteValues.Get<IndexName>("target");

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

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }
  }
}

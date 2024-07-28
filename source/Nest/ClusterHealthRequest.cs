// Decompiled with JetBrains decompiler
// Type: Nest.ClusterHealthRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ClusterHealthRequest : 
    PlainRequestBase<ClusterHealthRequestParameters>,
    IClusterHealthRequest,
    IRequest<ClusterHealthRequestParameters>,
    IRequest
  {
    protected IClusterHealthRequest Self => (IClusterHealthRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterHealth;

    public ClusterHealthRequest()
    {
    }

    public ClusterHealthRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IClusterHealthRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public Elasticsearch.Net.Level? Level
    {
      get => this.Q<Elasticsearch.Net.Level?>("level");
      set => this.Q("level", (object) value);
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

    public Elasticsearch.Net.WaitForEvents? WaitForEvents
    {
      get => this.Q<Elasticsearch.Net.WaitForEvents?>("wait_for_events");
      set => this.Q("wait_for_events", (object) value);
    }

    public bool? WaitForNoInitializingShards
    {
      get => this.Q<bool?>("wait_for_no_initializing_shards");
      set => this.Q("wait_for_no_initializing_shards", (object) value);
    }

    public bool? WaitForNoRelocatingShards
    {
      get => this.Q<bool?>("wait_for_no_relocating_shards");
      set => this.Q("wait_for_no_relocating_shards", (object) value);
    }

    public string WaitForNodes
    {
      get => this.Q<string>("wait_for_nodes");
      set => this.Q("wait_for_nodes", (object) value);
    }

    public Elasticsearch.Net.WaitForStatus? WaitForStatus
    {
      get => this.Q<Elasticsearch.Net.WaitForStatus?>("wait_for_status");
      set => this.Q("wait_for_status", (object) value);
    }
  }
}

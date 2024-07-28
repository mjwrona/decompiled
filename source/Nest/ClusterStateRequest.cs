// Decompiled with JetBrains decompiler
// Type: Nest.ClusterStateRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ClusterStateRequest : 
    PlainRequestBase<ClusterStateRequestParameters>,
    IClusterStateRequest,
    IRequest<ClusterStateRequestParameters>,
    IRequest
  {
    protected IClusterStateRequest Self => (IClusterStateRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterState;

    public ClusterStateRequest()
    {
    }

    public ClusterStateRequest(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public ClusterStateRequest(Metrics metric, Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric).Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Metrics IClusterStateRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    [IgnoreDataMember]
    Indices IClusterStateRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? FlatSettings
    {
      get => this.Q<bool?>("flat_settings");
      set => this.Q("flat_settings", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
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

    public long? WaitForMetadataVersion
    {
      get => this.Q<long?>("wait_for_metadata_version");
      set => this.Q("wait_for_metadata_version", (object) value);
    }

    public Time WaitForTimeout
    {
      get => this.Q<Time>("wait_for_timeout");
      set => this.Q("wait_for_timeout", (object) value);
    }
  }
}

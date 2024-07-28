// Decompiled with JetBrains decompiler
// Type: Nest.ClusterStateDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using System;

namespace Nest
{
  public class ClusterStateDescriptor : 
    RequestDescriptorBase<ClusterStateDescriptor, ClusterStateRequestParameters, IClusterStateRequest>,
    IClusterStateRequest,
    IRequest<ClusterStateRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterState;

    public ClusterStateDescriptor()
    {
    }

    public ClusterStateDescriptor(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public ClusterStateDescriptor(Metrics metric, Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric).Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Metrics IClusterStateRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    Indices IClusterStateRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public ClusterStateDescriptor Metric(Metrics metric) => this.Assign<Metrics>(metric, (Action<IClusterStateRequest, Metrics>) ((a, v) => a.RouteValues.Optional(nameof (metric), v)));

    public ClusterStateDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IClusterStateRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public ClusterStateDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IClusterStateRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public ClusterStateDescriptor AllIndices() => this.Index(Indices.All);

    public ClusterStateDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public ClusterStateDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public ClusterStateDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public ClusterStateDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public ClusterStateDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public ClusterStateDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public ClusterStateDescriptor WaitForMetadataVersion(long? waitformetadataversion) => this.Qs("wait_for_metadata_version", (object) waitformetadataversion);

    public ClusterStateDescriptor WaitForTimeout(Time waitfortimeout) => this.Qs("wait_for_timeout", (object) waitfortimeout);
  }
}

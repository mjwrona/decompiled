// Decompiled with JetBrains decompiler
// Type: Nest.ClusterHealthDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using System;

namespace Nest
{
  public class ClusterHealthDescriptor : 
    RequestDescriptorBase<ClusterHealthDescriptor, ClusterHealthRequestParameters, IClusterHealthRequest>,
    IClusterHealthRequest,
    IRequest<ClusterHealthRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterHealth;

    public ClusterHealthDescriptor()
    {
    }

    public ClusterHealthDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IClusterHealthRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public ClusterHealthDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IClusterHealthRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public ClusterHealthDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IClusterHealthRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public ClusterHealthDescriptor AllIndices() => this.Index(Indices.All);

    public ClusterHealthDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public ClusterHealthDescriptor Level(Elasticsearch.Net.Level? level) => this.Qs(nameof (level), (object) level);

    public ClusterHealthDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public ClusterHealthDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public ClusterHealthDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public ClusterHealthDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    public ClusterHealthDescriptor WaitForEvents(Elasticsearch.Net.WaitForEvents? waitforevents) => this.Qs("wait_for_events", (object) waitforevents);

    public ClusterHealthDescriptor WaitForNoInitializingShards(bool? waitfornoinitializingshards = true) => this.Qs("wait_for_no_initializing_shards", (object) waitfornoinitializingshards);

    public ClusterHealthDescriptor WaitForNoRelocatingShards(bool? waitfornorelocatingshards = true) => this.Qs("wait_for_no_relocating_shards", (object) waitfornorelocatingshards);

    public ClusterHealthDescriptor WaitForNodes(string waitfornodes) => this.Qs("wait_for_nodes", (object) waitfornodes);

    public ClusterHealthDescriptor WaitForStatus(Elasticsearch.Net.WaitForStatus? waitforstatus) => this.Qs("wait_for_status", (object) waitforstatus);
  }
}

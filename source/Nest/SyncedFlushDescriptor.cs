// Decompiled with JetBrains decompiler
// Type: Nest.SyncedFlushDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class SyncedFlushDescriptor : 
    RequestDescriptorBase<SyncedFlushDescriptor, SyncedFlushRequestParameters, ISyncedFlushRequest>,
    ISyncedFlushRequest,
    IRequest<SyncedFlushRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesSyncedFlush;

    public SyncedFlushDescriptor()
    {
    }

    public SyncedFlushDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ISyncedFlushRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public SyncedFlushDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ISyncedFlushRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public SyncedFlushDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ISyncedFlushRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public SyncedFlushDescriptor AllIndices() => this.Index(Indices.All);

    public SyncedFlushDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public SyncedFlushDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public SyncedFlushDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);
  }
}

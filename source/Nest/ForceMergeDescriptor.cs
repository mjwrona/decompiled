// Decompiled with JetBrains decompiler
// Type: Nest.ForceMergeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class ForceMergeDescriptor : 
    RequestDescriptorBase<ForceMergeDescriptor, ForceMergeRequestParameters, IForceMergeRequest>,
    IForceMergeRequest,
    IRequest<ForceMergeRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesForceMerge;

    public ForceMergeDescriptor()
    {
    }

    public ForceMergeDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IForceMergeRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public ForceMergeDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IForceMergeRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public ForceMergeDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IForceMergeRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public ForceMergeDescriptor AllIndices() => this.Index(Indices.All);

    public ForceMergeDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public ForceMergeDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public ForceMergeDescriptor Flush(bool? flush = true) => this.Qs(nameof (flush), (object) flush);

    public ForceMergeDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public ForceMergeDescriptor MaxNumSegments(long? maxnumsegments) => this.Qs("max_num_segments", (object) maxnumsegments);

    public ForceMergeDescriptor OnlyExpungeDeletes(bool? onlyexpungedeletes = true) => this.Qs("only_expunge_deletes", (object) onlyexpungedeletes);
  }
}

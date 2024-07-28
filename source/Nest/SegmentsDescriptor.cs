// Decompiled with JetBrains decompiler
// Type: Nest.SegmentsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class SegmentsDescriptor : 
    RequestDescriptorBase<SegmentsDescriptor, SegmentsRequestParameters, ISegmentsRequest>,
    ISegmentsRequest,
    IRequest<SegmentsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesSegments;

    public SegmentsDescriptor()
    {
    }

    public SegmentsDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ISegmentsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public SegmentsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ISegmentsRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public SegmentsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ISegmentsRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public SegmentsDescriptor AllIndices() => this.Index(Indices.All);

    public SegmentsDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public SegmentsDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public SegmentsDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public SegmentsDescriptor Verbose(bool? verbose = true) => this.Qs(nameof (verbose), (object) verbose);
  }
}

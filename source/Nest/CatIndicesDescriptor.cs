// Decompiled with JetBrains decompiler
// Type: Nest.CatIndicesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatIndicesDescriptor : 
    RequestDescriptorBase<CatIndicesDescriptor, CatIndicesRequestParameters, ICatIndicesRequest>,
    ICatIndicesRequest,
    IRequest<CatIndicesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatIndices;

    public CatIndicesDescriptor()
    {
    }

    public CatIndicesDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ICatIndicesRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public CatIndicesDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ICatIndicesRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public CatIndicesDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICatIndicesRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public CatIndicesDescriptor AllIndices() => this.Index(Indices.All);

    public CatIndicesDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatIndicesDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public CatIndicesDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatIndicesDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatIndicesDescriptor Health(Elasticsearch.Net.Health? health) => this.Qs(nameof (health), (object) health);

    public CatIndicesDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatIndicesDescriptor IncludeUnloadedSegments(bool? includeunloadedsegments = true) => this.Qs("include_unloaded_segments", (object) includeunloadedsegments);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.11.0, reason: This parameter does not affect the request. It will be removed in a future release.")]
    public CatIndicesDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatIndicesDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatIndicesDescriptor Pri(bool? pri = true) => this.Qs(nameof (pri), (object) pri);

    public CatIndicesDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatIndicesDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}

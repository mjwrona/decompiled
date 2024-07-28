// Decompiled with JetBrains decompiler
// Type: Nest.CatSegmentsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatSegmentsDescriptor : 
    RequestDescriptorBase<CatSegmentsDescriptor, CatSegmentsRequestParameters, ICatSegmentsRequest>,
    ICatSegmentsRequest,
    IRequest<CatSegmentsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatSegments;

    public CatSegmentsDescriptor()
    {
    }

    public CatSegmentsDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ICatSegmentsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public CatSegmentsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ICatSegmentsRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public CatSegmentsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICatSegmentsRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public CatSegmentsDescriptor AllIndices() => this.Index(Indices.All);

    public CatSegmentsDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatSegmentsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatSegmentsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatSegmentsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatSegmentsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatSegmentsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}

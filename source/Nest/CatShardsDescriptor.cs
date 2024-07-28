// Decompiled with JetBrains decompiler
// Type: Nest.CatShardsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatShardsDescriptor : 
    RequestDescriptorBase<CatShardsDescriptor, CatShardsRequestParameters, ICatShardsRequest>,
    ICatShardsRequest,
    IRequest<CatShardsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatShards;

    public CatShardsDescriptor()
    {
    }

    public CatShardsDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ICatShardsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public CatShardsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ICatShardsRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public CatShardsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICatShardsRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public CatShardsDescriptor AllIndices() => this.Index(Indices.All);

    public CatShardsDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatShardsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatShardsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatShardsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.11.0, reason: This parameter does not affect the request. It will be removed in a future release.")]
    public CatShardsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatShardsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatShardsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatShardsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}

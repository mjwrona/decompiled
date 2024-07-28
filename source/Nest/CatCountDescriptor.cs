// Decompiled with JetBrains decompiler
// Type: Nest.CatCountDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatCountDescriptor : 
    RequestDescriptorBase<CatCountDescriptor, CatCountRequestParameters, ICatCountRequest>,
    ICatCountRequest,
    IRequest<CatCountRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatCount;

    public CatCountDescriptor()
    {
    }

    public CatCountDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ICatCountRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public CatCountDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ICatCountRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public CatCountDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICatCountRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public CatCountDescriptor AllIndices() => this.Index(Indices.All);

    public CatCountDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatCountDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatCountDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public CatCountDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public CatCountDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatCountDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatCountDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}

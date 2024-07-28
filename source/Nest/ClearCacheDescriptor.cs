// Decompiled with JetBrains decompiler
// Type: Nest.ClearCacheDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class ClearCacheDescriptor : 
    RequestDescriptorBase<ClearCacheDescriptor, ClearCacheRequestParameters, IClearCacheRequest>,
    IClearCacheRequest,
    IRequest<ClearCacheRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesClearCache;

    public ClearCacheDescriptor()
    {
    }

    public ClearCacheDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IClearCacheRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public ClearCacheDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IClearCacheRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public ClearCacheDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IClearCacheRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public ClearCacheDescriptor AllIndices() => this.Index(Indices.All);

    public ClearCacheDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public ClearCacheDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public ClearCacheDescriptor Fielddata(bool? fielddata = true) => this.Qs(nameof (fielddata), (object) fielddata);

    public ClearCacheDescriptor Fields(Nest.Fields fields) => this.Qs(nameof (fields), (object) fields);

    public ClearCacheDescriptor Fields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs(nameof (fields), fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public ClearCacheDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public ClearCacheDescriptor Query(bool? query = true) => this.Qs(nameof (query), (object) query);

    public ClearCacheDescriptor Request(bool? request = true) => this.Qs(nameof (request), (object) request);
  }
}

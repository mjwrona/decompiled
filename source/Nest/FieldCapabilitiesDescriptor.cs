// Decompiled with JetBrains decompiler
// Type: Nest.FieldCapabilitiesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class FieldCapabilitiesDescriptor : 
    RequestDescriptorBase<FieldCapabilitiesDescriptor, FieldCapabilitiesRequestParameters, IFieldCapabilitiesRequest>,
    IFieldCapabilitiesRequest,
    IRequest<FieldCapabilitiesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceFieldCapabilities;

    public FieldCapabilitiesDescriptor()
    {
    }

    public FieldCapabilitiesDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IFieldCapabilitiesRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public FieldCapabilitiesDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IFieldCapabilitiesRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public FieldCapabilitiesDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IFieldCapabilitiesRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public FieldCapabilitiesDescriptor AllIndices() => this.Index(Indices.All);

    public FieldCapabilitiesDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public FieldCapabilitiesDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public FieldCapabilitiesDescriptor Fields(Nest.Fields fields) => this.Qs(nameof (fields), (object) fields);

    public FieldCapabilitiesDescriptor Fields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs(nameof (fields), fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public FieldCapabilitiesDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public FieldCapabilitiesDescriptor IncludeUnmapped(bool? includeunmapped = true) => this.Qs("include_unmapped", (object) includeunmapped);

    QueryContainer IFieldCapabilitiesRequest.IndexFilter { get; set; }

    protected override HttpMethod HttpMethod => this.Self.IndexFilter == null ? HttpMethod.GET : HttpMethod.POST;

    public FieldCapabilitiesDescriptor IndexFilter<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer> query)
      where T : class
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(query, (Action<IFieldCapabilitiesRequest, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.IndexFilter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}

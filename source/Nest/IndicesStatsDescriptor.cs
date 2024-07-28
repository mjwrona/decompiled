// Decompiled with JetBrains decompiler
// Type: Nest.IndicesStatsDescriptor
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
  public class IndicesStatsDescriptor : 
    RequestDescriptorBase<IndicesStatsDescriptor, IndicesStatsRequestParameters, IIndicesStatsRequest>,
    IIndicesStatsRequest,
    IRequest<IndicesStatsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesStats;

    public IndicesStatsDescriptor()
    {
    }

    public IndicesStatsDescriptor(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    public IndicesStatsDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    public IndicesStatsDescriptor(Indices index, Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Optional(nameof (metric), metric)))
    {
    }

    Metrics IIndicesStatsRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    Indices IIndicesStatsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public IndicesStatsDescriptor Metric(Metrics metric) => this.Assign<Metrics>(metric, (Action<IIndicesStatsRequest, Metrics>) ((a, v) => a.RouteValues.Optional(nameof (metric), v)));

    public IndicesStatsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IIndicesStatsRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public IndicesStatsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IIndicesStatsRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public IndicesStatsDescriptor AllIndices() => this.Index(Indices.All);

    public IndicesStatsDescriptor CompletionFields(Nest.Fields completionfields) => this.Qs("completion_fields", (object) completionfields);

    public IndicesStatsDescriptor CompletionFields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs("completion_fields", fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public IndicesStatsDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public IndicesStatsDescriptor FielddataFields(Nest.Fields fielddatafields) => this.Qs("fielddata_fields", (object) fielddatafields);

    public IndicesStatsDescriptor FielddataFields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs("fielddata_fields", fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public IndicesStatsDescriptor Fields(Nest.Fields fields) => this.Qs(nameof (fields), (object) fields);

    public IndicesStatsDescriptor Fields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs(nameof (fields), fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public IndicesStatsDescriptor ForbidClosedIndices(bool? forbidclosedindices = true) => this.Qs("forbid_closed_indices", (object) forbidclosedindices);

    public IndicesStatsDescriptor Groups(params string[] groups) => this.Qs(nameof (groups), (object) groups);

    public IndicesStatsDescriptor IncludeSegmentFileSizes(bool? includesegmentfilesizes = true) => this.Qs("include_segment_file_sizes", (object) includesegmentfilesizes);

    public IndicesStatsDescriptor IncludeUnloadedSegments(bool? includeunloadedsegments = true) => this.Qs("include_unloaded_segments", (object) includeunloadedsegments);

    public IndicesStatsDescriptor Level(Elasticsearch.Net.Level? level) => this.Qs(nameof (level), (object) level);
  }
}

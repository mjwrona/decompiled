// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation.WikiAggregationBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation
{
  public class WikiAggregationBuilder : IAggregationBuilder
  {
    private readonly IVssRequestContext m_requestContext;

    protected bool EnableAggregations { get; private set; }

    protected bool AddCollectionAggregation { get; private set; }

    protected IDictionary<string, IEnumerable<string>> SearchFilters { get; private set; }

    public WikiAggregationBuilder(
      IDictionary<string, IEnumerable<string>> searchFilters,
      bool enableAggregations,
      IVssRequestContext requestContext,
      bool addCollectionAggregation = false)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_requestContext = requestContext;
      this.EnableAggregations = enableAggregations;
      this.AddCollectionAggregation = addCollectionAggregation;
      if (!this.EnableAggregations)
        return;
      this.SearchFilters = searchFilters;
    }

    [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
    public virtual AggregationContainerDescriptor<T> Aggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      if (!this.EnableAggregations)
        return aggDescriptor;
      this.GetAggregate<T>(aggDescriptor, "ProjectFilters", "filtered_project_aggs", "project_aggs", FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "projectName", (object) "raw")));
      this.GetAggregate<T>(aggDescriptor, "Wiki", "filtered_wiki_aggs", "wiki_aggs", FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "wikiName", (object) "raw")));
      this.GetAggregate<T>(aggDescriptor, "TagFilters", "filtered_tags_aggs", "tags_aggs", FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "tags", (object) "raw")));
      if (this.AddCollectionAggregation)
        this.GetAggregate<T>(aggDescriptor, "CollectionFilters", "filtered_collection_aggs", "collection_aggs", FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "collectionName", (object) "raw")));
      return aggDescriptor;
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Aggregations(new Func<AggregationContainerDescriptor<object>, IAggregationContainer>(this.Aggregates<object>)))).PrettyJson();

    protected void GetAggregate<T>(
      AggregationContainerDescriptor<T> aggDescriptor,
      string inputFilterName,
      string filteredAggsName,
      string termsAggsName,
      string platformFieldName)
      where T : class
    {
      Dictionary<string, IEnumerable<string>> allButInputFilter = this.SearchFilters.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (kvp => !kvp.Key.Equals(inputFilterName, StringComparison.OrdinalIgnoreCase))).ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (kvp => kvp.Value));
      if (allButInputFilter.Count > 0)
      {
        // ISSUE: method pointer
        aggDescriptor.Filter(filteredAggsName, (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (f => (IFilterAggregation) f.Filter((Func<QueryContainerDescriptor<T>, QueryContainer>) (fd => new WikiFilterBuilder((IDictionary<string, IEnumerable<string>>) allButInputFilter, this.m_requestContext).Filters<T>(fd))).Aggregations(new Func<AggregationContainerDescriptor<T>, IAggregationContainer>((object) this, __methodptr(\u003CGetAggregate\u003Eg__TermsAggContainerDescriptor\u007C0)))));
      }
      else
        TermsAggContainerDescriptor(aggDescriptor);

      IAggregationContainer TermsAggContainerDescriptor(AggregationContainerDescriptor<T> agg) => (IAggregationContainer) agg.Terms(termsAggsName, (Func<TermsAggregationDescriptor<T>, ITermsAggregation>) (t => (ITermsAggregation) t.Field((Field) platformFieldName).Size(new int?(CommonConstants.MaxNumberOfBucketsInTermsAggregations))));
    }

    internal static class Constants
    {
      public const string ProjectFilteredAggsName = "filtered_project_aggs";
      public const string ProjectTermsAggsName = "project_aggs";
      public const string WikiFilteredAggsName = "filtered_wiki_aggs";
      public const string WikiTermsAggsName = "wiki_aggs";
      public const string TagsFilteredAggsName = "filtered_tags_aggs";
      public const string TagsTermsAggsName = "tags_aggs";
      public const string CollectionFilteredAggsName = "filtered_collection_aggs";
      public const string CollectionTermsAggsName = "collection_aggs";
    }
  }
}

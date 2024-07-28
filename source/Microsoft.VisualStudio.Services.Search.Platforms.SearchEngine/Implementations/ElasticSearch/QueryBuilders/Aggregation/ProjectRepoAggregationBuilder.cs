// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation.ProjectRepoAggregationBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation
{
  internal class ProjectRepoAggregationBuilder : IAggregationBuilder
  {
    private readonly bool m_enableAggregations;
    private readonly IDictionary<string, IEnumerable<string>> m_searchFilters;
    private readonly IEnumerable<CustomAggregation> m_customAggregations;

    public ProjectRepoAggregationBuilder(
      IDictionary<string, IEnumerable<string>> searchFilters,
      bool enableAggregations,
      IEnumerable<CustomAggregation> customAggregations = null)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_enableAggregations = enableAggregations;
      if (!this.m_enableAggregations)
        return;
      this.m_searchFilters = searchFilters;
      this.m_customAggregations = customAggregations;
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Aggregations(new Func<AggregationContainerDescriptor<object>, IAggregationContainer>(this.Aggregates<object>)))).PrettyJson();

    public AggregationContainerDescriptor<T> Aggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      if (this.m_enableAggregations)
      {
        IEnumerable<string> strings = (IEnumerable<string>) new string[4]
        {
          Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Collections,
          Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Tags,
          Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Languages,
          Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Visibility
        };
        Dictionary<string, CustomAggregation> dictionary = new Dictionary<string, CustomAggregation>();
        if (this.m_customAggregations != null && this.m_customAggregations.Any<CustomAggregation>())
        {
          strings = this.m_customAggregations.Select<CustomAggregation, string>((Func<CustomAggregation, string>) (x => x.AggregationField));
          dictionary = this.m_customAggregations.ToDictionary<CustomAggregation, string>((Func<CustomAggregation, string>) (x => x.AggregationField));
        }
        foreach (string str in strings)
        {
          string filterCategory = str;
          string name = (string) null;
          string aggregationIdConstant = (string) null;
          string rawFieldConstant = (string) null;
          int aggsize = CommonConstants.MaxNumberOfBucketsInTermsAggregations;
          if (dictionary.ContainsKey(filterCategory))
            aggsize = dictionary[filterCategory].AggregationSize;
          if (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Collections.Equals(filterCategory, StringComparison.Ordinal))
          {
            name = "filtered_collection_aggs";
            aggregationIdConstant = "collection_aggs";
            rawFieldConstant = "collectionNameOriginal";
          }
          else if (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Tags.Equals(filterCategory, StringComparison.Ordinal))
          {
            name = "filtered_project_tags_aggs";
            aggregationIdConstant = "project_tags_aggs";
            rawFieldConstant = "tags.raw";
          }
          else if (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Languages.Equals(filterCategory, StringComparison.Ordinal))
          {
            name = "filtered_languages_aggs";
            aggregationIdConstant = "languages_aggs";
            rawFieldConstant = "languages.raw";
          }
          else if (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Visibility.Equals(filterCategory, StringComparison.Ordinal))
          {
            name = "filtered_visibility_aggs";
            aggregationIdConstant = "visibility_aggs";
            rawFieldConstant = "visibility";
          }
          else if (Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Projects.Equals(filterCategory, StringComparison.Ordinal))
          {
            name = "filtered_project_aggs";
            aggregationIdConstant = "project_aggs";
            rawFieldConstant = "name.raw";
          }
          if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(aggregationIdConstant) && !string.IsNullOrEmpty(rawFieldConstant))
          {
            KeyValuePair<string, IEnumerable<string>>[] allButCurrentFilter = this.m_searchFilters.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (kvp => !kvp.Key.Equals(filterCategory, StringComparison.OrdinalIgnoreCase))).ToArray<KeyValuePair<string, IEnumerable<string>>>();
            if (allButCurrentFilter.Length != 0)
              aggDescriptor.Filter(name, (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (f => (IFilterAggregation) f.Filter((Func<QueryContainerDescriptor<T>, QueryContainer>) (fd => new ProjectRepoFilterBuilder((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) allButCurrentFilter).Filters<T>(fd))).Aggregations(new Func<AggregationContainerDescriptor<T>, IAggregationContainer>(TermsAggContainerDescriptor))));
            else
              TermsAggContainerDescriptor(aggDescriptor);
          }

          IAggregationContainer TermsAggContainerDescriptor(AggregationContainerDescriptor<T> agg) => (IAggregationContainer) agg.Terms(aggregationIdConstant, (Func<TermsAggregationDescriptor<T>, ITermsAggregation>) (t => (ITermsAggregation) t.Field((Field) rawFieldConstant).Size(new int?(aggsize))));
        }
      }
      return aggDescriptor;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation.CollectionPackageAggregationBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation
{
  internal class CollectionPackageAggregationBuilder : PackageAggregationBuilder
  {
    public CollectionPackageAggregationBuilder(
      IDictionary<string, IEnumerable<string>> searchFilters,
      bool enableAggregations,
      int take,
      bool enableHighlighting)
      : base(searchFilters, enableAggregations, take, enableHighlighting)
    {
    }

    public override AggregationContainerDescriptor<T> Aggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
    {
      this.CreateMandatoryAggs<T>(aggDescriptor);
      if (this.m_enableAggregations)
      {
        string[] strArray = new string[3]
        {
          PackageSearchFilterCategories.ProtocolType,
          PackageSearchFilterCategories.Feeds,
          PackageSearchFilterCategories.View
        };
        foreach (string inputFilterName in strArray)
        {
          string filteredAggsName = (string) null;
          string termsAggsName = (string) null;
          string platformFieldName = (string) null;
          if (PackageSearchFilterCategories.ProtocolType.Equals(inputFilterName, StringComparison.Ordinal))
          {
            filteredAggsName = "filtered_protocol_tags_aggs";
            termsAggsName = "protocol_aggs";
            platformFieldName = "protocol.raw";
          }
          else if (PackageSearchFilterCategories.Feeds.Equals(inputFilterName, StringComparison.Ordinal))
          {
            filteredAggsName = "filtered_feed_aggs";
            termsAggsName = "feed_aggs";
            platformFieldName = "feedName.raw";
          }
          else if (PackageSearchFilterCategories.View.Equals(inputFilterName, StringComparison.Ordinal))
          {
            filteredAggsName = "filtered_view_aggs";
            termsAggsName = "view_aggs";
            platformFieldName = "views.viewNameOriginal";
          }
          if (!string.IsNullOrEmpty(filteredAggsName) && !string.IsNullOrEmpty(termsAggsName) && !string.IsNullOrEmpty(platformFieldName))
            this.GetAggregate<T>(aggDescriptor, inputFilterName, filteredAggsName, termsAggsName, platformFieldName);
        }
      }
      return aggDescriptor;
    }

    protected override void CreateMandatoryAggs<T>(AggregationContainerDescriptor<T> aggDescriptor)
    {
      if (this.m_searchFilters.Count > 0)
      {
        // ISSUE: method pointer
        aggDescriptor.Filter("Filtered_Results_Aggs", (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (ad => (IFilterAggregation) ad.Filter((Func<QueryContainerDescriptor<T>, QueryContainer>) (fd => new PackageFilterBuilder(this.m_searchFilters).Filters<T>(fd))).Aggregations(new Func<AggregationContainerDescriptor<T>, IAggregationContainer>((object) this, __methodptr(\u003CCreateMandatoryAggs\u003Eg__PackageTermsAggContainer\u007C2_0<T>)))));
      }
      else
        PackageTermsAggContainer(aggDescriptor);

      IAggregationContainer PackageTermsAggContainer<T>(AggregationContainerDescriptor<T> ag1) where T : class => (IAggregationContainer) ag1.Terms("Package_Aggs", (Func<TermsAggregationDescriptor<T>, ITermsAggregation>) (ad1 => (ITermsAggregation) ad1.Field((Field) "packageId").Size(new int?(this.m_take)).Order((Func<TermsOrderDescriptor<T>, IPromise<IList<TermsOrder>>>) (d => (IPromise<IList<TermsOrder>>) d.Descending("max_score_agg"))).Aggregations((Func<AggregationContainerDescriptor<T>, IAggregationContainer>) (ag2 => (IAggregationContainer) ag2.Terms("Feed_Aggs", (Func<TermsAggregationDescriptor<T>, ITermsAggregation>) (ad2 => (ITermsAggregation) ad2.Size(new int?(10)).Field((Field) "feedName.raw").Aggregations((Func<AggregationContainerDescriptor<T>, IAggregationContainer>) (ag3 => (IAggregationContainer) ag3.TopHits("Top_Versions_Aggs", (Func<TopHitsAggregationDescriptor<T>, ITopHitsAggregation>) (a => (ITopHitsAggregation) a.Sort((Func<SortDescriptor<T>, IPromise<IList<ISort>>>) (s => (IPromise<IList<ISort>>) s.Field((Field) "sortableVersion", SortOrder.Descending))).Size(new int?(1)).Highlight(new PackageHighlightBuilder(this.m_enableHighlighting).Highlight<T>()))))))).Max("max_score_agg", (Func<MaxAggregationDescriptor<T>, IMaxAggregation>) (ad2 => (IMaxAggregation) ad2.Script("_score"))))))).Cardinality("Package_Count_Aggs", (Func<CardinalityAggregationDescriptor<T>, ICardinalityAggregation>) (ad3 => (ICardinalityAggregation) ad3.Field((Field) "packageId")));
    }

    protected override void GetAggregate<T>(
      AggregationContainerDescriptor<T> aggDescriptor,
      string inputFilterName,
      string filteredAggsName,
      string termsAggsName,
      string platformFieldName)
    {
      IList<KeyValuePair<string, IEnumerable<string>>> allButInputFilter = (IList<KeyValuePair<string, IEnumerable<string>>>) this.m_searchFilters.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (kvp => !kvp.Key.Equals(inputFilterName, StringComparison.OrdinalIgnoreCase))).ToList<KeyValuePair<string, IEnumerable<string>>>();
      if (allButInputFilter.Count > 0)
      {
        // ISSUE: method pointer
        aggDescriptor.Filter(filteredAggsName, (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (f => (IFilterAggregation) f.Filter((Func<QueryContainerDescriptor<T>, QueryContainer>) (fd => new PackageFilterBuilder((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) allButInputFilter).Filters<T>(fd))).Aggregations(new Func<AggregationContainerDescriptor<T>, IAggregationContainer>((object) this, __methodptr(\u003CGetAggregate\u003Eg__TermsAggContainerDescriptor\u007C0)))));
      }
      else
        TermsAggContainerDescriptor(aggDescriptor);

      IAggregationContainer TermsAggContainerDescriptor(AggregationContainerDescriptor<T> agg) => (IAggregationContainer) agg.Terms(termsAggsName, (Func<TermsAggregationDescriptor<T>, ITermsAggregation>) (t => (ITermsAggregation) t.Field((Field) platformFieldName).Size(new int?(CommonConstants.MaxNumberOfBucketsInTermsAggregations)).Aggregations((Func<AggregationContainerDescriptor<T>, IAggregationContainer>) (ag2 => (IAggregationContainer) ag2.Cardinality("Package_Count_Aggs", (Func<CardinalityAggregationDescriptor<T>, ICardinalityAggregation>) (ad => (ICardinalityAggregation) ad.Field((Field) "packageId")))))));
    }

    protected override void CreateMandatoryAggsForCount<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
    {
      if (this.m_searchFilters.Count > 0)
        aggDescriptor.Filter("Filtered_Results_Aggs", (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (ad => (IFilterAggregation) ad.Filter((Func<QueryContainerDescriptor<T>, QueryContainer>) (fd => new PackageFilterBuilder(this.m_searchFilters).Filters<T>(fd))).Aggregations((Func<AggregationContainerDescriptor<T>, IAggregationContainer>) (ag1 => (IAggregationContainer) ag1.Cardinality("Package_Count_Aggs", (Func<CardinalityAggregationDescriptor<T>, ICardinalityAggregation>) (ad3 => (ICardinalityAggregation) ad3.Field((Field) "packageId")))))));
      else
        aggDescriptor.Cardinality("Package_Count_Aggs", (Func<CardinalityAggregationDescriptor<T>, ICardinalityAggregation>) (ad3 => (ICardinalityAggregation) ad3.Field((Field) "packageId")));
    }
  }
}

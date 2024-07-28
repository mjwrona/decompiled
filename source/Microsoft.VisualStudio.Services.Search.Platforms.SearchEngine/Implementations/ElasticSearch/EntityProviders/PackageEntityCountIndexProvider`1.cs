// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.PackageEntityCountIndexProvider`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Nest;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  internal sealed class PackageEntityCountIndexProvider<T> : PackageEntityIndexProvider<T> where T : class
  {
    public override void BuildSearchComponents(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query,
      out Func<QueryContainerDescriptor<T>, QueryContainer> filter,
      out Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations,
      out Func<HighlightDescriptor<T>, IHighlight> highlight,
      out Func<IVssRequestContext, SortDescriptor<T>> sort)
    {
      PackageSearchPlatformRequest searchPlatformRequest = request as PackageSearchPlatformRequest;
      query = this.GetFilteredQuery(rawQueryString, rawFilterString);
      filter = new Func<QueryContainerDescriptor<T>, QueryContainer>(new PackageFilterBuilder(searchPlatformRequest.SearchFilters).Filters<T>);
      aggregations = new Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>(new PackageAggregationBuilder(searchPlatformRequest.SearchFilters, searchPlatformRequest.Options.HasFlag((Enum) SearchOptions.Faceting), searchPlatformRequest.TakeAggResults, searchPlatformRequest.Options.HasFlag((Enum) SearchOptions.Highlighting)).AggregatesForCount<T>);
      highlight = (Func<HighlightDescriptor<T>, IHighlight>) null;
      sort = (Func<IVssRequestContext, SortDescriptor<T>>) null;
    }

    public override IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetSearchFacets(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      return (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) null;
    }

    public override List<SearchHit> GetSearchHits(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      return new List<SearchHit>();
    }
  }
}

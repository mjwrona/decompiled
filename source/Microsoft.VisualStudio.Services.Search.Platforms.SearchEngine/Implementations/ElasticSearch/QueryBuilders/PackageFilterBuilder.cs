// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.PackageFilterBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class PackageFilterBuilder
  {
    private readonly IDictionary<string, IEnumerable<string>> m_elasticsearchFilters;

    public PackageFilterBuilder(
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_elasticsearchFilters = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) searchFilters)
      {
        if (searchFilter.Key.Equals(PackageSearchFilterCategories.Collections, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["collectionName"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(PackageSearchFilterCategories.ProtocolType, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["protocol"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(PackageSearchFilterCategories.Feeds, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["feedName.lower"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(PackageSearchFilterCategories.View, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["views.viewName"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Filter category [{0}] is not supported.", (object) searchFilter.Key)));
      }
    }

    public PackageFilterBuilder(
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> searchFilters)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_elasticsearchFilters = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in searchFilters)
      {
        if (searchFilter.Key.Equals(PackageSearchFilterCategories.Collections, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["collectionName"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(PackageSearchFilterCategories.ProtocolType, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["protocol"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(PackageSearchFilterCategories.Feeds, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["feedName.lower"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(PackageSearchFilterCategories.View, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["views.viewName"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Filter category [{0}] is not supported.", (object) searchFilter.Key)));
      }
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<BoolQueryDescriptor<object>>(new BoolQueryDescriptor<object>().Filter(new Func<QueryContainerDescriptor<object>, QueryContainer>(this.Filters<object>)))).PrettyJson();

    internal QueryContainer Filters<T>(QueryContainerDescriptor<T> filterDescriptor) where T : class => filterDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bm => (IBoolQuery) bm.Must(this.m_elasticsearchFilters.Select<KeyValuePair<string, IEnumerable<string>>, QueryContainer>((Func<KeyValuePair<string, IEnumerable<string>>, QueryContainer>) (filterCategory => Query<T>.Terms((Func<TermsQueryDescriptor<T>, ITermsQuery>) (t => (ITermsQuery) t.Field((Field) filterCategory.Key).Terms<string>(filterCategory.Value))))).ToArray<QueryContainer>())));
  }
}

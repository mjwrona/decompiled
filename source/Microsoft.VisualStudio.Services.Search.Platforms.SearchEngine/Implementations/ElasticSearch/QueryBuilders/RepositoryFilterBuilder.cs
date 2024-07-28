// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.RepositoryFilterBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class RepositoryFilterBuilder
  {
    private readonly Dictionary<string, IEnumerable<string>> m_elasticsearchFilters;

    public RepositoryFilterBuilder(
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, IEnumerable<string>>>(searchFilters, nameof (searchFilters));
      this.m_elasticsearchFilters = new Dictionary<string, IEnumerable<string>>();
      if (searchFilters.ContainsKey(Constants.FilterCategories.Collections))
        this.m_elasticsearchFilters["collectionName"] = searchFilters[Constants.FilterCategories.Collections].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
      if (searchFilters.ContainsKey(Constants.FilterCategories.Languages))
        this.m_elasticsearchFilters["languages"] = searchFilters[Constants.FilterCategories.Languages].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
      if (!searchFilters.ContainsKey(Constants.FilterCategories.Visibility))
        return;
      List<string> list = searchFilters[Constants.FilterCategories.Visibility].Select<string, string>((Func<string, string>) (f => Enum.Parse(typeof (ProjectVisibility), f, true).ToString())).ToList<string>();
      if (list.Contains("Organization") && !list.Contains("Enterprise"))
        list.Add("Enterprise");
      else if (list.Contains("Enterprise") && !list.Contains("Organization"))
        list.Add("Organization");
      this.m_elasticsearchFilters["visibility"] = (IEnumerable<string>) list;
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<BoolQueryDescriptor<object>>(new BoolQueryDescriptor<object>().Filter(new Func<QueryContainerDescriptor<object>, QueryContainer>(this.Filters<object>)))).PrettyJson();

    internal QueryContainer Filters<T>(QueryContainerDescriptor<T> filterDescriptor) where T : class => filterDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bm => (IBoolQuery) bm.Must(this.m_elasticsearchFilters.Select<KeyValuePair<string, IEnumerable<string>>, QueryContainer>((Func<KeyValuePair<string, IEnumerable<string>>, QueryContainer>) (filterCategory => Query<T>.Terms((Func<TermsQueryDescriptor<T>, ITermsQuery>) (t => (ITermsQuery) t.Field((Field) filterCategory.Key).Terms<string>(filterCategory.Value))))).ToArray<QueryContainer>())));
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.ProjectRepoFilterBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class ProjectRepoFilterBuilder
  {
    private readonly Dictionary<string, IEnumerable<string>> m_elasticsearchFilters;

    public ProjectRepoFilterBuilder(
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_elasticsearchFilters = new Dictionary<string, IEnumerable<string>>();
      if (searchFilters.ContainsKey(Constants.FilterCategories.Collections))
        this.m_elasticsearchFilters["collectionName"] = searchFilters[Constants.FilterCategories.Collections].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
      if (searchFilters.ContainsKey(Constants.FilterCategories.Tags))
        this.m_elasticsearchFilters["tags.lower"] = searchFilters[Constants.FilterCategories.Tags].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
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

    public ProjectRepoFilterBuilder(
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> searchFilters)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_elasticsearchFilters = new Dictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in searchFilters)
      {
        if (searchFilter.Key.Equals(Constants.FilterCategories.Collections, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["collectionName"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(Constants.FilterCategories.Tags, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["tags.lower"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(Constants.FilterCategories.Languages, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters["languages"] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(Constants.FilterCategories.Visibility, StringComparison.OrdinalIgnoreCase))
        {
          List<string> list = searchFilter.Value.Select<string, string>((Func<string, string>) (f => Enum.Parse(typeof (ProjectVisibility), f, true).ToString())).ToList<string>();
          if (list.Contains("Organization") && !list.Contains("Enterprise"))
            list.Add("Enterprise");
          else if (list.Contains("Enterprise") && !list.Contains("Organization"))
            list.Add("Organization");
          this.m_elasticsearchFilters["visibility"] = (IEnumerable<string>) list;
        }
        else
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Filter category [{0}] is not supported.", (object) searchFilter.Key)));
      }
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<BoolQueryDescriptor<object>>(new BoolQueryDescriptor<object>().Filter(new Func<QueryContainerDescriptor<object>, QueryContainer>(this.Filters<object>)))).PrettyJson();

    internal QueryContainer Filters<T>(QueryContainerDescriptor<T> filterDescriptor) where T : class => filterDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bm => (IBoolQuery) bm.Must(this.m_elasticsearchFilters.Select<KeyValuePair<string, IEnumerable<string>>, QueryContainer>((Func<KeyValuePair<string, IEnumerable<string>>, QueryContainer>) (filterCategory => Query<T>.Terms((Func<TermsQueryDescriptor<T>, ITermsQuery>) (t => (ITermsQuery) t.Field((Field) filterCategory.Key).Terms<string>(filterCategory.Value))))).ToArray<QueryContainer>())));

    public IExpression GetQueryFilterExpression()
    {
      List<IExpression> source = new List<IExpression>();
      foreach (KeyValuePair<string, IEnumerable<string>> elasticsearchFilter in this.m_elasticsearchFilters)
        source.Add((IExpression) new TermsExpression(elasticsearchFilter.Key, Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, elasticsearchFilter.Value.Select<string, string>((Func<string, string>) (x => x.ToString((IFormatProvider) CultureInfo.InvariantCulture).ToLowerInvariant()))));
      IExpression filterExpression = (IExpression) new EmptyExpression();
      if (source.Count > 0)
        filterExpression = source.Count == 1 ? source[0] : source.Aggregate<IExpression>((Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
        {
          current,
          filter
        })));
      return filterExpression;
    }
  }
}

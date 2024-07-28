// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.WikiFilterBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class WikiFilterBuilder
  {
    private readonly IDictionary<string, IEnumerable<string>> m_elasticsearchFilters;

    public WikiFilterBuilder(
      IDictionary<string, IEnumerable<string>> searchFilters,
      IVssRequestContext requestContext)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_elasticsearchFilters = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) searchFilters)
      {
        if (searchFilters[searchFilter.Key] != null)
        {
          int localeId = requestContext.GetLocaleId();
          switch (searchFilter.Key)
          {
            case "CollectionFilters":
              this.m_elasticsearchFilters["collectionName"] = searchFilters["CollectionFilters"].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
              continue;
            case "ProjectFilters":
              this.m_elasticsearchFilters["projectName"] = localeId != 1055 || !requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsTurkishILocaleNormalizationEnabled") ? searchFilters["ProjectFilters"].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant())) : searchFilters["ProjectFilters"].Select<string, string>((Func<string, string>) (f => f.NormalizeStringForTurkishLocale()));
              continue;
            case "Wiki":
              this.m_elasticsearchFilters["wikiName"] = localeId != 1055 || !requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsTurkishILocaleNormalizationEnabled") ? searchFilters["Wiki"].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant())) : searchFilters["Wiki"].Select<string, string>((Func<string, string>) (f => f.NormalizeStringForTurkishLocale()));
              continue;
            case "TagFilters":
              this.m_elasticsearchFilters[FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "tags", (object) "lower"))] = searchFilters["TagFilters"].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
              continue;
            default:
              throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Filter category [{0}] is not supported.", (object) searchFilter.Key)));
          }
        }
      }
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<BoolQueryDescriptor<object>>(new BoolQueryDescriptor<object>().Filter(new Func<QueryContainerDescriptor<object>, QueryContainer>(this.Filters<object>)))).PrettyJson();

    internal QueryContainer Filters<T>(QueryContainerDescriptor<T> filterDescriptor) where T : class => filterDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bm => (IBoolQuery) bm.Must(this.m_elasticsearchFilters.Select<KeyValuePair<string, IEnumerable<string>>, QueryContainer>((Func<KeyValuePair<string, IEnumerable<string>>, QueryContainer>) (filterCategory => Query<T>.Terms((Func<TermsQueryDescriptor<T>, ITermsQuery>) (t => (ITermsQuery) t.Field((Field) filterCategory.Key).Terms<string>(filterCategory.Value))))).ToArray<QueryContainer>())));

    public IExpression GetQueryFilterExpression()
    {
      List<IExpression> source = new List<IExpression>();
      foreach (KeyValuePair<string, IEnumerable<string>> elasticsearchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) this.m_elasticsearchFilters)
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

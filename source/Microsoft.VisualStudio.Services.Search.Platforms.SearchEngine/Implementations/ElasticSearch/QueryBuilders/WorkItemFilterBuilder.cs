// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.WorkItemFilterBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class WorkItemFilterBuilder
  {
    private readonly Dictionary<string, IEnumerable<string>> m_elasticsearchFilters;

    public WorkItemFilterBuilder(
      IVssRequestContext requestContext,
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      int localeId = requestContext.GetLocaleId();
      this.m_elasticsearchFilters = new Dictionary<string, IEnumerable<string>>();
      if (searchFilters.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project))
        this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, "projectName")] = localeId != 1055 || !requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsTurkishILocaleNormalizationEnabled") ? searchFilters[Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant())) : searchFilters[Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project].Select<string, string>((Func<string, string>) (f => f.NormalizeStringForTurkishLocale()));
      if (searchFilters.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType))
        this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType)] = searchFilters[Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
      if (searchFilters.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState))
        this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState)] = searchFilters[Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
      if (searchFilters.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo))
        this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo)] = searchFilters[Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
      if (!searchFilters.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAreaPath))
        return;
      this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, WorkItemContract.PlatformFieldNames.AreaPath)] = searchFilters[Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAreaPath].Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
    }

    public WorkItemFilterBuilder(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> searchFilters)
    {
      int localeId = requestContext.GetLocaleId();
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_elasticsearchFilters = new Dictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in searchFilters)
      {
        if (searchFilter.Key.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, "projectName")] = localeId != 1055 || !requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsTurkishILocaleNormalizationEnabled") ? searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant())) : searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.NormalizeStringForTurkishLocale()));
        else if (searchFilter.Key.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType)] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState)] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo)] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else if (searchFilter.Key.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAreaPath, StringComparison.OrdinalIgnoreCase))
          this.m_elasticsearchFilters[this.GetPlatformFieldName(requestContext, WorkItemContract.PlatformFieldNames.AreaPath)] = searchFilter.Value.Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant()));
        else
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Filter category [{0}] is not supported.", (object) searchFilter.Key)));
      }
    }

    public IExpression GetQueryFilterExpression()
    {
      List<IExpression> source = new List<IExpression>();
      foreach (KeyValuePair<string, IEnumerable<string>> elasticsearchFilter in this.m_elasticsearchFilters)
        source.Add((IExpression) new TermsExpression(elasticsearchFilter.Key, Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, elasticsearchFilter.Value.Select<string, string>((Func<string, string>) (x => x.ToString((IFormatProvider) CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture)))));
      IExpression filterExpression = (IExpression) new EmptyExpression();
      if (source.Count > 0)
        filterExpression = source.Count == 1 ? source[0] : source.Aggregate<IExpression>((Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
        {
          current,
          filter
        })));
      return filterExpression;
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<BoolQueryDescriptor<object>>(new BoolQueryDescriptor<object>().Filter(new Func<QueryContainerDescriptor<object>, QueryContainer>(this.Filters<object>)))).PrettyJson();

    internal QueryContainer Filters<T>(QueryContainerDescriptor<T> filterDescriptor) where T : class => filterDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bm => (IBoolQuery) bm.Must(this.m_elasticsearchFilters.Select<KeyValuePair<string, IEnumerable<string>>, QueryContainer>((Func<KeyValuePair<string, IEnumerable<string>>, QueryContainer>) (filterCategory => Query<T>.Terms((Func<TermsQueryDescriptor<T>, ITermsQuery>) (t => (ITermsQuery) t.Field((Field) filterCategory.Key).Terms<string>(filterCategory.Value))))).ToArray<QueryContainer>())));

    private string GetPlatformFieldName(IVssRequestContext requestContext, string searchFilterKey)
    {
      bool flag1 = requestContext.IsFeatureEnabled("Search.Server.WorkItem.QueryIdentityFields");
      bool flag2 = (bool) requestContext.Items["isUserAnonymousKey"];
      if (searchFilterKey.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType, StringComparison.Ordinal))
        return WorkItemContract.NonAnalyzedPlatformFieldNames.WorkItemType;
      if (searchFilterKey.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState, StringComparison.Ordinal))
        return WorkItemContract.NonAnalyzedPlatformFieldNames.State;
      if (searchFilterKey.Equals(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo, StringComparison.Ordinal))
      {
        if (!flag1)
          return WorkItemContract.NonAnalyzedPlatformFieldNames.AssignedTo;
        return flag2 ? WorkItemContract.NonAnalyzedPlatformFieldNames.AssignedToName : WorkItemContract.NonAnalyzedPlatformFieldNames.AssignedToIdentity;
      }
      if (searchFilterKey.Equals("projectName", StringComparison.Ordinal))
        return "projectName";
      return searchFilterKey.Equals(WorkItemContract.PlatformFieldNames.AreaPath, StringComparison.Ordinal) ? FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemContract.PlatformFieldNames.AreaPath, (object) "lower")) : throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Filter category [{0}] is not supported.", (object) searchFilterKey)));
    }
  }
}

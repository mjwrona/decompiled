// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation.WorkItemAggregationBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation
{
  internal class WorkItemAggregationBuilder : IAggregationBuilder
  {
    private readonly bool m_enableAggregations;
    private readonly IDictionary<string, IEnumerable<string>> m_searchFilters;
    private readonly IVssRequestContext m_requestContext;
    private readonly bool m_isQueryingIdentityFieldsEnabled;

    public WorkItemAggregationBuilder(
      IVssRequestContext requestContext,
      IDictionary<string, IEnumerable<string>> searchFilters,
      bool enableAggregations)
    {
      if (searchFilters == null)
        throw new ArgumentNullException(nameof (searchFilters));
      this.m_enableAggregations = enableAggregations;
      if (this.m_enableAggregations)
        this.m_searchFilters = searchFilters;
      this.m_requestContext = requestContext;
      this.m_isQueryingIdentityFieldsEnabled = requestContext.IsFeatureEnabled("Search.Server.WorkItem.QueryIdentityFields");
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Aggregations(new Func<AggregationContainerDescriptor<object>, IAggregationContainer>(this.Aggregates<object>)))).PrettyJson();

    public AggregationContainerDescriptor<T> Aggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      if (this.m_enableAggregations)
      {
        string str = WorkItemContract.PlatformFieldNames.AssignedTo;
        if (this.m_isQueryingIdentityFieldsEnabled)
          str = !(bool) this.m_requestContext.Items["isUserAnonymousKey"] ? WorkItemContract.PlatformFieldNames.AssignedToIdentity : WorkItemContract.PlatformFieldNames.AssignedToName;
        this.GetAggregate<T>(aggDescriptor, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project, "filtered_project_aggs", "project_aggs", FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "projectName", (object) "raw")));
        this.GetAggregate<T>(aggDescriptor, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType, "filtered_type_aggs", "type_aggs", FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemContract.PlatformFieldNames.WorkItemType, (object) "raw")));
        this.GetAggregate<T>(aggDescriptor, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState, "filtered_state_aggs", "state_aggs", FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemContract.PlatformFieldNames.State, (object) "raw")));
        this.GetAggregate<T>(aggDescriptor, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo, "filtered_assignedto_aggs", "assignedto_aggs", FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) str, (object) "raw")));
      }
      return aggDescriptor;
    }

    private void GetAggregate<T>(
      AggregationContainerDescriptor<T> aggDescriptor,
      string inputFilterName,
      string filteredAggsName,
      string termsAggsName,
      string platformFieldName)
      where T : class
    {
      IList<KeyValuePair<string, IEnumerable<string>>> allButInputFilter = (IList<KeyValuePair<string, IEnumerable<string>>>) this.m_searchFilters.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (kvp => !kvp.Key.Equals(inputFilterName, StringComparison.OrdinalIgnoreCase))).ToList<KeyValuePair<string, IEnumerable<string>>>();
      if (allButInputFilter.Count > 0)
      {
        // ISSUE: method pointer
        aggDescriptor.Filter(filteredAggsName, (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (f => (IFilterAggregation) f.Filter((Func<QueryContainerDescriptor<T>, QueryContainer>) (fd => new WorkItemFilterBuilder(this.m_requestContext, (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) allButInputFilter).Filters<T>(fd))).Aggregations(new Func<AggregationContainerDescriptor<T>, IAggregationContainer>((object) this, __methodptr(\u003CGetAggregate\u003Eg__TermsAggContainerDescriptor\u007C0)))));
      }
      else
        TermsAggContainerDescriptor(aggDescriptor);

      IAggregationContainer TermsAggContainerDescriptor(AggregationContainerDescriptor<T> agg) => (IAggregationContainer) agg.Terms(termsAggsName, (Func<TermsAggregationDescriptor<T>, ITermsAggregation>) (t => (ITermsAggregation) t.Field((Field) platformFieldName).Size(new int?(CommonConstants.MaxNumberOfBucketsInTermsAggregations))));
    }

    internal static class Constants
    {
      public const string ProjectFilteredAggsName = "filtered_project_aggs";
      public const string ProjectTermsAggsName = "project_aggs";
      public const string TypeFilteredAggsName = "filtered_type_aggs";
      public const string TypeTermsAggsName = "type_aggs";
      public const string StateFilteredAggsName = "filtered_state_aggs";
      public const string StateTermsAggsName = "state_aggs";
      public const string AssignedToFilteredAggsName = "filtered_assignedto_aggs";
      public const string AssignedToTermsAggsName = "assignedto_aggs";
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.WorkItemHighlightBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class WorkItemHighlightBuilder
  {
    public const string HighlighterTypev5 = "workitem-unified";
    public const string HighlightStartTag = "<highlighthit>";
    public const string HighlightEndTag = "</highlighthit>";
    public const int MaxNumberOfFragmentsForStringField = 1;
    public const int MaxNumberOfFragmentsForHtmlField = 3;
    public const int MaxNumberOfFragmentsForUnknownField = 3;
    private readonly bool m_enableHighlighting;
    private readonly bool m_allowSpellingErrors;
    private readonly IEnumerable<string> m_inlineSearchFilters;
    private readonly IExpression m_queryExpression;
    private readonly bool m_isInstantSearch;
    private bool m_enableHighlightQuery;
    private string m_highlightQuery = string.Empty;

    public WorkItemHighlightBuilder(
      IExpression queryExpression,
      SearchOptions options,
      IEnumerable<string> inlineSearchFilters,
      bool isInstantSearch = false)
    {
      this.m_enableHighlighting = options.HasFlag((Enum) SearchOptions.Highlighting);
      this.m_allowSpellingErrors = options.HasFlag((Enum) SearchOptions.AllowSpellingErrors);
      this.m_inlineSearchFilters = inlineSearchFilters;
      this.m_queryExpression = queryExpression;
      this.m_isInstantSearch = isInstantSearch;
    }

    public string ToString(IVssRequestContext requestContext) => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Highlight(this.Highlight<object>(requestContext)))).PrettyJson();

    internal Func<HighlightDescriptor<T>, IHighlight> Highlight<T>(IVssRequestContext requestContext) where T : class
    {
      if (!this.m_enableHighlighting)
        return (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h);
      this.m_enableHighlightQuery = requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/WorkItemHighlightQueryOverride", TeamFoundationHostType.Deployment);
      List<Func<HighlightFieldDescriptor<T>, IHighlightField>> descriptors = new List<Func<HighlightFieldDescriptor<T>, IHighlightField>>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.m_isInstantSearch)
      {
        stringSet.UnionWith((IEnumerable<string>) WorkItemHighlightBuilder.GetWorkItemFieldsToHighlightForInstantSearch());
      }
      else
      {
        stringSet.UnionWith((IEnumerable<string>) WorkItemHighlightBuilder.GetWorkItemFieldsToHighlight(requestContext));
        stringSet.UnionWith(this.m_inlineSearchFilters);
      }
      foreach (string field in stringSet)
        descriptors.AddRange(this.GetDescriptorsForField<T>(requestContext, field));
      return (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h.PreTags("<highlighthit>").PostTags("</highlighthit>").RequireFieldMatch().Fields(descriptors.ToArray()));
    }

    private IEnumerable<Func<HighlightFieldDescriptor<T>, IHighlightField>> GetDescriptorsForField<T>(
      IVssRequestContext requestContext,
      string field)
      where T : class
    {
      List<Func<HighlightFieldDescriptor<T>, IHighlightField>> descriptors = new List<Func<HighlightFieldDescriptor<T>, IHighlightField>>();
      WorkItemField fieldData;
      if (requestContext.GetService<IWorkItemFieldCacheService>().TryGetFieldByRefName(requestContext, field, out fieldData))
      {
        foreach (string fieldName in this.GetPlatformFieldNamesForHighlighting(fieldData, requestContext))
          this.AddDescriptorForField<T>(descriptors, fieldName, "workitem-unified", this.GetHighlightFragmentCount(fieldData));
      }
      else
      {
        string nameForHighlighting = this.GetPlatformFieldNameFromReferenceOrDisplayNameForHighlighting(field);
        this.AddDescriptorForField<T>(descriptors, nameForHighlighting, "workitem-unified", 3);
      }
      return (IEnumerable<Func<HighlightFieldDescriptor<T>, IHighlightField>>) descriptors;
    }

    private void AddDescriptorForField<T>(
      List<Func<HighlightFieldDescriptor<T>, IHighlightField>> descriptors,
      string fieldName,
      string highlighterType,
      int numberOfFragments)
      where T : class
    {
      if (this.m_enableHighlightQuery)
      {
        bool isFilteredQuery = false;
        string format = string.IsNullOrEmpty(this.m_highlightQuery) ? this.m_queryExpression.ToElasticSearchHighlightQuery(fieldName, out isFilteredQuery) : this.m_highlightQuery;
        if (!isFilteredQuery && string.IsNullOrEmpty(this.m_highlightQuery))
          this.m_highlightQuery = format;
        if (string.IsNullOrEmpty(format))
          return;
        string highlightRawQueryWithFieldName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) fieldName);
        descriptors.Add((Func<HighlightFieldDescriptor<T>, IHighlightField>) (f => (IHighlightField) f.Field((Field) fieldName).NumberOfFragments(new int?(numberOfFragments)).Type(highlighterType).Order(new HighlighterOrder?(HighlighterOrder.Score)).HighlightQuery(new Func<QueryContainerDescriptor<T>, QueryContainer>(new RawQueryBuilder(highlightRawQueryWithFieldName).HighlighterQuery<T>))));
      }
      else
        descriptors.Add((Func<HighlightFieldDescriptor<T>, IHighlightField>) (f => (IHighlightField) f.Field((Field) fieldName).NumberOfFragments(new int?(numberOfFragments)).Type(highlighterType).Order(new HighlighterOrder?(HighlighterOrder.Score))));
    }

    private List<string> GetPlatformFieldNamesForHighlighting(
      WorkItemField resolvedField,
      IVssRequestContext requestContext)
    {
      WorkItemIndexedField itemIndexedField = !requestContext.IsFeatureEnabled("Search.Server.WorkItem.QueryIdentityFields") || !resolvedField.IsIdentity ? WorkItemIndexedField.FromWitField(resolvedField) : (!(bool) requestContext.Items["isUserAnonymousKey"] ? WorkItemIndexedField.FromWitField(resolvedField.ReferenceName, WorkItemContract.FieldType.Identity) : WorkItemIndexedField.FromWitField(resolvedField.ReferenceName, WorkItemContract.FieldType.Name));
      if (this.m_allowSpellingErrors || itemIndexedField.Type == WorkItemContract.FieldType.Path || itemIndexedField.Type == WorkItemContract.FieldType.Integer || itemIndexedField.Type == WorkItemContract.FieldType.Name || itemIndexedField.Type == WorkItemContract.FieldType.Identity || itemIndexedField.ReferenceName.Equals("System.State", StringComparison.OrdinalIgnoreCase) || itemIndexedField.ReferenceName.Equals("System.AssignedTo", StringComparison.OrdinalIgnoreCase) || itemIndexedField.ReferenceName.Equals("System.CreatedBy", StringComparison.OrdinalIgnoreCase) || itemIndexedField.ReferenceName.Equals("System.WorkItemType", StringComparison.OrdinalIgnoreCase))
      {
        if (itemIndexedField.Type == WorkItemContract.FieldType.Integer)
          return new List<string>()
          {
            WorkItemIndexedField.FromWitField(resolvedField.ReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName
          };
        return new List<string>()
        {
          itemIndexedField.PlatformFieldName
        };
      }
      return new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) itemIndexedField.PlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) itemIndexedField.PlatformFieldName, (object) "pattern"))
      };
    }

    private int GetHighlightFragmentCount(WorkItemField field)
    {
      if (field.ReferenceName.Equals("System.Title", StringComparison.OrdinalIgnoreCase) || field.ReferenceName.Equals("System.State", StringComparison.OrdinalIgnoreCase) || field.ReferenceName.Equals("System.AssignedTo", StringComparison.OrdinalIgnoreCase) || field.ReferenceName.Equals("System.CreatedBy", StringComparison.OrdinalIgnoreCase) || field.ReferenceName.Equals("System.Id", StringComparison.OrdinalIgnoreCase) || field.ReferenceName.Equals("System.Title", StringComparison.OrdinalIgnoreCase) || field.ReferenceName.Equals("System.Tags", StringComparison.OrdinalIgnoreCase) || field.ReferenceName.Equals("System.WorkItemType", StringComparison.OrdinalIgnoreCase) || field.Type == Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.TreePath || field.Type == Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Integer)
        return 0;
      return field.Type == Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Html || field.Type == Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.History ? 3 : 1;
    }

    public static IList<string> GetWorkItemFieldsToHighlight(IVssRequestContext requestContext)
    {
      string str = requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/WorkItemSearchController/HighlightedFieldsList");
      if (string.IsNullOrWhiteSpace(str))
        str = "system.title;system.tags;system.areapath;system.iterationpath;system.id;system.state;system.assignedto;system.createdby;system.description;system.history;microsoft.vsts.tcm.reprosteps;microsoft.vsts.tcm.steps";
      return (IList<string>) ((IEnumerable<string>) str.Split(';')).Select<string, string>((Func<string, string>) (p => p.Trim())).ToList<string>();
    }

    public static IList<string> GetWorkItemFieldsToHighlightForInstantSearch() => (IList<string>) new List<string>()
    {
      "system.title"
    };

    private string GetPlatformFieldNameFromReferenceOrDisplayNameForHighlighting(string fieldName)
    {
      bool flag = fieldName.Contains<char>('.');
      string str1;
      if (!this.m_allowSpellingErrors)
        str1 = string.Empty;
      else
        str1 = FormattableString.Invariant(FormattableStringFactory.Create(".{0}", (object) "stemmed"));
      string str2 = str1;
      return flag ? FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}", (object) WorkItemIndexedField.FromWitField(fieldName, WorkItemContract.FieldType.AllTypes).PlatformFieldName, (object) str2)) : FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}", (object) WorkItemIndexedField.FromWitField(FormattableString.Invariant(FormattableStringFactory.Create("*.{0}", (object) fieldName)), WorkItemContract.FieldType.AllTypes).PlatformFieldName, (object) str2));
    }
  }
}

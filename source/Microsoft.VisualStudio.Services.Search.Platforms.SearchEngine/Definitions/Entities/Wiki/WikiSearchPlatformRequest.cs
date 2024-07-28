// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.WikiSearchPlatformRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class WikiSearchPlatformRequest : EntitySearchPlatformRequest
  {
    public IReadOnlyList<string> HighlightFields { get; }

    public IReadOnlyList<string> SearchFields { get; }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    private WikiSearchPlatformRequest(
      SearchOptions searchOptions,
      string requestId,
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexInfo> indexInfo,
      IExpression queryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression scopeFiltersExpression,
      WikiSearchQuery searchQuery,
      IReadOnlyList<string> highlightFields,
      IReadOnlyList<string> searchFields,
      IReadOnlyList<string> fields)
    {
      this.Options = searchOptions;
      this.RequestId = requestId;
      this.IndexInfo = indexInfo;
      this.QueryParseTree = queryParseTree;
      this.SearchFilters = searchFilters;
      this.ScopeFiltersExpression = scopeFiltersExpression;
      this.SkipResults = searchQuery.SkipResults;
      this.TakeResults = searchQuery.TakeResults;
      this.HighlightFields = highlightFields;
      this.SearchFields = searchFields;
      this.Fields = (IEnumerable<string>) fields;
      this.ContractType = DocumentContractType.WikiContract;
      this.SortOptions = searchQuery.SortOptions;
    }

    public class Builder
    {
      private IReadOnlyList<string> HighlightFields { get; }

      private IReadOnlyList<string> SearchFields { get; }

      private IReadOnlyList<string> Fields { get; }

      public Builder()
      {
      }

      public Builder(
        IReadOnlyList<string> highlightFields,
        IReadOnlyList<string> searchFields,
        IReadOnlyList<string> fields)
      {
        this.HighlightFields = highlightFields;
        this.SearchFields = searchFields;
        this.Fields = fields;
      }

      public WikiSearchPlatformRequest Build(
        IVssRequestContext requestContext,
        SearchOptions searchOptions,
        string requestId,
        IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexInfo> indexInfo,
        IExpression queryParseTree,
        IDictionary<string, IEnumerable<string>> searchFilters,
        IExpression scopeFiltersExpression,
        WikiSearchQuery searchQuery)
      {
        IReadOnlyList<string> fields = this.Fields ?? this.GetDefaultFields();
        IReadOnlyList<string> highlightFields = this.HighlightFields ?? this.GetDefaultHighlightFields();
        IReadOnlyList<string> searchFields = this.SearchFields ?? this.GetDefaultSearchFields(requestContext);
        return new WikiSearchPlatformRequest(searchOptions, requestId, indexInfo, queryParseTree, searchFilters, scopeFiltersExpression, searchQuery, highlightFields, searchFields, fields);
      }

      private IReadOnlyList<string> GetDefaultHighlightFields() => (IReadOnlyList<string>) new List<string>()
      {
        "fileNames",
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "fileNames", (object) "lower")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "fileNames", (object) "pattern")),
        "content",
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "content", (object) "pattern")),
        "tags"
      };

      private IReadOnlyList<string> GetDefaultFields() => (IReadOnlyList<string>) new string[14]
      {
        "collectionName",
        "projectId",
        "projectName",
        "repoName",
        "repositoryId",
        "wikiName",
        "wikiId",
        "branchName",
        "mappedPath",
        "filePath",
        "contentId",
        "lastUpdated",
        "collectionUrl",
        "projectVisibility"
      };

      private IReadOnlyList<string> GetDefaultSearchFields(IVssRequestContext requestContext)
      {
        int configValueOrDefault1 = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiDocumentTitleBoostValue", 10);
        int configValueOrDefault2 = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiDocumentContentBoostValue", 1);
        double configValueOrDefault3 = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiStemmedFieldBoostFractionValue", 0.9);
        return (IReadOnlyList<string>) new List<string>()
        {
          FormattableString.Invariant(FormattableStringFactory.Create("{0}^{1}", (object) "fileNames", (object) ((double) configValueOrDefault1 * configValueOrDefault3))),
          FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "fileNames", (object) "unstemmed", (object) configValueOrDefault1)),
          FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "fileNames", (object) "lower", (object) configValueOrDefault1)),
          FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "fileNames", (object) "pattern", (object) configValueOrDefault1)),
          "tags",
          FormattableString.Invariant(FormattableStringFactory.Create("{0}^{1}", (object) "content", (object) ((double) configValueOrDefault2 * configValueOrDefault3))),
          FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "content", (object) "unstemmed", (object) configValueOrDefault2)),
          FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "content", (object) "pattern", (object) configValueOrDefault2)),
          "contentLinks"
        };
      }
    }
  }
}

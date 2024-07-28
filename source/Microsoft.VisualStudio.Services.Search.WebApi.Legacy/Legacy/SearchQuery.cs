// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchQuery
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  [DataContract]
  public class SearchQuery : EntitySearchQuery
  {
    private static HashSet<string> s_sortSupportedFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "path",
      "fileName"
    };
    private static readonly HashSet<string> s_validFilters = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ProjectFilters",
      "RepositoryFilters",
      "PathFilters",
      "BranchFilters",
      "CodeElementFilters"
    };
    public static readonly IReadOnlyDictionary<string, string> ParentOf = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ["PathFilters"] = "RepositoryFilters",
      ["RepositoryFilters"] = "ProjectFilters"
    };

    public override void ValidateQuery()
    {
      base.ValidateQuery();
      this.ValidateFilterHierarchy(SearchQuery.ParentOf);
      if (this.SortOptions == null)
        return;
      foreach (EntitySortOption sortOption in this.SortOptions)
      {
        if (!SearchQuery.s_sortSupportedFields.Contains(sortOption.Field))
          throw new InvalidQueryException("Sorting is not supported on field: " + sortOption.Field);
      }
    }

    public override bool IsSupportedFilter(string filterName) => SearchQuery.s_validFilters.Contains(filterName);

    public SearchQuery Clone()
    {
      SearchQuery searchQuery = new SearchQuery();
      searchQuery.SearchText = this.SearchText;
      searchQuery.SkipResults = this.SkipResults;
      searchQuery.TakeResults = this.TakeResults;
      searchQuery.Filters = this.Filters.Clone<SearchFilter>();
      searchQuery.SearchFilters = this.SearchFilters.Clone();
      searchQuery.SortOptions = this.SortOptions.Clone<EntitySortOption>();
      searchQuery.SummarizedHitCountsNeeded = this.SummarizedHitCountsNeeded;
      searchQuery.IsInstantSearch = this.IsInstantSearch;
      return searchQuery;
    }
  }
}

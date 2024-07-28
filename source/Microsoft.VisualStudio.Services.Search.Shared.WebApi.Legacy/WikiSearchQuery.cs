// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.WikiSearchQuery
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class WikiSearchQuery : EntitySearchQuery
  {
    private static HashSet<string> s_sortSupportedFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      "lastUpdated"
    };
    private static readonly HashSet<string> s_validFilters = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ProjectFilters",
      "CollectionFilters",
      "Wiki"
    };
    public static readonly IReadOnlyDictionary<string, string> ParentOf = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ["ProjectFilters"] = "CollectionFilters"
    };

    public override void ValidateQuery() => base.ValidateQuery();

    public void ValidateFilterHierarchy() => this.ValidateFilterHierarchy(WikiSearchQuery.ParentOf);

    public override bool IsSupportedFilter(string filterName) => WikiSearchQuery.s_validFilters.Contains(filterName);

    public WikiSearchQuery Clone()
    {
      WikiSearchQuery wikiSearchQuery = new WikiSearchQuery();
      wikiSearchQuery.SearchText = this.SearchText;
      wikiSearchQuery.SkipResults = this.SkipResults;
      wikiSearchQuery.TakeResults = this.TakeResults;
      wikiSearchQuery.Filters = this.Filters.Clone<SearchFilter>();
      wikiSearchQuery.SearchFilters = this.SearchFilters.Clone();
      wikiSearchQuery.SortOptions = this.SortOptions.Clone<EntitySortOption>();
      return wikiSearchQuery;
    }
  }
}

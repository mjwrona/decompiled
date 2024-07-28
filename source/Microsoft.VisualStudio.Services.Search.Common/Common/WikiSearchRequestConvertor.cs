// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.WikiSearchRequestConvertor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class WikiSearchRequestConvertor
  {
    private static readonly IDictionary<string, string> s_filterKeyMapping = (IDictionary<string, string>) new FriendlyDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Project",
        "ProjectFilters"
      },
      {
        "Collection",
        "CollectionFilters"
      },
      {
        "Wiki",
        "Wiki"
      }
    };

    public static WikiSearchQuery ToOldRequestContract(this WikiSearchRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      WikiSearchQuery oldRequestContract = new WikiSearchQuery();
      oldRequestContract.SearchText = request.SearchText;
      oldRequestContract.SkipResults = request.Skip;
      oldRequestContract.TakeResults = request.Top;
      IDictionary<string, IEnumerable<string>> filters = request.Filters;
      oldRequestContract.SearchFilters = filters != null ? (IDictionary<string, IEnumerable<string>>) filters.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (filter => WikiSearchRequestConvertor.s_filterKeyMapping[filter.Key]), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (filter => filter.Value)) : (IDictionary<string, IEnumerable<string>>) null;
      IEnumerable<SortOption> orderBy = request.OrderBy;
      oldRequestContract.SortOptions = orderBy != null ? (IEnumerable<EntitySortOption>) orderBy.Select<SortOption, EntitySortOption>((Func<SortOption, EntitySortOption>) (x => new EntitySortOption()
      {
        Field = x.Field,
        SortOrderStr = x.SortOrderStr
      })).ToList<EntitySortOption>() : (IEnumerable<EntitySortOption>) null;
      oldRequestContract.SummarizedHitCountsNeeded = request.IncludeFacets;
      return oldRequestContract;
    }
  }
}

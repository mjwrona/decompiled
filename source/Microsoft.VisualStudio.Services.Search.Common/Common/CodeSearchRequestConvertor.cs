// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CodeSearchRequestConvertor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class CodeSearchRequestConvertor
  {
    internal static readonly IDictionary<string, string> FilterKeyMapping = (IDictionary<string, string>) new FriendlyDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Project",
        "ProjectFilters"
      },
      {
        "Repository",
        "RepositoryFilters"
      },
      {
        "Path",
        "PathFilters"
      },
      {
        "Branch",
        "BranchFilters"
      },
      {
        "CodeElement",
        "CodeElementFilters"
      }
    };

    public static SearchQuery ToOldRequestContract(this CodeSearchRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      SearchQuery oldRequestContract = new SearchQuery();
      oldRequestContract.SearchText = request.SearchText;
      oldRequestContract.SkipResults = request.Skip;
      oldRequestContract.TakeResults = request.Top;
      oldRequestContract.SearchFilters = CodeSearchRequestConvertor.UpdateFilterMap(request.Filters);
      IEnumerable<SortOption> orderBy = request.OrderBy;
      oldRequestContract.SortOptions = orderBy != null ? (IEnumerable<EntitySortOption>) orderBy.Select<SortOption, EntitySortOption>((Func<SortOption, EntitySortOption>) (x => new EntitySortOption()
      {
        Field = x.Field,
        SortOrderStr = x.SortOrderStr
      })).ToList<EntitySortOption>() : (IEnumerable<EntitySortOption>) null;
      oldRequestContract.SummarizedHitCountsNeeded = request.IncludeFacets;
      return oldRequestContract;
    }

    public static SearchQuery ToOldRequestContract(this ScrollSearchRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      SearchQuery oldRequestContract = new SearchQuery();
      oldRequestContract.SearchText = request.SearchText;
      oldRequestContract.SearchFilters = CodeSearchRequestConvertor.UpdateFilterMap(request.Filters);
      return oldRequestContract;
    }

    public static IDictionary<string, IEnumerable<string>> UpdateFilterMap(
      IDictionary<string, IEnumerable<string>> filters)
    {
      return filters == null ? (IDictionary<string, IEnumerable<string>>) null : (IDictionary<string, IEnumerable<string>>) filters.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (filter => CodeSearchRequestConvertor.FilterKeyMapping[filter.Key]), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (filter => filter.Value));
    }
  }
}

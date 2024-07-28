// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.WorkItemSearchRequestConvertor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class WorkItemSearchRequestConvertor
  {
    private static readonly IDictionary<string, string> s_filterKeyMapping = (IDictionary<string, string>) new FriendlyDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.Project,
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project
      },
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.WorkItemAreaPath,
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAreaPath
      },
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.WorkItemAssignedTo,
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo
      },
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.WorkItemState,
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState
      },
      {
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.Constants.FacetNames.WorkItemType,
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType
      }
    };

    public static Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchRequest ToOldRequestContract(
      this Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchRequest oldRequestContract = new Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchRequest();
      oldRequestContract.SearchText = request.SearchText;
      oldRequestContract.SkipResults = request.Skip;
      oldRequestContract.TakeResults = request.Top;
      IDictionary<string, IEnumerable<string>> filters = request.Filters;
      oldRequestContract.SearchFilters = filters != null ? (IDictionary<string, IEnumerable<string>>) filters.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (filter => WorkItemSearchRequestConvertor.s_filterKeyMapping[filter.Key]), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (filter => filter.Value)) : (IDictionary<string, IEnumerable<string>>) null;
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

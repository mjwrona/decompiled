// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchRequest
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem
{
  [DataContract]
  public class WorkItemSearchRequest : EntitySearchQuery
  {
    private static HashSet<string> s_sortSupportedFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "system.id",
      "system.title",
      "system.createddate",
      "system.changeddate",
      "system.assignedto",
      "system.state",
      "system.tags",
      "system.workitemtype"
    };
    private static readonly HashSet<string> s_validFilters = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      Constants.FilterCategories.Project,
      Constants.FilterCategories.WorkItemAreaPath,
      Constants.FilterCategories.WorkItemAssignedTo,
      Constants.FilterCategories.WorkItemState,
      Constants.FilterCategories.WorkItemType
    };

    public override bool IsSupportedFilter(string filterName) => WorkItemSearchRequest.s_validFilters.Contains(filterName);

    public override void ValidateQuery()
    {
      base.ValidateQuery();
      if (this.SortOptions == null)
        return;
      foreach (EntitySortOption sortOption in this.SortOptions)
      {
        if (!WorkItemSearchRequest.s_sortSupportedFields.Contains(sortOption.Field))
          throw new InvalidQueryException("Sorting is not supported on field: " + sortOption.Field);
      }
    }
  }
}

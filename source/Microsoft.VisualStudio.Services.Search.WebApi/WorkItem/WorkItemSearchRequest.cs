// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchRequest
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem
{
  [DataContract]
  public class WorkItemSearchRequest : EntitySearchRequest
  {
    private static readonly HashSet<string> s_sortSupportedFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "System.Id",
      "System.Title",
      "System.CreatedDate",
      "System.ChangedDate",
      "System.AssignedTo",
      "System.State",
      "System.Tags",
      "System.WorkItemType"
    };
    private static readonly HashSet<string> s_validFilters = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      Constants.FacetNames.Project,
      Constants.FacetNames.WorkItemAreaPath,
      Constants.FacetNames.WorkItemAssignedTo,
      Constants.FacetNames.WorkItemState,
      Constants.FacetNames.WorkItemType
    };

    public override bool IsSupportedFilter(string filterName) => WorkItemSearchRequest.s_validFilters.Contains(filterName);

    public override void ValidateQuery()
    {
      base.ValidateQuery();
      if (this.OrderBy == null)
        return;
      foreach (SortOption sortOption in this.OrderBy)
      {
        if (!WorkItemSearchRequest.s_sortSupportedFields.Contains(sortOption.Field))
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.SortingOnFieldNotSupportedMessageFormat, (object) sortOption.Field));
      }
    }
  }
}

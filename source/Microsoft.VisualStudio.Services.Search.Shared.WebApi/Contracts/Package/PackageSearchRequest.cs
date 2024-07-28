// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package.PackageSearchRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package
{
  [DataContract]
  public class PackageSearchRequest : EntitySearchRequest
  {
    private static readonly HashSet<string> s_validFilters = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      PackageSearchFilterCategories.Feeds,
      PackageSearchFilterCategories.View,
      PackageSearchFilterCategories.ProtocolType,
      PackageSearchFilterCategories.Collections
    };
    private static HashSet<string> s_sortSupportedFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      PackageSearchSortOptionSupportedFields.ProtocolType
    };

    public override bool IsSupportedFilter(string filterName) => PackageSearchRequest.s_validFilters.Contains(filterName);

    public override void ValidateQuery()
    {
      base.ValidateQuery();
      if (this.OrderBy == null)
        return;
      foreach (SortOption sortOption in this.OrderBy)
      {
        if (!PackageSearchRequest.s_sortSupportedFields.Contains(sortOption.Field))
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.SortingOnFieldNotSupportedMessageFormat, (object) sortOption.Field));
      }
    }
  }
}

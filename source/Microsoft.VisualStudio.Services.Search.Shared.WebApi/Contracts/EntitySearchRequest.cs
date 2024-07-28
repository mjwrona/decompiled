// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.EntitySearchRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public abstract class EntitySearchRequest : EntitySearchRequestBase
  {
    [DataMember(Name = "$skip")]
    public int Skip { get; set; }

    [DataMember(Name = "$top")]
    public int Top { get; set; }

    [DataMember(Name = "$orderBy", IsRequired = false)]
    public IEnumerable<SortOption> OrderBy { get; set; }

    [DataMember(Name = "includeFacets", IsRequired = false)]
    public bool IncludeFacets { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      SearchSecuredV2Object.SetSecuredObject((IEnumerable<SearchSecuredV2Object>) this.OrderBy, namespaceId, requiredPermissions, token);
    }

    public override void ValidateQuery()
    {
      base.ValidateQuery();
      this.ValidateQueryInternal();
    }

    public override void ValidateQuery(IReadOnlyDictionary<string, string> ParentOf)
    {
      this.ValidateQueryInternal();
      base.ValidateQuery(ParentOf);
    }

    private void ValidateQueryInternal()
    {
      if (this.OrderBy == null)
        return;
      HashSet<SortOption> sortOptionSet = new HashSet<SortOption>();
      foreach (SortOption sortOption in this.OrderBy)
      {
        if (sortOption == null)
          throw new InvalidQueryException(SearchSharedWebApiResources.EmptySortOptionInOrderByMessage);
        if (string.IsNullOrWhiteSpace(sortOption.Field))
          throw new InvalidQueryException(SearchSharedWebApiResources.SortOptionFieldNameShouldNotBeEmptyOrNullMessage);
        if (sortOption.SortOrder == SortOrder.Undefined)
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.EmptySortOrderGivenInSortOptionsMessageFormat, (object) sortOption.Field));
        if (!sortOptionSet.Add(sortOption))
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.FieldSpecifiedMoreThanOnceInTheSortOptionsMessageFormat, (object) sortOption.Field));
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySearchParameters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentitySearchParameters
  {
    public string Query;
    public IdentitySearchKind SearchKind;
    public IdentitySearchType IdentityTypes;
    public IEnumerable<IdentityDescriptor> FilterByAncestors;
    public IEnumerable<IdentityDescriptor> FilterByIdentities;
    public IEnumerable<string> PropertyNameFilters;
    public int PageSize;
    public string PagingContext;
    public Guid ScopeId;

    public IdentitySearchParameters(
      string query,
      IdentitySearchKind searchKind,
      IdentitySearchType identityTypes,
      IEnumerable<string> propertyNameFilters,
      int pageSize,
      string pagingContext)
    {
      this.Query = query;
      this.SearchKind = searchKind;
      this.IdentityTypes = identityTypes;
      this.PropertyNameFilters = propertyNameFilters;
      this.PageSize = pageSize;
      this.PagingContext = pagingContext;
    }

    public IdentitySearchParameters(
      string query,
      IdentitySearchKind searchKind,
      IdentitySearchType identityTypes,
      IEnumerable<string> propertyNameFilters,
      int pageSize,
      string pagingContext,
      IEnumerable<IdentityDescriptor> filterByAncestors,
      IEnumerable<IdentityDescriptor> filterByIdentities)
      : this(query, searchKind, identityTypes, propertyNameFilters, pageSize, pagingContext)
    {
      this.FilterByAncestors = filterByAncestors != null ? (IEnumerable<IdentityDescriptor>) filterByAncestors.ToList<IdentityDescriptor>() : (IEnumerable<IdentityDescriptor>) null;
      this.FilterByIdentities = filterByIdentities != null ? (IEnumerable<IdentityDescriptor>) filterByIdentities.ToList<IdentityDescriptor>() : (IEnumerable<IdentityDescriptor>) null;
    }
  }
}

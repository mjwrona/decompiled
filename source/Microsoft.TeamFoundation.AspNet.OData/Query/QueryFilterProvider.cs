// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.QueryFilterProvider
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.AspNet.OData.Query
{
  public class QueryFilterProvider : IFilterProvider
  {
    public QueryFilterProvider(IActionFilter queryFilter) => this.QueryFilter = queryFilter != null ? queryFilter : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryFilter));

    public IActionFilter QueryFilter { get; private set; }

    public IEnumerable<FilterInfo> GetFilters(
      HttpConfiguration configuration,
      HttpActionDescriptor actionDescriptor)
    {
      if (actionDescriptor == null || !TypeHelper.IsIQueryable(actionDescriptor.ReturnType) && !typeof (SingleResult).IsAssignableFrom(actionDescriptor.ReturnType) || actionDescriptor.GetParameters().Any<HttpParameterDescriptor>((Func<HttpParameterDescriptor, bool>) (parameter => typeof (ODataQueryOptions).IsAssignableFrom(parameter.ParameterType))))
        return Enumerable.Empty<FilterInfo>();
      return (IEnumerable<FilterInfo>) new FilterInfo[1]
      {
        new FilterInfo((IFilter) this.QueryFilter, FilterScope.Global)
      };
    }
  }
}

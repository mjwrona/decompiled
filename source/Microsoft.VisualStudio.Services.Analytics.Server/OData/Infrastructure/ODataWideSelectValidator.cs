// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataWideSelectValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataWideSelectValidator : IODataValidator
  {
    public const int MaxNumberOfColumns = 800;

    public Action WarningCallback { get; set; }

    public string RequiredFeatureFlag => "Analytics.OData.BlockWideSelectExpand";

    public void Validate(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
      ApplyQueryOption apply = queryOptions.Apply;
      if ((apply != null ? (apply.ApplyClause.IsAggregated() ? 1 : 0) : 0) != 0)
        return;
      SelectExpandClause selectExpandClause = queryOptions.SelectExpand?.SelectExpandClause;
      ComputeClause computeClause = queryOptions.Compute?.ComputeClause;
      IEdmEntityType elementType = (IEdmEntityType) queryOptions.Context.ElementType;
      ComputeClause compute = computeClause;
      IEdmEntityType entityType = elementType;
      int selectedPropertiesCount = ODataWideSelectValidator.GetSelectedPropertiesCount(selectExpandClause, compute, entityType);
      if (selectedPropertiesCount > 800)
        throw new ODataException(AnalyticsResources.ODATA_QUERY_SELECT_EXPAND_TOO_WIDE((object) selectedPropertiesCount, (object) 800));
    }

    private static int GetSelectedPropertiesCount(
      SelectExpandClause select,
      ComputeClause compute,
      IEdmEntityType entityType)
    {
      int selectedPropertiesCount = 0;
      if (select == null || select.AllSelected)
      {
        selectedPropertiesCount += entityType.Properties().Count<IEdmProperty>();
        if (compute != null)
          selectedPropertiesCount += compute.ComputedItems.Count<ComputeExpression>();
      }
      if (select != null)
      {
        selectedPropertiesCount += select.SelectedItems.Count<SelectItem>();
        foreach (ExpandedNavigationSelectItem navigationSelectItem in select.SelectedItems.OfType<ExpandedNavigationSelectItem>())
        {
          if (!navigationSelectItem.ApplyOption.IsAggregated())
            selectedPropertiesCount += ODataWideSelectValidator.GetSelectedPropertiesCount(navigationSelectItem.SelectAndExpand, navigationSelectItem.ComputeOption, navigationSelectItem.NavigationSource.EntityType());
        }
      }
      return selectedPropertiesCount;
    }
  }
}

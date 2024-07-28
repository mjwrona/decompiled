// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataWorkItemDescendantsExpandValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataWorkItemDescendantsExpandValidator : IODataValidator
  {
    public const int MaxNumberOfColumns = 16;

    public Action WarningCallback { get; set; }

    public string RequiredFeatureFlag => (string) null;

    public void Validate(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
      if (!(queryOptions.Context.ElementClrType == typeof (WorkItem)) || queryOptions.SelectExpand == null)
        return;
      List<ExpandedNavigationSelectItem> list = queryOptions.SelectExpand.SelectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Where<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (e => e.NavigationSource.EntityType().Name == "WorkItem" && e.PathToNavigationProperty.LastSegment.Identifier == "Descendants")).ToList<ExpandedNavigationSelectItem>();
      if (!list.Any<ExpandedNavigationSelectItem>())
        return;
      int num = 0;
      foreach (ExpandedNavigationSelectItem navigationSelectItem in list)
      {
        if (!navigationSelectItem.ApplyOption.IsAggregated())
        {
          if (navigationSelectItem.SelectAndExpand.AllSelected)
            num += navigationSelectItem.NavigationSource.EntityType().Properties().Count<IEdmProperty>();
          else
            num += navigationSelectItem.SelectAndExpand.SelectedItems.Count<SelectItem>();
        }
      }
      if (num > 16)
        throw new ODataException(AnalyticsResources.ODATA_QUERY_WIT_DESCENDANTS_TOO_WIDE((object) num, (object) 16));
    }
  }
}

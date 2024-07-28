// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQueryParentDepthValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataQueryParentDepthValidator : IODataValidator
  {
    public Action WarningCallback { get; set; }

    public string RequiredFeatureFlag => (string) null;

    public void Validate(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
      if (queryOptions.SelectExpand != null && queryOptions.SelectExpand.SelectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Where<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (e => e.NavigationSource.EntityType().Name == "WorkItem" && e.SelectAndExpand != null)).SelectMany<ExpandedNavigationSelectItem, SelectItem>((Func<ExpandedNavigationSelectItem, IEnumerable<SelectItem>>) (e => e.SelectAndExpand.SelectedItems)).OfType<ExpandedNavigationSelectItem>().Any<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (e => e.NavigationSource.EntityType().Name == "WorkItem")))
        throw new ODataException(AnalyticsResources.ODATA_QUERY_PARENT_EXPAND_TOO_DEEP());
    }
  }
}

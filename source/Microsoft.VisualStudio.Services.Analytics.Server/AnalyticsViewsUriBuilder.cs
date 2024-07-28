// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsUriBuilder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsViewsUriBuilder
  {
    public static string GetViewUrl(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      Guid viewId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary[nameof (viewId)] = (object) viewId.ToString();
      if (viewScope.Type == AnalyticsViewScopeType.Project)
        dictionary["project"] = (object) viewScope.Name;
      IVssRequestContext requestContext1 = requestContext;
      Guid analyticsViewsResourceId = ResourceIds.AnalyticsViewsResourceId;
      Dictionary<string, object> routeValues = dictionary;
      return service.GetResourceUri(requestContext1, "AnalyticsViews", analyticsViewsResourceId, (object) routeValues).AbsoluteUri;
    }

    public static string GetEndpointUrl(
      IVssRequestContext requestContext,
      string endpointVersion,
      string entitySet)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      var data = new
      {
        oDataVersion = endpointVersion,
        entityType = entitySet
      };
      IVssRequestContext requestContext1 = requestContext;
      Guid analyticsResourceId = ResourceIds.AnalyticsResourceId;
      var routeValues = data;
      return service.GetResourceUri(requestContext1, "Analytics", analyticsResourceId, (object) routeValues).AbsoluteUri;
    }

    public static string GetODataUrl(
      IVssRequestContext requestContext,
      string endpointVersion,
      string entitySet,
      string oDataQuery)
    {
      return new UriBuilder(AnalyticsViewsUriBuilder.GetEndpointUrl(requestContext, endpointVersion, entitySet))
      {
        Query = oDataQuery
      }.Uri.AbsoluteUri;
    }
  }
}

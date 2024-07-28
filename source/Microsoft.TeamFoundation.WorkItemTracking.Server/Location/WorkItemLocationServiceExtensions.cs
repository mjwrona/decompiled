// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Location.WorkItemLocationServiceExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Location
{
  public static class WorkItemLocationServiceExtensions
  {
    public static Uri GetResourceUri(
      this ILocationService locationService,
      IVssRequestContext requestContext,
      string areaName,
      Guid locationId,
      Guid projectId,
      Guid teamId,
      object routeValues,
      bool requireExplicitRouteParams,
      Guid serviceOwner = default (Guid))
    {
      Dictionary<string, object> routeDictionary = WorkItemLocationServiceExtensions.GetRouteDictionary(projectId, teamId, routeValues);
      return locationService.GetLocationData(requestContext, serviceOwner).GetResourceUri(requestContext, areaName, locationId, (object) routeDictionary, requireExplicitRouteParams);
    }

    public static string GetCoreResourceUriString(
      this ILocationService locationService,
      IVssRequestContext requestContext,
      Guid locationId,
      object routeValues)
    {
      return locationService.GetLocationData(requestContext, CoreConstants.AreaIdGuid).GetResourceUri(requestContext, "core", locationId, routeValues).ToString();
    }

    private static Dictionary<string, object> GetRouteDictionary(
      Guid projectId,
      Guid teamId,
      object routeValues)
    {
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues);
      if (projectId != Guid.Empty)
      {
        routeDictionary["project"] = (object) projectId;
        if (teamId != Guid.Empty)
          routeDictionary["team"] = (object) teamId;
      }
      return routeDictionary;
    }
  }
}

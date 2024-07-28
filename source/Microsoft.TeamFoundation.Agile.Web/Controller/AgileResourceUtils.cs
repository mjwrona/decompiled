// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.AgileResourceUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public class AgileResourceUtils
  {
    public static string GetAgileResourceUriString(
      IVssRequestContext requestContext,
      Guid locationId,
      Guid projectId,
      Guid teamId,
      object routeValues,
      bool requireExplicitRouteParams = false)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      Dictionary<string, object> routeDictionary = AgileResourceUtils.GetRouteDictionary(projectId, teamId, routeValues);
      IVssRequestContext requestContext1 = requestContext;
      Guid empty = Guid.Empty;
      return service.GetLocationData(requestContext1, empty).GetResourceUri(requestContext, "work", locationId, (object) routeDictionary, requireExplicitRouteParams).ToString();
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

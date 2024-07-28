// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.TfsRouteArea
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  public static class TfsRouteArea
  {
    public const string All = null;
    public const string Root = "";
    public const string Admin = "Admin";
    public const string Api = "Api";
    public const string Public = "Public";
    private static HashSet<string> sm_areas = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    internal static HashSet<string> Areas => TfsRouteArea.sm_areas;

    internal static string GetRouteArea(this RouteData routeData) => routeData.GetRouteValue<string>("routeArea", "");

    internal static string GetRouteArea(this Route route) => route is TfsRoute ? ((TfsRoute) route).RouteArea : route.DataTokens.GetValue<string>("routeArea", "");

    internal static void SetRouteArea(this Route route, string area)
    {
      route.DataTokens["routeArea"] = (object) area;
      if (!(route is TfsRoute))
        return;
      ((TfsRoute) route).RouteArea = area;
    }

    internal static void RegisterRouteArea(string area)
    {
      ArgumentUtility.CheckForNull<string>(area, nameof (area));
      if (TfsRouteArea.sm_areas.Contains(area))
        throw new InvalidOperationException(WACommonResources.ErrorDuplicateRouteArea.FormatInvariant((object) area));
      TfsRouteArea.sm_areas.Add(area);
    }

    public static string GetControllerArea(string controllerName)
    {
      string controllerArea = "";
      if (string.IsNullOrEmpty(controllerName))
        return controllerArea;
      foreach (string area in TfsRouteArea.Areas)
      {
        if (!string.IsNullOrEmpty(area) && controllerName.StartsWith(area, StringComparison.Ordinal))
        {
          controllerArea = area;
          break;
        }
      }
      return controllerArea;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.TfsRouteAreaConstraint
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  public class TfsRouteAreaConstraint : IRouteConstraint
  {
    private static readonly Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData> sm_nullAreaRouteConstraintTable = new Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData>> sm_routeConstraintTable = new Dictionary<string, Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private static bool ValidTfsName(string name)
    {
      switch (name[0])
      {
        case '.':
        case '_':
          return false;
        default:
          return true;
      }
    }

    public bool Match(
      HttpContextBase httpContext,
      Route route,
      string parameterName,
      RouteValueDictionary values,
      RouteDirection routeDirection)
    {
      string routeArea = route.GetRouteArea();
      string controllerName = values.GetValue<string>("controller", (string) null);
      if (!string.IsNullOrEmpty(controllerName))
      {
        if (TfsRouteArea.Areas.Contains(controllerName))
          return false;
        foreach (string area in TfsRouteArea.Areas)
        {
          if (!string.IsNullOrEmpty(area) && !StringComparer.OrdinalIgnoreCase.Equals(routeArea, area) && controllerName.StartsWith(area, StringComparison.Ordinal))
            return false;
        }
      }
      NavigationContextLevels levels = NavigationContextLevels.None;
      IVssRequestContext requestContext = httpContext.TfsRequestContext();
      if (requestContext == null)
        return false;
      TeamFoundationHostType foundationHostType;
      if (routeDirection == RouteDirection.IncomingRequest)
      {
        foundationHostType = requestContext.IntendedHostType();
      }
      else
      {
        if (!(values.GetValue<object>("serviceHost") is TfsServiceHostDescriptor serviceHostDescriptor))
          return false;
        foundationHostType = requestContext.IntendedHostType(serviceHostDescriptor.HostType);
      }
      if ((foundationHostType & TeamFoundationHostType.ProjectCollection) == TeamFoundationHostType.ProjectCollection)
        levels = NavigationContextLevels.Collection;
      else if ((foundationHostType & TeamFoundationHostType.Application) == TeamFoundationHostType.Application)
        levels = NavigationContextLevels.Application;
      else if ((foundationHostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        levels = NavigationContextLevels.Deployment;
      if (levels == NavigationContextLevels.None)
        return false;
      string name1 = values.GetValue<string>("project", (string) null);
      if (!string.IsNullOrEmpty(name1))
      {
        if (levels != NavigationContextLevels.Collection || !TfsRouteAreaConstraint.ValidTfsName(name1))
          return false;
        levels = NavigationContextLevels.Project;
      }
      string name2 = values.GetValue<string>("team", (string) null);
      if (!string.IsNullOrEmpty(name2))
      {
        if (levels != NavigationContextLevels.Project || !TfsRouteAreaConstraint.ValidTfsName(name2))
          return false;
        levels = NavigationContextLevels.Team;
      }
      return string.IsNullOrEmpty(controllerName) || TfsRouteAreaConstraint.IsControllerSupported(routeArea, controllerName, levels);
    }

    public IReadOnlyDictionary<string, TfsRouteAreaConstraint.RouteConstraintData> GetControllersForRouteArea(
      string routeArea)
    {
      Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData> dictionary;
      return TfsRouteAreaConstraint.sm_routeConstraintTable.TryGetValue(routeArea, out dictionary) ? (IReadOnlyDictionary<string, TfsRouteAreaConstraint.RouteConstraintData>) dictionary : (IReadOnlyDictionary<string, TfsRouteAreaConstraint.RouteConstraintData>) new Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData>();
    }

    internal static void RegisterControllerRouteAreas(IEnumerable<Type> controllerTypes)
    {
      foreach (Type controllerType in controllerTypes)
      {
        object[] customAttributes = controllerType.GetCustomAttributes(typeof (SupportedRouteAreaAttribute), false);
        if (customAttributes.Length != 0)
        {
          string controllerName = controllerType.Name.Substring(0, controllerType.Name.Length - "controller".Length);
          foreach (SupportedRouteAreaAttribute routeAreaAttribute in customAttributes)
            TfsRouteAreaConstraint.RegisterController(controllerName, routeAreaAttribute.Area, routeAreaAttribute.NavigationLevels);
        }
      }
    }

    private static void RegisterController(
      string controllerName,
      string routeArea,
      NavigationContextLevels navigationContextLevels)
    {
      string key = controllerName;
      if (!string.IsNullOrEmpty(routeArea) && controllerName.StartsWith(routeArea))
        key = controllerName.Substring(routeArea.Length);
      Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData> dictionary;
      if (routeArea == null)
        dictionary = TfsRouteAreaConstraint.sm_nullAreaRouteConstraintTable;
      else if (!TfsRouteAreaConstraint.sm_routeConstraintTable.TryGetValue(routeArea, out dictionary))
      {
        dictionary = new Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        TfsRouteAreaConstraint.sm_routeConstraintTable[routeArea] = dictionary;
      }
      dictionary[key] = new TfsRouteAreaConstraint.RouteConstraintData()
      {
        ControllerName = controllerName,
        Levels = navigationContextLevels
      };
    }

    internal static bool IsControllerSupported(
      string routeArea,
      string controllerName,
      NavigationContextLevels levels)
    {
      return (TfsRouteAreaConstraint.GetSupportedLevels(routeArea, controllerName) & levels) != 0;
    }

    internal static NavigationContextLevels GetSupportedLevels(
      string routeArea,
      string controllerName)
    {
      return TfsRouteAreaConstraint.GetSupportedLevels(routeArea, controllerName, out string _);
    }

    internal static NavigationContextLevels GetSupportedLevels(
      string routeArea,
      string controllerName,
      out string actualControllerName)
    {
      NavigationContextLevels supportedLevels = NavigationContextLevels.None;
      actualControllerName = (string) null;
      TfsRouteAreaConstraint.RouteConstraintData routeConstraintData;
      if (TfsRouteAreaConstraint.sm_nullAreaRouteConstraintTable.TryGetValue(controllerName, out routeConstraintData))
      {
        supportedLevels = routeConstraintData.Levels;
        actualControllerName = routeConstraintData.ControllerName;
      }
      Dictionary<string, TfsRouteAreaConstraint.RouteConstraintData> dictionary;
      if (routeArea != null && TfsRouteAreaConstraint.sm_routeConstraintTable.TryGetValue(routeArea, out dictionary) && dictionary.TryGetValue(controllerName, out routeConstraintData))
      {
        supportedLevels |= routeConstraintData.Levels;
        actualControllerName = routeConstraintData.ControllerName;
      }
      return supportedLevels;
    }

    public class RouteConstraintData
    {
      public string ControllerName;
      public NavigationContextLevels Levels;
    }
  }
}

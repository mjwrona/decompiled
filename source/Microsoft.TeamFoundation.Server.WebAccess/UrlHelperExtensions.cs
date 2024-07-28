// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UrlHelperExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class UrlHelperExtensions
  {
    public static string ActionWithParameters(this UrlHelper urlHelper, string actionName) => urlHelper.ActionWithParameters(actionName, (string) null, (RouteValueDictionary) null, (string) null, (string) null);

    public static string ActionWithParameters(
      this UrlHelper urlHelper,
      string actionName,
      object routeValues)
    {
      return urlHelper.ActionWithParameters(actionName, (string) null, routeValues, (string) null);
    }

    public static string ActionWithParameters(
      this UrlHelper urlHelper,
      string actionName,
      RouteValueDictionary routeValues)
    {
      return urlHelper.ActionWithParameters(actionName, (string) null, routeValues, (string) null, (string) null);
    }

    public static string ActionWithParameters(
      this UrlHelper urlHelper,
      string actionName,
      string controllerName)
    {
      return urlHelper.ActionWithParameters(actionName, controllerName, (RouteValueDictionary) null, (string) null, (string) null);
    }

    public static string ActionWithParameters(
      this UrlHelper urlHelper,
      string actionName,
      string controllerName,
      object routeValues)
    {
      return urlHelper.ActionWithParameters(actionName, controllerName, routeValues, (string) null);
    }

    public static string ActionWithParameters(
      this UrlHelper urlHelper,
      string actionName,
      string controllerName,
      RouteValueDictionary routeValues)
    {
      return urlHelper.ActionWithParameters(actionName, controllerName, routeValues, (string) null, (string) null);
    }

    public static string ActionWithParameters(
      this UrlHelper urlHelper,
      string actionName,
      string controllerName,
      object routeValues,
      string protocol)
    {
      return urlHelper.ActionWithParameters(actionName, controllerName, new RouteValueDictionary(routeValues), protocol, (string) null);
    }

    public static string ActionWithParameters(
      this UrlHelper urlHelper,
      string actionName,
      string controllerName,
      RouteValueDictionary routeValues,
      string protocol,
      string hostName)
    {
      ArgumentUtility.CheckForNull<UrlHelper>(urlHelper, nameof (urlHelper));
      return UrlHelper.GenerateUrl(UrlHelperExtensions.GetParametersRouteName(routeValues, urlHelper.RequestContext), actionName, controllerName, protocol, hostName, (string) null, routeValues, urlHelper.RouteCollection, urlHelper.RequestContext, true);
    }

    public static string RouteUrl(
      this UrlHelper urlHelper,
      string routeName,
      string actionName,
      string controllerName,
      object routeValues)
    {
      return UrlHelperExtensions.RouteUrl(urlHelper, routeName, actionName, controllerName, new RouteValueDictionary(routeValues));
    }

    public static string RouteUrl(
      this UrlHelper urlHelper,
      string routeName,
      string actionName,
      string controllerName,
      RouteValueDictionary routeValues)
    {
      return UrlHelper.GenerateUrl(routeName, actionName, controllerName, (string) null, (string) null, (string) null, routeValues, urlHelper.RouteCollection, urlHelper.RequestContext, false) ?? urlHelper.Action(actionName, controllerName, routeValues);
    }

    public static string FragmentRouteUrl(
      this UrlHelper urlHelper,
      string routeName,
      string serverActionName,
      string serverControllerName,
      object serverRouteValues,
      string clientAction,
      object clientRouteValues)
    {
      return UrlHelperExtensions.FragmentRouteUrl(urlHelper, routeName, serverActionName, serverControllerName, new RouteValueDictionary(serverRouteValues), clientAction, clientRouteValues);
    }

    public static string FragmentRouteUrl(
      this UrlHelper urlHelper,
      string routeName,
      string serverActionName,
      string serverControllerName,
      RouteValueDictionary serverRouteValues,
      string clientAction,
      object clientRouteValues)
    {
      return UrlHelperExtensions.RouteUrl(urlHelper, routeName, serverActionName, serverControllerName, serverRouteValues) + urlHelper.FragmentAction(clientAction, clientRouteValues);
    }

    private static string GetParametersRouteName(
      RouteValueDictionary routeValues,
      RequestContext requestContext)
    {
      return (PlatformRouteHelpers.ExtractTargetRouteArea(requestContext, routeValues) ?? "") + UrlHelperExtensions.GetRouteLevel(routeValues, requestContext) + "ControllerActionWithParameters";
    }

    private static string GetRouteLevel(
      RouteValueDictionary routeValues,
      RequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<RequestContext>(requestContext, "requstContext");
      return string.IsNullOrEmpty(UrlHelperExtensions.TryGetRouteValueString("team", routeValues, requestContext)) ? (string.IsNullOrEmpty(UrlHelperExtensions.TryGetRouteValueString("project", routeValues, requestContext)) ? "ServiceHost" : "Project") : "Team";
    }

    private static string TryGetRouteValueString(
      string valueName,
      RouteValueDictionary routeValues,
      RequestContext requestContext)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(valueName, nameof (valueName));
      ArgumentUtility.CheckForNull<RequestContext>(requestContext, "requstContext");
      string routeValueString = (string) null;
      object obj;
      if (routeValues != null && routeValues.TryGetValue(valueName, out obj))
        routeValueString = obj as string;
      if (routeValueString == null && requestContext.RouteData.Values.TryGetValue(valueName, out obj))
        routeValueString = obj as string;
      return routeValueString;
    }
  }
}

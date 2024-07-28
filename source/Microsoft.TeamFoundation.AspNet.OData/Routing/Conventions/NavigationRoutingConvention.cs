// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.NavigationRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public class NavigationRoutingConvention : NavigationSourceRoutingConvention
  {
    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return NavigationRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      ODataRequestMethod method = controllerContext.Request.Method;
      string actionMethodPrefix = NavigationRoutingConvention.GetActionMethodPrefix(method);
      if (actionMethodPrefix == null)
        return (string) null;
      if (odataPath.PathTemplate == "~/entityset/key/navigation" || odataPath.PathTemplate == "~/entityset/key/navigation/$count" || odataPath.PathTemplate == "~/entityset/key/cast/navigation" || odataPath.PathTemplate == "~/entityset/key/cast/navigation/$count" || odataPath.PathTemplate == "~/singleton/navigation" || odataPath.PathTemplate == "~/singleton/navigation/$count" || odataPath.PathTemplate == "~/singleton/cast/navigation" || odataPath.PathTemplate == "~/singleton/cast/navigation/$count")
      {
        if (!(odataPath.Segments.Last<ODataPathSegment>() is NavigationPropertySegment navigationPropertySegment))
          navigationPropertySegment = odataPath.Segments[odataPath.Segments.Count - 2] as NavigationPropertySegment;
        IEdmNavigationProperty navigationProperty = navigationPropertySegment.NavigationProperty;
        IEdmEntityType declaringType = navigationProperty.DeclaringType as IEdmEntityType;
        if (navigationProperty.TargetMultiplicity() != EdmMultiplicity.Many && ODataRequestMethod.Post == method)
          return (string) null;
        if (navigationProperty.TargetMultiplicity() == EdmMultiplicity.Many && (ODataRequestMethod.Put == method || ODataRequestMethod.Patch == method))
          return (string) null;
        if (odataPath.Segments.Last<ODataPathSegment>() is CountSegment && method != ODataRequestMethod.Get)
          return (string) null;
        if (declaringType != null)
        {
          string matchingAction = actionMap.FindMatchingAction(actionMethodPrefix + navigationProperty.Name + "From" + declaringType.Name, actionMethodPrefix + navigationProperty.Name);
          if (matchingAction != null)
          {
            if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
            {
              KeySegment segment = (KeySegment) odataPath.Segments[1];
              controllerContext.AddKeyValueToRouteData(segment);
            }
            return matchingAction;
          }
        }
      }
      return (string) null;
    }

    private static string GetActionMethodPrefix(ODataRequestMethod method)
    {
      switch (method)
      {
        case ODataRequestMethod.Get:
          return "Get";
        case ODataRequestMethod.Patch:
          return "PatchTo";
        case ODataRequestMethod.Post:
          return "PostTo";
        case ODataRequestMethod.Put:
          return "PutTo";
        default:
          return (string) null;
      }
    }
  }
}

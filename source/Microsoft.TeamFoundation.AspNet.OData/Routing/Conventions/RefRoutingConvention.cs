// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.RefRoutingConvention
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
  public class RefRoutingConvention : NavigationSourceRoutingConvention
  {
    private const string DeleteRefActionNamePrefix = "DeleteRef";
    private const string CreateRefActionNamePrefix = "CreateRef";
    private const string GetRefActionNamePrefix = "GetRef";

    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return RefRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      ODataRequestMethod method = controllerContext.Request.Method;
      if (!RefRoutingConvention.IsSupportedRequestMethod(method))
        return (string) null;
      if (odataPath.PathTemplate == "~/entityset/key/navigation/$ref" || odataPath.PathTemplate == "~/entityset/key/cast/navigation/$ref" || odataPath.PathTemplate == "~/singleton/navigation/$ref" || odataPath.PathTemplate == "~/singleton/cast/navigation/$ref")
      {
        NavigationPropertyLinkSegment propertyLinkSegment = (NavigationPropertyLinkSegment) odataPath.Segments.Last<ODataPathSegment>();
        IEdmNavigationProperty navigationProperty = propertyLinkSegment.NavigationProperty;
        IEdmEntityType declaringType = navigationProperty.DeclaringEntityType();
        string refActionName = RefRoutingConvention.FindRefActionName(actionMap, navigationProperty, declaringType, method);
        if (refActionName != null)
        {
          if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
            controllerContext.AddKeyValueToRouteData((KeySegment) odataPath.Segments[1]);
          controllerContext.RouteData.Add(ODataRouteConstants.NavigationProperty, (object) propertyLinkSegment.NavigationProperty.Name);
          return refActionName;
        }
      }
      else if (ODataRequestMethod.Delete == method && (odataPath.PathTemplate == "~/entityset/key/navigation/key/$ref" || odataPath.PathTemplate == "~/entityset/key/cast/navigation/key/$ref" || odataPath.PathTemplate == "~/singleton/navigation/key/$ref" || odataPath.PathTemplate == "~/singleton/cast/navigation/key/$ref"))
      {
        NavigationPropertyLinkSegment segment = (NavigationPropertyLinkSegment) odataPath.Segments[odataPath.Segments.Count - 2];
        IEdmNavigationProperty navigationProperty = segment.NavigationProperty;
        IEdmEntityType declaringType = navigationProperty.DeclaringEntityType();
        string refActionName = RefRoutingConvention.FindRefActionName(actionMap, navigationProperty, declaringType, method);
        if (refActionName != null)
        {
          if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
            controllerContext.AddKeyValueToRouteData((KeySegment) odataPath.Segments[1]);
          controllerContext.RouteData.Add(ODataRouteConstants.NavigationProperty, (object) segment.NavigationProperty.Name);
          controllerContext.AddKeyValueToRouteData((KeySegment) odataPath.Segments.Last<ODataPathSegment>((Func<ODataPathSegment, bool>) (e => e is KeySegment)), ODataRouteConstants.RelatedKey);
          return refActionName;
        }
      }
      return (string) null;
    }

    private static string FindRefActionName(
      IWebApiActionMap actionMap,
      IEdmNavigationProperty navigationProperty,
      IEdmEntityType declaringType,
      ODataRequestMethod method)
    {
      string str;
      switch (method)
      {
        case ODataRequestMethod.Get:
          str = "GetRef";
          break;
        case ODataRequestMethod.Delete:
          str = "DeleteRef";
          break;
        default:
          str = "CreateRef";
          break;
      }
      return actionMap.FindMatchingAction(str + "To" + navigationProperty.Name + "From" + declaringType.Name, str + "To" + navigationProperty.Name, str);
    }

    private static bool IsSupportedRequestMethod(ODataRequestMethod method) => ODataRequestMethod.Delete == method || ODataRequestMethod.Put == method || ODataRequestMethod.Post == method || method == ODataRequestMethod.Get;
  }
}

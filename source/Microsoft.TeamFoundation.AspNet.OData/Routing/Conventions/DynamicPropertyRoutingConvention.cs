// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.DynamicPropertyRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public class DynamicPropertyRoutingConvention : NavigationSourceRoutingConvention
  {
    private const string ActionName = "DynamicProperty";

    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return DynamicPropertyRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      string str1 = (string) null;
      dynamicPathSegment2 = (DynamicPathSegment) null;
      switch (odataPath.PathTemplate)
      {
        case "~/entityset/key/cast/dynamicproperty":
        case "~/entityset/key/dynamicproperty":
        case "~/singleton/cast/dynamicproperty":
        case "~/singleton/dynamicproperty":
          if (!(odataPath.Segments.Last<ODataPathSegment>() is DynamicPathSegment dynamicPathSegment2))
            return (string) null;
          if (controllerContext.Request.Method == ODataRequestMethod.Get)
          {
            string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Get{0}", new object[1]
            {
              (object) "DynamicProperty"
            });
            str1 = actionMap.FindMatchingAction(str2);
            break;
          }
          break;
        case "~/entityset/key/cast/property/dynamicproperty":
        case "~/entityset/key/property/dynamicproperty":
        case "~/singleton/cast/property/dynamicproperty":
        case "~/singleton/property/dynamicproperty":
          if (!(odataPath.Segments.Last<ODataPathSegment>() is DynamicPathSegment dynamicPathSegment2))
            return (string) null;
          if (!(odataPath.Segments[odataPath.Segments.Count - 2] is PropertySegment segment1))
            return (string) null;
          if (!(segment1.Property.Type.Definition is EdmComplexType))
            return (string) null;
          if (controllerContext.Request.Method == ODataRequestMethod.Get)
          {
            string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Get{0}", new object[1]
            {
              (object) "DynamicProperty"
            });
            str1 = actionMap.FindMatchingAction(str3 + "From" + segment1.Property.Name);
            break;
          }
          break;
      }
      if (str1 == null)
        return (string) null;
      if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
      {
        KeySegment segment2 = (KeySegment) odataPath.Segments[1];
        controllerContext.AddKeyValueToRouteData(segment2);
      }
      controllerContext.RouteData.Add(ODataRouteConstants.DynamicProperty, (object) dynamicPathSegment2.Identifier);
      string key = "DF908045-6922-46A0-82F2-2F6E7F43D1B1_" + ODataRouteConstants.DynamicProperty;
      ODataParameterValue odataParameterValue = new ODataParameterValue((object) dynamicPathSegment2.Identifier, (IEdmTypeReference) EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(typeof (string)));
      controllerContext.RouteData.Add(key, (object) odataParameterValue);
      controllerContext.Request.Context.RoutingConventionsStore.Add(key, (object) odataParameterValue);
      return str1;
    }
  }
}

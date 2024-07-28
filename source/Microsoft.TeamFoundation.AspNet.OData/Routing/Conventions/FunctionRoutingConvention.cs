// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.FunctionRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public class FunctionRoutingConvention : NavigationSourceRoutingConvention
  {
    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return FunctionRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      if (controllerContext.Request.Method == ODataRequestMethod.Get)
      {
        string str = (string) null;
        OperationSegment operationSegment = (OperationSegment) null;
        switch (odataPath.PathTemplate)
        {
          case "~/entityset/cast/function":
          case "~/entityset/function":
            operationSegment = odataPath.Segments.Last<ODataPathSegment>() as OperationSegment;
            str = FunctionRoutingConvention.GetFunction(operationSegment).SelectAction(actionMap, true);
            break;
          case "~/entityset/cast/function/$count":
          case "~/entityset/function/$count":
            operationSegment = odataPath.Segments[odataPath.Segments.Count - 2] as OperationSegment;
            str = FunctionRoutingConvention.GetFunction(operationSegment).SelectAction(actionMap, true);
            break;
          case "~/entityset/key/cast/function":
          case "~/entityset/key/function":
            operationSegment = odataPath.Segments.Last<ODataPathSegment>() as OperationSegment;
            str = FunctionRoutingConvention.GetFunction(operationSegment).SelectAction(actionMap, false);
            if (str != null)
            {
              controllerContext.AddKeyValueToRouteData((KeySegment) odataPath.Segments[1]);
              break;
            }
            break;
          case "~/entityset/key/cast/function/$count":
          case "~/entityset/key/function/$count":
            operationSegment = odataPath.Segments[odataPath.Segments.Count - 2] as OperationSegment;
            str = FunctionRoutingConvention.GetFunction(operationSegment).SelectAction(actionMap, false);
            if (str != null)
            {
              controllerContext.AddKeyValueToRouteData((KeySegment) odataPath.Segments[1]);
              break;
            }
            break;
          case "~/singleton/cast/function":
          case "~/singleton/function":
            operationSegment = odataPath.Segments.Last<ODataPathSegment>() as OperationSegment;
            str = FunctionRoutingConvention.GetFunction(operationSegment).SelectAction(actionMap, false);
            break;
          case "~/singleton/cast/function/$count":
          case "~/singleton/function/$count":
            operationSegment = odataPath.Segments[odataPath.Segments.Count - 2] as OperationSegment;
            str = FunctionRoutingConvention.GetFunction(operationSegment).SelectAction(actionMap, false);
            break;
        }
        if (str != null)
        {
          controllerContext.AddFunctionParameterToRouteData(operationSegment);
          return str;
        }
      }
      return (string) null;
    }

    private static IEdmFunction GetFunction(OperationSegment segment) => segment != null ? segment.Operations.First<IEdmOperation>() as IEdmFunction : (IEdmFunction) null;
  }
}

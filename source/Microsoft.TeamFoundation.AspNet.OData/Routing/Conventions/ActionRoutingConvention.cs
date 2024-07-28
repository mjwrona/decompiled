// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.ActionRoutingConvention
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
  public class ActionRoutingConvention : NavigationSourceRoutingConvention
  {
    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return ActionRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      if (ODataRequestMethod.Post == controllerContext.Request.Method)
      {
        switch (odataPath.PathTemplate)
        {
          case "~/entityset/key/cast/action":
          case "~/entityset/key/action":
            string str = ActionRoutingConvention.GetAction(odataPath).SelectAction(actionMap, false);
            if (str == null)
              return str;
            KeySegment segment = (KeySegment) odataPath.Segments[1];
            controllerContext.AddKeyValueToRouteData(segment);
            return str;
          case "~/entityset/cast/action":
          case "~/entityset/action":
            return ActionRoutingConvention.GetAction(odataPath).SelectAction(actionMap, true);
          case "~/singleton/action":
          case "~/singleton/cast/action":
            return ActionRoutingConvention.GetAction(odataPath).SelectAction(actionMap, false);
        }
      }
      return (string) null;
    }

    private static IEdmAction GetAction(Microsoft.AspNet.OData.Routing.ODataPath odataPath)
    {
      ODataPathSegment odataPathSegment = odataPath.Segments.Last<ODataPathSegment>();
      IEdmAction action = (IEdmAction) null;
      if (odataPathSegment is OperationSegment operationSegment)
        action = operationSegment.Operations.First<IEdmOperation>() as IEdmAction;
      return action;
    }
  }
}

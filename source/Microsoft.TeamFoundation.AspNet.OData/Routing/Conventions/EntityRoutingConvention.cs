// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.EntityRoutingConvention
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
  public class EntityRoutingConvention : NavigationSourceRoutingConvention
  {
    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return EntityRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      if (odataPath.PathTemplate == "~/entityset/key" || odataPath.PathTemplate == "~/entityset/key/cast")
      {
        string str;
        switch (controllerContext.Request.Method)
        {
          case ODataRequestMethod.Get:
            str = "Get";
            break;
          case ODataRequestMethod.Delete:
            str = "Delete";
            break;
          case ODataRequestMethod.Merge:
          case ODataRequestMethod.Patch:
            str = "Patch";
            break;
          case ODataRequestMethod.Put:
            str = "Put";
            break;
          default:
            return (string) null;
        }
        IEdmEntityType edmType = (IEdmEntityType) odataPath.EdmType;
        string matchingAction = actionMap.FindMatchingAction(str + edmType.Name, str);
        if (matchingAction != null)
        {
          KeySegment segment = (KeySegment) odataPath.Segments[1];
          controllerContext.AddKeyValueToRouteData(segment);
          return matchingAction;
        }
      }
      return (string) null;
    }
  }
}

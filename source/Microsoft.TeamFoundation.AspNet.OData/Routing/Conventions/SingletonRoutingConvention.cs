// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.SingletonRoutingConvention
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
  public class SingletonRoutingConvention : NavigationSourceRoutingConvention
  {
    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return SingletonRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      if (odataPath.PathTemplate == "~/singleton")
      {
        SingletonSegment segment = (SingletonSegment) odataPath.Segments[0];
        string actionNamePrefix = SingletonRoutingConvention.GetActionNamePrefix(controllerContext.Request.Method);
        if (actionNamePrefix != null)
          return actionMap.FindMatchingAction(actionNamePrefix + segment.Singleton.Name, actionNamePrefix);
      }
      else if (odataPath.PathTemplate == "~/singleton/cast")
      {
        SingletonSegment segment = (SingletonSegment) odataPath.Segments[0];
        IEdmEntityType edmType = (IEdmEntityType) odataPath.EdmType;
        string actionNamePrefix = SingletonRoutingConvention.GetActionNamePrefix(controllerContext.Request.Method);
        if (actionNamePrefix != null)
          return actionMap.FindMatchingAction(actionNamePrefix + segment.Singleton.Name + "From" + edmType.Name, actionNamePrefix + "From" + edmType.Name);
      }
      return (string) null;
    }

    private static string GetActionNamePrefix(ODataRequestMethod method)
    {
      switch (method)
      {
        case ODataRequestMethod.Get:
          return "Get";
        case ODataRequestMethod.Merge:
        case ODataRequestMethod.Patch:
          return "Patch";
        case ODataRequestMethod.Put:
          return "Put";
        default:
          return (string) null;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.EntitySetRoutingConvention
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
  public class EntitySetRoutingConvention : NavigationSourceRoutingConvention
  {
    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return EntitySetRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      if (odataPath.PathTemplate == "~/entityset")
      {
        IEdmEntitySetBase entitySet = (IEdmEntitySetBase) ((EntitySetSegment) odataPath.Segments[0]).EntitySet;
        if (controllerContext.Request.Method == ODataRequestMethod.Get)
          return actionMap.FindMatchingAction("Get" + entitySet.Name, "Get");
        if (ODataRequestMethod.Post == controllerContext.Request.Method)
          return actionMap.FindMatchingAction("Post" + entitySet.EntityType().Name, "Post");
      }
      else
      {
        if (odataPath.PathTemplate == "~/entityset/$count" && controllerContext.Request.Method == ODataRequestMethod.Get)
        {
          IEdmEntitySetBase entitySet = (IEdmEntitySetBase) ((EntitySetSegment) odataPath.Segments[0]).EntitySet;
          return actionMap.FindMatchingAction("Get" + entitySet.Name, "Get");
        }
        if (odataPath.PathTemplate == "~/entityset/cast")
        {
          IEdmEntitySetBase entitySet = (IEdmEntitySetBase) ((EntitySetSegment) odataPath.Segments[0]).EntitySet;
          IEdmEntityType definition = (IEdmEntityType) ((IEdmCollectionType) odataPath.EdmType).ElementType.Definition;
          if (controllerContext.Request.Method == ODataRequestMethod.Get)
            return actionMap.FindMatchingAction("Get" + entitySet.Name + "From" + definition.Name, "GetFrom" + definition.Name);
          if (ODataRequestMethod.Post == controllerContext.Request.Method)
            return actionMap.FindMatchingAction("Post" + entitySet.EntityType().Name + "From" + definition.Name, "PostFrom" + definition.Name);
        }
        else if (odataPath.PathTemplate == "~/entityset/cast/$count" && controllerContext.Request.Method == ODataRequestMethod.Get)
        {
          IEdmEntitySetBase entitySet = (IEdmEntitySetBase) ((EntitySetSegment) odataPath.Segments[0]).EntitySet;
          IEdmEntityType definition = (IEdmEntityType) ((IEdmCollectionType) odataPath.Segments[1].EdmType).ElementType.Definition;
          return actionMap.FindMatchingAction("Get" + entitySet.Name + "From" + definition.Name, "GetFrom" + definition.Name);
        }
      }
      return (string) null;
    }
  }
}

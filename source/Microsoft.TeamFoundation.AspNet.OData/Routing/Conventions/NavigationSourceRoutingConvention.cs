// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.NavigationSourceRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.UriParser;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public abstract class NavigationSourceRoutingConvention : IODataRoutingConvention
  {
    public virtual string SelectController(Microsoft.AspNet.OData.Routing.ODataPath odataPath, HttpRequestMessage request)
    {
      if (odataPath == null)
        throw Error.ArgumentNull(nameof (odataPath));
      if (request == null)
        throw Error.ArgumentNull(nameof (request));
      SelectControllerResult controllerResult = NavigationSourceRoutingConvention.SelectControllerImpl(odataPath);
      if (controllerResult != null)
        request.Properties["AttributeRouteData"] = (object) controllerResult.Values;
      return controllerResult?.ControllerName;
    }

    public abstract string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap);

    internal static void ValidateSelectActionParameters(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      if (odataPath == null)
        throw Error.ArgumentNull(nameof (odataPath));
      if (controllerContext == null)
        throw Error.ArgumentNull(nameof (controllerContext));
      if (actionMap == null)
        throw Error.ArgumentNull(nameof (actionMap));
    }

    internal static SelectControllerResult GetControllerResult(
      HttpControllerContext controllerContext)
    {
      string controllerName = (string) null;
      object values = (object) null;
      if (controllerContext != null)
      {
        if (controllerContext.Request != null)
          controllerContext.Request.Properties.TryGetValue("AttributeRouteData", out values);
        if (controllerContext.ControllerDescriptor != null)
          controllerName = controllerContext.ControllerDescriptor.ControllerName;
      }
      return new SelectControllerResult(controllerName, values as IDictionary<string, object>);
    }

    internal static SelectControllerResult SelectControllerImpl(Microsoft.AspNet.OData.Routing.ODataPath odataPath)
    {
      if (odataPath.Segments.FirstOrDefault<ODataPathSegment>() is EntitySetSegment entitySetSegment)
        return new SelectControllerResult(entitySetSegment.EntitySet.Name, (IDictionary<string, object>) null);
      return odataPath.Segments.FirstOrDefault<ODataPathSegment>() is SingletonSegment singletonSegment ? new SelectControllerResult(singletonSegment.Singleton.Name, (IDictionary<string, object>) null) : (SelectControllerResult) null;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.MetadataRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public class MetadataRoutingConvention : IODataRoutingConvention
  {
    public string SelectController(ODataPath odataPath, HttpRequestMessage request) => MetadataRoutingConvention.SelectControllerImpl(odataPath, (IWebApiRequestMessage) new WebApiRequestMessage(request))?.ControllerName;

    public string SelectAction(
      ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      if (odataPath == null)
        throw Error.ArgumentNull(nameof (odataPath));
      if (controllerContext == null)
        throw Error.ArgumentNull(nameof (controllerContext));
      if (actionMap == null)
        throw Error.ArgumentNull(nameof (actionMap));
      SelectControllerResult controllerResult = new SelectControllerResult(controllerContext.ControllerDescriptor.ControllerName, (IDictionary<string, object>) null);
      return MetadataRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, controllerResult), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static SelectControllerResult SelectControllerImpl(
      ODataPath odataPath,
      IWebApiRequestMessage request)
    {
      if (odataPath == null)
        throw Error.ArgumentNull(nameof (odataPath));
      if (request == null)
        throw Error.ArgumentNull(nameof (request));
      return odataPath.PathTemplate == "~" || odataPath.PathTemplate == "~/$metadata" ? new SelectControllerResult("Metadata", (IDictionary<string, object>) null) : (SelectControllerResult) null;
    }

    internal static string SelectActionImpl(
      ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      if (odataPath == null)
        throw Error.ArgumentNull(nameof (odataPath));
      if (controllerContext == null)
        throw Error.ArgumentNull(nameof (controllerContext));
      if (actionMap == null)
        throw Error.ArgumentNull(nameof (actionMap));
      if (odataPath.PathTemplate == "~")
        return "GetServiceDocument";
      return odataPath.PathTemplate == "~/$metadata" ? "GetMetadata" : (string) null;
    }
  }
}

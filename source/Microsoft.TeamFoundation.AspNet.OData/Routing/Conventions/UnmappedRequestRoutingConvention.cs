// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.UnmappedRequestRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Interfaces;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public class UnmappedRequestRoutingConvention : NavigationSourceRoutingConvention
  {
    private const string UnmappedRequestActionName = "HandleUnmappedRequest";

    public override string SelectAction(
      ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return UnmappedRequestRoutingConvention.SelectActionImpl((IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(IWebApiActionMap actionMap) => actionMap.Contains("HandleUnmappedRequest") ? "HandleUnmappedRequest" : (string) null;
  }
}

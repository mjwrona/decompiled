// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.IODataRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public interface IODataRoutingConvention
  {
    string SelectController(ODataPath odataPath, HttpRequestMessage request);

    string SelectAction(
      ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap);
  }
}

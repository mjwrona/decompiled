// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiControllerContext
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Routing.Conventions;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiControllerContext : IWebApiControllerContext
  {
    private HttpControllerContext innerContext;

    public WebApiControllerContext(
      HttpControllerContext controllerContext,
      SelectControllerResult controllerResult)
    {
      if (controllerContext == null)
        throw Error.ArgumentNull(nameof (controllerContext));
      if (controllerResult == null)
        throw Error.ArgumentNull(nameof (controllerResult));
      this.innerContext = controllerContext;
      this.ControllerResult = controllerResult;
      HttpRequestMessage request = controllerContext.Request;
      if (request == null)
        return;
      this.Request = (IWebApiRequestMessage) new WebApiRequestMessage(request);
    }

    public SelectControllerResult ControllerResult { get; private set; }

    public IWebApiRequestMessage Request { get; private set; }

    public IDictionary<string, object> RouteData => this.innerContext.RouteData.Values;
  }
}

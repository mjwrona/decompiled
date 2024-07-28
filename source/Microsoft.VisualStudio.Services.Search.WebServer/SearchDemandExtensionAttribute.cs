// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SearchDemandExtensionAttribute
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
  public sealed class SearchDemandExtensionAttribute : AuthorizationFilterAttribute
  {
    public SearchDemandExtensionAttribute(string publisherName, string extensionName)
    {
      this.PublisherName = publisherName;
      this.ExtensionName = extensionName;
    }

    public string ExtensionName { get; private set; }

    public string PublisherName { get; private set; }

    public override void OnAuthorization(HttpActionContext actionContext)
    {
      IHttpController controller = actionContext.ControllerContext.Controller;
      new DemandExtensionAttribute(this.PublisherName, this.ExtensionName, true).OnAuthorization(actionContext);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.DemandExtensionAttribute
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class DemandExtensionAttribute : AuthorizationFilterAttribute
  {
    public DemandExtensionAttribute(
      string publisherName,
      string extensionName,
      bool checkLicenseOnFallback = false,
      bool alwaysAllowSystemContexts = false,
      bool alwaysAllowDeploymentServiceIdentities = false)
    {
      this.PublisherName = publisherName;
      this.ExtensionName = extensionName;
      this.CheckLicenseOnFallback = checkLicenseOnFallback;
      this.AlwaysAllowSystemContexts = alwaysAllowSystemContexts;
      this.AlwaysAllowDeploymentServiceIdentities = alwaysAllowDeploymentServiceIdentities;
    }

    public string ExtensionName { get; private set; }

    public string PublisherName { get; private set; }

    public bool CheckLicenseOnFallback { get; private set; }

    public bool AlwaysAllowSystemContexts { get; }

    public bool AlwaysAllowDeploymentServiceIdentities { get; }

    public override void OnAuthorization(HttpActionContext actionContext)
    {
      base.OnAuthorization(actionContext);
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      tfsRequestContext.GetService<IDemandExtensionService>().Demand(tfsRequestContext, this.PublisherName, this.ExtensionName, this.CheckLicenseOnFallback, this.AlwaysAllowSystemContexts, this.AlwaysAllowDeploymentServiceIdentities);
    }
  }
}

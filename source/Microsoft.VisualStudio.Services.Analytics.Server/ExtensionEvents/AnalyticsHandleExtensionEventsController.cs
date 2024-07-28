// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ExtensionEvents.AnalyticsHandleExtensionEventsController
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.ExtensionEvents
{
  [VersionedApiControllerCustomName(Area = "Analytics", ResourceName = "Handle")]
  public class AnalyticsHandleExtensionEventsController : TfsApiController
  {
    [HttpPost]
    [ClientIgnore]
    public ExtensionEventCallbackResult Handle(ExtensionEventCallbackData data)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (data.Operation == ExtensionOperation.PreInstall || data.Operation == ExtensionOperation.PostInstall || data.Operation == ExtensionOperation.PostEnable)
          return new ExtensionEventCallbackResult()
          {
            Allow = false,
            Message = AnalyticsResources.ANALYTICS_EXTENSION_DENIED()
          };
        if (data.Operation == ExtensionOperation.PostUninstall)
          return new ExtensionEventCallbackResult()
          {
            Allow = true
          };
      }
      return new ExtensionEventCallbackResult()
      {
        Allow = false
      };
    }
  }
}

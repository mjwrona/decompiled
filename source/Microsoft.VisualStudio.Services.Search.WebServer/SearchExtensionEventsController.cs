// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SearchExtensionEventsController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "preinstall")]
  public class SearchExtensionEventsController : SearchApiController
  {
    [HttpPost]
    [ClientLocationId("f1573d34-597e-416c-aa9e-c1ff4503adc9")]
    public ExtensionEventCallbackResult PreInstall(ExtensionEventCallbackData data)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080080, "REST-API", "REST-API", "ExtensionPreInstall");
      try
      {
        bool flag1 = data.ExtensionName.Equals("vss-code-search", StringComparison.OrdinalIgnoreCase);
        bool flag2 = data.ExtensionName.Equals("vss-workitem-search", StringComparison.OrdinalIgnoreCase);
        bool flag3 = data.ExtensionName.Equals("vss-workitem-searchonprem", StringComparison.OrdinalIgnoreCase);
        bool flag4 = data.ExtensionName.Equals("vss-wiki-searchonprem", StringComparison.OrdinalIgnoreCase);
        if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
          if (string.IsNullOrEmpty(vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/ATSearchPlatformConnectionString", false, (string) null)))
            return new ExtensionEventCallbackResult()
            {
              Allow = false,
              Message = SearchWebApiResources.SearchNotConfiguredMessage
            };
        }
        if (flag1)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfCodeSearchExtensionPreinstallCallbacks", "Query Pipeline", 1.0);
        else if (flag2)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfWorkItemSearchExtensionPreinstallCallbacks", "Query Pipeline", 1.0);
        else if (flag3)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfWorkItemSearchOnPremiseExtensionPreinstallCallbacks", "Query Pipeline", 1.0);
        else if (flag4)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfWikiSearchOnPremiseExtensionPreinstallCallbacks", "Query Pipeline", 1.0);
        return new ExtensionEventCallbackResult()
        {
          Allow = true
        };
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080081, "REST-API", "REST-API", "ExtensionPreInstall");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}

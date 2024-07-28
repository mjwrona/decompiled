// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionAuditLogController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "AuditLog")]
  [ClientInternalUseOnly(false)]
  public class ExtensionAuditLogController : TfsApiController
  {
    public override string TraceArea => "ExtensionRequest";

    public override string ActivityLogArea => "WebApi";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InstalledExtensionNotFoundException>(HttpStatusCode.NotFound);
    }

    [HttpGet]
    [ClientLocationId("23A312E0-562D-42FB-A505-5A046B5635DB")]
    public ExtensionAuditLog GetAuditLog(string publisherName, string extensionName)
    {
      this.TfsRequestContext.GetService<IInstalledExtensionService>().GetInstalledExtension(this.TfsRequestContext, publisherName, extensionName);
      return this.TfsRequestContext.GetService<IExtensionAuditLogService>().GetAuditLog(this.TfsRequestContext, publisherName, extensionName);
    }
  }
}

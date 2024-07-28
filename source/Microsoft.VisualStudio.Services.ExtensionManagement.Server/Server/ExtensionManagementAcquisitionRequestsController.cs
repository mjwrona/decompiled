// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionManagementAcquisitionRequestsController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.AcquisitionRequest;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "AcquisitionRequests")]
  [ClientInternalUseOnly(false)]
  public class ExtensionManagementAcquisitionRequestsController : TfsApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ExtensionAlreadyInstalledException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<InvalidInstallationTargetException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionEventCallbackDeniedException>(HttpStatusCode.BadRequest);
    }

    [HttpPost]
    public ExtensionAcquisitionRequest RequestAcquisition(
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      return this.TfsRequestContext.GetService<AcquisitionService>().RequestAcquisition(this.TfsRequestContext, acquisitionRequest);
    }
  }
}

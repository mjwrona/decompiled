// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AcquisitionRequestController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(2.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "acquisitionrequests")]
  public class AcquisitionRequestController : GalleryController
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

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AcquisitionOptionsController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionOption;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "acquisitionoptions")]
  public class AcquisitionOptionsController : GalleryController
  {
    [HttpGet]
    public AcquisitionOptions GetAcquisitionOptions(
      string itemId,
      [FromUri] string installationTarget,
      bool testCommerce = false,
      bool isFreeOrTrialInstall = false)
    {
      return this.TfsRequestContext.GetService<AcquisitionService>().GetAcquisitionOptions(this.TfsRequestContext, itemId, installationTarget, testCommerce, isFreeOrTrialInstall);
    }
  }
}

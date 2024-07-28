// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Controllers.VSCodeWebExtensionController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Statistic;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Controllers
{
  [ControllerApiVersion(6.1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "vscodewebextension")]
  public class VSCodeWebExtensionController : GalleryController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("205c91a8-7841-4fd3-ae4f-5a745d5a8df5")]
    public void UpdateVSCodeWebExtensionStatistics(
      string itemName,
      string version,
      VSCodeWebExtensionStatisicsType statType)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(itemName, nameof (itemName));
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
      VSCodeWebExtensionService service = this.TfsRequestContext.GetService<VSCodeWebExtensionService>();
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableStatisticsUpdateForVSCodeWeb"))
        service.UpdateVSCodeWebExtensionStatistics(this.TfsRequestContext, itemName, version, statType);
      else
        service.LogVSCodeWebExtensionTelemetry(this.TfsRequestContext, itemName, version, statType, "failure", "Statistics tracking is not enabled for VSCode Web extensions");
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionManagementAcquisitionOptionsController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "AcquisitionOptions")]
  [ClientInternalUseOnly(false)]
  public class ExtensionManagementAcquisitionOptionsController : TfsApiController
  {
    [HttpGet]
    public AcquisitionOptions GetAcquisitionOptions(
      string itemId,
      bool testCommerce = false,
      bool isFreeOrTrialInstall = false,
      bool isAccountOwner = false,
      bool isLinked = false,
      bool isConnectedServer = false,
      bool isBuyOperationValid = false)
    {
      AcquisitionService service = this.TfsRequestContext.GetService<AcquisitionService>();
      if (this.Request?.Headers?.Referrer?.LocalPath != null && this.Request.Headers.Referrer.LocalPath.Equals("/acquisition", StringComparison.OrdinalIgnoreCase) && this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        isBuyOperationValid = true;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string galleryItemName = itemId;
      int num1 = testCommerce ? 1 : 0;
      int num2 = isFreeOrTrialInstall ? 1 : 0;
      int num3 = isBuyOperationValid ? 1 : 0;
      int num4 = isAccountOwner ? 1 : 0;
      int num5 = isLinked ? 1 : 0;
      int num6 = isConnectedServer ? 1 : 0;
      return service.GetAcquisitionOptions(tfsRequestContext, galleryItemName, num1 != 0, num2 != 0, num3 != 0, num4 != 0, num5 != 0, num6 != 0);
    }
  }
}

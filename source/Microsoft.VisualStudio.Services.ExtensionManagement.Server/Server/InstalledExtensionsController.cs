// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.InstalledExtensionsController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "InstalledExtensions")]
  public class InstalledExtensionsController : TfsApiController
  {
    public override string TraceArea => "InstalledExtension";

    public override string ActivityLogArea => "WebApi";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InstalledExtensionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ExtensionAlreadyInstalledException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<InvalidInstallationTargetException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionDemandsNotSupportedException>(HttpStatusCode.BadRequest);
    }

    [HttpGet]
    [ClientLocationId("275424D0-C844-4FE2-BDA6-04933A1357D8")]
    [ClientExample("ListInstalledExtensions.json", null, null, null)]
    public List<InstalledExtension> GetInstalledExtensions(
      bool includeDisabledExtensions = true,
      bool includeErrors = false,
      [ClientParameterAsIEnumerable(typeof (string), ':')] string assetTypes = null,
      bool includeInstallationIssues = false)
    {
      string[] assetTypes1 = (string[]) null;
      if (assetTypes != null)
        assetTypes1 = assetTypes.Split(new char[1]{ ':' }, StringSplitOptions.RemoveEmptyEntries);
      return this.TfsRequestContext.GetService<IInstalledExtensionService>().GetInstalledExtensions(this.TfsRequestContext, includeDisabledExtensions, includeErrors, (IEnumerable<string>) assetTypes1, includeInstallationIssues);
    }

    [HttpPatch]
    [ClientLocationId("275424D0-C844-4FE2-BDA6-04933A1357D8")]
    public InstalledExtension UpdateInstalledExtension(InstalledExtension extension)
    {
      ArgumentUtility.CheckForNull<InstalledExtension>(extension, "extensionToInstall");
      ArgumentUtility.CheckForNull<InstalledExtensionState>(extension.InstallState, "installState");
      return this.TfsRequestContext.GetService<IInstalledExtensionService>().UpdateExtension(this.TfsRequestContext, extension.PublisherName, extension.ExtensionName, extension.InstallState.Flags, extension.Version);
    }
  }
}

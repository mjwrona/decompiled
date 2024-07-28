// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.InstalledExtensionsByNameController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "InstalledExtensionsByName")]
  [ClientGroupByResource("InstalledExtensions")]
  public class InstalledExtensionsByNameController : TfsApiController
  {
    public override string TraceArea => "InstalledExtensionByName";

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
    [ClientLocationId("FB0DA285-F23E-4B56-8B53-3EF5F9F6DE66")]
    [ClientExample("GetInstalledSampleExtension.json", null, null, null)]
    public InstalledExtension GetInstalledExtensionByName(
      string publisherName,
      string extensionName,
      [ClientParameterAsIEnumerable(typeof (string), ':')] string assetTypes = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      string[] assetTypes1 = (string[]) null;
      if (assetTypes != null)
        assetTypes1 = assetTypes.Split(new char[1]{ ':' }, StringSplitOptions.RemoveEmptyEntries);
      return this.TfsRequestContext.GetService<InstalledExtensionService>().GetInstalledExtension(this.TfsRequestContext, publisherName, extensionName, (IEnumerable<string>) assetTypes1, false);
    }

    [HttpPost]
    [ClientLocationId("FB0DA285-F23E-4B56-8B53-3EF5F9F6DE66")]
    [ClientExample("InstallSampleExtension.json", null, null, null)]
    public InstalledExtension InstallExtensionByName(
      string publisherName,
      string extensionName,
      string version = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      IInstalledExtensionService service = this.TfsRequestContext.GetService<IInstalledExtensionService>();
      InstalledExtension installedExtension = service.InstallExtension(this.TfsRequestContext, publisherName, extensionName, version);
      service.UpdateExtensionInstallCount(this.TfsRequestContext, publisherName, extensionName);
      return installedExtension;
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("FB0DA285-F23E-4B56-8B53-3EF5F9F6DE66")]
    [ClientExample("UninstallSampleExtension.json", null, null, null)]
    public HttpResponseMessage UninstallExtensionByName(
      string publisherName,
      string extensionName,
      string reason = null,
      string reasonCode = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      this.TfsRequestContext.GetService<IInstalledExtensionService>().UninstallExtension(this.TfsRequestContext, publisherName, extensionName, reason, reasonCode);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}

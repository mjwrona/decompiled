// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionAuthorizationsController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "Authorizations")]
  [ClientInternalUseOnly(false)]
  public class ExtensionAuthorizationsController : TfsApiController
  {
    public override string TraceArea => "Authorizations";

    public override string ActivityLogArea => "WebApi";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap) => base.InitializeExceptionMap(exceptionMap);

    [HttpPut]
    [ClientLocationId("F21CFC80-D2D2-4248-98BB-7820C74C4606")]
    public ExtensionAuthorization RegisterAuthorization(
      string publisherName,
      string extensionName,
      Guid registrationId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      return this.TfsRequestContext.GetService<IInstalledExtensionService>().AuthorizeExtension(this.TfsRequestContext, publisherName, extensionName, registrationId);
    }
  }
}

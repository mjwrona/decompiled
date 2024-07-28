// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionStatesController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "ExtensionStates")]
  [ClientInternalUseOnly(false)]
  public class ExtensionStatesController : TfsApiController
  {
    public override string TraceArea => "ExtensionStates";

    public override string ActivityLogArea => "WebApi";

    [HttpGet]
    public List<ExtensionState> GetStates(
      bool includeDisabled = true,
      bool includeErrors = false,
      bool includeInstallationIssues = false,
      bool forceRefresh = false)
    {
      return this.TfsRequestContext.GetService<IInstalledExtensionService>().GetInstalledExtensionStates(this.TfsRequestContext, includeDisabled, includeErrors, includeInstallationIssues, forceRefresh);
    }
  }
}

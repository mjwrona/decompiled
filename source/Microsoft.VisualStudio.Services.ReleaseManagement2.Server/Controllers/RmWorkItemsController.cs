// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmWorkItemsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "workitems")]
  public class RmWorkItemsController : ReleaseManagementProjectControllerBase
  {
    private const int DefaultMaxItems = 250;

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    public IEnumerable<ReleaseWorkItemRef> GetReleaseWorkItemsRefs(
      int releaseId,
      int baseReleaseId = 0,
      [FromUri(Name = "$top")] int top = 250,
      string artifactAlias = null)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "ReleaseWorkItemsService.GetReleaseWorkItemRefs", 1973109, 5, true))
      {
        IEnumerable<ReleaseWorkItemRef> releaseWorkItemRefs = this.TfsRequestContext.GetService<ReleaseWorkItemsService>().GetReleaseWorkItemRefs(this.TfsRequestContext, this.ProjectInfo, baseReleaseId, releaseId, top, artifactAlias);
        this.TfsRequestContext.SetSecuredObjects<ReleaseWorkItemRef>(releaseWorkItemRefs);
        return releaseWorkItemRefs;
      }
    }
  }
}

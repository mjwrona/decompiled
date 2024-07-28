// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseHistoryController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "history")]
  public class ReleaseHistoryController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    public IEnumerable<ReleaseRevision> GetReleaseHistory(int releaseId)
    {
      if (releaseId <= 0)
        return Enumerable.Empty<ReleaseRevision>();
      List<ReleaseRevision> list = this.GetService<ReleaseHistoryService>().GetHistory(this.TfsRequestContext, this.ProjectId, releaseId).ToWebApiContract().ToList<ReleaseRevision>();
      this.TfsRequestContext.SetSecuredObjects<ReleaseRevision>((IEnumerable<ReleaseRevision>) list);
      return (IEnumerable<ReleaseRevision>) list;
    }
  }
}

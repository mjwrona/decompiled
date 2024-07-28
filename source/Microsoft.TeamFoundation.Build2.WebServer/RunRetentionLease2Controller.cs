// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.RunRetentionLease2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "leases", ResourceVersion = 2)]
  [CheckWellFormedProject(Required = true)]
  public class RunRetentionLease2Controller : RunRetentionLease1Controller
  {
    [HttpPatch]
    public virtual async Task<Microsoft.TeamFoundation.Build.WebApi.RetentionLease> UpdateRetentionLease(
      int leaseId,
      RetentionLeaseUpdate leaseUpdate)
    {
      RunRetentionLease2Controller lease2Controller = this;
      Microsoft.TeamFoundation.Build2.Server.RetentionLease lease = await lease2Controller.BuildService.UpdateRetentionLease(lease2Controller.TfsRequestContext, lease2Controller.ProjectId, leaseId, leaseUpdate);
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (lease2Controller.ProjectInfo);
      TeamProjectReference projectReference = projectInfo != null ? projectInfo.ToTeamProjectReference(lease2Controller.TfsRequestContext) : (TeamProjectReference) null;
      return lease.ToWebApiRetentionLease((ISecuredObject) projectReference);
    }
  }
}

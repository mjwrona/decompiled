// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentPoolPermissionsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "poolpermissions")]
  public class TaskAgentPoolPermissionsController : DistributedTaskApiController
  {
    [HttpGet]
    public bool HasPoolPermissions(int poolId, int permissions) => this.TfsRequestContext.GetService<DistributedTaskResourceService>().GetAgentPoolSecurity(this.TfsRequestContext.ToPoolRequestContext(), poolId).HasPoolPermission(this.TfsRequestContext, poolId, permissions);
  }
}

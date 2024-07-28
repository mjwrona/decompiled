// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentPoolsRolesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(2.1)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "poolroles")]
  public class TaskAgentPoolsRolesController : DistributedTaskApiController
  {
    [HttpGet]
    public IEnumerable<IdentityRef> GetAgentPoolRoles(int? poolId = null) => Enumerable.Empty<IdentityRef>();
  }
}

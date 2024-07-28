// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubPlanGroupsQueue2Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "plangroupsqueue")]
  public class TaskHubPlanGroupsQueue2Controller : TaskHubPlanGroupsQueueController
  {
    [HttpGet]
    [ClientLocationId("65FD0708-BC1E-447B-A731-0587C5464E5B")]
    public TaskOrchestrationQueuedPlanGroup GetQueuedPlanGroup(string planGroup) => this.Hub.GetQueuedPlanGroup(this.TfsRequestContext, this.ScopeIdentifier, planGroup);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubCompatOrchestrationPlanEventsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "events")]
  public sealed class TaskHubCompatOrchestrationPlanEventsController : TaskHubCompatApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public Task PostEvent(Guid planId, Guid jobId, string eventName, JobEvent eventData) => this.Hub.RaiseJobEventAsync(this.TfsRequestContext, Guid.Empty, planId, jobId, eventName, eventData);
  }
}

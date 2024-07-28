// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubOrchestrationPlanEventsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "events")]
  public sealed class TaskHubOrchestrationPlanEventsController : TaskHubApiController
  {
    [HttpPost]
    [ClientInclude(RestClientLanguages.Swagger2)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("POST_distributedtask_PostEvent_.json", "Send a TaskCompletedEvent", "Send a TaskCompletedEvent with a stage access decision when processing asynchronous checks in Callback mode.", null)]
    [ClientSuppressWarning(ClientWarnings.NamingGuidelines)]
    public Task PostEvent(Guid planId, JobEvent eventData)
    {
      ArgumentUtility.CheckForNull<JobEvent>(eventData, nameof (eventData), "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(eventData.JobId, "eventData.JobId", "DistributedTask");
      ArgumentUtility.CheckStringForNullOrEmpty(eventData.Name, "eventData.Name", "DistributedTask");
      TaskEvent eventData1 = eventData as TaskEvent;
      if (eventData.IsServerTaskEvent())
        return this.Hub.RaiseTaskEventAsync(this.TfsRequestContext, this.ScopeIdentifier, planId, eventData1.JobId, eventData1.TaskId, eventData1.Name, eventData1);
      if (eventData1 != null)
        this.TfsRequestContext.TraceWarning(10015554, "DistributedTask", string.Format("TaskHubOrchestrationPlanEventsController::PostEvent -- {0} event is invoked. ScopId: {1}, PlanId: {2} and JobId: {3}. TaskId, either not provided or Empty Guid.", (object) eventData.Name, (object) this.ScopeIdentifier, (object) planId, (object) eventData.JobId));
      return this.Hub.RaiseJobEventAsync(this.TfsRequestContext, this.ScopeIdentifier, planId, eventData.JobId, eventData.Name, eventData);
    }
  }
}

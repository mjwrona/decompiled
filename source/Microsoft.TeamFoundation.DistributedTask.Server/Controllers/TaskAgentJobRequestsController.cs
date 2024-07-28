// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentJobRequestsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "agentrequests")]
  public sealed class TaskAgentJobRequestsController : DistributedTaskProjectApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (TaskAgentJobRequest), null, null)]
    public Task<TaskAgentJobRequest> QueueAgentRequest(int queueId, TaskAgentJobRequest request)
    {
      ArgumentUtility.CheckForNull<TaskAgentJobRequest>(request, nameof (request), "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(request.HostId, "request.HostId", "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(request.PlanId, "request.PlanId", "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(request.JobId, "request.JobId", "DistributedTask");
      if (!this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        ArgumentUtility.CheckForEmptyGuid(request.ServiceOwner, "request.ServiceOwner", "DistributedTask");
      return this.ResourceService.QueueAgentRequestAsync(this.TfsRequestContext, this.ProjectId, queueId, request);
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<TaskAgentJobRequest>), null, null)]
    public async Task<HttpResponseMessage> GetAgentRequestsForQueue(
      int queueId,
      [FromUri(Name = "$top")] int? top,
      [ClientQueryParameter] string continuationToken = "")
    {
      TaskAgentJobRequestsController requestsController = this;
      IPagedList<TaskAgentJobRequest> requestsForQueueAsync = await requestsController.ResourceService.GetAgentRequestsForQueueAsync(requestsController.TfsRequestContext, requestsController.ProjectId, queueId, continuationToken, top);
      HttpResponseMessage response = requestsController.Request.CreateResponse<IPagedList<TaskAgentJobRequest>>(HttpStatusCode.OK, requestsForQueueAsync);
      if (requestsForQueueAsync.ContinuationToken != null)
        DistributedTaskProjectApiController.SetContinuationToken(response, requestsForQueueAsync.ContinuationToken);
      return response;
    }
  }
}

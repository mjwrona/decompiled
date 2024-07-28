// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentJobRequestsLegacyController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "jobrequests")]
  [TaskYieldOnException]
  public sealed class TaskAgentJobRequestsLegacyController : DistributedTaskApiController
  {
    [HttpDelete]
    public void DeleteAgentRequest(
      int poolId,
      long requestId,
      Guid lockToken,
      TaskResult? result = null,
      bool agentShuttingDown = false)
    {
      this.ResourceService.FinishAgentRequest(this.TfsRequestContext, poolId, requestId, result, agentShuttingDown);
    }

    [HttpGet]
    public TaskAgentJobRequest GetAgentRequest(int poolId, long requestId, bool includeStatus = false) => this.ResourceService.GetAgentRequest(this.TfsRequestContext, poolId, requestId, includeStatus) ?? throw new TaskAgentJobNotFoundException(TaskResources.AgentRequestNotFound((object) requestId));

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<TaskAgentJobRequest>), null, null)]
    public async Task<HttpResponseMessage> GetAgentRequests(
      int poolId,
      [FromUri(Name = "$top")] int? top,
      [ClientQueryParameter] string continuationToken = "")
    {
      TaskAgentJobRequestsLegacyController legacyController = this;
      IPagedList<TaskAgentJobRequest> agentRequestsAsync = await legacyController.ResourceService.GetAgentRequestsAsync(legacyController.TfsRequestContext, poolId, continuationToken, top);
      HttpResponseMessage response = legacyController.Request.CreateResponse<IPagedList<TaskAgentJobRequest>>(HttpStatusCode.OK, agentRequestsAsync);
      if (agentRequestsAsync.ContinuationToken != null)
        DistributedTaskApiController.SetContinuationToken(response, agentRequestsAsync.ContinuationToken);
      return response;
    }

    [HttpGet]
    public IList<TaskAgentJobRequest> GetAgentRequestsForAgent(
      int poolId,
      int agentId,
      int completedRequestCount = 50)
    {
      return this.ResourceService.GetAgentRequestsForAgent(this.TfsRequestContext, poolId, agentId, completedRequestCount);
    }

    [HttpGet]
    public IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      int poolId,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string agentIds = null,
      int completedRequestCount = 50)
    {
      return this.ResourceService.GetAgentRequestsForAgents(this.TfsRequestContext, poolId, this.ParseArray(agentIds), completedRequestCount);
    }

    [HttpGet]
    public IList<TaskAgentJobRequest> GetAgentRequestsForPlan(int poolId, Guid planId, Guid? jobId = null) => this.ResourceService.GetAgentRequestsForPlan(this.TfsRequestContext, poolId, planId, jobId);

    [HttpPost]
    [ClientResponseType(typeof (TaskAgentJobRequest), null, null)]
    public Task<TaskAgentJobRequest> QueueAgentRequestByPool(
      int poolId,
      TaskAgentJobRequest request)
    {
      ArgumentUtility.CheckForNull<TaskAgentJobRequest>(request, nameof (request), "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(request.HostId, "request.HostId", "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(request.PlanId, "request.PlanId", "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(request.JobId, "request.JobId", "DistributedTask");
      if (!this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        ArgumentUtility.CheckForEmptyGuid(request.ServiceOwner, "request.ServiceOwner", "DistributedTask");
      return this.ResourceService.QueueAgentRequestByPoolAsync(this.TfsRequestContext, poolId, request);
    }

    [HttpPatch]
    [ClientResponseType(typeof (TaskAgentJobRequest), null, null)]
    public async Task<TaskAgentJobRequest> UpdateAgentRequest(
      int poolId,
      long requestId,
      Guid lockToken,
      TaskAgentJobRequest request,
      [ClientQueryParameter] TaskAgentRequestUpdateOptions updateOptions = TaskAgentRequestUpdateOptions.None)
    {
      TaskAgentJobRequestsLegacyController legacyController = this;
      ArgumentUtility.CheckForNull<TaskAgentJobRequest>(request, nameof (request), "DistributedTask");
      try
      {
        if (updateOptions.HasFlag((System.Enum) TaskAgentRequestUpdateOptions.BumpRequestToTop))
          return await legacyController.ResourceService.BumpAgentRequestPriorityAsync(legacyController.TfsRequestContext, poolId, requestId);
        if (!request.LockedUntil.HasValue && !request.ReceiveTime.HasValue && !request.FinishTime.HasValue && !request.Result.HasValue)
        {
          request.ReceiveTime = new DateTime?(request.ReceiveTime ?? DateTime.UtcNow);
          return await legacyController.ResourceService.UpdateAgentRequestLockedUntilAsync(legacyController.TfsRequestContext, poolId, requestId, request.LockedUntil, request.ReceiveTime);
        }
        request.ReceiveTime = new DateTime?(request.ReceiveTime ?? DateTime.UtcNow);
        return await legacyController.ResourceService.UpdateAgentRequestAsync(legacyController.TfsRequestContext, poolId, requestId, request.LockedUntil, request.ReceiveTime, request.FinishTime, request.Result);
      }
      catch (TaskAgentJobTokenExpiredException ex)
      {
        if (!string.IsNullOrEmpty(legacyController.TfsRequestContext.UserAgent) && (legacyController.TfsRequestContext.UserAgent.IndexOf("vso-build-api", StringComparison.OrdinalIgnoreCase) >= 0 || legacyController.TfsRequestContext.UserAgent.IndexOf("node-taskagent-api", StringComparison.OrdinalIgnoreCase) >= 0))
          throw new TaskAgentJobNotFoundException(ex.Message, (Exception) ex);
        throw;
      }
    }
  }
}

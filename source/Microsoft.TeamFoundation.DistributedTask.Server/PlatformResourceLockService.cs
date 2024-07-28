// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformResourceLockService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.Checks.Server;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class PlatformResourceLockService : 
    IDistributedTaskResourceLockService,
    IVssFrameworkService
  {
    private const string Layer = "PlatformResourceLockService";
    private const string BuildHub = "Build";
    private const int AssignDelay = 3;
    private const string DaysToKeepRegistryKey = "/Service/DistributedTask/ResourceLocks/DaysToKeepRequests";

    public ResourceLockRequest GetRequestByCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkRunId)
    {
      using (new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (GetRequestByCheckRun)))
      {
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
          return component.GetResourceLockRequestByCheckRun(projectId, checkRunId);
      }
    }

    public List<ResourceLockRequest> GetRequestsByCheckRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> checkRunIds)
    {
      using (new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (GetRequestsByCheckRuns)))
      {
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
          return component.GetResourceLockRequestsByCheckRuns(projectId, checkRunIds);
      }
    }

    public ResourceLockRequest GetActiveResourceLock(
      IVssRequestContext requestContext,
      string resourceId,
      string resourceType)
    {
      return this.GetActiveResourceLocks(requestContext, (IEnumerable<CheckResource>) new CheckResource[1]
      {
        new CheckResource(resourceId, resourceType)
      }).FirstOrDefault<ResourceLockRequest>();
    }

    public List<ResourceLockRequest> GetActiveResourceLocks(
      IVssRequestContext requestContext,
      IEnumerable<CheckResource> resources)
    {
      using (new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (GetActiveResourceLocks)))
      {
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
          return component.GetActiveResourceLocks(resources);
      }
    }

    public List<ResourceLockRequest> QueueResourceLockRequests(
      IVssRequestContext requestContext,
      IEnumerable<ResourceLockRequest> requests)
    {
      using (new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (QueueResourceLockRequests)))
      {
        bool flag = requestContext.IsFeatureEnabled("DistributedTask.UseQueueResourceLockRequestsV2Sproc");
        List<ResourceLockRequest> resourceLockRequestList;
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
          resourceLockRequestList = component.Version >= 5 & flag ? component.QueueResourceLockRequestsV2(requests) : component.QueueResourceLockRequests(requests);
        PlatformResourceLockService.RunResourceLockAssignmentJob(requestContext, new int?(3));
        foreach (ResourceLockRequest request in requests)
          requestContext.TraceAlways(10015205, nameof (PlatformResourceLockService), "Requested exclusive lock (type {0}) for resource {1}:{2}", (object) request.LockType, (object) request.ResourceId, (object) request.ResourceType);
        return resourceLockRequestList;
      }
    }

    public ResourceLockRequest UpdateResourceLockRequest(
      IVssRequestContext requestContext,
      long requestId,
      ResourceLockStatus status,
      DateTime? assignTime = null,
      DateTime? finishTime = null)
    {
      return this.UpdateResourceLockRequests(requestContext, (IEnumerable<ResourceLockRequest>) new ResourceLockRequest[1]
      {
        new ResourceLockRequest()
        {
          RequestId = requestId,
          Status = status,
          AssignTime = assignTime,
          FinishTime = finishTime
        }
      }).FirstOrDefault<ResourceLockRequest>();
    }

    public List<ResourceLockRequest> UpdateResourceLockRequests(
      IVssRequestContext requestContext,
      IEnumerable<ResourceLockRequest> requestsToUpdate)
    {
      using (new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (UpdateResourceLockRequests)))
      {
        List<ResourceLockRequest> resourceLockRequestList;
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
          resourceLockRequestList = component.UpdateResourceLockRequests(requestsToUpdate);
        PlatformResourceLockService.RunResourceLockAssignmentJob(requestContext);
        return resourceLockRequestList;
      }
    }

    public async Task<List<ResourceLockRequest>> FreeResourceLocksAsync(
      IVssRequestContext requestContext,
      Guid planId,
      string nodeName = null,
      int? nodeAttempt = null)
    {
      List<ResourceLockRequest> resourceLockRequestList1;
      using (new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (FreeResourceLocksAsync)))
      {
        List<ResourceLockRequest> resourceLockRequestList2;
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
          resourceLockRequestList2 = await component.FreeResourceLocksAsync(planId, nodeName, nodeAttempt);
        PlatformResourceLockService.RunResourceLockAssignmentJob(requestContext);
        resourceLockRequestList1 = resourceLockRequestList2;
      }
      return resourceLockRequestList1;
    }

    public async Task<AssignResourceLockRequestsResult> AssignResourceLockRequestsAsync(
      IVssRequestContext requestContext)
    {
      AssignResourceLockRequestsResult lockRequestsResult;
      using (new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (AssignResourceLockRequestsAsync)))
      {
        AssignResourceLockRequestsResult resourceLockRequests;
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
          resourceLockRequests = await component.GetAssignableResourceLockRequests();
        List<ResourceLockRequest> requestsToUpdate = new List<ResourceLockRequest>();
        foreach (ResourceLockRequest assignedRequest in resourceLockRequests.AssignedRequests)
        {
          if (this.UpdateCheckRun(requestContext, assignedRequest, CheckRunStatus.Approved))
            requestsToUpdate.Add(assignedRequest);
        }
        foreach (ResourceLockRequest canceledRequest in resourceLockRequests.CanceledRequests)
        {
          if (this.UpdateCheckRun(requestContext, canceledRequest, CheckRunStatus.Canceled))
            requestsToUpdate.Add(canceledRequest);
        }
        if (requestsToUpdate.Count > 0)
          this.UpdateResourceLockRequests(requestContext, (IEnumerable<ResourceLockRequest>) requestsToUpdate);
        resourceLockRequests.ChecksUpdateSucceeded = requestsToUpdate.Count == resourceLockRequests.AssignedRequests.Count + resourceLockRequests.CanceledRequests.Count;
        lockRequestsResult = resourceLockRequests;
      }
      return lockRequestsResult;
    }

    private bool UpdateCheckRun(
      IVssRequestContext requestContext,
      ResourceLockRequest request,
      CheckRunStatus updateStatus)
    {
      using (requestContext.CreateOrchestrationIdScope(PlatformResourceLockService.GetCheckpointInstanceId(request)))
      {
        try
        {
          ICheckSuiteService service = requestContext.GetService<ICheckSuiteService>();
          CheckRunResult checkRunResult = new CheckRunResult()
          {
            Status = updateStatus
          };
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId = request.ProjectId;
          Guid checkRunId = request.CheckRunId;
          CheckRunResult result = checkRunResult;
          service.UpdateCheckRun(requestContext1, projectId, checkRunId, result);
          switch (updateStatus)
          {
            case CheckRunStatus.Approved:
              requestContext.TraceAlways(10015206, nameof (PlatformResourceLockService), "Assigned exclusive lock (type {0}) for resource {1}:{2}", (object) request.LockType, (object) request.ResourceId, (object) request.ResourceType);
              break;
            case CheckRunStatus.Canceled:
              requestContext.TraceAlways(10015206, nameof (PlatformResourceLockService), "Exclusive lock not assigned for resource {0}:{1}. Canceling.", (object) request.ResourceId, (object) request.ResourceType);
              break;
          }
          return true;
        }
        catch (CheckSuiteIsAlreadyCompleted ex)
        {
          requestContext.TraceError(10015207, nameof (PlatformResourceLockService), "Check suite is already completed {0}", (object) request.RequestId);
          return true;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10015207, nameof (PlatformResourceLockService), ex);
          requestContext.TraceError(10015207, nameof (PlatformResourceLockService), "Failed to update check service for request {0}", (object) request.RequestId);
          return false;
        }
      }
    }

    public void CleanupRequestTable(IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (CleanupRequestTable)))
      {
        int daysToKeep = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ResourceLocks/DaysToKeepRequests", 14);
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
          component.CleanupResourceLockTable(daysToKeep);
      }
    }

    public async Task CleanupAbanonedResourceLocks(IVssRequestContext requestContext)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (PlatformResourceLockService), nameof (CleanupAbanonedResourceLocks));
      try
      {
        List<ResourceLockRequest> activeOrPendingLocks = new List<ResourceLockRequest>();
        using (ResourceLockComponent component = requestContext.CreateComponent<ResourceLockComponent>())
        {
          activeOrPendingLocks.AddRange((IEnumerable<ResourceLockRequest>) component.GetActiveResourceLocks((IEnumerable<CheckResource>) null));
          AssignResourceLockRequestsResult resourceLockRequests = await component.GetAssignableResourceLockRequests();
          activeOrPendingLocks.AddRange((IEnumerable<ResourceLockRequest>) resourceLockRequests.AssignedRequests);
          activeOrPendingLocks.AddRange((IEnumerable<ResourceLockRequest>) resourceLockRequests.CanceledRequests);
        }
        TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
        List<ResourceLockRequest> requestsToUpdate = new List<ResourceLockRequest>();
        foreach (ResourceLockRequest resourceLockRequest in activeOrPendingLocks)
        {
          TaskOrchestrationPlan plan = taskHub.GetPlan(requestContext, resourceLockRequest.ProjectId, resourceLockRequest.PlanId);
          if (plan == null || plan.State == TaskOrchestrationPlanState.Completed)
          {
            requestsToUpdate.Add(resourceLockRequest);
            resourceLockRequest.Status = ResourceLockStatus.Abandoned;
          }
        }
        if (requestsToUpdate.Count > 0)
          this.UpdateResourceLockRequests(requestContext, (IEnumerable<ResourceLockRequest>) requestsToUpdate);
        foreach (ResourceLockRequest resourceLockRequest in requestsToUpdate)
        {
          using (requestContext.CreateOrchestrationIdScope(resourceLockRequest.PlanId.ToString()))
            requestContext.TraceError(10015208, nameof (PlatformResourceLockService), "Freed an abandoned resource lock. RequestId:{0}, ResourceId:{1}, ResourceType:{2}, PlanId:{3}, StageName:{4}, Attempt:{5}, Type:{6}", (object) resourceLockRequest.RequestId, (object) resourceLockRequest.ResourceId, (object) resourceLockRequest.ResourceType, (object) resourceLockRequest.PlanId, (object) resourceLockRequest.NodeName, (object) resourceLockRequest.NodeAttempt, (object) resourceLockRequest.LockType);
        }
        activeOrPendingLocks = (List<ResourceLockRequest>) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private static void RunResourceLockAssignmentJob(IVssRequestContext requestContext, int? delay = null)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      if (delay.HasValue)
        service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          TaskConstants.ResourceAssignmentJobId
        }, delay.Value);
      else
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          TaskConstants.ResourceAssignmentJobId
        });
    }

    private static string GetCheckpointInstanceId(ResourceLockRequest request)
    {
      PipelineIdGenerator pipelineIdGenerator = new PipelineIdGenerator();
      return pipelineIdGenerator.GetInstanceName(new string[3]
      {
        request.PlanId.ToString("D"),
        pipelineIdGenerator.GetStageInstanceName(request.NodeName, request.NodeAttempt),
        "checkpoint"
      }).ToLower();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}

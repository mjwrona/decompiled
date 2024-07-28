// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentJobRequestExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskAgentJobRequestExtensions
  {
    public static string GetParallelismTag(this TaskAgentJobRequest jobRequest)
    {
      string str;
      return jobRequest.Data.TryGetValue(TaskAgentRequestConstants.ParallelismTag, out str) ? str : "Private";
    }

    public static bool GetIsScheduledTag(this TaskAgentJobRequest jobRequest)
    {
      string str;
      bool result;
      return jobRequest.Data.TryGetValue(TaskAgentRequestConstants.IsScheduledKey, out str) && bool.TryParse(str, out result) && result;
    }

    public static string GetPerformanceMetrics(this TaskAgentJobRequest jobRequest)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("DistributedTaskQueuedTime", jobRequest.QueueTime.ToString("o"));
      DateTime? assignTime = jobRequest.AssignTime;
      ref DateTime? local = ref assignTime;
      dictionary.Add("DistributedTaskAssignedTime", local.HasValue ? local.GetValueOrDefault().ToString("o") : (string) null);
      dictionary.Add("UserDelayed", jobRequest.UserDelayed.ToString());
      return dictionary.Serialize<Dictionary<string, string>>();
    }

    public static IList<TaskAgentJobRequest> PopulateReferenceLinks(
      this IList<TaskAgentJobRequest> requests,
      IVssRequestContext requestContext,
      int poolId)
    {
      Dictionary<int, TaskAgentReference> resolvedAgents = new Dictionary<int, TaskAgentReference>();
      foreach (TaskAgentJobRequest request in (IEnumerable<TaskAgentJobRequest>) requests)
        request.PopulateReferenceLinks(requestContext, poolId, (IDictionary<int, TaskAgentReference>) resolvedAgents);
      return requests;
    }

    public static TaskAgentJobRequest PopulateReferenceLinks(
      this TaskAgentJobRequest request,
      IVssRequestContext requestContext,
      int poolId,
      IDictionary<int, TaskAgentReference> resolvedAgents = null)
    {
      using (new MethodScope(requestContext, nameof (TaskAgentJobRequestExtensions), nameof (PopulateReferenceLinks)))
      {
        if (request == null)
          return (TaskAgentJobRequest) null;
        foreach (TaskAgentReference matchedAgent in request.MatchedAgents)
          matchedAgent.PopulateReferenceLinks<TaskAgentReference>(requestContext, poolId, resolvedAgents);
        if (request.ReservedAgent != null)
          request.ReservedAgent.PopulateReferenceLinks<TaskAgentReference>(requestContext, poolId, resolvedAgents);
        return request;
      }
    }

    public static async Task AddIssueAsync(
      this TaskAgentJobRequest request,
      IVssRequestContext requestContext,
      IssueType issueType,
      string issueMessage)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskAgentJobRequestExtensions), nameof (AddIssueAsync));
      try
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ServiceInstanceType() == request.ServiceOwner)
        {
          TaskHub taskHub = requestContext.GetService<DistributedTaskHubService>().GetTaskHub(requestContext, request.PlanType);
          Timeline timeline = taskHub.GetTimeline(requestContext, request.ScopeId, request.PlanId, Guid.Empty, includeRecords: true);
          if (timeline != null)
          {
            TimelineRecord record = timeline.Records.FirstOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.Id == request.JobId));
            if (record != null)
            {
              record.AddIssue(issueType, issueMessage);
              taskHub.UpdateTimeline(requestContext, request.ScopeId, request.PlanId, timeline.Id, (IList<TimelineRecord>) new TimelineRecord[1]
              {
                record
              });
            }
          }
        }
        else
        {
          TaskHttpClient taskHttpClient = requestContext.GetClient<TaskHttpClient>(request.ServiceOwner, request.HostId);
          TaskHttpClient taskHttpClient1 = taskHttpClient;
          Guid scopeId = request.ScopeId;
          string planType = request.PlanType;
          Guid planId = request.PlanId;
          Guid empty = Guid.Empty;
          bool? nullable = new bool?(true);
          int? changeId = new int?();
          bool? includeRecords = nullable;
          CancellationToken cancellationToken = new CancellationToken();
          Timeline timelineAsync = await taskHttpClient1.GetTimelineAsync(scopeId, planType, planId, empty, changeId, includeRecords, cancellationToken: cancellationToken);
          if (timelineAsync != null)
          {
            TimelineRecord record = timelineAsync.Records.FirstOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.Id == request.JobId));
            if (record != null)
            {
              record.AddIssue(issueType, issueMessage);
              List<TimelineRecord> timelineRecordList = await taskHttpClient.UpdateTimelineRecordsAsync(request.ScopeId, request.PlanType, request.PlanId, timelineAsync.Id, (IEnumerable<TimelineRecord>) new TimelineRecord[1]
              {
                record
              });
            }
          }
          taskHttpClient = (TaskHttpClient) null;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceError(nameof (TaskAgentJobRequestExtensions), "Failed to post issue \"{0}\" to job {1} for plan {2}", (object) issueMessage, (object) request.JobId, (object) request.PlanId);
        requestContext.TraceException(nameof (TaskAgentJobRequestExtensions), ex);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public static async Task AddMessageAsync(
      this TaskAgentJobRequest request,
      IVssRequestContext requestContext,
      AgentRequestMessage message)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (TaskAgentJobRequestExtensions), nameof (AddMessageAsync));
      try
      {
        if (message.Verbosity != AgentRequestMessageVerbosity.Warning)
        {
          if (message.Verbosity != AgentRequestMessageVerbosity.Error)
            goto label_6;
        }
        await request.AddIssueAsync(requestContext, message.Verbosity == AgentRequestMessageVerbosity.Error ? IssueType.Error : IssueType.Warning, message.Message);
      }
      finally
      {
        methodScope.Dispose();
      }
label_6:
      methodScope = new MethodScope();
    }

    public static async Task<AgentRequestJob> GetAgentRequestJobAsync(
      this TaskAgentJobRequest request,
      IVssRequestContext requestContext)
    {
      AgentRequestJob agentRequestJobAsync;
      using (new MethodScope(requestContext, nameof (TaskAgentJobRequestExtensions), nameof (GetAgentRequestJobAsync)))
      {
        AgentRequestJob agentRequestJob = (AgentRequestJob) null;
        try
        {
          if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ServiceInstanceType() == request.ServiceOwner)
            agentRequestJob = PoolProviderAgentRequestJobHelper.GetAgentRequestJob(requestContext, request, await requestContext.GetService<DistributedTaskHubService>().GetTaskHub(requestContext, request.PlanType).GetAgentRequestJobAsync(requestContext, request.ScopeId, request.OrchestrationId));
          else
            agentRequestJob = PoolProviderAgentRequestJobHelper.ConvertJobContract(requestContext, request, await requestContext.GetClient<TaskHttpClient>(request.ServiceOwner, request.HostId).GetAgentRequestJobAsync(request.ScopeId, request.PlanType, request.OrchestrationId));
        }
        catch (Exception ex)
        {
          requestContext.TraceError(nameof (TaskAgentJobRequestExtensions), "Failed to get Job associated with request \"{0}\"", (object) request.RequestId);
          requestContext.TraceException(nameof (TaskAgentJobRequestExtensions), ex);
        }
        agentRequestJobAsync = agentRequestJob;
      }
      return agentRequestJobAsync;
    }

    public static async Task<TaskOrchestrationPlan> GetPlanAsync(
      this TaskAgentJobRequest request,
      IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (TaskAgentJobRequestExtensions), nameof (GetPlanAsync)))
        return requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ServiceInstanceType() == request.ServiceOwner ? await requestContext.GetService<DistributedTaskHubService>().GetTaskHub(requestContext, request.PlanType).GetPlanAsync(requestContext, request.ScopeId, request.PlanId) : await requestContext.GetClient<TaskHttpClient>(request.ServiceOwner, request.HostId).GetPlanAsync(request.ScopeId, request.PlanType, request.PlanId);
    }

    internal static async Task<bool> TryUpdateAgentSpecificationForPoolAsync(
      this TaskAgentJobRequest request,
      IVssRequestContext requestContext,
      TaskAgentPoolReference pool)
    {
      string vmImage = (string) null;
      string requestVmImage = (string) null;
      string vmImageByPoolName = HostedTaskAgentExtension.GetMachineImageLabel(requestContext, pool.Name);
      if (!string.IsNullOrEmpty(vmImageByPoolName))
      {
        if (request.AgentSpecification == null)
        {
          request.AgentSpecification = new JObject();
          request.AgentSpecification.Add((object) new JProperty("VMImage", (object) vmImageByPoolName));
        }
        else
        {
          JToken jtoken;
          if (request.AgentSpecification.TryGetValue("VMImage", StringComparison.OrdinalIgnoreCase, out jtoken))
          {
            requestVmImage = jtoken.Value<string>();
            if (!requestVmImage.Equals(vmImageByPoolName, StringComparison.InvariantCultureIgnoreCase))
            {
              requestContext.TraceAlways(10015264, TraceLevel.Info, nameof (TaskAgentJobRequestExtensions), nameof (TryUpdateAgentSpecificationForPoolAsync), "Replacing AgentSpec " + requestVmImage + " with " + vmImageByPoolName);
              request.AgentSpecification.ReplaceAll((object) new JProperty("VMImage", (object) vmImageByPoolName));
            }
          }
          else
            request.AgentSpecification.Add((object) new JProperty("VMImage", (object) vmImageByPoolName));
        }
        vmImage = vmImageByPoolName;
      }
      else
      {
        JToken jtoken;
        if (request.AgentSpecification != null && request.AgentSpecification.TryGetValue("VMImage", StringComparison.OrdinalIgnoreCase, out jtoken))
          vmImage = jtoken.Value<string>();
      }
      bool result = false;
      string imageLabel = (string) null;
      if (!string.IsNullOrEmpty(vmImage))
      {
        InternalCloudAgentDefinition definitionAsync = await FindDefinitionAsync(requestContext, vmImage);
        IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (definitionAsync == null)
          definitionAsync = await FindDefinitionAsync(deploymentContext, vmImage);
        if (definitionAsync != null)
        {
          imageLabel = TaskAgentJobRequestExtensions.UpdateImageForRollout(deploymentContext, requestContext.ServiceHost.InstanceId, definitionAsync.Identifier, definitionAsync.ImageLabel);
          if (request.AgentSpecification.ContainsKey("ImageLabel"))
            request.AgentSpecification.Remove("ImageLabel");
          request.AgentSpecification.Add((object) new JProperty("ImageLabel", (object) imageLabel));
          result = true;
        }
        deploymentContext = (IVssRequestContext) null;
      }
      requestContext.TraceAlways(10015263, TraceLevel.Info, "DistributedTask", nameof (TryUpdateAgentSpecificationForPoolAsync), string.Format("{0}. poolName: {1}. poolIsHosted: {2}. vmImageByPoolName: {3}. requestVmImage: {4}. vmImage: {5}. imageLabel: {6}. result: {7}.", (object) nameof (TryUpdateAgentSpecificationForPoolAsync), (object) pool.Name, (object) pool.IsHosted, (object) vmImageByPoolName, (object) requestVmImage, (object) vmImage, (object) imageLabel, (object) result));
      bool flag = result;
      vmImage = (string) null;
      requestVmImage = (string) null;
      vmImageByPoolName = (string) null;
      imageLabel = (string) null;
      return flag;

      static async Task<InternalCloudAgentDefinition> FindDefinitionAsync(
        IVssRequestContext rc,
        string agentSpec)
      {
        return await rc.GetService<IInternalCloudAgentDefinitionService>().GetInternalCloudAgentDefinitionAsync(rc, agentSpec);
      }
    }

    private static string UpdateImageForRollout(
      IVssRequestContext deploymentContext,
      Guid hostId,
      string identifier,
      string imageLabel)
    {
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      string str = service.GetValue<string>(deploymentContext, (RegistryQuery) string.Format(RegistryKeys.InternalCloudAgentDefinitionTargetImageLabel, (object) identifier), false, (string) null);
      int num = service.GetValue<int>(deploymentContext, (RegistryQuery) string.Format(RegistryKeys.InternalCloudAgentDefinitionTargetPercent, (object) identifier), false, 0);
      return !string.IsNullOrEmpty(str) && num > 0 && Math.Abs(hostId.GetHashCode() % 100) <= num ? str : imageLabel;
    }

    internal static void UpdateForDisableInlineRedirect(
      this AgentRequest agentRequest,
      IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.DisableMMSInlineRedirect"))
        return;
      requestContext.TraceVerbose(nameof (TaskAgentJobRequestExtensions), "Adding RequestTag to disable MMS Inline Redirect");
      JToken jtoken;
      JArray jarray;
      if (agentRequest.AgentSpecification.TryGetValue("RequestTags", out jtoken))
      {
        jarray = (JArray) jtoken;
      }
      else
      {
        jarray = new JArray();
        agentRequest.AgentSpecification.Add("RequestTags", (JToken) jarray);
      }
      jarray.Add((JToken) "PreventInlineRedirect");
    }
  }
}

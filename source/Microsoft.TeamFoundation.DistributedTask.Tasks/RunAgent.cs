// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.RunAgent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public sealed class RunAgent : TaskOrchestration<bool, RunAgentInput, object, string>
  {
    public static readonly string Name = nameof (RunAgent);
    public static readonly string Version = "1.0";
    private TaskCompletionSource<AgentRequestProvisioningResult> m_agentProvisionedEvent;
    private TaskCompletionSource<DateTime> m_agentConnectedEvent;
    private TaskCompletionSource<long> m_requestAssignedEvent;
    private TaskCompletionSource<long> m_requestFinishedEvent;
    private TaskCompletionSource<string> m_deprovisionEvent;
    private RunAgentSettings m_settings;
    private IAgentExtension m_agentClient;
    private IAgentCloudExtension m_cloudClient;
    private IPoolProviderExtension m_providerClient;
    private IRunAgentSettingsExtension m_agentSettingsClient;

    public RunAgent()
    {
      this.m_agentProvisionedEvent = new TaskCompletionSource<AgentRequestProvisioningResult>();
      this.m_agentConnectedEvent = new TaskCompletionSource<DateTime>();
      this.m_requestAssignedEvent = new TaskCompletionSource<long>();
      this.m_requestFinishedEvent = new TaskCompletionSource<long>();
      this.m_deprovisionEvent = new TaskCompletionSource<string>();
    }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "AgentProvisioned":
          if (this.m_agentProvisionedEvent.TrySetResult((AgentRequestProvisioningResult) input))
            break;
          context.TraceError("Received duplicate AgentProvisioned event!");
          break;
        case "AgentConnected":
          if (this.m_agentConnectedEvent.TrySetResult((DateTime) input))
            break;
          context.TraceError("Received duplicate AgentConnected event!");
          break;
        case "RequestAssigned":
          this.m_requestAssignedEvent.TrySetResult((long) input);
          break;
        case "RequestFinished":
          this.m_requestFinishedEvent.TrySetResult((long) input);
          break;
        case "Deprovision":
          this.m_deprovisionEvent.TrySetResult((string) input);
          break;
      }
    }

    public override async Task<bool> RunTask(OrchestrationContext context, RunAgentInput input)
    {
      bool deprovision = false;
      this.EnsureClients(context, input.ActivityDispatcherShardsCount);
      this.m_settings = await this.m_agentSettingsClient.GetAgentSettingsAsync();
      context.TraceInfo(string.Format("Getting information about Agent Cloud Request {0} for Agent Cloud {1}", (object) input.AgentCloudRequestId, (object) input.AgentCloudId));
      TaskAgentCloud agentCloud = await this.m_cloudClient.GetAgentCloudAsync(input.AgentCloudId);
      TaskAgentCloudRequest agentCloudRequest = await this.m_cloudClient.GetAgentCloudRequestAsync(input.AgentCloudId, input.AgentCloudRequestId);
      context.TraceInfo(string.Format("Getting information about Agent {0} in Agent Pool {1}", (object) input.AgentId, (object) input.AgentPoolId));
      TaskAgentPool pool = await this.m_agentClient.GetAgentPoolAsync(input.AgentPoolId);
      TaskAgent agent = await this.m_agentClient.GetAgentAsync(input.AgentPoolId, input.AgentId);
      string dispatcherType = !input.ShardActivityDispatcherByPoolProvider ? (agentCloud.Internal.GetValueOrDefault() ? "Internal" : (string) null) : (!(AgentCloudType.OneBranch.AgentCloudName == agentCloud.Name) ? AgentCloudType.ByPoolType(agentCloud.Type).DispatcherType : AgentCloudType.OneBranch.DispatcherType);
      this.m_providerClient = context.CreateShardedClient<IPoolProviderExtension>(true, input.ActivityDispatcherShardsCount, dispatcherType);
      string orchestrationId = agent.AssignedRequest?.OrchestrationId;
      context.Trace(10018500, TraceLevel.Info, string.Format("Build/AgentCloud oid mapping: Build:{0} AgentCloud:{1} agentCloud.Name:{2} agentCloud.Type:{3} dispatcherType:{4}", (object) orchestrationId, (object) input.AgentCloudRequestId, (object) agentCloud?.Name, (object) agentCloud?.Type, (object) dispatcherType));
      DateTime? nullable = agentCloudRequest.ProvisionRequestTime;
      int provisionAttempts;
      DateTime retryAfter;
      CancellationTokenSource timerCancel;
      if (!nullable.HasValue)
      {
        context.TraceInfo("Preparing to send Provision Request");
        provisionAttempts = 0;
        while (agent.AssignedRequest != null)
        {
          context.TraceInfo(string.Format("Sending provision attempt {0}", (object) (provisionAttempts + 1)));
          await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.SendingProvisioningRequest((object) agentCloud.AcquireAgentEndpoint), AgentRequestMessageVerbosity.Info);
          AgentProvisionResponse response = await this.m_providerClient.ProvisionAgentAsync(input.AgentCloudId, agentCloudRequest, (TaskAgentPoolReference) pool, (TaskAgentReference) agent);
          ++provisionAttempts;
          if (response.ResponseType == ProvisionResponseType.Success)
          {
            context.TraceInfo(string.Format("Provision request succeeded on attempt {0}", (object) provisionAttempts));
            await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.ProvisioningRequestAccepted(), AgentRequestMessageVerbosity.Info);
            agentCloudRequest = await this.m_cloudClient.SetAgentCloudRequestProvisionSentAsync(input.AgentCloudId, input.AgentCloudRequestId, response.SentAt, response.AgentData);
            agent = await this.m_agentClient.SetAgentProvisioningStateAsync(input.AgentPoolId, input.AgentId, "Provisioning");
            goto label_35;
          }
          else
          {
            if (response.ResponseType == ProvisionResponseType.RetryAfter)
            {
              retryAfter = this.GetRetryAfter(context, input, response.RetryAfter);
              context.TraceWarning(string.Format("Received TooManyRequests from remote provider during provisioning, retrying at {0}", (object) retryAfter));
              await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.PoolProviderDelayingProvision((object) retryAfter), AgentRequestMessageVerbosity.Warning);
              timerCancel = new CancellationTokenSource();
              Task task = await Task.WhenAny((Task) this.m_requestFinishedEvent.Task, (Task) this.m_deprovisionEvent.Task, (Task) context.CreateTimer<string>(retryAfter, (string) null, timerCancel.Token));
              if (task == this.m_requestFinishedEvent.Task)
              {
                timerCancel.Cancel();
                context.TraceInfo("Request was finished before during RetryAfter wait. Deprovisioning Agent.");
                deprovision = true;
                goto label_35;
              }
              else if (task == this.m_deprovisionEvent.Task)
              {
                timerCancel.Cancel();
                string result = this.m_deprovisionEvent.Task.Result;
                await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.ReceivedDeprovisionEvent((object) result), AgentRequestMessageVerbosity.Warning);
                deprovision = true;
                goto label_35;
              }
              else
                timerCancel = (CancellationTokenSource) null;
            }
            else if (response.ResponseType == ProvisionResponseType.Fail)
            {
              context.TraceError("Provision responsed to provision request with failure: " + response.ErrorMessage);
              await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.ProvisionRequestFailedByProvider(), AgentRequestMessageVerbosity.Error);
              deprovision = true;
              goto label_35;
            }
            else if (response.ResponseType == ProvisionResponseType.FailedToSendMessage)
            {
              context.TraceError("Failed sending provision request to provider: " + response.ErrorMessage);
              await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.ProvisionRequestFailedToSend((object) response.ErrorMessage), AgentRequestMessageVerbosity.Warning);
            }
            if (provisionAttempts >= this.m_settings.MaxProvisionAttempts)
            {
              context.TraceError(string.Format("Reached max provision attempt count of {0}. Deprovisiong Agent.", (object) this.m_settings.MaxProvisionAttempts));
              await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.FailedProvisiongTooManyTimes((object) provisionAttempts, (object) this.m_settings.MaxDeprovisionAttempts), AgentRequestMessageVerbosity.Error);
              deprovision = true;
              goto label_35;
            }
            else
              response = (AgentProvisionResponse) null;
          }
        }
        context.TraceInfo("Agent request was cancelled before provision attempt. Deprovisioning Agent.");
        deprovision = true;
      }
label_35:
      if (!deprovision)
      {
        nullable = agentCloudRequest.AgentConnectedTime;
        if (!nullable.HasValue)
        {
          context.TraceInfo("Waiting on agent to connect");
          await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.WaitingForAgentConnection(), AgentRequestMessageVerbosity.Info);
          timerCancel = new CancellationTokenSource();
          int? acquisitionTimeout = agentCloud.AcquisitionTimeout;
          TimeSpan timeSpan;
          if (!acquisitionTimeout.HasValue)
          {
            timeSpan = this.m_settings.DefaultAgentConnectTimeout;
          }
          else
          {
            acquisitionTimeout = agentCloud.AcquisitionTimeout;
            timeSpan = TimeSpan.FromMinutes((double) acquisitionTimeout.Value);
          }
          TimeSpan agentConnectionTimeout = timeSpan;
          Task<string> connectedTimeout = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(agentConnectionTimeout), (string) null, timerCancel.Token);
          int num;
          do
          {
            Task<string> timer = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(this.m_settings.AgentConnectionRefreshRate), (string) null, timerCancel.Token);
            List<Task> taskList = new List<Task>()
            {
              (Task) this.m_agentConnectedEvent.Task,
              (Task) this.m_requestFinishedEvent.Task,
              (Task) this.m_deprovisionEvent.Task,
              (Task) connectedTimeout,
              (Task) timer
            };
            nullable = agentCloudRequest.ProvisionedTime;
            if (!nullable.HasValue)
              taskList.Insert(0, (Task) this.m_agentProvisionedEvent.Task);
            Task task = await Task.WhenAny((IEnumerable<Task>) taskList);
            if (task == this.m_agentProvisionedEvent.Task)
            {
              context.TraceInfo("Agent provisioned message received");
              AgentRequestProvisioningResult result = this.m_agentProvisionedEvent.Task.Result;
              agentCloudRequest = await this.m_cloudClient.SetAgentCloudRequestProvisionedAsync(input.AgentCloudId, input.AgentCloudRequestId, result.ProvisioningFinishTime);
              if (result.ProvisioningResult == RequestProvisioningResult.Failure)
              {
                context.TraceError("Provisiong reported as not a success from the remote provider. Deprovisioning Agent.");
                await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.ProvisionRequestFailedByProvider(), AgentRequestMessageVerbosity.Error);
                deprovision = true;
                timerCancel.Cancel();
                goto label_73;
              }
              else
              {
                await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.ProvisioningSucceeded((object) result.ProvisioningFinishTime), AgentRequestMessageVerbosity.Info);
                result = (AgentRequestProvisioningResult) null;
              }
            }
            else if (task == this.m_agentConnectedEvent.Task)
            {
              context.TraceInfo("Agent connected succesfully");
              timerCancel.Cancel();
              retryAfter = this.m_agentConnectedEvent.Task.Result;
              await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.AgentConnectedSuccessfully((object) retryAfter), AgentRequestMessageVerbosity.Info);
              agentCloudRequest = await this.m_cloudClient.SetAgentCloudRequestAgentConnectedAsync(input.AgentCloudId, input.AgentCloudRequestId, retryAfter);
              goto label_73;
            }
            else if (task == this.m_requestFinishedEvent.Task)
            {
              context.TraceInfo("Request was finished before agent connected, cancelling");
              timerCancel.Cancel();
              deprovision = true;
              goto label_73;
            }
            else if (task == this.m_deprovisionEvent.Task)
            {
              context.TraceWarning("Received deprovision event while waiting for agent to connect. Deprovisioning Agent.");
              timerCancel.Cancel();
              string result = this.m_deprovisionEvent.Task.Result;
              await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.ReceivedDeprovisionEvent((object) result), AgentRequestMessageVerbosity.Warning);
              deprovision = true;
              goto label_73;
            }
            else if (task == connectedTimeout)
            {
              context.TraceError(string.Format("Agent did not connect within the timeout of {0} min(s).", (object) agentConnectionTimeout.TotalMinutes));
              await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.AgentConnectionTimeout((object) agentConnectionTimeout.TotalMinutes), AgentRequestMessageVerbosity.Error);
              timerCancel.Cancel();
              deprovision = true;
              goto label_73;
            }
            else
            {
              context.TraceInfo("Refreshing agent information, to see if agent connected event missed.");
              agent = await this.m_agentClient.GetAgentAsync(input.AgentPoolId, input.AgentId);
              if (agent.Status == TaskAgentStatus.Online)
              {
                context.TraceInfo("After refresh, agent was online. Setting agent connection time");
                timerCancel.Cancel();
                IAgentCloudExtension cloudClient1 = this.m_cloudClient;
                int agentCloudId1 = input.AgentCloudId;
                Guid agentCloudRequestId1 = input.AgentCloudRequestId;
                nullable = agent.StatusChangedOn;
                DateTime connectionTime = nullable ?? context.CurrentUtcDateTime;
                agentCloudRequest = await cloudClient1.SetAgentCloudRequestAgentConnectedAsync(agentCloudId1, agentCloudRequestId1, connectionTime);
                IAgentCloudExtension cloudClient2 = this.m_cloudClient;
                int agentCloudId2 = input.AgentCloudId;
                Guid agentCloudRequestId2 = input.AgentCloudRequestId;
                nullable = agent.StatusChangedOn;
                string message = TasksResources.AgentConnectedSuccessfully((object) (nullable ?? context.CurrentUtcDateTime));
                await cloudClient2.AddRequestMessageAsync(agentCloudId2, agentCloudRequestId2, message, AgentRequestMessageVerbosity.Info);
                goto label_73;
              }
              else
              {
                TaskAgentJobRequest assignedRequest = agent.AssignedRequest;
                if (assignedRequest == null)
                {
                  num = 1;
                }
                else
                {
                  long requestId = assignedRequest.RequestId;
                  num = 0;
                }
              }
            }
          }
          while (num == 0);
          context.TraceInfo("After refresh, no assigned request found associated with the agent. Deprovisioning.");
          deprovision = true;
label_73:
          timerCancel = (CancellationTokenSource) null;
          agentConnectionTimeout = new TimeSpan();
          connectedTimeout = (Task<string>) null;
        }
      }
      TaskAgentJobRequest assignedRequest1 = agent.AssignedRequest;
      int num1;
      if (assignedRequest1 == null)
      {
        num1 = 0;
      }
      else
      {
        long requestId = assignedRequest1.RequestId;
        num1 = 1;
      }
      bool hasAssignedRequest = num1 != 0;
      TaskAgentJobRequest assignedRequest2 = agent.AssignedRequest;
      int num2;
      if (assignedRequest2 == null)
      {
        num2 = 0;
      }
      else
      {
        nullable = assignedRequest2.ReceiveTime;
        num2 = nullable.HasValue ? 1 : 0;
      }
      bool sentNotification = num2 != 0;
      CancellationTokenSource activeRequestRefreshCancel = new CancellationTokenSource();
      while (!deprovision)
      {
        if (hasAssignedRequest)
        {
          if (!sentNotification)
          {
            context.TraceInfo("Request assigned to Agent. Disabling Agent and notifying SDK of Agent readyness.");
            agent = await this.m_agentClient.SetAgentProvisioningStateAsync(pool.Id, agent.Id, "RunningRequest");
            await this.m_agentClient.NotifyAgentReadyAsync(input.AgentPoolId, agent.AssignedRequest.RequestId);
            sentNotification = true;
          }
          Task task = await Task.WhenAny((Task) this.m_requestFinishedEvent.Task, (Task) this.m_deprovisionEvent.Task, (Task) context.CreateTimer<string>(context.CurrentUtcDateTime.Add(this.m_settings.ActiveRequestResfreshRate), (string) null, activeRequestRefreshCancel.Token));
          if (task == this.m_requestFinishedEvent.Task)
          {
            context.TraceInfo("Request finished.");
            activeRequestRefreshCancel.Cancel();
            hasAssignedRequest = false;
          }
          else if (task == this.m_deprovisionEvent.Task)
          {
            context.TraceInfo("Deprovision request recevied. Deprovisioning Agent.");
            activeRequestRefreshCancel.Cancel();
            string result = this.m_deprovisionEvent.Task.Result;
            await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.ReceivedDeprovisionEvent((object) result), AgentRequestMessageVerbosity.Warning);
            deprovision = true;
          }
          else
          {
            context.TraceInfo("Refreshing Agent to see if RequestFinished Event was missed.");
            agent = await this.m_agentClient.GetAgentAsync(input.AgentPoolId, input.AgentId);
            TaskAgentJobRequest assignedRequest3 = agent.AssignedRequest;
            int num3;
            if (assignedRequest3 == null)
            {
              num3 = 0;
            }
            else
            {
              long requestId = assignedRequest3.RequestId;
              num3 = 1;
            }
            hasAssignedRequest = num3 != 0;
          }
        }
        else
        {
          context.TraceInfo("No request is currently assigned to Agent, and only one request is supposed to be run. Deprovisioning Agent.");
          deprovision = true;
        }
      }
      PoolProviderResponse deprovisionResponse = (PoolProviderResponse) null;
      nullable = agentCloudRequest.ReleaseRequestTime;
      if (!nullable.HasValue)
      {
        TaskAgent taskAgent1 = await this.m_agentClient.SetAgentProvisioningStateAsync(input.AgentPoolId, input.AgentId, "Deprovisioning");
        provisionAttempts = 0;
        while (provisionAttempts < this.m_settings.MaxDeprovisionAttempts)
        {
          nullable = agentCloudRequest.ProvisionRequestTime;
          if (nullable.HasValue)
          {
            context.TraceInfo("Sending deprovision request to remote provider.");
            await this.m_cloudClient.AddRequestMessageAsync(input.AgentCloudId, input.AgentCloudRequestId, TasksResources.SendingDeprovisionRequest((object) agentCloud.ReleaseAgentEndpoint), AgentRequestMessageVerbosity.Info);
            deprovisionResponse = await this.m_providerClient.DeprovisionAgentAsync(input.AgentCloudId, agentCloudRequest.RequestId, agentCloudRequest.AgentData, pool.Name, orchestrationId);
          }
          ++provisionAttempts;
          if (provisionAttempts == 1)
          {
            context.TraceInfo("Cleaning up agent");
            TaskAgent taskAgent2 = await this.m_agentClient.SetAgentProvisioningStateAsync(input.AgentPoolId, input.AgentId, "Deallocated");
            await this.m_agentClient.ClearAgentSlot(input.AgentPoolId, input.AgentId);
            IAgentCloudExtension cloudClient = this.m_cloudClient;
            int agentCloudId = input.AgentCloudId;
            Guid agentCloudRequestId = input.AgentCloudRequestId;
            PoolProviderResponse providerResponse = deprovisionResponse;
            DateTime sentTime = providerResponse != null ? providerResponse.SentAt : context.CurrentUtcDateTime;
            agentCloudRequest = await cloudClient.SetAgentCloudRequestDeprovisionedAsync(agentCloudId, agentCloudRequestId, sentTime);
          }
          nullable = agentCloudRequest.ProvisionRequestTime;
          if (!nullable.HasValue)
          {
            context.TraceInfo("Provision message never sent. Setting cloud request to deprovisioned.");
            break;
          }
          PoolProviderResponse providerResponse1 = deprovisionResponse;
          if ((providerResponse1 != null ? (providerResponse1.ResponseType == ProvisionResponseType.Success ? 1 : 0) : 0) != 0)
          {
            context.TraceInfo("Deprovision successful");
            break;
          }
          PoolProviderResponse providerResponse2 = deprovisionResponse;
          if ((providerResponse2 != null ? (providerResponse2.ResponseType == ProvisionResponseType.RetryAfter ? 1 : 0) : 0) != 0)
          {
            timerCancel = new CancellationTokenSource();
            if (await Task.WhenAny((Task) context.CreateTimer<string>(this.GetRetryAfter(context, input, deprovisionResponse.RetryAfter), (string) null, timerCancel.Token)) == this.m_deprovisionEvent.Task)
            {
              timerCancel.Cancel();
              deprovision = true;
              break;
            }
            timerCancel = (CancellationTokenSource) null;
          }
          else
            context.TraceError("Failed sending deprovision message to remote provider: " + deprovisionResponse.ErrorMessage);
        }
        PoolProviderResponse providerResponse3 = deprovisionResponse;
        if ((providerResponse3 != null ? (providerResponse3.ResponseType != 0 ? 1 : 0) : 1) != 0)
          context.TraceError("Deprovision message never succesfully sent to remote provider");
      }
      bool flag = true;
      agentCloud = (TaskAgentCloud) null;
      agentCloudRequest = (TaskAgentCloudRequest) null;
      pool = (TaskAgentPool) null;
      agent = (TaskAgent) null;
      orchestrationId = (string) null;
      activeRequestRefreshCancel = (CancellationTokenSource) null;
      deprovisionResponse = (PoolProviderResponse) null;
      return flag;
    }

    private void EnsureClients(OrchestrationContext context, int activityDispatcherShardsCount)
    {
      this.m_agentClient = context.CreateShardedClient<IAgentExtension>(true, activityDispatcherShardsCount);
      this.m_cloudClient = context.CreateShardedClient<IAgentCloudExtension>(true, activityDispatcherShardsCount);
      this.m_agentSettingsClient = context.CreateShardedClient<IRunAgentSettingsExtension>(true, activityDispatcherShardsCount);
    }

    private DateTime GetRetryAfter(
      OrchestrationContext context,
      RunAgentInput input,
      DateTime retryAfter)
    {
      DateTime retryAfter1 = context.CurrentUtcDateTime.Add(this.m_settings.MaxRetryAfter);
      if (retryAfter < retryAfter1)
        return retryAfter;
      context.TraceWarning(string.Format("RetryAfter {0} is set to longer than max wait time of {1}. Only waiting max wait time.", (object) retryAfter, (object) retryAfter1));
      return retryAfter1;
    }
  }
}

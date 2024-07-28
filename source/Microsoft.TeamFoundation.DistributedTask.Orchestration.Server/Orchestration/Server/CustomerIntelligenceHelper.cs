// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.CustomerIntelligenceHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class CustomerIntelligenceHelper
  {
    private const string c_area = "TaskHub";
    private const int c_serverPoolId = -1;

    public static void PublishTaskHubPlanStarted(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan)
    {
      using (new MethodScope(requestContext, "CustomerIntelligence", nameof (PublishTaskHubPlanStarted)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("PlanArtifactUri", plan.ArtifactUri.ToString());
          intelligenceData.Add("PlanGroup", plan.PlanGroup);
          intelligenceData.Add("PlanId", (object) plan.PlanId);
          intelligenceData.Add("PlanTemplateType", plan.TemplateType.ToString());
          intelligenceData.Add("ScopeId", (object) plan.ScopeIdentifier);
          intelligenceData.Add("DefinitionId", (object) plan.Definition?.Id);
          intelligenceData.Add("TaskHubName", taskHub.Name);
          intelligenceData.Add("TaskHubVersion", (double) taskHub.Version);
          if (plan.ProcessType == OrchestrationProcessType.Pipeline)
          {
            PipelineProcess process = plan.GetProcess<PipelineProcess>();
            ArgumentUtility.CheckForNull<PipelineProcess>(process, "process");
            if (process.Stages.Count == 1 && process.Stages[0].Name.Equals(PipelineConstants.DefaultJobName))
            {
              intelligenceData.Add("StageCount", 0.0);
              intelligenceData.Add("PhaseCount", (double) process.Stages[0].Phases.Count);
            }
            else
            {
              intelligenceData.Add("StageCount", (double) process.Stages.Count);
              intelligenceData.Add("PhaseCount", (double) process.Stages.SelectMany<Stage, PhaseNode>((Func<Stage, IEnumerable<PhaseNode>>) (x => (IEnumerable<PhaseNode>) x.Phases)).Count<PhaseNode>());
            }
          }
          CustomerIntelligenceHelper.LogDataspaceDetails(requestContext, plan.ScopeIdentifier, intelligenceData);
          service.Publish(requestContext, "TaskHub", "PlanStarted", intelligenceData);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("CustomerIntelligence", ex);
        }
      }
    }

    public static void PublishTaskHubPlanCompleted(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan)
    {
      using (new MethodScope(requestContext, "CustomerIntelligence", nameof (PublishTaskHubPlanCompleted)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("PlanArtifactUri", plan.ArtifactUri.ToString());
          intelligenceData.Add("PlanGroup", plan.PlanGroup);
          intelligenceData.Add("PlanId", (object) plan.PlanId);
          intelligenceData.Add("PlanTemplateType", plan.TemplateType.ToString());
          intelligenceData.Add("ScopeId", (object) plan.ScopeIdentifier);
          intelligenceData.Add("DefinitionId", (object) plan.Definition?.Id);
          intelligenceData.Add("StartTime", (object) plan.StartTime);
          intelligenceData.Add("FinishTime", (object) plan.FinishTime);
          intelligenceData.Add("Result", (object) plan.Result);
          CustomerIntelligenceHelper.LogDataspaceDetails(requestContext, plan.ScopeIdentifier, intelligenceData);
          service.Publish(requestContext, "TaskHub", "PlanCompleted", intelligenceData);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("CustomerIntelligence", ex);
        }
      }
    }

    public static void PublishTaskHubPhaseStarted(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      Phase phase,
      ExpandPhaseResult expandPhaseResult)
    {
      using (new MethodScope(requestContext, "CustomerIntelligence", nameof (PublishTaskHubPhaseStarted)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          CustomerIntelligenceData intelligenceData1 = new CustomerIntelligenceData();
          intelligenceData1.Add("PlanArtifactUri", plan.ArtifactUri.ToString());
          intelligenceData1.Add("PlanGroup", plan.PlanGroup);
          intelligenceData1.Add("PlanId", (object) plan.PlanId);
          intelligenceData1.Add("ScopeId", (object) plan.ScopeIdentifier);
          intelligenceData1.Add("DefinitionId", (object) plan.Definition?.Id);
          intelligenceData1.Add("DependencyCount", (double) phase.DependsOn.Count);
          intelligenceData1.Add("ConditionType", CustomerIntelligenceHelper.GetConditionType(phase.Condition));
          intelligenceData1.Add("ContinueOnErrorIsExpression", !string.IsNullOrEmpty(phase.ContinueOnError?.Expression));
          intelligenceData1.Add("ContinueOnError", expandPhaseResult.ContinueOnError);
          intelligenceData1.Add("VariableCount", (double) phase.Variables.Count);
          intelligenceData1.Add("JobCount", (double) expandPhaseResult.Jobs.Count);
          intelligenceData1.Add("MaxConcurrency", (double) Math.Max(1, expandPhaseResult.MaxConcurrency));
          CustomerIntelligenceData intelligenceData2 = new CustomerIntelligenceData();
          intelligenceData2.Add("Type", phase.Target.Type.ToString());
          intelligenceData2.Add("CancelTimeoutInMinutesIsExpression", !string.IsNullOrEmpty(phase.Target.CancelTimeoutInMinutes?.Expression));
          intelligenceData2.Add("ContinueOnErrorIsExpression", !string.IsNullOrEmpty(phase.Target.ContinueOnError?.Expression));
          intelligenceData2.Add("TimeoutInMinutesIsExpression", !string.IsNullOrEmpty(phase.Target.TimeoutInMinutes?.Expression));
          if (phase.Target is AgentQueueTarget target2)
          {
            intelligenceData2.Add("MatrixIsExpression", !string.IsNullOrEmpty(target2.Execution?.Matrix?.Expression));
            intelligenceData2.Add("MaxConcurrencyIsExpression", !string.IsNullOrEmpty(target2.Execution?.MaxConcurrency?.Expression));
          }
          else if (phase.Target is ServerTarget target1)
          {
            intelligenceData2.Add("MatrixIsExpression", !string.IsNullOrEmpty(target1.Execution?.Matrix?.Expression));
            intelligenceData2.Add("MaxConcurrencyIsExpression", !string.IsNullOrEmpty(target1.Execution?.MaxConcurrency?.Expression));
          }
          intelligenceData1.Add("Target", (object) intelligenceData2.GetData());
          CustomerIntelligenceHelper.LogDataspaceDetails(requestContext, plan.ScopeIdentifier, intelligenceData1);
          service.Publish(requestContext, "TaskHub", "PhaseStarted", intelligenceData1);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("CustomerIntelligence", ex);
        }
      }
    }

    public static void PublishTaskHubSendJobData(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      Guid jobId,
      string jobName,
      string jobContainer,
      TaskOrchestrationPlanReference plan,
      IList<JobStep> steps,
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> endpointsInUse,
      IDictionary<string, string> environmentVariables,
      JobResources jobResources,
      string agentVersion,
      string agentOS,
      string planTemplateType,
      int poolId = -1)
    {
      using (new MethodScope(requestContext, "CustomerIntelligence", nameof (PublishTaskHubSendJobData)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          CustomerIntelligenceData intelligenceData1 = new CustomerIntelligenceData();
          bool isMobileCenterAccount = false;
          if (poolId != -1)
          {
            TaskAgentPool agentPool = requestContext.GetService<IDistributedTaskPoolService>().GetAgentPool(requestContext, poolId);
            intelligenceData1.Add("PoolIsHosted", agentPool.IsHosted);
            if (agentPool.IsHosted)
              isMobileCenterAccount = requestContext.TryGetIsMobileCenter();
          }
          intelligenceData1.Add("Name", taskHub.Name);
          intelligenceData1.Add("PlanArtifactUri", plan.ArtifactUri.ToString());
          intelligenceData1.Add("JobId", jobId.ToString());
          intelligenceData1.Add("Version", (double) taskHub.Version);
          intelligenceData1.Add("PlanId", (object) plan.PlanId);
          intelligenceData1.Add("AgentVersion", agentVersion);
          intelligenceData1.Add("AgentOS", agentOS);
          if (!string.IsNullOrEmpty(jobContainer))
            intelligenceData1.Add("JobContainer", jobContainer);
          int num1 = 0;
          foreach (JobStep step in (IEnumerable<JobStep>) steps)
          {
            if (step.Type == StepType.Task)
              ++num1;
            else if (step.Type == StepType.Group)
              num1 += (step as GroupStep).Steps.Count;
          }
          intelligenceData1.Add("TaskCount", (double) num1);
          bool isAzure = false;
          bool isAzureNationalCloud = false;
          Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          if (endpointsInUse != null)
          {
            IEnumerable<IGrouping<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>> groupings1 = endpointsInUse.Where<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (e => !string.IsNullOrWhiteSpace(e.Type))).GroupBy<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, string>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, string>) (e1 => e1.Type));
            IEnumerable<IGrouping<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>> groupings2 = endpointsInUse.Where<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (e => !string.IsNullOrEmpty(e.Authorization?.Scheme))).GroupBy<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, string>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, string>) (e1 => e1.Authorization.Scheme));
            foreach (IGrouping<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> source in groupings1)
              intelligenceData1.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EndpointInUse_{0}", (object) source.Key), (double) source.Count<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>());
            foreach (IGrouping<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> source in groupings2)
              intelligenceData1.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EndpointSchemeInUse_{0}", (object) source.Key), (double) source.Count<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>());
            Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
            List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> all = endpointsInUse.ToList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>().FindAll((Predicate<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) (endPoint => CustomerIntelligenceHelper.IsEndpointTargetingAzure(endPoint.Type)));
            if (all != null && all.Count > 0)
            {
              isAzure = true;
              foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint in all)
              {
                string type = serviceEndpoint.Type;
                IDictionary<string, string> data = serviceEndpoint.Data;
                if (data != null)
                {
                  string valueOrDefault = data.GetValueOrDefault<string, string>("subscriptionId");
                  string str = data.GetValueOrDefault<string, string>("environment") ?? string.Empty;
                  if (!string.IsNullOrWhiteSpace(valueOrDefault) && !dictionary1.TryAdd<string, string>(valueOrDefault, string.Format("{0}-{1}", (object) type, (object) str)))
                    dictionary1[valueOrDefault] = string.Format("{0},{1}-{2}", (object) dictionary1[valueOrDefault], (object) type, (object) str);
                  if (!string.IsNullOrWhiteSpace(str))
                  {
                    if (!dictionary2.TryAdd<string, int>(str, 1))
                      dictionary2[str]++;
                    if (CustomerIntelligenceHelper.IsEndpointTargetingAzureNationalCloud(str))
                      isAzureNationalCloud = true;
                  }
                }
              }
              foreach (KeyValuePair<string, int> keyValuePair in dictionary2)
                intelligenceData1.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EndpointEnvironmentInUse_{0}", (object) keyValuePair.Key), keyValuePair.Value.ToString());
            }
          }
          if (!string.IsNullOrWhiteSpace(planTemplateType))
            intelligenceData1.Add("PlanTemplateType", planTemplateType);
          intelligenceData1.Add("IsAzure", isAzure);
          intelligenceData1.Add("IsAzureNationalCloud", isAzureNationalCloud);
          string jobRequestingUser = string.Empty;
          Guid jobRequesterUserId = Guid.Empty;
          IDictionary<string, object> dictionary3 = taskHub.Extension.GetJobTelemetryDetails(requestContext, plan, environmentVariables, out jobRequestingUser, out jobRequesterUserId) ?? (IDictionary<string, object>) new Dictionary<string, object>();
          intelligenceData1.GetData().AddRange<KeyValuePair<string, object>, IDictionary<string, object>>((IEnumerable<KeyValuePair<string, object>>) dictionary3);
          CustomerIntelligenceHelper.LogDataspaceDetails(requestContext, plan.ScopeIdentifier, intelligenceData1);
          service.Publish(requestContext, requestContext.ServiceHost.InstanceId, jobRequestingUser, jobRequesterUserId, IdentityCuidHelper.GetCuidByVsid(requestContext, jobRequesterUserId), DateTime.UtcNow, "TaskHub", "SendJob", intelligenceData1);
          foreach (JobStep step1 in (IEnumerable<JobStep>) steps)
          {
            if (step1.Type == StepType.Task)
            {
              CustomerIntelligenceData properties = new CustomerIntelligenceData();
              CustomerIntelligenceHelper.PopulateTaskStepCIData(step1 as TaskStep, properties, taskHub, jobId, plan, dictionary3, isMobileCenterAccount, isAzure, isAzureNationalCloud);
              properties.Add("AgentVersion", agentVersion);
              properties.Add("AgentOS", agentOS);
              service.Publish(requestContext, requestContext.ServiceHost.InstanceId, "TaskHub", "SendJob_TaskInstance", properties);
            }
            else if (step1.Type == StepType.Group)
            {
              GroupStep groupStep = step1 as GroupStep;
              foreach (TaskStep step2 in (IEnumerable<TaskStep>) groupStep.Steps)
              {
                CustomerIntelligenceData properties1 = new CustomerIntelligenceData();
                properties1.Add("GroupName", groupStep.Name);
                CustomerIntelligenceData properties2 = properties1;
                TaskHub taskHub1 = taskHub;
                Guid jobId1 = jobId;
                TaskOrchestrationPlanReference plan1 = plan;
                IDictionary<string, object> jobTelemetryDetails = dictionary3;
                int num2 = isMobileCenterAccount ? 1 : 0;
                int num3 = isAzure ? 1 : 0;
                int num4 = isAzureNationalCloud ? 1 : 0;
                CustomerIntelligenceHelper.PopulateTaskStepCIData(step2, properties2, taskHub1, jobId1, plan1, jobTelemetryDetails, num2 != 0, num3 != 0, num4 != 0);
                properties1.Add("AgentVersion", agentVersion);
                properties1.Add("AgentOS", agentOS);
                service.Publish(requestContext, requestContext.ServiceHost.InstanceId, "TaskHub", "SendJob_TaskInstance", properties1);
              }
            }
          }
          if (jobResources != null)
          {
            List<RepositoryResource> repositories = jobResources.Repositories;
            bool? nullable = repositories != null ? new bool?(repositories.Any<RepositoryResource>()) : new bool?();
            bool flag = true;
            if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
            {
              IDictionary<string, object> data = intelligenceData1.GetData();
              CustomerIntelligenceData properties = new CustomerIntelligenceData(data);
              properties.Add("Repositories", (object) jobResources.Repositories.Select<RepositoryResource, RepositoryResource>(new Func<RepositoryResource, RepositoryResource>(sanitizeRepositoryResource)));
              properties.Add("IsMultiRepoTrigger", CustomerIntelligenceHelper.isMultiRepoTrigger(jobResources.Repositories));
              service.Publish(requestContext, requestContext.ServiceHost.InstanceId, "TaskHub", "SendJob_Resources", properties);
              foreach (RepositoryResource repository in jobResources.Repositories)
              {
                CustomerIntelligenceData intelligenceData2 = new CustomerIntelligenceData(data);
                CustomerIntelligenceHelper.PopulateJobRepositoryCIData(intelligenceData2, repository);
                service.Publish(requestContext, requestContext.ServiceHost.InstanceId, "TaskHub", "SendJob_Repository", intelligenceData2);
              }
            }
          }
          foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
          {
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add("PlanArtifactUri", plan.ArtifactUri.ToString());
            properties.Add("JobId", jobId.ToString());
            properties.Add("TaskHubName", taskHub.Name);
            properties.Add("SubscriptionId", keyValuePair.Key);
            properties.Add("ConnectionType", keyValuePair.Value);
            service.Publish(requestContext, requestContext.ServiceHost.InstanceId, "TaskHub", "SendJob_AzureSubscription", properties);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException("CustomerIntelligence", ex);
        }
      }

      static RepositoryResource sanitizeRepositoryResource(RepositoryResource dirty)
      {
        RepositoryResource repositoryResource = new RepositoryResource()
        {
          Id = dirty.Id,
          Type = dirty.Type,
          Url = dirty.Url
        };
        repositoryResource.Properties.Set<string>(RepositoryPropertyNames.Name, dirty.Properties.Get<string>(RepositoryPropertyNames.Name));
        if (string.Equals(repositoryResource.Url.Host, "github.com", StringComparison.OrdinalIgnoreCase))
        {
          string id = repositoryResource.Id;
          string str1;
          if (id == null)
            str1 = (string) null;
          else
            str1 = ((IEnumerable<string>) id.Split('/')).Last<string>();
          if (str1 == null)
            str1 = ((IEnumerable<string>) repositoryResource.Url.AbsolutePath.Split('/')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).Last<string>();
          string str2 = str1;
          repositoryResource.Id = "[NonEmail:" + str2 + "]";
          repositoryResource.Properties.Set<string>(RepositoryPropertyNames.Name, str2);
          repositoryResource.Url = new UriBuilder(repositoryResource.Url.Scheme, repositoryResource.Url.Host, repositoryResource.Url.Port, ".../" + str2)
          {
            UserName = ((string) null),
            Password = ((string) null)
          }.Uri;
        }
        else
        {
          repositoryResource.Id = "[NonEmail:" + repositoryResource.Id + "]";
          repositoryResource.Url = new UriBuilder(repositoryResource.Url)
          {
            UserName = ((string) null),
            Password = ((string) null)
          }.Uri;
        }
        return repositoryResource;
      }
    }

    private static bool isMultiRepoTrigger(List<RepositoryResource> repositoryResources) => repositoryResources != null && repositoryResources.Count<RepositoryResource>((Func<RepositoryResource, bool>) (r => r.Alias != PipelineConstants.SelfAlias && r.Properties.TryGetValue<string>("system.istriggeringrepository", out string _))) > 0;

    private static void PopulateJobRepositoryCIData(
      CustomerIntelligenceData ciData,
      RepositoryResource jobRepository)
    {
      ciData.Add("RepositoryType", jobRepository.Type);
      ciData.Add("RepositoryUrl", CustomerIntelligenceHelper.AnonymizeRepositoryUri(jobRepository.Url));
      string str;
      if (!jobRepository.Properties.TryGetValue<string>(RepositoryPropertyNames.Name, out str))
        return;
      ciData.Add("RepositoryFullName", str);
    }

    private static string AnonymizeRepositoryUri(Uri uri)
    {
      if ((object) uri == null || !uri.IsAbsoluteUri)
        return string.Empty;
      return new UriBuilder(uri)
      {
        UserName = ((string) null),
        Password = ((string) null)
      }.AbsoluteUri();
    }

    private static void PopulateTaskStepCIData(
      TaskStep task,
      CustomerIntelligenceData properties,
      TaskHub taskHub,
      Guid jobId,
      TaskOrchestrationPlanReference plan,
      IDictionary<string, object> jobTelemetryDetails,
      bool isMobileCenterAccount,
      bool isAzure,
      bool isAzureNationalCloud)
    {
      properties.Add("ConditionType", CustomerIntelligenceHelper.GetConditionType(task.Condition));
      if (TaskConditions.IsCustomCondition(task.Condition))
        properties.Add("HasCondition", true);
      properties.Add("ContinueOnError", task.ContinueOnError);
      properties.Add("Enabled", task.Enabled);
      properties.Add("TimeOutInMinutes", (double) task.TimeoutInMinutes);
      if (isMobileCenterAccount)
        properties.Add("DisplayName", task.DisplayName);
      properties.Add("TaskInstanceId", (object) task.Id);
      properties.Add("IsAzure", isAzure);
      properties.Add("IsAzureNationalCloud", isAzureNationalCloud);
      foreach (string key in (IEnumerable<string>) task.Inputs.Keys)
      {
        if (!string.IsNullOrEmpty(task.Inputs[key]) && !string.Equals(task.Inputs[key], "false", StringComparison.OrdinalIgnoreCase))
          properties.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Input_{0}", (object) key), "IsSet");
      }
      properties.Add("Id", task.Reference.Id.ToString());
      properties.Add("Name", task.Reference.Name);
      properties.Add("Version", task.Reference.Version);
      properties.Add("PlanArtifactUri", plan.ArtifactUri.ToString());
      properties.Add("JobId", jobId.ToString());
      properties.Add("TaskHubName", taskHub.Name);
      properties.Add("PlanId", (object) plan.PlanId);
      properties.GetData().AddRange<KeyValuePair<string, object>, IDictionary<string, object>>((IEnumerable<KeyValuePair<string, object>>) jobTelemetryDetails);
    }

    public static bool IsEndpointTargetingAzure(string endpointType) => new List<string>()
    {
      "azure",
      "servicefabric",
      "azurerm"
    }.Any<string>((Func<string, bool>) (azureEndpointType => string.Equals(azureEndpointType, endpointType, StringComparison.OrdinalIgnoreCase)));

    public static bool IsEndpointTargetingAzureNationalCloud(string endpointEnvironment) => new List<string>()
    {
      "AzureChinaCloud",
      "AzureUSGovernment",
      "AzureGermanCloud"
    }.Any<string>((Func<string, bool>) (azureEndpointEnvironment => string.Equals(azureEndpointEnvironment, endpointEnvironment, StringComparison.OrdinalIgnoreCase)));

    public static bool DoesEndpointContainsAzure(string endpointType) => endpointType.ToUpperInvariant().Contains("AZURE");

    public static bool DoesContainEndpointTargetingAzure(
      IEnumerable<string> endpointsUsedInEnvironment)
    {
      if (endpointsUsedInEnvironment == null)
        return false;
      return endpointsUsedInEnvironment.Any<string>((Func<string, bool>) (endpoint => CustomerIntelligenceHelper.DoesEndpointContainsAzure(endpoint))) || endpointsUsedInEnvironment.Any<string>((Func<string, bool>) (endpoint => CustomerIntelligenceHelper.IsEndpointTargetingAzure(endpoint)));
    }

    public static void PublishServiceEndpointProxyTelemetry(
      IVssRequestContext requestContext,
      HttpStatusCode httpStatusCode,
      string featureName,
      string url,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      TaskDefinitionEndpoint taskDefinitionEndpoint,
      string taskDefinitionEndpointType,
      string resultTemplate,
      string resultSelector,
      Exception exception)
    {
      try
      {
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        int num = (int) httpStatusCode;
        if (num >= 200 && num < 300)
          intelligenceData.Add("RequestSucceeded", true.ToString());
        else
          intelligenceData.Add("RequestSucceeded", false.ToString());
        intelligenceData.Add("HttpStatusCode", (object) httpStatusCode);
        if (serviceEndpoint != null)
        {
          if (!string.IsNullOrEmpty(url))
            intelligenceData.Add("EndpointUrl", url);
          intelligenceData.Add("EndpointType", serviceEndpoint.Type);
          if (!string.IsNullOrEmpty(resultTemplate))
            intelligenceData.Add("ResultTemplate", resultTemplate);
          if (!string.IsNullOrEmpty(resultSelector))
            intelligenceData.Add("ResultSelector", resultSelector);
        }
        else if (taskDefinitionEndpoint != null)
        {
          if (!string.IsNullOrEmpty(taskDefinitionEndpoint.Selector))
            intelligenceData.Add("Selector", taskDefinitionEndpoint.Selector);
          if (!string.IsNullOrEmpty(taskDefinitionEndpoint.KeySelector))
            intelligenceData.Add("KeySelector", taskDefinitionEndpoint.KeySelector);
          intelligenceData.Add("EndpointType", taskDefinitionEndpointType);
        }
        IVssRequestContext requestContext1 = requestContext;
        string feature = featureName;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, "ServiceEndpointProxy", feature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException("CustomerIntelligence", ex);
      }
    }

    internal static void PublishTaskHubJobCompleted(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan,
      Guid jobId,
      Timeline timeline)
    {
      using (new MethodScope(requestContext, "CustomerIntelligence", nameof (PublishTaskHubJobCompleted)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("PlanId", (object) plan.PlanId);
          intelligenceData.Add("PlanGroup", plan.PlanGroup);
          intelligenceData.Add("PlanArtifactUri", plan.ArtifactUri.ToString());
          intelligenceData.Add("JobId", (object) jobId);
          TimelineRecord timelineRecord1 = timeline.Records.FirstOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.Id == jobId));
          if (timelineRecord1 != null)
          {
            intelligenceData.Add("JobName", timelineRecord1.RefName);
            intelligenceData.Add("StartTime", (object) timelineRecord1.StartTime);
            intelligenceData.Add("FinishTime", (object) timelineRecord1.FinishTime);
            intelligenceData.Add("Result", (object) timelineRecord1.Result);
            intelligenceData.Add("State", (object) timelineRecord1.State);
            intelligenceData.Add("WorkerName", timelineRecord1?.WorkerName);
            intelligenceData.Add("CurrentOperation", timelineRecord1?.CurrentOperation);
            intelligenceData.Add("PercentComplete", (object) (int?) timelineRecord1?.PercentComplete);
            intelligenceData.Add("LogId", (object) timelineRecord1?.Log?.Id);
            intelligenceData.Add("LogLocation", (object) timelineRecord1?.Log?.Location);
            intelligenceData.Add("ErrorCount", (object) (int?) timelineRecord1?.ErrorCount);
            intelligenceData.Add("WarningCount", (object) (int?) timelineRecord1?.WarningCount);
          }
          CustomerIntelligenceHelper.LogDataspaceDetails(requestContext, plan.ScopeIdentifier, intelligenceData);
          service.Publish(requestContext, "TaskHub", "JobCompleted", intelligenceData);
          bool isMobileCenter = requestContext.TryGetIsMobileCenter();
          foreach (TimelineRecord timelineRecord2 in timeline.Records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x =>
          {
            Guid? parentId = x.ParentId;
            Guid guid = jobId;
            return (parentId.HasValue ? (parentId.HasValue ? (parentId.GetValueOrDefault() == guid ? 1 : 0) : 1) : 0) != 0 && string.Equals(x.RecordType, "Task", StringComparison.OrdinalIgnoreCase);
          })))
          {
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add("PlanArtifactUri", (object) plan.ArtifactUri);
            properties.Add("JobId", (object) jobId);
            if (isMobileCenter)
              properties.Add("TaskName", timelineRecord2.Name);
            properties.Add("TimelineResult", (object) timelineRecord2.Result);
            properties.Add("PlanId", (object) plan.PlanId);
            properties.Add("TimelineRecordId", (object) timelineRecord2.Id);
            DateTime? nullable1 = timelineRecord2.StartTime;
            if (nullable1.HasValue)
            {
              nullable1 = timelineRecord2.FinishTime;
              if (nullable1.HasValue)
              {
                nullable1 = timelineRecord2.FinishTime;
                DateTime? startTime = timelineRecord2.StartTime;
                TimeSpan? nullable2;
                TimeSpan? nullable3;
                if (!(nullable1.HasValue & startTime.HasValue))
                {
                  nullable2 = new TimeSpan?();
                  nullable3 = nullable2;
                }
                else
                  nullable3 = new TimeSpan?(nullable1.GetValueOrDefault() - startTime.GetValueOrDefault());
                nullable2 = nullable3;
                TimeSpan valueOrDefault = nullable2.GetValueOrDefault();
                properties.Add("TimelineDuration", valueOrDefault.TotalSeconds);
              }
            }
            service.Publish(requestContext, "TaskHub", "JobCompleted_TimelineInstance", properties);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException("CustomerIntelligence", ex);
        }
      }
    }

    internal static void PublishPlanQueueEvaluationJobPlanStarted(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationQueuedPlan queuedPlan)
    {
      using (new MethodScope(requestContext, "CustomerIntelligence", nameof (PublishPlanQueueEvaluationJobPlanStarted)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("HubName", taskHub.Name);
          properties.Add("PlanId", (object) queuedPlan.PlanId);
          properties.Add("QueueTime", (object) queuedPlan.QueueTime);
          properties.Add("AssignTime", (object) DateTime.UtcNow);
          service.Publish(requestContext, "TaskHub", "OrchestrationPlanQueueEvaluationJob_PlanStarted", properties);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("CustomerIntelligence", ex);
        }
      }
    }

    internal static void PublishPlanQueueEvaluationJobTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      int licensingLimit,
      int runnablePlansFetchedCount,
      int startedPlansCount,
      int startedPlanGroupsCount)
    {
      using (new MethodScope(requestContext, "CustomerIntelligence", nameof (PublishPlanQueueEvaluationJobTelemetry)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("HubName", taskHub.Name);
          properties.Add("LicensingLimit", (double) licensingLimit);
          properties.Add("NoOfRunnablePlansFetched", (double) runnablePlansFetchedCount);
          properties.Add("NoOfStartedPlans", (double) startedPlansCount);
          properties.Add("NoOfStartedPlanGroups", (double) startedPlanGroupsCount);
          service.Publish(requestContext, "TaskHub", "PlanQueueEvaluationJob", properties);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("CustomerIntelligence", ex);
        }
      }
    }

    private static string GetConditionType(string condition)
    {
      condition = condition?.Trim();
      if (string.IsNullOrEmpty(condition) || string.Equals(condition, "succeeded()", StringComparison.OrdinalIgnoreCase))
        return "succeeded";
      if (string.Equals(condition, "always()", StringComparison.OrdinalIgnoreCase))
        return "always";
      if (string.Equals(condition, "canceled()", StringComparison.OrdinalIgnoreCase))
        return "canceled";
      if (string.Equals(condition, "failed()", StringComparison.OrdinalIgnoreCase))
        return "failed";
      return string.Equals(condition, "succeededOrFailed()", StringComparison.OrdinalIgnoreCase) ? "succeededOrFailed" : "custom";
    }

    private static void LogDataspaceDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      CustomerIntelligenceData data)
    {
      try
      {
        if (!(projectId != Guid.Empty))
          return;
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        data.AddDataspaceInformation(CustomerIntelligenceDataspaceType.Project, project.Id.ToString(), ((int) project.Visibility).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      catch (Exception ex)
      {
        requestContext.TraceException("CustomerIntelligence", ex);
      }
    }
  }
}

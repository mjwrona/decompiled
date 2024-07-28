// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentPhaseService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class DeploymentPhaseService : IDeploymentPhaseService, IVssFrameworkService
  {
    private const int c_deploymentPhaseInputLatestVersion = 4;

    public async Task StartPhaseAsync(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest,
      PhaseExecutionContext phaseExecutionContext)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (DeploymentPhaseService), nameof (StartPhaseAsync));
      try
      {
        EnvironmentDeploymentExecutionRecord request = (EnvironmentDeploymentExecutionRecord) null;
        try
        {
          this.ValidatePhaseRequest(phaseRequest);
          await this.PopulateResourceFilter(requestContext, phaseRequest);
          request = this.QueueEnvironmentDeploymentRequest(requestContext, phaseRequest);
          ValidationResult result = new ValidationResult();
          string orchestrationName = string.Empty;
          string orchestrationVersion = string.Empty;
          DeploymentStrategyBase2 strategy = DeploymentStrategyHelper.ProcessAndGetStrategy(requestContext, phaseRequest.Project.Id, phaseRequest.ProviderPhase.EnvironmentTarget, phaseRequest.ProviderPhase.Target, phaseRequest.ProviderPhase.Strategy, phaseRequest.Phase.Name, phaseExecutionContext.ResourceStore, result);
          object input;
          if (DeploymentPhaseHelper.ShouldUseOrchestrationV2(requestContext, strategy, phaseRequest.ProviderPhase.EnvironmentTarget))
          {
            input = (object) this.GetRunPhaseInputV2(requestContext, phaseRequest, request, strategy, out orchestrationName, out orchestrationVersion);
          }
          else
          {
            DeploymentStrategyBase deploymentStrategyBase = this.ConvertToDeploymentStrategyBase(strategy);
            input = (object) this.GetRunPhaseInput(phaseRequest, request, deploymentStrategyBase, out orchestrationName, out orchestrationVersion);
          }
          if (result.Errors.Any<PipelineValidationError>())
            throw new PipelineValidationException((IEnumerable<PipelineValidationError>) result.Errors);
          EnvironmentTelemetryFactory.GetLogger(requestContext).PublishDeploymentPhaseStarted(requestContext, phaseRequest);
          OrchestrationHubDescription orchestrationHubDescription = EnvironmentTaskHubHelper.EnsureHubExists(requestContext, EnvironmentConstants.DeploymentPhaseHubName);
          OrchestrationService service = requestContext.GetService<OrchestrationService>();
          requestContext.TraceInfo(10015192, nameof (DeploymentPhaseService), "DeploymentPhaseService: Creating orchestration instance with hubName: " + orchestrationHubDescription.HubName + ", orchestrationName: " + orchestrationName + ", orchestrationVersion: " + orchestrationVersion + ", PhaseOrchestrationId: " + phaseRequest.PhaseOrchestrationId);
          service.CreateOrchestrationInstance(requestContext, orchestrationHubDescription.HubName, orchestrationName, orchestrationVersion, phaseRequest.PhaseOrchestrationId, input);
        }
        catch (EnvironmentNotFoundException ex)
        {
          requestContext.TraceError(10015193, nameof (DeploymentPhaseService), string.Format("Execution of Phase orchestration {0} failed with error {1}.", (object) 0, (object) 1), (object) phaseRequest.PhaseOrchestrationId, (object) ex.Message);
          throw new PipelineValidationException(ex.Message);
        }
        catch (Exception ex)
        {
          requestContext.TraceError(10015193, nameof (DeploymentPhaseService), string.Format("Execution of Phase orchestration {0} failed with error {1}.", (object) 0, (object) 1), (object) phaseRequest.PhaseOrchestrationId, (object) ex.Message);
          TaskHub taskHub = this.GetTaskHub(requestContext, phaseRequest.PlanType);
          TaskOrchestrationPlan planAsync = await taskHub.GetPlanAsync(requestContext, phaseRequest.Project.Id, phaseRequest.PlanId);
          IList<TimelineRecord> records = (IList<TimelineRecord>) new List<TimelineRecord>()
          {
            this.CreateTimelineRecord(phaseRequest, ex.Message)
          };
          Timeline timeline = await taskHub.UpdateTimelineAsync(requestContext, phaseRequest.Project.Id, phaseRequest.PlanId, planAsync.Timeline.Id, records);
          if (request != null)
            requestContext.GetService<IEnvironmentDeploymentExecutionHistoryService>().UpdateEnvironmentDeploymentRequest(requestContext, phaseRequest.ProviderPhase.EnvironmentTarget.EnvironmentId, request.Id, new DateTime?(DateTime.UtcNow), new DateTime?(DateTime.UtcNow), new TaskResult?(TaskResult.Failed));
          await taskHub.CompleteProviderPhaseAsync(requestContext, phaseRequest.Project.Id, phaseRequest.PlanId, phaseRequest.Stage.Name, phaseRequest.Phase.Name, phaseRequest.Phase.Attempt, TaskResult.Failed);
          taskHub = (TaskHub) null;
        }
        request = (EnvironmentDeploymentExecutionRecord) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task JobCompletedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance job)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (DeploymentPhaseService), nameof (JobCompletedAsync));
      try
      {
        EnvironmentTelemetryFactory.GetLogger(requestContext).PublishJobCompleted(requestContext, phaseOrchestrationId, job);
        string hookOrchestrationId = LifeCycleHookHelper.GetLifeCycleHookOrchestrationId(phaseOrchestrationId, job);
        await EnvironmentTaskHubHelper.PublishOrchestrationEvent(requestContext, EnvironmentConstants.DeploymentPhaseHubName, hookOrchestrationId, "JobCompleted", (object) job);
        await EnvironmentTaskHubHelper.PublishOrchestrationEvent(requestContext, EnvironmentConstants.DeploymentPhaseHubName, phaseOrchestrationId, "JobCompleted", (object) job);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task JobStartedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobStartedEventData eventData)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (DeploymentPhaseService), nameof (JobStartedAsync));
      try
      {
        await EnvironmentTaskHubHelper.PublishOrchestrationEvent(requestContext, EnvironmentConstants.DeploymentPhaseHubName, phaseOrchestrationId, "JobStarted", (object) eventData);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task CancelPhaseAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      string reason)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (DeploymentPhaseService), nameof (CancelPhaseAsync));
      try
      {
        await EnvironmentTaskHubHelper.PublishOrchestrationEvent(requestContext, EnvironmentConstants.DeploymentPhaseHubName, phaseOrchestrationId, "PhaseCanceled", (object) new CanceledEvent()
        {
          Reason = reason
        });
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private EnvironmentDeploymentExecutionRecord GetEnvironmentExecutionRecordObject(
      ProviderPhaseRequest phaseRequest)
    {
      return new EnvironmentDeploymentExecutionRecord()
      {
        EnvironmentId = phaseRequest.ProviderPhase.EnvironmentTarget.EnvironmentId,
        ScopeId = phaseRequest.Project.Id,
        ServiceOwner = phaseRequest.ServiceOwner,
        PlanId = phaseRequest.PlanId,
        RequestIdentifier = phaseRequest.PhaseOrchestrationId,
        ResourceId = phaseRequest.ProviderPhase.EnvironmentTarget.Resource?.Id,
        PlanType = phaseRequest.PlanType,
        Definition = phaseRequest.Pipeline,
        Owner = phaseRequest.Run,
        StageName = phaseRequest.Stage.Name,
        StageAttempt = phaseRequest.Stage.Attempt,
        JobName = phaseRequest.Phase.Name,
        JobAttempt = phaseRequest.Phase.Attempt
      };
    }

    private TaskHub GetTaskHub(IVssRequestContext requestContext, string hubName) => requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, hubName, false);

    private RunDeploymentPhaseInput GetRunPhaseInput(
      ProviderPhaseRequest phaseRequest,
      EnvironmentDeploymentExecutionRecord request,
      DeploymentStrategyBase strategy,
      out string orchestrationName,
      out string orchestrationVersion)
    {
      if (phaseRequest.ProviderPhase.EnvironmentTarget.Resource == null)
      {
        orchestrationName = "RunDefaultDeploymentPhase";
        orchestrationVersion = "1.0";
      }
      else
      {
        switch (phaseRequest.ProviderPhase.EnvironmentTarget.Resource.Type)
        {
          case EnvironmentResourceType.VirtualMachine:
            orchestrationName = "RunVMDeploymentPhase";
            orchestrationVersion = "2.0";
            break;
          case EnvironmentResourceType.Kubernetes:
            orchestrationName = "RunKubernetesDeploymentPhase";
            orchestrationVersion = "1.0";
            break;
          default:
            throw new NotSupportedException("Deployment to provided resources type is not supported.");
        }
      }
      return new RunDeploymentPhaseInput()
      {
        ScopeId = phaseRequest.Project.Id,
        RequestId = request.Id,
        ProviderPhase = phaseRequest.ProviderPhase,
        Strategy = strategy,
        PlanId = phaseRequest.PlanId,
        PlanType = phaseRequest.PlanType,
        Stage = phaseRequest.Stage,
        Phase = phaseRequest.Phase
      };
    }

    private RunDeploymentPhaseInput2 GetRunPhaseInputV2(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest,
      EnvironmentDeploymentExecutionRecord request,
      DeploymentStrategyBase2 strategy,
      out string orchestrationName,
      out string orchestrationVersion)
    {
      switch (DeploymentPhaseHelper.GetEnvironmentResourceType(phaseRequest.ProviderPhase.EnvironmentTarget))
      {
        case EnvironmentResourceType.Undefined:
          orchestrationName = "RunDefaultDeploymentPhase";
          orchestrationVersion = "2.0";
          break;
        case EnvironmentResourceType.VirtualMachine:
          orchestrationName = "RunVMDeploymentPhase";
          orchestrationVersion = "3.0";
          break;
        case EnvironmentResourceType.Kubernetes:
          orchestrationName = "RunKubernetesDeploymentPhase";
          orchestrationVersion = "2.0";
          break;
        default:
          throw new NotSupportedException("Deployment to provided resources type is not supported.");
      }
      return new RunDeploymentPhaseInput2()
      {
        ScopeId = phaseRequest.Project.Id,
        RequestId = request.Id,
        ProviderPhase = phaseRequest.ProviderPhase,
        Strategy = strategy,
        PlanId = phaseRequest.PlanId,
        PlanType = phaseRequest.PlanType,
        Stage = phaseRequest.Stage,
        Phase = phaseRequest.Phase,
        Version = this.GetDeploymentPhaseInputVersion(requestContext)
      };
    }

    private DeploymentStrategyBase ConvertToDeploymentStrategyBase(DeploymentStrategyBase2 strategy2)
    {
      DeploymentStrategyBase deploymentStrategyBase;
      if (strategy2 is RollingDeploymentStrategy2)
      {
        RollingDeploymentStrategy2 deploymentStrategy2 = strategy2 as RollingDeploymentStrategy2;
        deploymentStrategyBase = (DeploymentStrategyBase) new RollingDeploymentStrategy(deploymentStrategy2.DeploymentOption, deploymentStrategy2.DeploymentOptionValue, (IList<string>) null);
      }
      else
        deploymentStrategyBase = (DeploymentStrategyBase) new RunOnceDeploymentStrategy();
      DeploymentLifeCycleHookBase lifeCycleHookBase = strategy2.Hooks.FirstOrDefault<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => h.Type == DeploymentLifeCycleHookType.Deploy));
      if (lifeCycleHookBase != null)
      {
        DeploymentStrategyDeployAction strategyDeployAction1 = new DeploymentStrategyDeployAction();
        strategyDeployAction1.Steps = (IList<Step>) lifeCycleHookBase.Steps;
        DeploymentStrategyDeployAction strategyDeployAction2 = strategyDeployAction1;
        deploymentStrategyBase.Actions.Add((DeploymentStrategyBaseAction) strategyDeployAction2);
      }
      return deploymentStrategyBase;
    }

    private EnvironmentDeploymentExecutionRecord QueueEnvironmentDeploymentRequest(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest)
    {
      EnvironmentDeploymentExecutionRecord executionRecordObject = this.GetEnvironmentExecutionRecordObject(phaseRequest);
      int num = executionRecordObject.ResourceId.HasValue ? executionRecordObject.ResourceId.Value : 0;
      EnvironmentDeploymentExecutionRecord deploymentExecutionRecord = requestContext.GetService<IEnvironmentDeploymentExecutionHistoryService>().QueueEnvironmentDeploymentRequest(requestContext, executionRecordObject);
      requestContext.TraceInfo(10015191, nameof (DeploymentPhaseService), "DeploymentPhaseService: Created environment deployment execution record for: " + string.Format("ScopeId: {0}, ", (object) deploymentExecutionRecord.ScopeId) + "PlanType: " + deploymentExecutionRecord.PlanType + ", " + string.Format("PlanId: {0}, ", (object) deploymentExecutionRecord.PlanId) + string.Format("EnvironmentId: {0}, ", (object) deploymentExecutionRecord.EnvironmentId) + string.Format("ResourceId: {0}, ", (object) num) + string.Format("DefinitionId: {0}, ", (object) deploymentExecutionRecord.Definition.Id) + string.Format("OwnerId: {0}, ", (object) deploymentExecutionRecord.Owner.Id) + "StageName: " + deploymentExecutionRecord.StageName + ", " + string.Format("StageAttempt: {0}, ", (object) deploymentExecutionRecord.StageAttempt) + "JobName: " + deploymentExecutionRecord.JobName + ", " + string.Format("JobAttempt: {0}", (object) deploymentExecutionRecord.JobAttempt));
      return deploymentExecutionRecord;
    }

    private void ValidatePhaseRequest(ProviderPhaseRequest phaseRequest)
    {
      if (phaseRequest == null)
        throw new ArgumentNullException(nameof (phaseRequest));
      if (phaseRequest.ProviderPhase == null)
        throw new ArgumentNullException("ProviderPhase");
      if (phaseRequest.ProviderPhase.EnvironmentTarget == null)
        throw new ArgumentNullException("EnvironmentTarget");
      if (phaseRequest.Phase == null)
        throw new ArgumentNullException("Phase");
      if (phaseRequest.Stage == null)
        throw new ArgumentNullException("Stage");
      if (phaseRequest.Project == null)
        throw new ArgumentNullException("Project");
      if (phaseRequest.Pipeline == null)
        throw new ArgumentNullException("Pipeline");
      if (phaseRequest.Run == null)
        throw new ArgumentNullException("Run");
    }

    private TimelineRecord CreateTimelineRecord(
      ProviderPhaseRequest phaseRequest,
      string logMessage)
    {
      DateTime utcNow = DateTime.UtcNow;
      string jobName = string.IsNullOrWhiteSpace(phaseRequest.Phase.Name) ? "Default" : phaseRequest.Phase.Name;
      PipelineIdGenerator pipelineIdGenerator = new PipelineIdGenerator();
      Guid phaseInstanceId = PipelineUtilities.GetPhaseInstanceId(phaseRequest.Stage.Name, phaseRequest.Phase.Name, phaseRequest.Phase.Attempt);
      Guid jobInstanceId = PipelineUtilities.GetJobInstanceId(phaseRequest.Stage.Name, phaseRequest.Phase.Name, jobName, phaseRequest.Phase.Attempt);
      return new TimelineRecord()
      {
        Id = jobInstanceId,
        Name = jobName,
        StartTime = new DateTime?(utcNow),
        FinishTime = new DateTime?(utcNow),
        RecordType = "Job",
        State = new TimelineRecordState?(TimelineRecordState.Completed),
        Result = new TaskResult?(TaskResult.Failed),
        ParentId = new Guid?(phaseInstanceId),
        RefName = pipelineIdGenerator.GetJobInstanceName(phaseRequest.Stage.Name, phaseRequest.Phase.Name, jobName, phaseRequest.Phase.Attempt, 1),
        Identifier = pipelineIdGenerator.GetJobIdentifier(phaseRequest.Stage.Name, phaseRequest.Phase.Name, jobName, 1),
        Issues = {
          new Issue() { Type = IssueType.Error, Message = logMessage }
        }
      };
    }

    private async Task PopulateResourceFilter(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest)
    {
      EnvironmentDeploymentTarget environmentTarget = phaseRequest.ProviderPhase.EnvironmentTarget;
      EnvironmentResourceFilter resourceFilter = environmentTarget.ResourceFilter;
      if (!this.IsResourceFilterValid(resourceFilter))
      {
        phaseRequest.ProviderPhase.EnvironmentTarget.Resource = (EnvironmentResourceReference) null;
        environmentTarget = (EnvironmentDeploymentTarget) null;
        resourceFilter = (EnvironmentResourceFilter) null;
      }
      else
      {
        List<EnvironmentResourceReference> filteredResources = await this.GetFilteredResources(requestContext, phaseRequest.Project.Id, environmentTarget.EnvironmentName, resourceFilter);
        if (filteredResources.Count == 0)
        {
          phaseRequest.ProviderPhase.EnvironmentTarget.Resource = await this.CreateAndGetDynamicResourceIfRequired(requestContext, phaseRequest) ?? throw new ResourceNotFoundWithSpecifiedCriteria(TaskResources.NoResourceFoundWithSpecifiedFilter((object) environmentTarget.EnvironmentId, resourceFilter.Id.HasValue ? (object) environmentTarget.ResourceFilter.Id.ToString() : (object) string.Empty, (object) (resourceFilter.Name ?? string.Empty), resourceFilter.Type.HasValue ? (object) environmentTarget.ResourceFilter.Type.Value.ToString() : (object) string.Empty, (object) string.Join(",", (IEnumerable<string>) resourceFilter.Tags)));
          environmentTarget = (EnvironmentDeploymentTarget) null;
          resourceFilter = (EnvironmentResourceFilter) null;
        }
        else if (filteredResources.Count == 1)
        {
          phaseRequest.ProviderPhase.EnvironmentTarget.Resource = filteredResources[0];
          environmentTarget = (EnvironmentDeploymentTarget) null;
          resourceFilter = (EnvironmentResourceFilter) null;
        }
        else
        {
          if (!resourceFilter.Type.HasValue || resourceFilter.Type.Value != EnvironmentResourceType.VirtualMachine)
            throw new NotSupportedException("Orchestration not supported for more than one resources for provided resource type.");
          environmentTarget = (EnvironmentDeploymentTarget) null;
          resourceFilter = (EnvironmentResourceFilter) null;
        }
      }
    }

    private bool IsResourceFilterValid(EnvironmentResourceFilter resourceFilter) => resourceFilter.Id.HasValue || resourceFilter.Type.HasValue || !string.IsNullOrEmpty(resourceFilter.Name);

    private async Task<List<EnvironmentResourceReference>> GetFilteredResources(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentResourceFilter filter)
    {
      IEnvironmentResourceReferenceService environmentResourceReferenceService = requestContext.GetService<IEnvironmentResourceReferenceService>();
      List<EnvironmentResourceReference> resourceReferences = new List<EnvironmentResourceReference>();
      string continuationToken = string.Empty;
      while (true)
      {
        IPagedList<EnvironmentResourceReference> resourceReferencesAsync = await environmentResourceReferenceService.GetEnvironmentResourceReferencesAsync(requestContext, projectId, environmentName, filter.Id, filter.Name, filter.Type, filter.Tags, continuationToken: continuationToken);
        resourceReferences.AddRange((IEnumerable<EnvironmentResourceReference>) resourceReferencesAsync);
        if (!string.IsNullOrEmpty(resourceReferencesAsync.ContinuationToken))
          continuationToken = resourceReferencesAsync.ContinuationToken;
        else
          break;
      }
      List<EnvironmentResourceReference> filteredResources = resourceReferences;
      environmentResourceReferenceService = (IEnvironmentResourceReferenceService) null;
      resourceReferences = (List<EnvironmentResourceReference>) null;
      return filteredResources;
    }

    private async Task<EnvironmentResourceReference> CreateAndGetDynamicResourceIfRequired(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest)
    {
      if (string.IsNullOrEmpty(phaseRequest.ProviderPhase.EnvironmentTarget.ResourceFilter.Name))
        return (EnvironmentResourceReference) null;
      JObject deploymentStrategy = JObject.FromObject((object) phaseRequest.ProviderPhase.Strategy);
      if (!DynamicResourceCreationHelper.NeedsDynamicResourceCreation((JToken) deploymentStrategy))
        return (EnvironmentResourceReference) null;
      EnvironmentDeploymentTarget environmentTarget = phaseRequest.ProviderPhase.EnvironmentTarget;
      EnvironmentResourceReference sourceResource = await this.GetSourceResource(requestContext, phaseRequest.Project.Id, environmentTarget.EnvironmentName, (JToken) deploymentStrategy);
      if (sourceResource != null && sourceResource.Type == EnvironmentResourceType.Kubernetes)
        return await KubernetesResourcePlugin.CreateDynamicResourceAsync(requestContext, phaseRequest.Project.Id, sourceResource, environmentTarget.EnvironmentId, environmentTarget.ResourceFilter.Name);
      throw new NotSupportedException("Review app is supported only for Kubernetes Resource type.");
    }

    private async Task<EnvironmentResourceReference> GetSourceResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      JToken deploymentStrategy)
    {
      string resourceSourceName = DynamicResourceCreationHelper.GetDynamicResourceSourceName(deploymentStrategy);
      EnvironmentResourceFilter filter = !string.IsNullOrEmpty(resourceSourceName) ? new EnvironmentResourceFilter()
      {
        Name = resourceSourceName
      } : throw new ArgumentException("source resource name not provided for the dynamic resource creation");
      List<EnvironmentResourceReference> filteredResources = await this.GetFilteredResources(requestContext, projectId, environmentName, filter);
      return filteredResources.Count == 1 ? filteredResources[0] : throw new ArgumentException("Source resource not found for given review app.");
    }

    private int GetDeploymentPhaseInputVersion(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("Pipelines.Environments.DownloadArtifactInDeployHook"))
        return 1;
      return !requestContext.IsFeatureEnabled("Pipelines.Environments.EnableNewOutputVariableFormat") ? 3 : 4;
    }
  }
}

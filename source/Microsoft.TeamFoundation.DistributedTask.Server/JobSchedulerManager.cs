// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.JobSchedulerManager
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class JobSchedulerManager : IJobSchedulerManager
  {
    private readonly IVssRequestContext requestContext;

    public JobSchedulerManager(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public async Task CompletePhaseAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      TaskResult result)
    {
      TaskHub taskHub = this.GetTaskHub(this.requestContext, planType, scopeId);
      await taskHub.CompleteProviderPhaseAsync(this.requestContext, scopeId, planId, stageName, phaseName, phaseAttempt, result);
      try
      {
        TaskOrchestrationPlan planAsync = await taskHub.GetPlanAsync(this.requestContext, scopeId, planId);
        this.PublishJobCommitterDetails(scopeId, planType, planId, stageName, phaseName, planAsync);
        taskHub = (TaskHub) null;
      }
      catch (Exception ex)
      {
        this.requestContext.Trace(10015195, TraceLevel.Info, "DistributedTask", "PublishJobCommitterDetails", "Could not queue PublishJobCommitterDetails job for planId: {0}. Exception: {1}", (object) planId, (object) ex.Message);
        taskHub = (TaskHub) null;
      }
    }

    private void PublishJobCommitterDetails(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      string phaseName,
      TaskOrchestrationPlan plan)
    {
      if (plan == null)
        return;
      string phaseOrchestrationId = JobSchedulerManager.GetPhaseOrchestrationId(planId, stageName, phaseName);
      Stage stage = plan.GetProcess<PipelineProcess>().Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
      if (!((stage != null ? stage.Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null) is ProviderPhase providerPhase))
        return;
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new PublishDeploymentJobCommitterDetailsData()
      {
        ProjectId = scopeId,
        RunId = plan.Owner.Id,
        DefinitionId = plan.Definition.Id,
        JobRequestId = phaseOrchestrationId,
        PlanType = planType,
        StageName = stageName,
        PhaseName = phaseName,
        PlanTemplateType = plan.TemplateType.ToString(),
        TargetEnvironmentId = providerPhase.EnvironmentTarget.EnvironmentId
      });
      string extensionName = "Microsoft.TeamFoundation.DistributedTask.Server.Extensions.PublishDeploymentJobCommitterDetailsJob";
      string jobName = "PublishDeploymentJobCommitterDetails";
      Guid guid = this.requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(this.requestContext, jobName, extensionName, xml, JobPriorityLevel.Normal);
      this.requestContext.Trace(10015195, TraceLevel.Info, "DistributedTask", nameof (PublishJobCommitterDetails), "{0} job started, JobId: {1}", (object) jobName, (object) guid);
    }

    public async Task<JobInstance> QueueJobWithJobOrderAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job,
      int jobOrder)
    {
      JobSchedulerManager schedulerManager = this;
      Guid jobInstanceId = PipelineUtilities.GetJobInstanceId(stageName, phaseName, job.Name, job.Attempt);
      TimelineRecord timelineRecord = new TimelineRecord()
      {
        Id = jobInstanceId,
        Order = new int?(jobOrder)
      };
      JobInstance jobInstance = await schedulerManager.QueueJobAsync(scopeId, planType, planId, stageName, stageAttempt, phaseName, phaseAttempt, job);
      await schedulerManager.UpdateTimeline(scopeId, planType, planId, (IList<TimelineRecord>) new List<TimelineRecord>()
      {
        timelineRecord
      });
      JobInstance jobInstance1 = jobInstance;
      timelineRecord = (TimelineRecord) null;
      jobInstance = (JobInstance) null;
      return jobInstance1;
    }

    public async Task<JobInstance> QueueJobAsync2(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job,
      Dictionary<string, string> jobMetaData)
    {
      JobSchedulerManager schedulerManager = this;
      Guid jobInstanceId = PipelineUtilities.GetJobInstanceId(stageName, phaseName, job.Name, job.Attempt);
      TimelineRecord timelineRecord = new TimelineRecord()
      {
        Id = jobInstanceId,
        Order = new int?(JobSchedulerHelper.GetJobOrder(jobMetaData))
      };
      JobInstance jobInstance = await schedulerManager.QueueJobInternalAsync(scopeId, planType, planId, stageName, stageAttempt, phaseName, phaseAttempt, job, jobMetaData);
      await schedulerManager.UpdateTimeline(scopeId, planType, planId, (IList<TimelineRecord>) new List<TimelineRecord>()
      {
        timelineRecord
      });
      JobInstance jobInstance1 = jobInstance;
      timelineRecord = (TimelineRecord) null;
      jobInstance = (JobInstance) null;
      return jobInstance1;
    }

    public Task<JobInstance> QueueJobAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job)
    {
      return this.QueueJobInternalAsync(scopeId, planType, planId, stageName, stageAttempt, phaseName, phaseAttempt, job, new Dictionary<string, string>());
    }

    public async Task CancelJobAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job)
    {
      JobInstance jobInstance = await this.GetTaskHub(this.requestContext, planType, scopeId).CancelProviderJobAsync(this.requestContext, scopeId, planId, stageName, phaseName, phaseAttempt, job);
    }

    public async Task UpdateTimeline(
      Guid scopeId,
      string planType,
      Guid planId,
      IList<TimelineRecord> records)
    {
      TaskHub taskHub = this.GetTaskHub(this.requestContext, planType, scopeId);
      TaskOrchestrationPlan planAsync = await taskHub.GetPlanAsync(this.requestContext, scopeId, planId);
      Timeline timeline = await taskHub.UpdateTimelineAsync(this.requestContext, scopeId, planId, planAsync.Timeline.Id, records);
      taskHub = (TaskHub) null;
    }

    private async Task<JobInstance> QueueJobInternalAsync(
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job,
      Dictionary<string, string> jobMetaData)
    {
      TaskHub taskHub = this.GetTaskHub(this.requestContext, planType, scopeId);
      JobInstance jobInstance1 = await this.PrepareDeploymentJobAsync(this.requestContext, scopeId, planType, planId, stageName, stageAttempt, phaseName, phaseAttempt, job, jobMetaData);
      JobInstance jobInstance2 = await taskHub.QueueProviderJobAsync(this.requestContext, scopeId, planId, stageName, phaseName, phaseAttempt, jobInstance1);
      taskHub = (TaskHub) null;
      return jobInstance2;
    }

    private TaskHub GetTaskHub(IVssRequestContext requestContext, string hubName, Guid scopeId)
    {
      TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, hubName, false);
      taskHub.CreateScope(requestContext, scopeId);
      return taskHub;
    }

    private async Task<JobInstance> PrepareDeploymentJobAsync(
      IVssRequestContext requestContext,
      Guid scopeId,
      string planType,
      Guid planId,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job,
      Dictionary<string, string> jobMetaData)
    {
      if (job.Definition.Target is ServerTarget)
        return job;
      TaskHub taskHub = this.GetTaskHub(requestContext, planType, scopeId);
      TaskOrchestrationPlan planAsync = await taskHub.GetPlanAsync(requestContext, scopeId, planId);
      if (planAsync == null)
        throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
      JobExecutionContext executionContext = this.GetJobExecutionContext(requestContext, scopeId, planType, planAsync, stageName, stageAttempt, phaseName, phaseAttempt, job);
      List<JobStep> jobStepList = this.NormalizeDeploymentJobSteps(requestContext, scopeId, executionContext, (IList<Step>) job.Definition.Steps.OfType<Step>().ToList<Step>(), jobMetaData);
      this.AddReviewAppStepsIfRequired(executionContext, (IList<JobStep>) jobStepList);
      if (PublishPipelineMetadataStepProvider.IsPublishPipelineMetadataProjectSettingEnabled(requestContext, scopeId))
        this.AddPublishMetadataStep(executionContext, (IList<JobStep>) jobStepList);
      IEnumerable<JobStep> source = jobStepList.Except<JobStep>((IEnumerable<JobStep>) job.Definition.Steps);
      ISet<Demand> demandSet = this.GetDemandSet((IPipelineContext) executionContext, source.OfType<TaskStep>());
      job.Definition.Steps.Clear();
      job.Definition.Steps.AddRange<JobStep, IList<JobStep>>((IEnumerable<JobStep>) jobStepList);
      demandSet.AddRange<Demand, ISet<Demand>>((IEnumerable<Demand>) this.ResolveTargetDemmands(job.Definition.Target?.Demands, executionContext));
      job.Definition.Demands.AddRange<Demand, IList<Demand>>((IEnumerable<Demand>) demandSet);
      taskHub.SaveArtifactTraceabilityDataForJob(requestContext, this.GetPhaseExecutionContext(requestContext, scopeId, planType, planAsync, stageName, stageAttempt, phaseName, phaseAttempt), job, planAsync);
      return job;
    }

    private JobExecutionContext GetJobExecutionContext(
      IVssRequestContext requestContext,
      Guid scopeId,
      string planType,
      TaskOrchestrationPlan plan,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt,
      JobInstance job)
    {
      return this.GetPhaseExecutionContext(requestContext, scopeId, planType, plan, stageName, stageAttempt, phaseName, phaseAttempt).CreateJobContext(job);
    }

    private PhaseExecutionContext GetPhaseExecutionContext(
      IVssRequestContext requestContext,
      Guid scopeId,
      string planType,
      TaskOrchestrationPlan plan,
      string stageName,
      int stageAttempt,
      string phaseName,
      int phaseAttempt)
    {
      PipelineProcess process = plan.GetProcess<PipelineProcess>();
      stageName = stageName ?? PipelineConstants.DefaultJobName;
      Stage stage1 = process.Stages.SingleOrDefault<Stage>((Func<Stage, bool>) (x => x.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase)));
      PhaseNode phase1 = stage1 != null ? stage1.Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase))) : (PhaseNode) null;
      StageInstance stage2 = new StageInstance(stage1);
      int attempt = phaseAttempt;
      PhaseInstance phase2 = new PhaseInstance(phase1, attempt);
      return requestContext.GetService<IPipelineBuilderService>().GetBuilder(requestContext, plan).CreatePhaseExecutionContext(stage2, phase2);
    }

    private List<JobStep> NormalizeDeploymentJobSteps(
      IVssRequestContext requestContext,
      Guid scopeId,
      JobExecutionContext context,
      IList<Step> steps,
      Dictionary<string, string> jobMetaData)
    {
      List<JobStep> list1 = new List<JobStep>();
      List<JobStep> list2 = steps.OfType<JobStep>().ToList<JobStep>();
      ArtifactResolver artifactResolver = new ArtifactResolver(requestContext, scopeId, context.ResourceStore?.Endpoints, context.TaskStore);
      IList<TaskStep> preJobSteps = this.GetPreJobSteps(requestContext, scopeId, context, (IReadOnlyList<JobStep>) list2, jobMetaData);
      if (preJobSteps != null && preJobSteps.Count > 0)
      {
        for (int index = 0; index < preJobSteps.Count; ++index)
        {
          preJobSteps[index].Name = string.Format("__system_{0}", (object) (index + 1));
          list1.AddRange((IEnumerable<JobStep>) this.ResolveStep(context, (IArtifactResolver) artifactResolver, (JobStep) preJobSteps[index]));
        }
      }
      foreach (JobStep step in list2)
        list1.AddRange((IEnumerable<JobStep>) this.ResolveStep(context, (IArtifactResolver) artifactResolver, step));
      IList<TaskStep> decoratorPostJobSteps = JobSchedulerHelper.GetDecoratorPostJobSteps(requestContext, (IPipelineContext) context, (IReadOnlyList<JobStep>) new ReadOnlyCollection<JobStep>((IList<JobStep>) list1));
      if (decoratorPostJobSteps != null && decoratorPostJobSteps.Count > 0)
      {
        for (int index = 0; index < decoratorPostJobSteps.Count; ++index)
        {
          decoratorPostJobSteps[index].Name = string.Format("__system_post_{0}", (object) (index + 1));
          list1.AddRange((IEnumerable<JobStep>) this.ResolveStep(context, (IArtifactResolver) artifactResolver, (JobStep) decoratorPostJobSteps[index]));
        }
      }
      foreach (TaskStep taskStep in list1)
        taskStep.DisplayName = context.ExpandVariables(taskStep.DisplayName, true);
      return list1;
    }

    private void AddReviewAppStepsIfRequired(
      JobExecutionContext jobExecutionContext,
      IList<JobStep> jobSteps)
    {
      if (!jobSteps.Any<JobStep>((Func<JobStep, bool>) (step => step.Type == StepType.Task && (step as TaskStep).Reference.Id.ToString().Equals("DEEAFED4-0B18-4F58-968D-86655B4D2CE9", StringComparison.OrdinalIgnoreCase))))
        return;
      RepositoryResource repositoryResource = jobExecutionContext.ResourceStore.Repositories.GetAll().FirstOrDefault<RepositoryResource>((Func<RepositoryResource, bool>) (repo => repo.Type.Equals("Github", StringComparison.OrdinalIgnoreCase)));
      if (repositoryResource == null)
        return;
      TaskDefinition taskDefinition = jobExecutionContext.TaskStore.ResolveTask("DEEA6198-ADF8-4B16-9939-7ADDF85708B2", "0");
      IList<JobStep> jobStepList = jobSteps;
      TaskInstance legacyTaskInstance = new TaskInstance();
      legacyTaskInstance.Name = taskDefinition.Name;
      legacyTaskInstance.Id = taskDefinition.Id;
      legacyTaskInstance.Version = (string) taskDefinition.Version;
      legacyTaskInstance.Inputs.Add("githubConnection", repositoryResource.Endpoint.Id.ToString());
      TaskStep taskStep = new TaskStep(legacyTaskInstance);
      jobStepList.Add((JobStep) taskStep);
    }

    private ISet<Demand> GetDemandSet(IPipelineContext context, IEnumerable<TaskStep> steps)
    {
      HashSet<Demand> collection = new HashSet<Demand>();
      foreach (TaskStep step in steps)
      {
        TaskDefinition taskDefinition = context.TaskStore.ResolveTask(step.Reference.Id, step.Reference.Version);
        if (taskDefinition != null)
        {
          collection.AddRange<Demand, HashSet<Demand>>((IEnumerable<Demand>) taskDefinition.Demands);
          bool newMinimum;
          string minimumAgentVersion = taskDefinition.GetMinimumAgentVersion((string) null, out newMinimum);
          if (newMinimum)
            collection.Add((Demand) new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, minimumAgentVersion, new DemandSource()
            {
              SourceName = taskDefinition.Name,
              SourceVersion = (string) taskDefinition.Version,
              SourceType = DemandSourceType.Task
            }));
        }
      }
      return (ISet<Demand>) collection;
    }

    private List<JobStep> ResolveStep(
      JobExecutionContext context,
      IArtifactResolver artifactResolver,
      JobStep step)
    {
      List<JobStep> jobStepList = new List<JobStep>();
      IList<JobStep> resolvedSteps = (IList<JobStep>) new List<JobStep>();
      if (this.ResolveTaskStep(context, artifactResolver, step, out resolvedSteps))
        jobStepList.AddRange((IEnumerable<JobStep>) resolvedSteps);
      else
        jobStepList.Add((JobStep) (step as TaskStep));
      return jobStepList;
    }

    private IList<TaskStep> GetPreJobSteps(
      IVssRequestContext requestContext,
      Guid scopeId,
      JobExecutionContext context,
      IReadOnlyList<JobStep> steps,
      Dictionary<string, string> jobMetaData)
    {
      List<TaskStep> preJobSteps = new List<TaskStep>();
      preJobSteps.AddRange((IEnumerable<TaskStep>) this.GetSecreteVariablesSteps(requestContext, context, scopeId));
      preJobSteps.AddRange((IEnumerable<TaskStep>) this.GetDownloadSteps(requestContext, context, scopeId, steps, jobMetaData));
      preJobSteps.AddRange((IEnumerable<TaskStep>) JobSchedulerHelper.GetDecoratorPreJobSteps(requestContext, (IPipelineContext) context, steps));
      return (IList<TaskStep>) preJobSteps;
    }

    private void AddPublishMetadataStep(
      JobExecutionContext jobExecutionContext,
      IList<JobStep> jobSteps)
    {
      TaskDefinition taskDefinition = jobExecutionContext.TaskStore.ResolveTask("01FA79EB-4C54-41B5-A16F-5CD8D60DB88D", "0");
      if (taskDefinition == null)
        return;
      IList<JobStep> jobStepList = jobSteps;
      TaskInstance legacyTaskInstance = new TaskInstance();
      legacyTaskInstance.Name = taskDefinition.Name;
      legacyTaskInstance.Id = taskDefinition.Id;
      legacyTaskInstance.Version = (string) taskDefinition.Version;
      TaskStep taskStep = new TaskStep(legacyTaskInstance);
      jobStepList.Add((JobStep) taskStep);
    }

    private IList<TaskStep> GetDownloadSteps(
      IVssRequestContext requestContext,
      JobExecutionContext context,
      Guid scopeId,
      IReadOnlyList<JobStep> steps,
      Dictionary<string, string> jobMetaData)
    {
      List<TaskStep> downloadSteps = new List<TaskStep>();
      if (JobSchedulerHelper.ShouldAutoInjectDownloadArtifactStep(jobMetaData) && !steps.IsDownloadPipelineArtifactStepExists())
      {
        ArtifactResolver artifactResolver = new ArtifactResolver(requestContext, scopeId);
        TaskStep taskStep1 = new TaskStep();
        taskStep1.DisplayName = PipelineArtifactConstants.DownloadTask.FriendlyName;
        taskStep1.Enabled = true;
        taskStep1.Reference = new TaskStepDefinitionReference()
        {
          Id = PipelineArtifactConstants.DownloadTask.Id,
          Name = PipelineArtifactConstants.DownloadTask.Name,
          Version = (string) PipelineArtifactConstants.DownloadTask.Version
        };
        taskStep1.Inputs.Add("alias", "current");
        taskStep1.Inputs.Add("mode", "automatic");
        TaskStep taskStep2 = taskStep1;
        downloadSteps.Add(taskStep2);
        foreach (PipelineResource pipelineResource in context.ResourceStore?.Pipelines?.GetAll() ?? (IEnumerable<PipelineResource>) new List<PipelineResource>())
        {
          TaskStep taskStep3 = new TaskStep();
          taskStep3.DisplayName = PipelineArtifactConstants.DownloadTask.FriendlyName;
          taskStep3.Enabled = true;
          taskStep3.Reference = new TaskStepDefinitionReference()
          {
            Id = PipelineArtifactConstants.DownloadTask.Id,
            Name = PipelineArtifactConstants.DownloadTask.Name,
            Version = (string) PipelineArtifactConstants.DownloadTask.Version
          };
          taskStep3.Inputs.Add("alias", pipelineResource.Alias);
          taskStep3.Inputs.Add("mode", "automatic");
          TaskStep taskStep4 = taskStep3;
          downloadSteps.Add(taskStep4);
        }
      }
      return (IList<TaskStep>) downloadSteps;
    }

    private IList<TaskStep> GetSecreteVariablesSteps(
      IVssRequestContext requestContext,
      JobExecutionContext context,
      Guid projectId)
    {
      List<TaskStep> collection = new List<TaskStep>();
      foreach (VariableGroupReference group in context.ReferencedResources.VariableGroups.Where<VariableGroupReference>((Func<VariableGroupReference, bool>) (x => x.GroupType == "AzureKeyVault" && x.SecretStore != null && x.SecretStore.Keys.Count > 0)))
      {
        AzureKeyVaultValueProvider vaultValueProvider = new AzureKeyVaultValueProvider(requestContext, projectId);
        collection.AddRangeIfRangeNotNull<TaskStep, List<TaskStep>>((IEnumerable<TaskStep>) vaultValueProvider.GetSteps((IPipelineContext) context, group, (IEnumerable<string>) group.SecretStore.Keys));
      }
      return (IList<TaskStep>) collection;
    }

    private bool ResolveTaskStep(
      JobExecutionContext context,
      IArtifactResolver artifactResolver,
      JobStep step,
      out IList<JobStep> resolvedSteps)
    {
      bool flag = false;
      IList<TaskStep> resolvedSteps1 = (IList<TaskStep>) new List<TaskStep>();
      resolvedSteps = (IList<JobStep>) new List<JobStep>();
      if (step.IsDownloadStepDisabled())
      {
        flag = true;
      }
      else
      {
        IResourceStore resourceStore = context.ResourceStore;
        if ((resourceStore != null ? (resourceStore.ResolveStep((IPipelineContext) context, step, out resolvedSteps1) ? 1 : 0) : 0) != 0)
        {
          foreach (TaskStep taskStep in (IEnumerable<TaskStep>) resolvedSteps1)
            resolvedSteps.Add((JobStep) taskStep);
          flag = true;
        }
        else if (step.IsDownloadBuildTask() || step.IsGetPackageTask())
        {
          string errorMessage = string.Empty;
          if (!artifactResolver.ResolveStep(context.ResourceStore, step as TaskStep, out errorMessage))
            throw new PipelineValidationException(errorMessage);
          resolvedSteps.Add(step);
        }
      }
      return flag;
    }

    private ISet<Demand> ResolveTargetDemmands(
      ISet<Demand> demands,
      JobExecutionContext jobExecutionContext)
    {
      ISet<Demand> demandSet = (ISet<Demand>) new HashSet<Demand>();
      if (demands == null || !demands.Any<Demand>())
        return demandSet;
      foreach (Demand demand1 in (IEnumerable<Demand>) demands)
      {
        if (demand1 != null)
        {
          string str1 = demand1.Value;
          if (string.IsNullOrEmpty(str1))
          {
            demandSet.Add(demand1.Clone());
          }
          else
          {
            string str2 = jobExecutionContext.ExpandVariables(str1, true);
            try
            {
              Demand demand2 = demand1.Clone();
              demand2.Update(str2);
              demandSet.Add(demand2);
            }
            catch (Exception ex)
            {
              throw new PipelineValidationException(PipelineStrings.DemandExpansionInvalid((object) demand1.ToString(), (object) demand1.Value, (object) str2), ex);
            }
          }
        }
      }
      return demandSet;
    }

    private static string GetPhaseOrchestrationId(Guid planId, string stageName, string phaseName)
    {
      string phaseInstanceName = new PipelineIdGenerator().GetPhaseInstanceName(stageName, phaseName, 1);
      return string.Format("{0}.{1}", (object) planId, (object) phaseInstanceName.ToLowerInvariant());
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CleanupOrchestrationHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class CleanupOrchestrationHelper : OrchestrationHelper
  {
    private Version m_minAgentVersion;

    public CleanupOrchestrationHelper(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      Microsoft.VisualStudio.Services.Identity.Identity requestedBy,
      Microsoft.VisualStudio.Services.Identity.Identity requestedFor)
      : base(requestContext, definition, requestedBy, requestedFor)
    {
      this.m_minAgentVersion = new Version(BuildRequestHelper.MinServerSupportedAgentVersion) < ArtifactCleanupConstants.RequiredAgentVersion ? ArtifactCleanupConstants.RequiredAgentVersion : new Version(BuildRequestHelper.MinServerSupportedAgentVersion);
    }

    public void RunOrchestration(
      IVssRequestContext requestContext,
      BuildData build,
      IList<BuildDefinitionStep> cleanupTasks)
    {
      using (requestContext.TraceScope(nameof (CleanupOrchestrationHelper), nameof (RunOrchestration)))
      {
        requestContext.TraceInfo(nameof (CleanupOrchestrationHelper), "Running the cleanup orchestration for build {0} of project {1} for definition {2}", (object) build.BuildNumber, (object) build.ProjectId, (object) this.m_definition.Name);
        if (!requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build2.Retention.FilePathArtifactsAndSymbolsDelete"))
          requestContext.TraceInfo(12030051, nameof (CleanupOrchestrationHelper), "Delete artifacts and symbols Feature is not enabled for build {0} of project {1} for definition {2}", (object) build.BuildNumber, (object) build.ProjectId, (object) this.m_definition.Name);
        else if (cleanupTasks.Count == 0)
        {
          requestContext.TraceInfo(12030051, nameof (CleanupOrchestrationHelper), "There are no cleanup tasks for build {0} of project {1} for definition {2}", (object) build.BuildNumber, (object) build.ProjectId, (object) this.m_definition.Name);
        }
        else
        {
          try
          {
            Guid projectId = build.ProjectId;
            BuildDeleteValidationContext validationContext = new BuildDeleteValidationContext()
            {
              Build = build,
              Definition = this.m_definition
            };
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add("Build Number", validationContext.Build.BuildNumber);
            properties.Add("Build Definition Name", validationContext.Definition.Name);
            properties.Add("Build Project Name", (object) validationContext.Build.ProjectId);
            TaskAgentQueue queue = CleanupOrchestrationHelper.FindQueue(requestContext, validationContext);
            properties.Add("Queue Id", (double) queue.Id);
            if (queue?.Pool == null)
            {
              requestContext.TraceError(12030051, nameof (CleanupOrchestrationHelper), "Pool does not exist for build {0} of project {1} for definition {2}.", (object) build.BuildNumber, (object) build.ProjectId, (object) this.m_definition.Name);
              requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Build2", nameof (CleanupOrchestrationHelper), properties);
            }
            else
            {
              properties.Add("Pool Id", (double) queue.Pool.Id);
              validationContext.DemandsValidationSet = this.FindDemandsValidationSet(requestContext, build);
              List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> matchingDemands = new DemandsRequestValidator().GetMatchingDemands(requestContext, validationContext, queue.AsServerBuildQueue());
              StringBuilder stringBuilder = new StringBuilder();
              if (matchingDemands != null && matchingDemands.Count > 0)
              {
                foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand in matchingDemands)
                  stringBuilder.Append(demand.Name).Append(".");
                properties.Add("DistributedTask.WebApi.Demands for cleanup orchestration", (object) stringBuilder);
              }
              requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Build2", nameof (CleanupOrchestrationHelper), properties);
              if (queue.Pool == null)
                requestContext.TraceError(12030051, nameof (CleanupOrchestrationHelper), "Can't run cleanup orchestration for build {0} of project {1} for definition {2} because the pool to run orchestration is null", (object) build.BuildNumber, (object) build.ProjectId, (object) this.m_definition.Name);
              else if (queue.Pool.IsHosted)
              {
                requestContext.TraceInfo(12030051, nameof (CleanupOrchestrationHelper), "Can't run cleanup orchestration for build {0} of project {1} for definition {2} because build was queued to a hosted pool", (object) build.BuildNumber, (object) build.ProjectId, (object) this.m_definition.Name);
              }
              else
              {
                this.m_definition.Demands.Clear();
                Phase phase = new Phase();
                phase.Steps.AddRange((IEnumerable<BuildDefinitionStep>) cleanupTasks);
                this.m_definition.Process = (BuildProcess) new DesignerProcess()
                {
                  Phases = {
                    phase
                  }
                };
                using (IDisposableReadOnlyList<IBuildOption> extensions = requestContext.GetExtensions<IBuildOption>())
                {
                  List<IBuildOption> list = extensions.OrderBy<IBuildOption, int>((Func<IBuildOption, int>) (feature => feature.GetOrdinal())).ToList<IBuildOption>();
                  IOrchestrationEnvironment environment;
                  IOrchestrationProcess process;
                  this.SetupEnvironment(requestContext, (IList<IBuildOption>) list, matchingDemands.ToBuildTaskDemands().ToList<Demand>(), out environment, out process);
                  IBuildOrchestrator service = requestContext.GetService<IBuildOrchestrator>();
                  TaskOrchestrationPlanReference build1 = requestContext.GetService<BuildService>().AttachOrchestrationToBuild(requestContext, projectId, build.Id, 2);
                  build.Plans.Add(build1);
                  (string, string)[] valueTupleArray = new (string, string)[4];
                  valueTupleArray[0] = ("BuildOrchestrationType", 2.ToString());
                  valueTupleArray[1] = ("projectId", projectId.ToString());
                  valueTupleArray[2] = ("path", build.Definition.Path);
                  int id = build.Definition.Id;
                  valueTupleArray[3] = ("definitionId", id.ToString());
                  (string, string)[] queryParams = valueTupleArray;
                  id = build.Id;
                  Uri artifactUriWithParams = UriHelper.CreateArtifactUriWithParams("Build", id.ToString(), queryParams);
                  service.RunPlan(requestContext, build, queue.Pool, projectId, build1.PlanId, PlanTemplateType.System, artifactUriWithParams, environment, process, (BuildOptions) null, build.RequestedFor, build.QueueOptions);
                }
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(12030046, "Build2", nameof (CleanupOrchestrationHelper), ex);
          }
        }
      }
    }

    internal static TaskAgentQueue FindQueue(
      IVssRequestContext requestContext,
      BuildDeleteValidationContext validationContext)
    {
      ArgumentUtility.CheckForNull<BuildDeleteValidationContext>(validationContext, nameof (validationContext));
      TaskAgentQueue queue = (TaskAgentQueue) null;
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      if (validationContext.Build.QueueId.HasValue)
        queue = service.GetAgentQueue(requestContext, validationContext.Build.ProjectId, validationContext.Build.QueueId.Value);
      if (queue?.Pool == null && validationContext.Definition.Queue != null)
        queue = service.GetAgentQueue(requestContext, validationContext.Build.ProjectId, validationContext.Definition.Queue.Id);
      if (queue == null)
        throw new QueueNotFoundException(BuildServerResources.QueueNotFound((object) validationContext.Build.Id));
      requestContext.TraceInfo("DemandsRequestValidator", "Selected queue {0}", (object) queue.Name);
      return queue;
    }

    private List<List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>> FindDemandsValidationSet(
      IVssRequestContext requestContext,
      BuildData build)
    {
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demandList = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>()
      {
        (Microsoft.TeamFoundation.DistributedTask.WebApi.Demand) new Microsoft.TeamFoundation.DistributedTask.WebApi.DemandExists("Build.Cleanup")
      };
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> list1 = build.Demands.ToDistributedTaskDemands().ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> list2 = this.m_definition.Demands.ToDistributedTaskDemands().ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> buildStepsDemands = this.GetBuildStepsDemands(requestContext, this.m_definition, build);
      List<List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>> demandsValidationSet = new List<List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>();
      if (list1.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>())
        demandsValidationSet.Add(this.InheritDemands((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demandList, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) list1, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) list2, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) buildStepsDemands));
      if (list2.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>())
        demandsValidationSet.Add(this.InheritDemands((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demandList, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) list2, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) buildStepsDemands));
      if (buildStepsDemands.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>())
        demandsValidationSet.Add(this.InheritDemands((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demandList, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) buildStepsDemands));
      demandsValidationSet.Add(this.InheritDemands((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demandList));
      if (list1.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>())
        demandsValidationSet.Add(this.InheritDemands((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) list1, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) list2, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) buildStepsDemands));
      if (list2.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>())
        demandsValidationSet.Add(this.InheritDemands((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) list2, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) buildStepsDemands));
      if (buildStepsDemands.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>())
        demandsValidationSet.Add(this.InheritDemands((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) buildStepsDemands));
      demandsValidationSet.Add((List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) null);
      return demandsValidationSet;
    }

    internal override void SetupEnvironment(
      IVssRequestContext requestContext,
      PipelineBuilder builder)
    {
      throw new NotSupportedException();
    }

    private void SetupEnvironment(
      IVssRequestContext requestContext,
      IList<IBuildOption> options,
      List<Demand> demands,
      out IOrchestrationEnvironment environment,
      out IOrchestrationProcess process)
    {
      environment = this.GetContainerEnvironment(requestContext, out List<TaskInstance> _);
      TaskOrchestrationJob job = this.GetJob(requestContext, demands);
      ref IOrchestrationProcess local = ref process;
      IVssRequestContext requestContext1 = requestContext;
      List<TaskOrchestrationJob> jobs = new List<TaskOrchestrationJob>();
      jobs.Add(job);
      IOrchestrationEnvironment environment1 = environment;
      IList<IBuildOption> options1 = options;
      TaskOrchestrationContainer orchestrationContainer = this.CreateTaskOrchestrationContainer(requestContext1, jobs, environment1, options1);
      local = (IOrchestrationProcess) orchestrationContainer;
    }

    internal override PlanEnvironment BuildContainerEnvironment(
      IVssRequestContext requestContext,
      out List<TaskInstance> tasksToInject)
    {
      tasksToInject = (List<TaskInstance>) null;
      PlanEnvironment planEnvironment = new PlanEnvironment();
      planEnvironment.Variables.Add("build.syncSources", bool.FalseString);
      this.PopulateWellKnownVariables(requestContext, ((IOrchestrationEnvironment) planEnvironment).Variables);
      return planEnvironment;
    }

    private TaskOrchestrationContainer CreateTaskOrchestrationContainer(
      IVssRequestContext requestContext,
      List<TaskOrchestrationJob> jobs,
      IOrchestrationEnvironment environment,
      IList<IBuildOption> options)
    {
      this.m_definition.ApplyPreValidationOptions(options, environment, jobs);
      TaskOrchestrationContainer orchestrationContainer = new TaskOrchestrationContainer();
      orchestrationContainer.Children.AddRange((IEnumerable<TaskOrchestrationItem>) jobs);
      return orchestrationContainer;
    }

    private TaskOrchestrationJob GetJob(
      IVssRequestContext requestContext,
      List<Demand> queueTimeDemands)
    {
      TaskHub taskHub = requestContext.GetService<DistributedTaskHubService>().GetTaskHub(requestContext, "Build");
      DesignerProcess process = this.m_definition.Process as DesignerProcess;
      IVssRequestContext requestContext1 = requestContext;
      string jobName = BuildServerResources.CleanupJobName();
      List<TaskInstance> list1 = process.Phases[0].Steps.Where<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (step => step.Enabled)).Select<BuildDefinitionStep, TaskInstance>((Func<BuildDefinitionStep, TaskInstance>) (x => x.ToTaskInstance())).ToList<TaskInstance>();
      List<TaskInstance> taskInstanceList;
      ref List<TaskInstance> local1 = ref taskInstanceList;
      TaskOrchestrationJob job;
      ref TaskOrchestrationJob local2 = ref job;
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> list2 = queueTimeDemands.ToDistributedTaskDemands().ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
      string minAgentVersion = this.m_minAgentVersion.ToString();
      if (!taskHub.TryCreateJob(requestContext1, jobName, "DeleteArtifacts", list1, out local1, out local2, list2, minAgentVersion: minAgentVersion))
      {
        string str = string.Empty;
        foreach (object obj in taskInstanceList)
          str = obj.ToString() + ", ";
        throw new MissingTasksForDefinition(BuildServerResources.MissingTasksForDefinition((object) str, (object) this.m_definition.Name));
      }
      return job;
    }

    internal List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> InheritDemands(
      params IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>[] items)
    {
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demandList = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
      foreach (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> collection in items)
        demandList.AddRange((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) collection);
      for (int index = demandList.Count - 1; index >= 0; --index)
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand = demandList[index];
        if (demand is DemandMinimumVersion && demand.IsAgentMinimumVersionDemand())
        {
          Version version = new Version(demand.Value);
          if (version >= this.m_minAgentVersion)
            this.m_minAgentVersion = version;
          demandList.RemoveAt(index);
        }
      }
      demandList.Add((Microsoft.TeamFoundation.DistributedTask.WebApi.Demand) new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, this.m_minAgentVersion.ToString()));
      return demandList;
    }

    private List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> GetBuildStepsDemands(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildData build)
    {
      HashSet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demandSet = new HashSet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      foreach (BuildDefinitionStep buildDefinitionStep in definition.AllSteps().Where<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (step => step.Enabled)))
      {
        TaskDefinition taskDefinition = service.GetTaskDefinition(requestContext, buildDefinitionStep.TaskDefinition.Id, buildDefinitionStep.TaskDefinition.VersionSpec);
        if (taskDefinition != null)
          demandSet.AddRange<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand, HashSet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) taskDefinition.Demands);
        else
          requestContext.TraceWarning(12030068, nameof (CleanupOrchestrationHelper), "Cleanup orchestration helper did not find task with id {0} for definition {1} while gathering demands to clean up build {2}.", (object) buildDefinitionStep.TaskDefinition.Id, (object) definition.Id, (object) build.Id);
      }
      return demandSet.ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
    }
  }
}

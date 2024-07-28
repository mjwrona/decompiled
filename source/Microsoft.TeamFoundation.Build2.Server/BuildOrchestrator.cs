// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildOrchestrator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildOrchestrator : IBuildOrchestrator, IVssFrameworkService
  {
    private const string c_layer = "BuildOrchestrator";

    public void CreatePlan(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      PlanTemplateType templateType,
      BuildData build,
      Uri artifactUri,
      IOrchestrationEnvironment environment,
      IOrchestrationProcess implementation,
      BuildOptions buildOptions,
      Guid requestedForId,
      string pipelineInitializationLog = null,
      string pipelineExpandedYaml = null)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (CreatePlan)))
      {
        TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
        string str = build.Id.ToString();
        ISecuredObject securedObject = build.ToSecuredObject();
        TaskOrchestrationOwner definitionReference1 = BuildOrchestrator.GetPlanDefinitionReference(requestContext, securedObject, projectId, build.Definition.Id, build.Definition.Name);
        TaskOrchestrationOwner planOwnerReference = BuildOrchestrator.GetPlanOwnerReference(requestContext, securedObject, projectId, build.Id, build.BuildNumber);
        IVssRequestContext requestContext1 = requestContext;
        Guid scopeIdentifier = projectId;
        Guid planId1 = build.OrchestrationPlan.PlanId;
        string planGroup = str;
        int templateType1 = (int) templateType;
        Uri uri = build.Uri;
        IOrchestrationEnvironment environment1 = environment;
        IOrchestrationProcess process = implementation;
        BuildOptions validationOptions = buildOptions;
        Guid requestedForId1 = requestedForId;
        TaskOrchestrationOwner definitionReference2 = definitionReference1;
        TaskOrchestrationOwner ownerReference = planOwnerReference;
        string pipelineInitializationLog1 = pipelineInitializationLog;
        string pipelineExpandedYaml1 = pipelineExpandedYaml;
        taskHub.CreatePlan(requestContext1, scopeIdentifier, planId1, planGroup, (PlanTemplateType) templateType1, uri, environment1, process, validationOptions, requestedForId1, definitionReference2, ownerReference, pipelineInitializationLog1, pipelineExpandedYaml1);
      }
    }

    public void CancelPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      TimeSpan timeout,
      string reason)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (CancelPlan)))
        requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").CancelPlan(requestContext, projectId, planId, timeout, reason);
    }

    public void CancelStage(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string StageId,
      TimeSpan timeout,
      string reason)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (CancelStage)))
        requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").CancelStage(requestContext, projectId, planId, StageId, timeout, reason);
    }

    public void DeletePlans(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> planIds)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (DeletePlans)))
        requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").DeletePlans(requestContext, projectId, planIds);
    }

    public void RunPlan(
      IVssRequestContext requestContext,
      BuildData build,
      Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolReference pool,
      Guid projectId,
      Guid planId,
      PlanTemplateType templateType,
      Uri artifactUri,
      IOrchestrationEnvironment environment,
      IOrchestrationProcess implementation,
      BuildOptions buildOptions,
      Guid requestedForId,
      QueueOptions queueOptions = QueueOptions.None,
      string pipelineInitializationLog = null,
      string pipelineExpandedYaml = null)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (RunPlan)))
      {
        TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
        ISecuredObject securedObject = build.ToSecuredObject();
        string planGroup = string.Format("{0}:{1}:{2}", (object) taskHub.Name, (object) projectId, (object) build.Id);
        TaskOrchestrationOwner definitionReference = BuildOrchestrator.GetPlanDefinitionReference(requestContext, securedObject, projectId, build.Definition.Id, build.Definition.Name);
        TaskOrchestrationOwner planOwnerReference = BuildOrchestrator.GetPlanOwnerReference(requestContext, securedObject, projectId, build.Id, build.BuildNumber);
        try
        {
          this.SetTriggerVariables(environment, build);
          if (queueOptions.HasFlag((Enum) QueueOptions.DoNotRun))
            taskHub.CreatePlan(requestContext, projectId, planId, planGroup, templateType, artifactUri, environment, implementation, buildOptions, requestedForId, definitionReference, planOwnerReference, pipelineInitializationLog, pipelineExpandedYaml);
          else
            taskHub.RunPlan(requestContext, pool, projectId, planId, planGroup, templateType, artifactUri, environment, implementation, buildOptions, requestedForId, definitionReference, planOwnerReference, pipelineInitializationLog, pipelineExpandedYaml);
        }
        catch (Exception ex1)
        {
          requestContext.TraceException(0, "Build2", "Orchestrator", ex1);
          IBuildService service = requestContext.GetService<IBuildService>();
          List<BuildRequestValidationResult> issues = new List<BuildRequestValidationResult>()
          {
            new BuildRequestValidationResult()
            {
              Result = ValidationResult.Error,
              Message = ex1.Message
            }
          };
          try
          {
            service.AbortBuild(requestContext, projectId, build, issues);
          }
          catch (Exception ex2)
          {
            if (requestContext.IsFeatureEnabled("Build2.AbortPoisonedBuildJobEnabled"))
              requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
              {
                JobExtensionIds.AbortPoisonedBuildsJob
              }, 10);
            requestContext.TraceException(0, "Build2", "Orchestrator", ex2);
          }
        }
      }
    }

    public void StartPlan(IVssRequestContext requestContext, Guid projectId, BuildData build)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (StartPlan)))
      {
        if (build == null)
          requestContext.TraceError(12030023, "Orchestrator", "Unable to start plan for project {0} because the build is null", (object) projectId);
        else if (!build.QueueId.HasValue)
        {
          requestContext.TraceError(12030023, "Orchestrator", "Unable to start plan {0} for build {1} because build.QueueId is null", (object) build.OrchestrationPlan.PlanId, (object) build.Id);
        }
        else
        {
          TaskAgentQueue agentQueue = requestContext.GetService<IDistributedTaskPoolService>().GetAgentQueue(requestContext, build.ProjectId, build.QueueId.Value);
          if (agentQueue?.Pool == null)
          {
            requestContext.TraceError(12030023, "Orchestrator", "Unable to start plan {0} for build {1} because queue.Pool is null", (object) build.OrchestrationPlan.PlanId, (object) build.Id);
          }
          else
          {
            TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
            TaskOrchestrationPlan plan = taskHub.GetPlan(requestContext, projectId, build.OrchestrationPlan.PlanId);
            if (plan != null)
              taskHub.StartPlan(requestContext, agentQueue.Pool, plan);
            else
              requestContext.TraceError(12030023, "Orchestrator", "Unable to start plan {0} for build {1} because it was not found", (object) build.OrchestrationPlan.PlanId, (object) build.Id);
          }
        }
      }
    }

    public ITimelineRecordContext StartJob(
      IVssRequestContext requestContext,
      BuildData build,
      Guid jobId,
      string jobName,
      int? order = null,
      bool discardIfEmpty = false)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (StartJob)))
        return this.StartTimelineRecord(requestContext, build, jobId, "Job", jobName, (ITimelineRecordContext) null, order, discardIfEmpty);
    }

    public ITimelineRecordContext StartTask(
      IVssRequestContext requestContext,
      ITimelineRecordContext parentContext,
      BuildData build,
      Guid taskId,
      string taskName,
      int? order = null)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (StartTask)))
        return this.StartTimelineRecord(requestContext, build, taskId, "Task", taskName, parentContext, order, false);
    }

    public TaskOrchestrationPlan GetPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetPlan)))
        return requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetPlan(requestContext, projectId, planId);
    }

    public IEnumerable<TaskLog> GetLogs(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetLogs)))
        return requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetLogs(requestContext, projectId, planId);
    }

    public Timeline GetTimeline(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = false)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetTimeline)))
        return requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetTimeline(requestContext, projectId, planId, timelineId, changeId, includeRecords);
    }

    public IEnumerable<Timeline> GetTimelines(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetTimelines)))
        return requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetTimelines(requestContext, projectId, planId);
    }

    public IList<StageAttempt> GetAttempts(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string stageName)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetAttempts)))
      {
        TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
        IPipelineRuntimeService pipelineRuntime = requestContext.GetService<IPipelineRuntimeService>();
        return requestContext.RunSynchronously<IList<StageAttempt>>((Func<Task<IList<StageAttempt>>>) (() => pipelineRuntime.GetAllStageAttemptsAsync(requestContext, taskHub.Name, projectId, planId, stageName)));
      }
    }

    public BuildLogLinesResult GetLogLines(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      ISecuredObject securedObject,
      int logId,
      long startLine,
      long endLine)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetLogLines)))
      {
        long totalLines;
        return new BuildLogLinesResult(requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetLogLines(requestContext, projectId, planId, logId, securedObject, ref startLine, ref endLine, out totalLines))
        {
          StartLine = startLine,
          EndLine = endLine,
          TotalLines = totalLines
        };
      }
    }

    public Dictionary<int, BuildLogLinesResult> GetLogLinesBatch(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      ISet<int> filterLogIds)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetLogLinesBatch)))
      {
        TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
        Dictionary<int, BuildLogLinesResult> logLinesBatch = new Dictionary<int, BuildLogLinesResult>();
        IVssRequestContext requestContext1 = requestContext;
        Guid scopeIdentifier = projectId;
        Guid planId1 = planId;
        ISet<int> filterLogIds1 = filterLogIds;
        foreach (KeyValuePair<int, TeamFoundationLogItemData> logsReader in taskHub.GetLogsReaders(requestContext1, scopeIdentifier, planId1, filterLogIds1))
        {
          BuildLogLinesResult buildLogLinesResult = new BuildLogLinesResult(logsReader.Value.Reader)
          {
            StartLine = 0,
            EndLine = logsReader.Value.EndLine,
            TotalLines = logsReader.Value.TotalLines
          };
          logLinesBatch.Add(logsReader.Key, buildLogLinesResult);
        }
        return logLinesBatch;
      }
    }

    public IEnumerable<TaskAttachment> GetAttachments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string type)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetAttachments)))
        return (IEnumerable<TaskAttachment>) requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetAttachments(requestContext, projectId, planId, type);
    }

    public Stream GetAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      TaskAttachment attachment)
    {
      using (requestContext.TraceScope(nameof (BuildOrchestrator), nameof (GetAttachment)))
        return requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build").GetAttachment(requestContext, projectId, planId, attachment.TimelineId, attachment.RecordId, attachment.Type, attachment.Name);
    }

    public static TaskOrchestrationOwner GetPlanOwnerReference(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Guid projectId,
      int buildId,
      string buildNumber)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      TaskOrchestrationOwner planOwnerReference = new TaskOrchestrationOwner();
      planOwnerReference.Id = buildId;
      planOwnerReference.Name = buildNumber;
      IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
      planOwnerReference.Links.TryAddLink("web", securedObject, (Func<string>) (() => routeService.GetBuildWebUrl(requestContext, projectId, buildId)));
      planOwnerReference.Links.TryAddLink("self", securedObject, (Func<string>) (() => routeService.GetBuildRestUrl(requestContext, projectId, buildId)));
      return planOwnerReference;
    }

    public static TaskOrchestrationOwner GetPlanDefinitionReference(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Guid projectId,
      int definitionId,
      string definitionName)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      TaskOrchestrationOwner definitionReference = new TaskOrchestrationOwner();
      definitionReference.Id = definitionId;
      definitionReference.Name = definitionName;
      IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
      definitionReference.Links.TryAddLink("web", securedObject, (Func<string>) (() => routeService.GetDefinitionWebUrl(requestContext, projectId, definitionId)));
      definitionReference.Links.TryAddLink("self", securedObject, (Func<string>) (() => routeService.GetDefinitionRestUrl(requestContext, projectId, definitionId)));
      return definitionReference;
    }

    private ITimelineRecordContext StartTimelineRecord(
      IVssRequestContext requestContext,
      BuildData build,
      Guid recordId,
      string type,
      string name,
      ITimelineRecordContext parentContext,
      int? order,
      bool discardIfEmpty)
    {
      TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
      Timeline timeline = taskHub.GetTimeline(requestContext, build.ProjectId, build.OrchestrationPlan.PlanId, Guid.Empty);
      ArgumentUtility.CheckForNull<Timeline>(timeline, "timeline");
      TimelineRecord timelineRecord = this.CreateTimelineRecord(recordId, type, name, parentContext?.Id, order);
      Guid jobRecordId = !(type == "Job") ? parentContext.Id : recordId;
      if (discardIfEmpty)
        return (ITimelineRecordContext) new LazyTimelineRecordContext(requestContext, taskHub, build.ProjectId, build.OrchestrationPlan.PlanId, timeline.Id, jobRecordId, timelineRecord, parentContext);
      taskHub.UpdateTimeline(requestContext, build.ProjectId, build.OrchestrationPlan.PlanId, timeline.Id, (IList<TimelineRecord>) new TimelineRecord[1]
      {
        timelineRecord
      });
      return (ITimelineRecordContext) new TimelineRecordContext(requestContext, taskHub, build.ProjectId, build.OrchestrationPlan.PlanId, timeline.Id, jobRecordId, timelineRecord, parentContext);
    }

    private TimelineRecord CreateTimelineRecord(
      Guid recordId,
      string type,
      string name,
      Guid? parentId,
      int? order)
    {
      return new TimelineRecord()
      {
        Id = recordId,
        Name = name,
        Order = order,
        RecordType = type,
        StartTime = new DateTime?(DateTime.UtcNow),
        State = new TimelineRecordState?(TimelineRecordState.InProgress),
        ParentId = parentId
      };
    }

    private void SetTriggerVariables(IOrchestrationEnvironment environment, BuildData build)
    {
      if (!(environment is PipelineEnvironment pipelineEnvironment))
        return;
      if (pipelineEnvironment.UserVariables.OfType<Variable>().FirstOrDefault<Variable>((Func<Variable, bool>) (x => x.Name == "resources.triggeringCategory")) == null)
      {
        string str = string.Empty;
        if (BuildReason.ResourceTrigger == build.Reason)
          str = BuildOrchestrator.GetTriggerCategoryFromType(build.TriggerInfo.GetValueOrDefault<string, string>("pipelineTriggerType", string.Empty));
        pipelineEnvironment.UserVariables.Add((IVariable) new Variable()
        {
          Name = "resources.triggeringCategory",
          Value = str
        });
      }
      if (pipelineEnvironment.UserVariables.OfType<Variable>().FirstOrDefault<Variable>((Func<Variable, bool>) (x => x.Name == "resources.triggeringAlias")) != null)
        return;
      string str1 = "";
      if (BuildReason.ResourceTrigger == build.Reason)
        str1 = build.TriggerInfo.GetValueOrDefault<string, string>("alias", string.Empty);
      pipelineEnvironment.UserVariables.Add((IVariable) new Variable()
      {
        Name = "resources.triggeringAlias",
        Value = str1
      });
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private static string GetTriggerCategoryFromType(string triggerType)
    {
      PipelineTriggerType result;
      if (!EnumUtility.TryParse<PipelineTriggerType>(triggerType, out result))
        return string.Empty;
      string categoryFromType;
      switch (result)
      {
        case PipelineTriggerType.ContinuousIntegration:
          categoryFromType = "repository";
          break;
        case PipelineTriggerType.PullRequest:
          categoryFromType = "repository";
          break;
        case PipelineTriggerType.PipelineCompletion:
          categoryFromType = "pipeline";
          break;
        case PipelineTriggerType.ContainerImage:
          categoryFromType = "container";
          break;
        case PipelineTriggerType.BuildResourceCompletion:
          categoryFromType = "build";
          break;
        case PipelineTriggerType.PackageUpdate:
          categoryFromType = "package";
          break;
        case PipelineTriggerType.WebhookTriggeredEvent:
          categoryFromType = "webhook";
          break;
        default:
          categoryFromType = string.Empty;
          break;
      }
      return categoryFromType;
    }
  }
}

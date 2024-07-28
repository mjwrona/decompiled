// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildOrchestrator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildOrchestrator))]
  public interface IBuildOrchestrator : IVssFrameworkService
  {
    void CreatePlan(
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
      string pipelineExpandedYaml = null);

    void CancelPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      TimeSpan timeout,
      string reason);

    void RunPlan(
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
      string pipelineExpandedYaml = null);

    ITimelineRecordContext StartJob(
      IVssRequestContext requestContext,
      BuildData build,
      Guid jobId,
      string jobName,
      int? order = null,
      bool discardIfEmpty = false);

    ITimelineRecordContext StartTask(
      IVssRequestContext requestContext,
      ITimelineRecordContext parentContext,
      BuildData build,
      Guid taskId,
      string taskName,
      int? order = null);

    void StartPlan(IVssRequestContext requestContext, Guid projectId, BuildData build);

    void DeletePlans(IVssRequestContext requestContext, Guid projectId, IEnumerable<Guid> planIds);

    TaskOrchestrationPlan GetPlan(IVssRequestContext requestContext, Guid projectId, Guid planId);

    IEnumerable<TaskLog> GetLogs(IVssRequestContext requestContext, Guid projectId, Guid planId);

    Timeline GetTimeline(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = false);

    IEnumerable<Timeline> GetTimelines(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId);

    IList<StageAttempt> GetAttempts(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string stageName);

    BuildLogLinesResult GetLogLines(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      ISecuredObject securedObject,
      int logId,
      long startLine,
      long endLine);

    Dictionary<int, BuildLogLinesResult> GetLogLinesBatch(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      ISet<int> filterLogIds);

    IEnumerable<TaskAttachment> GetAttachments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string type);

    Stream GetAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      TaskAttachment attachment);

    void CancelStage(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string stageId,
      TimeSpan timeout,
      string reason);
  }
}

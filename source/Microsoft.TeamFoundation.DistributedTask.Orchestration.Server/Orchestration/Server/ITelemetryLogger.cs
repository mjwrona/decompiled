// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ITelemetryLogger
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public interface ITelemetryLogger
  {
    void PublishTaskHubPlanStartedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan);

    void PublishTaskHubPlanCompletedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan);

    void PublishTaskHubPhaseStartedTelemetry(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      Phase phase,
      ExpandPhaseResult expandPhaseResult);

    void PublishTaskHubExecuteTaskTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      Guid authorizationId,
      ServerTaskRequestMessage taskRequest,
      string planTemplateType);

    void PublishTaskHubSendJobTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      int poolId,
      Guid authorizationId,
      Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentJobRequestMessage jobRequest,
      TaskAgentReference agent,
      string planTemplateType);

    void PublishTaskHubJobCompletedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan,
      Timeline timelines,
      Guid jobId);

    void PublishPlanQueueEvaluationJobPlanStartedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationQueuedPlan queuedPlan);

    void PublishPlanQueueEvaluationJobTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      int licensingLimit,
      int runnablePlansFetchedCount,
      int startedPlansCount,
      int startedPlanGroupsCount);
  }
}

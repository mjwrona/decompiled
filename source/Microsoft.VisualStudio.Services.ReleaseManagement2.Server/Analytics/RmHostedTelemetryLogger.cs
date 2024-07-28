// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics.RmHostedTelemetryLogger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics
{
  public class RmHostedTelemetryLogger : IRmTelemetryLogger
  {
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    [StaticSafe]
    private static readonly RmHostedTelemetryLogger instance = new RmHostedTelemetryLogger();

    [StaticSafe]
    public static RmHostedTelemetryLogger Instance => RmHostedTelemetryLogger.instance;

    public void PublishReleaseCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Guid projectId)
    {
      CustomerIntelligenceHelper.PublishReleaseCreated(requestContext, release, projectId);
    }

    public void PublishEventsOnEnvironmentCompletion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      EnvironmentStatus status,
      Guid projectId,
      string message,
      Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int, Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>>> getChangesForEnvironment)
    {
      CustomerIntelligenceHelper.PublishEventsOnEnvironmentCompletion(requestContext, currentRelease, releaseEnvironment, status, projectId, message, getChangesForEnvironment);
    }

    public void PublishReleaseGetByUser(
      IVssRequestContext requestContext,
      int releaseId,
      Guid userId,
      Guid userCuid)
    {
      CustomerIntelligenceHelper.PublishReleaseGetByUser(requestContext, releaseId, userId, userCuid);
    }

    public void PublishUpdateRetainBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId,
      string jobResult)
    {
      CustomerIntelligenceHelper.PublishUpdateRetainBuild(requestContext, projectId, releaseDefinitionId, jobResult);
    }

    public void PublishDefinitionCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      CustomerIntelligenceHelper.PublishDefinitionCreated(requestContext, releaseDefinition);
    }

    public void PublishDefinitionDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId)
    {
      CustomerIntelligenceHelper.PublishDefinitionDeleted(requestContext, projectId, requestorId, releaseDefinitionId);
    }

    public void PublishReleaseCompleted(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release) => CustomerIntelligenceHelper.PublishReleaseCompleted(requestContext, release);

    public void PublishRunPlanCompleted(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      int releaseId,
      Guid projectId,
      TimelineRecord jobTimelineRecord,
      bool timelineRecordsPassed)
    {
      CustomerIntelligenceHelper.PublishRunPlanCompleted(requestContext, plan, releaseId, projectId, jobTimelineRecord, timelineRecordsPassed);
    }

    public void PublishWorkflowFailedEvent(
      IVssRequestContext requestContext,
      int releaseId,
      Guid projectId,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      string message)
    {
      CustomerIntelligenceHelper.PublishWorkflowFailedEvent(requestContext, releaseId, projectId, releaseEnvironmentStep, message);
    }

    public void PublishPlanGroupsStartedEvent(
      IVssRequestContext requestContext,
      IEnumerable<TaskOrchestrationPlanGroupReference> planGroupReferences)
    {
      CustomerIntelligenceHelper.PublishPlanGroupsStartedEvent(requestContext, planGroupReferences);
    }

    public void PublishDefinitionUpdated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
    }

    public void PublishApprovalUpdated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> approvals)
    {
      CustomerIntelligenceHelper.PublishManualApprovalCompletedEvent(requestContext, currentRelease, projectId, approvals);
    }

    public void PublishRevalidateApprovalIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Guid projectId,
      int environmentId,
      DeploymentAuthorizationInfo approverInfo)
    {
      CustomerIntelligenceHelper.PublishRevalidatedApprovalEvent(requestContext, currentRelease, projectId, environmentId, approverInfo);
    }
  }
}

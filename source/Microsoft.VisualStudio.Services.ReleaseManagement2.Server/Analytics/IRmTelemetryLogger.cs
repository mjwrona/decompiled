// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics.IRmTelemetryLogger
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
  public interface IRmTelemetryLogger
  {
    void PublishReleaseCreated(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, Guid projectId);

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the intended design")]
    void PublishEventsOnEnvironmentCompletion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      EnvironmentStatus status,
      Guid projectId,
      string message,
      Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int, Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>>> getChangesForEnvironment);

    void PublishDefinitionCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition);

    void PublishDefinitionDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId);

    void PublishReleaseCompleted(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release);

    void PublishRunPlanCompleted(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      int releaseId,
      Guid projectId,
      TimelineRecord jobTimelineRecord,
      bool timelineRecordsPassed);

    void PublishWorkflowFailedEvent(
      IVssRequestContext requestContext,
      int releaseId,
      Guid projectId,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      string message);

    void PublishReleaseGetByUser(
      IVssRequestContext requestContext,
      int releaseId,
      Guid userId,
      Guid userCuid);

    void PublishUpdateRetainBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId,
      string jobResult);

    void PublishDefinitionUpdated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition);

    void PublishApprovalUpdated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> approvals);

    void PublishRevalidateApprovalIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Guid projectId,
      int environmentId,
      DeploymentAuthorizationInfo approverInfo);

    void PublishPlanGroupsStartedEvent(
      IVssRequestContext requestContext,
      IEnumerable<TaskOrchestrationPlanGroupReference> planGroupReferences);
  }
}

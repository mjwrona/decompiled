// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildDispatcher
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi.Events;
using Microsoft.TeamFoundation.Build2.Server.Events;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildDispatcher))]
  public interface IBuildDispatcher : IVssFrameworkService
  {
    void SendArtifactAdded(
      IVssRequestContext requestContext,
      string clientId,
      int buildId,
      string artifactName);

    void SendChangesCalculated(IVssRequestContext requestContext, string clientId, int buildId);

    void SendBuildUpdated(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      int definitionId,
      int buildId,
      string definitionScope);

    Task SendBuildUpdatedAsync(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      int definitionId,
      int buildId,
      string definitionScope);

    void SendStagesUpdated(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      int buildId,
      Guid timelineId,
      List<TimelineRecordUpdate> stageUpdates);

    Task SendStagesUpdatedAsync(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      int buildId,
      Guid timelineId,
      List<TimelineRecordUpdate> stageUpdates);

    void SendTagsAdded(IVssRequestContext requestContext, string clientId, int buildId);

    void SendTimelineRecordsUpdated(
      IVssRequestContext requestContext,
      string clientId,
      int buildId,
      Guid planId,
      Guid timelineId,
      int changeId);

    Task SendTimelineRecordsUpdatedAsync(
      IVssRequestContext requestContext,
      string clientId,
      int buildId,
      Guid planId,
      Guid timelineId,
      int changeId);

    void SendTaskOrchestrationPlanGroupStarted(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      string planGroup);

    Task WatchBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string clientId);

    Task SyncState(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string clientId);

    Task WatchProject(IVssRequestContext requestContext, Guid projectId, string clientId);

    Task WatchCollection(IVssRequestContext requestContext, string clientId);

    void SendLegacyBuildUpdated(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.Events.BuildUpdatedEvent buildUpdatedEvent);

    void SendLegacyLogConsoleLines(
      IVssRequestContext requestContext,
      ConsoleLogEvent consoleLogEvent);

    Task SendLegacyLogConsoleLinesAsync(
      IVssRequestContext requestContext,
      ConsoleLogEvent consoleLogEvent);
  }
}

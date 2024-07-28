// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildHubClient
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
  public interface IBuildHubClient
  {
    Task buildArtifactAdded(IVssRequestContext requestContext, int buildId, string artifactName);

    Task changesCalculated(IVssRequestContext requestContext, int buildId);

    Task buildUpdated2(
      IVssRequestContext requestContext,
      int definitionId,
      int buildId,
      string definitionScope);

    Task stagesUpdated(
      IVssRequestContext requestContext,
      int buildId,
      Guid timelineId,
      List<TimelineRecordUpdate> stageUpdates);

    Task tagsAdded(IVssRequestContext requestContext, int buildId);

    Task timelineRecordsUpdated(
      IVssRequestContext requestContext,
      int buildId,
      Guid planId,
      Guid timelineId,
      int changeId);

    Task taskOrchestrationPlanGroupStarted(
      IVssRequestContext requestContext,
      Guid projectId,
      string planGroup);

    Task buildUpdated(IVssRequestContext requestContext, Microsoft.TeamFoundation.Build.WebApi.Events.BuildUpdatedEvent buildUpdatedEvent);

    Task logConsoleLines(IVssRequestContext requestContext, ConsoleLogEvent consoleLogEvent);
  }
}

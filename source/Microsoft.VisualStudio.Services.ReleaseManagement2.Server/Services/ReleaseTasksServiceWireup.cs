// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseTasksServiceWireup
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  internal class ReleaseTasksServiceWireup : IReleaseTasksServiceWireUp
  {
    public IEnumerable<ReleaseDeployPhase> GetDeployPhases(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int attemptId)
    {
      return (IEnumerable<ReleaseDeployPhase>) requestContext.GetService<ReleasesService>().GetDeployPhasesForReleaseEnvironment(requestContext, projectId, releaseId, environmentId, releaseDeployPhaseId, attemptId);
    }

    public IEnumerable<TimelineRecord> GetTimelineRecords(
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Guid projectId,
      Guid planId)
    {
      return distributedTaskOrchestrator?.GetTimelineRecords(planId);
    }

    public IEnumerable<TaskLog> GetLogs(
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Guid projectId,
      Guid planId)
    {
      return distributedTaskOrchestrator?.GetLogs(planId);
    }

    public ReleaseLogContainers GetLogContainer(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId)
    {
      return requestContext.GetService<ReleaseLogsService>().GetLogContainersForRelease(requestContext, projectId, releaseId, releaseEnvironmentId);
    }
  }
}

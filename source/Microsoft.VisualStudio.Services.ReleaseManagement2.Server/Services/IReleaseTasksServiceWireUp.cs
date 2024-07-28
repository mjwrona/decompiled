// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.IReleaseTasksServiceWireUp
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
  public interface IReleaseTasksServiceWireUp
  {
    IEnumerable<ReleaseDeployPhase> GetDeployPhases(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int attemptId);

    IEnumerable<TimelineRecord> GetTimelineRecords(
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Guid projectId,
      Guid planId);

    IEnumerable<TaskLog> GetLogs(
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Guid projectId,
      Guid runPlanId);

    ReleaseLogContainers GetLogContainer(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId);
  }
}

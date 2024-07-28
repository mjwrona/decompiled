// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.IPlanTrackingExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public interface IPlanTrackingExtension
  {
    Task LogMessage(
      Guid hostId,
      Guid planId,
      Guid jobId,
      DateTime timestamp,
      TraceLevel level,
      string message);

    Task PlanStarted(Guid hostId, Guid planId, DateTime timestamp);

    Task PlanCompleted(
      Guid hostId,
      Guid planId,
      DateTime timestamp,
      TaskResult result,
      string resultCode);
  }
}

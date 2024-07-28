// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IStrategyExecutor2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal interface IStrategyExecutor2
  {
    void Initialize();

    IList<RunDeploymentLifeCycleInput> GetNextLifeCycles(
      RunDeploymentPhaseInput2 phaseInput,
      out List<int> newTargetResourceIds);

    void OnLifeCycleCompleted(
      string cycleInstanceName,
      TaskResult result,
      out Dictionary<int, TaskResult> resourceDeploymentResult);

    TaskResult? GetDeploymentResult(out IList<TimelineRecord> timelineRecords);

    bool IsDeploymentCompleted();
  }
}

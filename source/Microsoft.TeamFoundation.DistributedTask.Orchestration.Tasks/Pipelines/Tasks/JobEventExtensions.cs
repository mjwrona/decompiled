// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.JobEventExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public static class JobEventExtensions
  {
    public static JobEvent TrimForRuntime(this JobEvent eventData)
    {
      if (eventData.Name.Equals("JobAssigned", StringComparison.OrdinalIgnoreCase))
        (eventData as JobAssignedEvent).Request.TrimForRuntime();
      return eventData;
    }
  }
}

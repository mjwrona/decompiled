// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.ActivityDispatcherPerformanceCounters
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class ActivityDispatcherPerformanceCounters
  {
    public readonly VssPerformanceCounter ActivitiesExecutedPerSec;
    public readonly VssPerformanceCounter ActivityMessagesQueuedPerSec;
    public readonly VssPerformanceCounter AverageActivityExecutionTime;
    public readonly VssPerformanceCounter AverageActivityExecutionTimeBase;
    public readonly VssPerformanceCounter AverageActivityJobDelay;
    public readonly VssPerformanceCounter AverageActivityJobDelayBase;
    public readonly VssPerformanceCounter AverageActivityMessageDelay;
    public readonly VssPerformanceCounter AverageActivityMessageDelayBase;
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_";

    public ActivityDispatcherPerformanceCounters(string hubName, string dispatcherType = null)
    {
      string instanceName = hubName;
      if (!string.IsNullOrEmpty(dispatcherType))
        instanceName = hubName + "-" + dispatcherType;
      this.ActivitiesExecutedPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_ActivitiesExecutedPerSec", instanceName);
      this.ActivityMessagesQueuedPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_ActivityMessagesQueuedPerSec", instanceName);
      this.AverageActivityExecutionTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageActivityExecutionTime", instanceName);
      this.AverageActivityExecutionTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageActivityExecutionTimeBase", instanceName);
      this.AverageActivityJobDelay = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageActivityJobDelay", instanceName);
      this.AverageActivityJobDelayBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageActivityJobDelayBase", instanceName);
      this.AverageActivityMessageDelay = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageActivityMessageDelay", instanceName);
      this.AverageActivityMessageDelayBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageActivityMessageDelayBase", instanceName);
    }
  }
}

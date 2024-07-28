// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationDispatcherPerformanceCounters
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationDispatcherPerformanceCounters
  {
    public readonly VssPerformanceCounter AverageOrchestrationExecutionTime;
    public readonly VssPerformanceCounter AverageOrchestrationExecutionTimeBase;
    public readonly VssPerformanceCounter AverageOrchestrationJobDelay;
    public readonly VssPerformanceCounter AverageOrchestrationJobDelayBase;
    public readonly VssPerformanceCounter AverageOrchestrationSessionDelay;
    public readonly VssPerformanceCounter AverageOrchestrationSessionDelayBase;
    public readonly VssPerformanceCounter AverageOrchestrationSessionUpdateTime;
    public readonly VssPerformanceCounter AverageOrchestrationSessionUpdateTimeBase;
    public readonly VssPerformanceCounter OrchestrationsExecutedPerSec;
    public readonly VssPerformanceCounter OrchestrationSessionsQueuedPerSec;
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_";

    public OrchestrationDispatcherPerformanceCounters(string hubName)
    {
      this.AverageOrchestrationExecutionTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageOrchestrationExecutionTime", hubName);
      this.AverageOrchestrationExecutionTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageOrchestrationExecutionTimeBase", hubName);
      this.AverageOrchestrationJobDelay = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageOrchestrationJobDelay", hubName);
      this.AverageOrchestrationJobDelayBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageOrchestrationJobDelayBase", hubName);
      this.AverageOrchestrationSessionDelay = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageOrchestrationSessionDelay", hubName);
      this.AverageOrchestrationSessionDelayBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageOrchestrationSessionDelayBase", hubName);
      this.AverageOrchestrationSessionUpdateTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageOrchestrationSessionUpdateTime", hubName);
      this.AverageOrchestrationSessionUpdateTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_AverageOrchestrationSessionUpdateTimeBase", hubName);
      this.OrchestrationsExecutedPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_OrchestrationsExecutedPerSec", hubName);
      this.OrchestrationSessionsQueuedPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Orchestration_OrchestrationSessionsQueuedPerSec", hubName);
    }
  }
}

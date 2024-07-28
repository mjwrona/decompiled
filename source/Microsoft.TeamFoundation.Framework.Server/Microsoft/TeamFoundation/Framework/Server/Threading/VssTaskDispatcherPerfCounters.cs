// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssTaskDispatcherPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  internal static class VssTaskDispatcherPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_";
    internal const string CurrentTasksExecutingCount = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_CurrentTasksExecutingCount";
    internal const string CurrentTasksQueuedCount = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_CurrentTasksQueuedCount";
    internal const string CurrentTasksScheduledCount = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_CurrentTasksScheduledCount";
    internal const string TasksExecutedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_TasksExecutedPerSec";
    internal const string TasksQueuedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_TasksQueuedPerSec";
    internal const string TasksScheduledPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_TasksScheduledPerSec";
    internal const string AverageTaskExecutionTime = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_AverageTaskExecutionTime";
    internal const string AverageTaskExecutionTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_AverageTaskExecutionTimeBase";
    internal const string AverageTaskQueueTime = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_AverageTaskQueueTime";
    internal const string AverageTaskQueueTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_TaskDispatcher_AverageTaskQueueTimeBase";
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobAgent.JobApplicationSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.JobAgent
{
  public class JobApplicationSettings
  {
    internal const int DefaultMaxJobsTotal = 2147483647;
    internal const int DefaultMaxJobsPerCPU = 8;
    internal const int DefaultRetryDelaySeconds = 20;
    internal const int DefaultMaxJobResultMessageLength = 8000;
    internal const bool DefaultUnregisterLocalInactiveProcesses = true;
    internal const int DefaultSlowQueueThresholdMilliseconds = 1000;

    internal JobApplicationSettings(
      int maxJobsTotal,
      int maxJobsPerProcessor,
      int retryDelaySeconds,
      int maxJobResultMessageLength,
      bool unregisterLocalInactiveProcesses,
      int slowQueueThresholdMilliseconds)
    {
      this.MaxJobsTotal = maxJobsTotal;
      this.MaxJobsPerProcessor = maxJobsPerProcessor;
      this.RetryDelaySeconds = retryDelaySeconds;
      this.MaxJobResultMessageLength = maxJobResultMessageLength;
      this.UnregisterLocalInnactiveProcesses = unregisterLocalInactiveProcesses;
      this.SlowQueueThresholdMilliseconds = slowQueueThresholdMilliseconds;
    }

    internal int MaxJobsTotal { get; }

    internal int MaxJobsPerProcessor { get; }

    internal int RetryDelaySeconds { get; }

    internal int MaxJobResultMessageLength { get; }

    internal bool UnregisterLocalInnactiveProcesses { get; }

    internal int SlowQueueThresholdMilliseconds { get; }

    public int MaxJobRunners => Math.Min(this.MaxJobsTotal, this.MaxJobsPerProcessor * Environment.ProcessorCount);
  }
}

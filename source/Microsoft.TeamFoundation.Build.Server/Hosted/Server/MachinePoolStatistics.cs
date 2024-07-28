// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Hosted.Server.MachinePoolStatistics
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build.Hosted.Server
{
  public class MachinePoolStatistics
  {
    public DateTime StartTime { get; internal set; }

    public DateTime EndTime { get; internal set; }

    public string MachinePool { get; internal set; }

    public int BuildCount { get; internal set; }

    public int MachineCount { get; internal set; }

    public TimeSpan AverageDuration { get; internal set; }

    public TimeSpan TotalDuration { get; internal set; }

    public TimeSpan AverageBuildDuration { get; internal set; }

    public TimeSpan TotalBuildDuration { get; internal set; }

    public TimeSpan MinimumQueuedDuration { get; internal set; }

    public TimeSpan AverageQueuedDuration { get; internal set; }

    public TimeSpan MaximumQueuedDuration { get; internal set; }

    public TimeSpan TotalIdleDuration { get; internal set; }

    public TimeSpan TotalUpDuration { get; internal set; }

    public int ThresholdViolations { get; internal set; }

    public int Failures { get; internal set; }

    public int ShortBuilds { get; internal set; }

    public double Availability => (1.0 - ((double) this.ThresholdViolations + (double) this.Failures) / (double) this.BuildCount) * 100.0;

    public double IdlePercentage => this.TotalIdleDuration.TotalSeconds / (this.EndTime - this.StartTime).TotalSeconds / (double) this.MachineCount * 100.0;

    public int DesiredMachineCount { get; internal set; }
  }
}

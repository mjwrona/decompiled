// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.Migration.ParallelHostMigrationConfig
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.Migration
{
  public class ParallelHostMigrationConfig
  {
    private const int c_totalBlobCopyJobs = 1;
    private const int c_maxCopyThreadsPerJob = 8;
    private const int c_delayInMilliSecondsBetweenBlobCopyJobs = 30000;
    private const int c_inContainerParallelism = 16;
    private const int c_blobMetadataTablePrefixParallelism = 16;

    public int? TotalParallelJobsPerShardingGroup { get; set; }

    public int? DelayInMilliSecondsBetweenCopyJobs { get; set; }

    public int? InContainerParallelism { get; set; }

    public int? MaxCopyThreadsPerCopyJob { get; set; }

    public int? MaxConcurrentJobsPerJobAgent { get; set; }

    public bool? PreMigrationStopOnPendingCopy { get; set; }

    public bool? Tracing { get; set; }

    public int? BlobMetadataTablePrefixParallelism { get; set; }

    public bool? EnableTableMigrationCheckpoints { get; set; }

    public bool? EnableTablePrefixParallelism { get; set; }

    public bool? EnableBlobMigrationParallelism { get; set; }

    public bool UnsetUnspecified { get; set; }

    internal bool Validate(bool isProd, Action<TraceLevel, string> log)
    {
      if (this.MaxCopyThreadsPerCopyJob.HasValue && this.MaxCopyThreadsPerCopyJob.Value < 1)
      {
        log(TraceLevel.Warning, string.Format("{0} < 1. Use {1} instead.", (object) "MaxCopyThreadsPerCopyJob", (object) 8));
        this.MaxCopyThreadsPerCopyJob = new int?(8);
      }
      if (this.TotalParallelJobsPerShardingGroup.HasValue && this.TotalParallelJobsPerShardingGroup.Value < 1)
      {
        log(TraceLevel.Warning, string.Format("{0} < 1. Use {1} instead.", (object) "TotalParallelJobsPerShardingGroup", (object) 1));
        this.TotalParallelJobsPerShardingGroup = new int?(1);
      }
      if (this.TotalParallelJobsPerShardingGroup.HasValue && this.TotalParallelJobsPerShardingGroup.Value > 1)
      {
        if (!this.DelayInMilliSecondsBetweenCopyJobs.HasValue)
        {
          log(TraceLevel.Warning, "TotalParallelJobsPerShardingGroup > 1, " + string.Format("but {0} is not set. Default to {1}.", (object) "DelayInMilliSecondsBetweenCopyJobs", (object) 30000));
          this.DelayInMilliSecondsBetweenCopyJobs = new int?(30000);
        }
        else if (isProd && this.DelayInMilliSecondsBetweenCopyJobs.Value < 10000)
        {
          log(TraceLevel.Error, "TotalParallelJobsPerShardingGroup > 1, but DelayInMilliSecondsBetweenCopyJobs < 10000. Must use a longer interval to help distribute jobs across JAs evenly.");
          return false;
        }
        if (!this.InContainerParallelism.HasValue)
        {
          log(TraceLevel.Warning, "TotalParallelJobsPerShardingGroup > 1, " + string.Format("but {0} is not set. Default to {1}.", (object) "InContainerParallelism", (object) 16));
          this.InContainerParallelism = new int?(16);
        }
        else if (this.InContainerParallelism.Value <= 1)
        {
          log(TraceLevel.Info, "TotalParallelJobsPerShardingGroup > 1. But InContainerParallelism <= 1. Depending on the number of storage containers to be moved, this may not achieve the best performance.");
        }
        else
        {
          int? containerParallelism = this.InContainerParallelism;
          int num1 = 16;
          if (!(containerParallelism.GetValueOrDefault() == num1 & containerParallelism.HasValue))
          {
            containerParallelism = this.InContainerParallelism;
            int num2 = 256;
            if (!(containerParallelism.GetValueOrDefault() == num2 & containerParallelism.HasValue))
              log(TraceLevel.Warning, "TotalParallelJobsPerShardingGroup > 1. But InContainerParallelism is not a valid value. Will approximate to the nearest valid value (16 and 256), rounded down.");
          }
        }
        if (this.PreMigrationStopOnPendingCopy.GetValueOrDefault())
          log(TraceLevel.Warning, "TotalParallelJobsPerShardingGroup > 1, but PreMigrationStopOnPendingCopy is set. This may cause excessive job reschedules in pre-migration.");
        if (!this.Tracing.GetValueOrDefault())
        {
          log(TraceLevel.Error, "TotalParallelJobsPerShardingGroup > 1, but Tracing is not set. To help diagnose the issue during migration for large-size account, make sure the tracing is set.");
          return false;
        }
      }
      return true;
    }
  }
}

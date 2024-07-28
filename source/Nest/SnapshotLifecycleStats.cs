// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotLifecycleStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class SnapshotLifecycleStats
  {
    [DataMember(Name = "retention_runs")]
    public long RetentionRuns { get; internal set; }

    [DataMember(Name = "retention_failed")]
    public long RetentionFailed { get; internal set; }

    [DataMember(Name = "retention_timed_out")]
    public long RetentionTimedOut { get; internal set; }

    [DataMember(Name = "retention_deletion_time")]
    public string RetentionDeletionTime { get; internal set; }

    [DataMember(Name = "retention_deletion_time_millis")]
    public long RetentionDeletionTimeMilliseconds { get; internal set; }

    [DataMember(Name = "total_snapshots_taken")]
    public long TotalSnapshotsTaken { get; internal set; }

    [DataMember(Name = "total_snapshots_failed")]
    public long TotalSnapshotsFailed { get; internal set; }

    [DataMember(Name = "total_snapshots_deleted")]
    public long TotalSnapshotsDeleted { get; internal set; }

    [DataMember(Name = "total_snapshot_deletion_failures")]
    public long TotalSnapshotsDeletionFailures { get; internal set; }
  }
}

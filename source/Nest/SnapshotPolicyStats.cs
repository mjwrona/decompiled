// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotPolicyStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class SnapshotPolicyStats
  {
    [DataMember(Name = "policy")]
    public string PolicyId { get; internal set; }

    [DataMember(Name = "snapshots_taken")]
    public long SnapshotsTaken { get; internal set; }

    [DataMember(Name = "snapshots_failed")]
    public long SnapshotsFailed { get; internal set; }

    [DataMember(Name = "snapshots_deleted")]
    public long SnapshotsDeleted { get; internal set; }

    [DataMember(Name = "snapshot_deletion_failures")]
    public long SnapshotsDeletionFailures { get; internal set; }
  }
}

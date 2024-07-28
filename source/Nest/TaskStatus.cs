// Decompiled with JetBrains decompiler
// Type: Nest.TaskStatus
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class TaskStatus
  {
    [DataMember(Name = "batches")]
    public long Batches { get; internal set; }

    [DataMember(Name = "created")]
    public long Created { get; internal set; }

    [DataMember(Name = "deleted")]
    public long Deleted { get; internal set; }

    [DataMember(Name = "noops")]
    public long Noops { get; internal set; }

    [DataMember(Name = "requests_per_second")]
    public float RequestsPerSecond { get; internal set; }

    [DataMember(Name = "retries")]
    public TaskRetries Retries { get; internal set; }

    [DataMember(Name = "throttled_millis")]
    public long ThrottledMilliseconds { get; internal set; }

    [DataMember(Name = "throttled_until_millis")]
    public long ThrottledUntilMilliseconds { get; internal set; }

    [DataMember(Name = "total")]
    public long Total { get; internal set; }

    [DataMember(Name = "updated")]
    public long Updated { get; internal set; }

    [DataMember(Name = "version_conflicts")]
    public long VersionConflicts { get; internal set; }
  }
}

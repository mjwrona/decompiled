// Decompiled with JetBrains decompiler
// Type: Nest.RecoveryIndexStatus
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class RecoveryIndexStatus
  {
    [Obsolete("Deprecated. Use Size instead. Will be removed in 8.0")]
    public RecoveryBytes Bytes => this.Size;

    [DataMember(Name = "files")]
    public RecoveryFiles Files { get; internal set; }

    [DataMember(Name = "size")]
    public RecoveryBytes Size { get; internal set; }

    [DataMember(Name = "source_throttle_time_in_millis")]
    public long SourceThrottleTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "target_throttle_time_in_millis")]
    public long TargetThrottleTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "total_time_in_millis")]
    public long TotalTimeInMilliseconds { get; internal set; }
  }
}

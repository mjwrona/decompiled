// Decompiled with JetBrains decompiler
// Type: Nest.TransformCheckpointStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class TransformCheckpointStats
  {
    [DataMember(Name = "checkpoint")]
    public long Checkpoint { get; internal set; }

    [DataMember(Name = "checkpoint_progress")]
    public TransformProgress CheckpointProgress { get; internal set; }

    [DataMember(Name = "timestamp_millis")]
    public long TimestampMilliseconds { get; internal set; }

    [IgnoreDataMember]
    public DateTimeOffset Timestamp => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.TimestampMilliseconds);

    [DataMember(Name = "time_upper_bound_millis")]
    public long TimeUpperBoundMilliseconds { get; internal set; }

    [IgnoreDataMember]
    public DateTimeOffset TimeUpperBound => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.TimeUpperBoundMilliseconds);
  }
}

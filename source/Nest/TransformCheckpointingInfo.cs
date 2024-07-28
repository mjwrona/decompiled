// Decompiled with JetBrains decompiler
// Type: Nest.TransformCheckpointingInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class TransformCheckpointingInfo
  {
    [DataMember(Name = "changes_last_detected_at")]
    public long ChangesLastDetectedAt { get; internal set; }

    [IgnoreDataMember]
    public DateTimeOffset ChangesLastDetectedAtDateTime => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.ChangesLastDetectedAt);

    [DataMember(Name = "last")]
    public TransformCheckpointStats Last { get; internal set; }

    [DataMember(Name = "next")]
    public TransformCheckpointStats Next { get; internal set; }

    [DataMember(Name = "operations_behind")]
    public long OperationsBehind { get; internal set; }
  }
}

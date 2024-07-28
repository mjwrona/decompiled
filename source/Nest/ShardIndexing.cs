// Decompiled with JetBrains decompiler
// Type: Nest.ShardIndexing
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardIndexing
  {
    [DataMember(Name = "delete_current")]
    public long DeleteCurrent { get; internal set; }

    [DataMember(Name = "delete_time_in_millis")]
    public long DeleteTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "delete_total")]
    public long DeleteTotal { get; internal set; }

    [DataMember(Name = "index_current")]
    public long IndexCurrent { get; internal set; }

    [DataMember(Name = "index_failed")]
    public long IndexFailed { get; internal set; }

    [DataMember(Name = "index_time_in_millis")]
    public long IndexTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "index_total")]
    public long IndexTotal { get; internal set; }

    [DataMember(Name = "is_throttled")]
    public bool IsThrottled { get; internal set; }

    [DataMember(Name = "noop_update_total")]
    public long NoopUpdateTotal { get; internal set; }

    [DataMember(Name = "throttle_time_in_millis")]
    public long ThrottleTimeInMilliseconds { get; internal set; }
  }
}

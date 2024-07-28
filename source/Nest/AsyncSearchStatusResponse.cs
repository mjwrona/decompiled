// Decompiled with JetBrains decompiler
// Type: Nest.AsyncSearchStatusResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class AsyncSearchStatusResponse : ResponseBase
  {
    [DataMember(Name = "_shards")]
    public ShardStatistics Shards { get; internal set; }

    [DataMember(Name = "completion_status")]
    public int? CompletionStatus { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "is_partial")]
    public bool IsPartial { get; internal set; }

    [DataMember(Name = "start_time_in_millis")]
    public long StartTimeInMilliseconds { get; internal set; }

    [IgnoreDataMember]
    public DateTimeOffset StartTime => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.StartTimeInMilliseconds);

    [DataMember(Name = "is_running")]
    public bool IsRunning { get; internal set; }

    [DataMember(Name = "expiration_time_in_millis")]
    public long ExpirationTimeInMilliseconds { get; internal set; }

    [IgnoreDataMember]
    public DateTimeOffset ExpirationTime => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.ExpirationTimeInMilliseconds);
  }
}

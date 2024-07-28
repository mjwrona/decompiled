// Decompiled with JetBrains decompiler
// Type: Nest.WatchRecordQueuedStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class WatchRecordQueuedStats
  {
    [DataMember(Name = "execution_time")]
    public DateTimeOffset? ExecutionTime { get; internal set; }

    [DataMember(Name = "triggered_time")]
    public DateTimeOffset? TriggeredTime { get; internal set; }

    [DataMember(Name = "watch_id")]
    public string WatchId { get; internal set; }

    [DataMember(Name = "watch_record_id")]
    public string WatchRecordId { get; internal set; }
  }
}

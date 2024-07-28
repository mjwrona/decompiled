// Decompiled with JetBrains decompiler
// Type: Nest.Snapshot
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class Snapshot
  {
    [DataMember(Name = "duration_in_millis")]
    public long DurationInMilliseconds { get; internal set; }

    [DataMember(Name = "end_time")]
    public DateTime EndTime { get; internal set; }

    [DataMember(Name = "end_time_in_millis")]
    public long EndTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "failures")]
    public IReadOnlyCollection<SnapshotShardFailure> Failures { get; internal set; }

    [DataMember(Name = "indices")]
    public IReadOnlyCollection<IndexName> Indices { get; internal set; }

    [DataMember(Name = "snapshot")]
    public string Name { get; internal set; }

    [DataMember(Name = "shards")]
    public ShardStatistics Shards { get; internal set; }

    [DataMember(Name = "start_time")]
    public DateTime StartTime { get; internal set; }

    [DataMember(Name = "start_time_in_millis")]
    public long StartTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "state")]
    public string State { get; internal set; }

    [DataMember(Name = "metadata")]
    public IReadOnlyDictionary<string, object> Metadata { get; internal set; } = EmptyReadOnly<string, object>.Dictionary;
  }
}

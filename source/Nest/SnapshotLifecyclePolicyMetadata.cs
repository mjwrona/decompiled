// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotLifecyclePolicyMetadata
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class SnapshotLifecyclePolicyMetadata
  {
    [DataMember(Name = "modified_date_millis")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset ModifiedDate { get; internal set; }

    [DataMember(Name = "next_execution_millis")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset NextExecution { get; internal set; }

    [DataMember(Name = "policy")]
    public SnapshotLifecyclePolicy Policy { get; internal set; }

    [DataMember(Name = "version")]
    public int Version { get; internal set; }

    [DataMember(Name = "in_progress")]
    public SnapshotLifecycleInProgress InProgress { get; internal set; }

    [DataMember(Name = "last_success")]
    public SnapshotLifecycleInvocationRecord LastSuccess { get; set; }

    [DataMember(Name = "last_failure")]
    public SnapshotLifecycleInvocationRecord LastFailure { get; set; }
  }
}

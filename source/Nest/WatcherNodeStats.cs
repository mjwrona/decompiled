// Decompiled with JetBrains decompiler
// Type: Nest.WatcherNodeStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class WatcherNodeStats
  {
    [DataMember(Name = "current_watches")]
    public IReadOnlyCollection<WatchRecordStats> CurrentWatches { get; internal set; } = EmptyReadOnly<WatchRecordStats>.Collection;

    [DataMember(Name = "execution_thread_pool")]
    public ExecutionThreadPool ExecutionThreadPool { get; internal set; }

    [DataMember(Name = "queued_watches")]
    public IReadOnlyCollection<WatchRecordQueuedStats> QueuedWatches { get; internal set; } = EmptyReadOnly<WatchRecordQueuedStats>.Collection;

    [DataMember(Name = "watch_count")]
    public long WatchCount { get; internal set; }

    [DataMember(Name = "watcher_state")]
    public WatcherState WatcherState { get; internal set; }
  }
}

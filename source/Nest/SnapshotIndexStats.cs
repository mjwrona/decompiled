// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotIndexStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class SnapshotIndexStats
  {
    [DataMember(Name = "shards")]
    public IReadOnlyDictionary<string, SnapshotShardsStats> Shards { get; internal set; } = EmptyReadOnly<string, SnapshotShardsStats>.Dictionary;

    [DataMember(Name = "shards_stats")]
    public SnapshotShardsStats ShardsStats { get; internal set; }

    [DataMember(Name = "stats")]
    public SnapshotStats Stats { get; internal set; }
  }
}

﻿// Decompiled with JetBrains decompiler
// Type: Nest.ShardHealthStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardHealthStats
  {
    [DataMember(Name = "active_shards")]
    public int ActiveShards { get; internal set; }

    [DataMember(Name = "initializing_shards")]
    public int InitializingShards { get; internal set; }

    [DataMember(Name = "primary_active")]
    public bool PrimaryActive { get; internal set; }

    [DataMember(Name = "relocating_shards")]
    public int RelocatingShards { get; internal set; }

    [DataMember(Name = "status")]
    public Health Status { get; internal set; }

    [DataMember(Name = "unassigned_shards")]
    public int UnassignedShards { get; internal set; }
  }
}

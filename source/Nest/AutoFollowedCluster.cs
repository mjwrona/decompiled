// Decompiled with JetBrains decompiler
// Type: Nest.AutoFollowedCluster
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class AutoFollowedCluster
  {
    [DataMember(Name = "cluster_name")]
    public string ClusterName { get; internal set; }

    [DataMember(Name = "time_since_last_check_millis")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset TimeSinceLastCheck { get; internal set; }

    [DataMember(Name = "last_seen_metadata_version")]
    public long LastSeenMetadataVersion { get; internal set; }
  }
}

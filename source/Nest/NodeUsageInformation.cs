// Decompiled with JetBrains decompiler
// Type: Nest.NodeUsageInformation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class NodeUsageInformation
  {
    [DataMember(Name = "aggregations")]
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, long>> Aggregations { get; internal set; }

    [DataMember(Name = "rest_actions")]
    public IReadOnlyDictionary<string, int> RestActions { get; internal set; }

    [DataMember(Name = "since")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset Since { get; internal set; }

    [DataMember(Name = "timestamp")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset Timestamp { get; internal set; }
  }
}

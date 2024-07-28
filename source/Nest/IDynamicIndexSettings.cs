// Decompiled with JetBrains decompiler
// Type: Nest.IDynamicIndexSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (DynamicIndexSettingsFormatter))]
  public interface IDynamicIndexSettings : 
    IIsADictionary<string, object>,
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable,
    IIsADictionary
  {
    IAnalysis Analysis { get; set; }

    AutoExpandReplicas AutoExpandReplicas { get; set; }

    bool? BlocksMetadata { get; set; }

    bool? BlocksRead { get; set; }

    bool? BlocksReadOnly { get; set; }

    bool? BlocksWrite { get; set; }

    bool? BlocksReadOnlyAllowDelete { get; set; }

    IMergeSettings Merge { get; set; }

    int? NumberOfReplicas { get; set; }

    int? Priority { get; set; }

    Union<int, Nest.RecoveryInitialShards> RecoveryInitialShards { get; set; }

    Time RefreshInterval { get; set; }

    bool? RequestsCacheEnabled { get; set; }

    int? RoutingAllocationTotalShardsPerNode { get; set; }

    ISimilarities Similarity { get; set; }

    ISlowLog SlowLog { get; set; }

    ITranslogSettings Translog { get; set; }

    Time UnassignedNodeLeftDelayedTimeout { get; set; }

    string DefaultPipeline { get; set; }

    [Obsolete("Use FinalPipeline")]
    string RequiredPipeline { get; set; }

    string FinalPipeline { get; set; }
  }
}

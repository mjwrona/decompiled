// Decompiled with JetBrains decompiler
// Type: Nest.DynamicIndexSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class DynamicIndexSettings : 
    IsADictionaryBase<string, object>,
    IDynamicIndexSettings,
    IIsADictionary<string, object>,
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable,
    IIsADictionary
  {
    private Time _refreshInterval;

    public DynamicIndexSettings()
    {
    }

    public DynamicIndexSettings(IDictionary<string, object> container)
      : base(container)
    {
    }

    public IAnalysis Analysis { get; set; }

    public AutoExpandReplicas AutoExpandReplicas { get; set; }

    public bool? BlocksMetadata { get; set; }

    public bool? BlocksRead { get; set; }

    public bool? BlocksReadOnly { get; set; }

    public bool? BlocksWrite { get; set; }

    public bool? BlocksReadOnlyAllowDelete { get; set; }

    public IMergeSettings Merge { get; set; }

    public int? NumberOfReplicas { get; set; }

    public int? Priority { get; set; }

    public Union<int, Nest.RecoveryInitialShards> RecoveryInitialShards { get; set; }

    public Time RefreshInterval
    {
      get => this._refreshInterval;
      set
      {
        this.BackingDictionary["index.refresh_interval"] = (object) value;
        this._refreshInterval = value;
      }
    }

    public bool? RequestsCacheEnabled { get; set; }

    public int? RoutingAllocationTotalShardsPerNode { get; set; }

    public ISimilarities Similarity { get; set; }

    public ISlowLog SlowLog { get; set; }

    public ITranslogSettings Translog { get; set; }

    public Time UnassignedNodeLeftDelayedTimeout { get; set; }

    public string DefaultPipeline { get; set; }

    [Obsolete("Use FinalPipeline")]
    public string RequiredPipeline { get; set; }

    public string FinalPipeline { get; set; }

    public void Add(string setting, object value) => this.BackingDictionary[setting] = value;
  }
}

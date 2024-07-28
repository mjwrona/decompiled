// Decompiled with JetBrains decompiler
// Type: Nest.DynamicIndexSettingsDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class DynamicIndexSettingsDescriptorBase<TDescriptor, TIndexSettings> : 
    IsADictionaryDescriptorBase<TDescriptor, TIndexSettings, string, object>
    where TDescriptor : DynamicIndexSettingsDescriptorBase<TDescriptor, TIndexSettings>
    where TIndexSettings : class, IDynamicIndexSettings
  {
    protected DynamicIndexSettingsDescriptorBase(TIndexSettings instance)
      : base(instance)
    {
    }

    public TDescriptor Setting(string setting, object value)
    {
      this.PromisedValue[setting] = value;
      return (TDescriptor) this;
    }

    public TDescriptor NumberOfReplicas(int? numberOfReplicas) => this.Assign<int?>(numberOfReplicas, (Action<TIndexSettings, int?>) ((a, v) => a.NumberOfReplicas = v));

    public TDescriptor AutoExpandReplicas(Nest.AutoExpandReplicas autoExpandReplicas) => this.Assign<Nest.AutoExpandReplicas>(autoExpandReplicas, (Action<TIndexSettings, Nest.AutoExpandReplicas>) ((a, v) => a.AutoExpandReplicas = v));

    public TDescriptor DefaultPipeline(string defaultPipeline) => this.Assign<string>(defaultPipeline, (Action<TIndexSettings, string>) ((a, v) => a.DefaultPipeline = v));

    [Obsolete("Use FinalPipeline")]
    public TDescriptor RequiredPipeline(string requiredPipeline) => this.Assign<string>(requiredPipeline, (Action<TIndexSettings, string>) ((a, v) => a.RequiredPipeline = v));

    public TDescriptor FinalPipeline(string finalPipeline) => this.Assign<string>(finalPipeline, (Action<TIndexSettings, string>) ((a, v) => a.FinalPipeline = v));

    public TDescriptor BlocksMetadata(bool? blocksMetadata = true) => this.Assign<bool?>(blocksMetadata, (Action<TIndexSettings, bool?>) ((a, v) => a.BlocksMetadata = v));

    public TDescriptor BlocksRead(bool? blocksRead = true) => this.Assign<bool?>(blocksRead, (Action<TIndexSettings, bool?>) ((a, v) => a.BlocksRead = v));

    public TDescriptor BlocksReadOnly(bool? blocksReadOnly = true) => this.Assign<bool?>(blocksReadOnly, (Action<TIndexSettings, bool?>) ((a, v) => a.BlocksReadOnly = v));

    public TDescriptor BlocksWrite(bool? blocksWrite = true) => this.Assign<bool?>(blocksWrite, (Action<TIndexSettings, bool?>) ((a, v) => a.BlocksWrite = v));

    public TDescriptor BlocksReadOnlyAllowDelete(bool? blocksReadOnlyAllowDelete = true) => this.Assign<bool?>(blocksReadOnlyAllowDelete, (Action<TIndexSettings, bool?>) ((a, v) => a.BlocksReadOnlyAllowDelete = v));

    public TDescriptor Priority(int? priority) => this.Assign<int?>(priority, (Action<TIndexSettings, int?>) ((a, v) => a.Priority = v));

    public TDescriptor Merge(
      Func<MergeSettingsDescriptor, IMergeSettings> merge)
    {
      return this.Assign<Func<MergeSettingsDescriptor, IMergeSettings>>(merge, (Action<TIndexSettings, Func<MergeSettingsDescriptor, IMergeSettings>>) ((a, v) => a.Merge = v != null ? v(new MergeSettingsDescriptor()) : (IMergeSettings) null));
    }

    public TDescriptor RecoveryInitialShards(Union<int, Nest.RecoveryInitialShards> initialShards) => this.Assign<Union<int, Nest.RecoveryInitialShards>>(initialShards, (Action<TIndexSettings, Union<int, Nest.RecoveryInitialShards>>) ((a, v) => a.RecoveryInitialShards = v));

    public TDescriptor RequestsCacheEnabled(bool? enable = true) => this.Assign<bool?>(enable, (Action<TIndexSettings, bool?>) ((a, v) => a.RequestsCacheEnabled = v));

    public TDescriptor RefreshInterval(Time time) => this.Assign<Time>(time, (Action<TIndexSettings, Time>) ((a, v) => a.RefreshInterval = v));

    public TDescriptor RoutingAllocationTotalShardsPerNode(int? totalShardsPerNode) => this.Assign<int?>(totalShardsPerNode, (Action<TIndexSettings, int?>) ((a, v) => a.RoutingAllocationTotalShardsPerNode = v));

    public TDescriptor SlowLog(Func<SlowLogDescriptor, ISlowLog> slowLogSelector) => this.Assign<Func<SlowLogDescriptor, ISlowLog>>(slowLogSelector, (Action<TIndexSettings, Func<SlowLogDescriptor, ISlowLog>>) ((a, v) => a.SlowLog = v != null ? v(new SlowLogDescriptor()) : (ISlowLog) null));

    public TDescriptor Translog(
      Func<TranslogSettingsDescriptor, ITranslogSettings> translogSelector)
    {
      return this.Assign<Func<TranslogSettingsDescriptor, ITranslogSettings>>(translogSelector, (Action<TIndexSettings, Func<TranslogSettingsDescriptor, ITranslogSettings>>) ((a, v) => a.Translog = v != null ? v(new TranslogSettingsDescriptor()) : (ITranslogSettings) null));
    }

    public TDescriptor UnassignedNodeLeftDelayedTimeout(Time time) => this.Assign<Time>(time, (Action<TIndexSettings, Time>) ((a, v) => a.UnassignedNodeLeftDelayedTimeout = v));

    public TDescriptor Analysis(Func<AnalysisDescriptor, IAnalysis> selector) => this.Assign<Func<AnalysisDescriptor, IAnalysis>>(selector, (Action<TIndexSettings, Func<AnalysisDescriptor, IAnalysis>>) ((a, v) => a.Analysis = v != null ? v(new AnalysisDescriptor()) : (IAnalysis) null));

    public TDescriptor Similarity(
      Func<SimilaritiesDescriptor, IPromise<ISimilarities>> selector)
    {
      return this.Assign<Func<SimilaritiesDescriptor, IPromise<ISimilarities>>>(selector, (Action<TIndexSettings, Func<SimilaritiesDescriptor, IPromise<ISimilarities>>>) ((a, v) => a.Similarity = v != null ? v(new SimilaritiesDescriptor())?.Value : (ISimilarities) null));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.DynamicIndexSettingsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class DynamicIndexSettingsFormatter : 
    IJsonFormatter<IDynamicIndexSettings>,
    IJsonFormatter
  {
    private static readonly DynamicIndexSettingsFormatter.IndexSettingsDictionaryFormatter Formatter = new DynamicIndexSettingsFormatter.IndexSettingsDictionaryFormatter();

    public IDynamicIndexSettings Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      IndexSettings s = new IndexSettings();
      DynamicIndexSettingsFormatter.SetKnownIndexSettings(ref reader, formatterResolver, (IIndexSettings) s);
      return (IDynamicIndexSettings) s;
    }

    public void Serialize(
      ref JsonWriter writer,
      IDynamicIndexSettings value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        IDictionary<string, object> d = (IDictionary<string, object>) value;
        Set("index.number_of_replicas", (object) value.NumberOfReplicas);
        Set("index.refresh_interval", (object) value.RefreshInterval);
        Set("index.default_pipeline", (object) value.DefaultPipeline);
        Set("index.required_pipeline", (object) value.RequiredPipeline);
        Set("index.final_pipeline", (object) value.FinalPipeline);
        Set("index.blocks.read_only", (object) value.BlocksReadOnly);
        Set("index.blocks.read", (object) value.BlocksRead);
        Set("index.blocks.write", (object) value.BlocksWrite);
        Set("index.blocks.metadata", (object) value.BlocksMetadata);
        Set("index.blocks.read_only_allow_delete", (object) value.BlocksReadOnlyAllowDelete);
        Set("index.priority", (object) value.Priority);
        Set("index.auto_expand_replicas", (object) value.AutoExpandReplicas);
        Set("index.recovery.initial_shards", (object) value.RecoveryInitialShards);
        Set("index.requests.cache.enable", (object) value.RequestsCacheEnabled);
        Set("index.routing.allocation.total_shards_per_node", (object) value.RoutingAllocationTotalShardsPerNode);
        Set("index.unassigned.node_left.delayed_timeout", (object) value.UnassignedNodeLeftDelayedTimeout);
        ITranslogSettings translog = value.Translog;
        Set("index.translog.sync_interval", (object) translog?.SyncInterval);
        Set("index.translog.durability", (object) (TranslogDurability?) translog?.Durability);
        ITranslogFlushSettings flush = value.Translog?.Flush;
        Set("index.translog.flush_threshold_size", (object) flush?.ThresholdSize);
        Set("index.translog.flush_threshold_period", (object) flush?.ThresholdPeriod);
        Set("index.merge.policy.expunge_deletes_allowed", (object) (int?) value.Merge?.Policy?.ExpungeDeletesAllowed);
        Set("index.merge.policy.floor_segment", (object) value.Merge?.Policy?.FloorSegment);
        Set("index.merge.policy.max_merge_at_once", (object) (int?) value.Merge?.Policy?.MaxMergeAtOnce);
        Set("index.merge.policy.max_merge_at_once_explicit", (object) (int?) value.Merge?.Policy?.MaxMergeAtOnceExplicit);
        Set("index.merge.policy.max_merged_segment", (object) value.Merge?.Policy?.MaxMergedSegment);
        Set("index.merge.policy.segments_per_tier", (object) (int?) value.Merge?.Policy?.SegmentsPerTier);
        Set("index.merge.policy.reclaim_deletes_weight", (object) (double?) value.Merge?.Policy?.ReclaimDeletesWeight);
        Set("index.merge.scheduler.max_thread_count", (object) (int?) value.Merge?.Scheduler?.MaxThreadCount);
        Set("index.merge.scheduler.auto_throttle", (object) (bool?) value.Merge?.Scheduler?.AutoThrottle);
        ISlowLog slowLog = value.SlowLog;
        ISlowLogSearch search = slowLog?.Search;
        ISlowLogIndexing indexing = slowLog?.Indexing;
        Set("index.search.slowlog.threshold.query.warn", (object) search?.Query?.ThresholdWarn);
        Set("index.search.slowlog.threshold.query.info", (object) search?.Query?.ThresholdInfo);
        Set("index.search.slowlog.threshold.query.debug", (object) search?.Query?.ThresholdDebug);
        Set("index.search.slowlog.threshold.query.trace", (object) search?.Query?.ThresholdTrace);
        Set("index.search.slowlog.threshold.fetch.warn", (object) search?.Fetch?.ThresholdWarn);
        Set("index.search.slowlog.threshold.fetch.info", (object) search?.Fetch?.ThresholdInfo);
        Set("index.search.slowlog.threshold.fetch.debug", (object) search?.Fetch?.ThresholdDebug);
        Set("index.search.slowlog.threshold.fetch.trace", (object) search?.Fetch?.ThresholdTrace);
        Set("index.search.slowlog.level", (object) (LogLevel?) search?.LogLevel);
        Set("index.indexing.slowlog.threshold.index.warn", (object) indexing?.ThresholdWarn);
        Set("index.indexing.slowlog.threshold.index.info", (object) indexing?.ThresholdInfo);
        Set("index.indexing.slowlog.threshold.index.debug", (object) indexing?.ThresholdDebug);
        Set("index.indexing.slowlog.threshold.index.trace", (object) indexing?.ThresholdTrace);
        Set("index.indexing.slowlog.level", (object) (LogLevel?) indexing?.LogLevel);
        Set("index.indexing.slowlog.source", (object) (int?) indexing?.Source);
        Set("analysis", (object) value.Analysis);
        Set("similarity", (object) value.Similarity);
        if (value is IIndexSettings indexSettings)
        {
          Set("index.store.type", (object) indexSettings.FileSystemStorageImplementation);
          Set("index.queries.cache.enabled", (object) (bool?) indexSettings.Queries?.Cache?.Enabled);
          Set("index.number_of_shards", (object) indexSettings.NumberOfShards);
          Set("index.number_of_routing_shards", (object) indexSettings.NumberOfRoutingShards);
          Set("index.routing_partition_size", (object) indexSettings.RoutingPartitionSize);
          Set("index.store.type", (object) indexSettings.FileSystemStorageImplementation);
          Set("index.queries.cache.enabled", (object) (bool?) indexSettings.Queries?.Cache?.Enabled);
          Set("index.number_of_shards", (object) indexSettings.NumberOfShards);
          Set("index.number_of_routing_shards", (object) indexSettings.NumberOfRoutingShards);
          Set("index.routing_partition_size", (object) indexSettings.RoutingPartitionSize);
          Set("index.hidden", (object) indexSettings.Hidden);
          if (indexSettings.SoftDeletes != null)
          {
            Set("index.soft_deletes.enabled", (object) indexSettings.SoftDeletes.Enabled);
            Set("index.soft_deletes.retention.operations", (object) (long?) indexSettings.SoftDeletes.Retention?.Operations);
          }
          if (indexSettings?.Sorting != null)
          {
            Set("index.sort.field", DynamicIndexSettingsFormatter.AsArrayOrSingleItem<Field>((IEnumerable<Field>) indexSettings.Sorting.Fields));
            Set("index.sort.order", DynamicIndexSettingsFormatter.AsArrayOrSingleItem<IndexSortOrder>((IEnumerable<IndexSortOrder>) indexSettings.Sorting.Order));
            Set("index.sort.mode", DynamicIndexSettingsFormatter.AsArrayOrSingleItem<IndexSortMode>((IEnumerable<IndexSortMode>) indexSettings.Sorting.Mode));
            Set("index.sort.missing", DynamicIndexSettingsFormatter.AsArrayOrSingleItem<IndexSortMissing>((IEnumerable<IndexSortMissing>) indexSettings.Sorting.Missing));
          }
        }
        DynamicIndexSettingsFormatter.Formatter.Serialize(ref writer, d, formatterResolver);

        void Set(string knownKey, object newValue)
        {
          if (newValue == null)
            return;
          d[knownKey] = newValue;
        }
      }
    }

    private static object AsArrayOrSingleItem<T>(IEnumerable<T> items)
    {
      if (items == null || !items.Any<T>())
        return (object) null;
      return items.Count<T>() == 1 ? (object) items.First<T>() : (object) items;
    }

    private static Dictionary<string, object> Flatten(
      Dictionary<string, object> original,
      string prefix = "",
      Dictionary<string, object> current = null)
    {
      if (current == null)
        current = new Dictionary<string, object>();
      foreach (KeyValuePair<string, object> keyValuePair in original)
      {
        if (keyValuePair.Value is Dictionary<string, object> original1 && keyValuePair.Key != "analysis" && keyValuePair.Key != "similarity")
          DynamicIndexSettingsFormatter.Flatten(original1, prefix + keyValuePair.Key + ".", current);
        else
          current.Add(prefix + keyValuePair.Key, keyValuePair.Value);
      }
      return current;
    }

    private static void SetKnownIndexSettings(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IIndexSettings s)
    {
      Dictionary<string, object> settings = DynamicIndexSettingsFormatter.Flatten(formatterResolver.GetFormatter<Dictionary<string, object>>().Deserialize(ref reader, formatterResolver));
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.number_of_replicas", (Action<int?>) (v => s.NumberOfReplicas = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<AutoExpandReplicas>(s, (IDictionary<string, object>) settings, "index.auto_expand_replicas", (Action<AutoExpandReplicas>) (v => s.AutoExpandReplicas = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.refresh_interval", (Action<Time>) (v => s.RefreshInterval = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.blocks.read_only", (Action<bool?>) (v => s.BlocksReadOnly = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.blocks.read", (Action<bool?>) (v => s.BlocksRead = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.blocks.write", (Action<bool?>) (v => s.BlocksWrite = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.blocks.metadata", (Action<bool?>) (v => s.BlocksMetadata = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.blocks.read_only_allow_delete", (Action<bool?>) (v => s.BlocksReadOnlyAllowDelete = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.priority", (Action<int?>) (v => s.Priority = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<string>(s, (IDictionary<string, object>) settings, "index.default_pipeline", (Action<string>) (v => s.DefaultPipeline = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<string>(s, (IDictionary<string, object>) settings, "index.required_pipeline", (Action<string>) (v => s.RequiredPipeline = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<string>(s, (IDictionary<string, object>) settings, "index.final_pipeline", (Action<string>) (v => s.FinalPipeline = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Union<int, RecoveryInitialShards>>(s, (IDictionary<string, object>) settings, "index.recovery.initial_shards", (Action<Union<int, RecoveryInitialShards>>) (v => s.RecoveryInitialShards = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.requests.cache.enable", (Action<bool?>) (v => s.RequestsCacheEnabled = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.routing.allocation.total_shards_per_node", (Action<int?>) (v => s.RoutingAllocationTotalShardsPerNode = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.unassigned.node_left.delayed_timeout", (Action<Time>) (v => s.UnassignedNodeLeftDelayedTimeout = v), formatterResolver);
      ITranslogSettings t = s.Translog = (ITranslogSettings) new TranslogSettings();
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.translog.sync_interval", (Action<Time>) (v => t.SyncInterval = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<TranslogDurability?>(s, (IDictionary<string, object>) settings, "index.translog.durability", (Action<TranslogDurability?>) (v => t.Durability = v), formatterResolver);
      ITranslogFlushSettings tf = s.Translog.Flush = (ITranslogFlushSettings) new TranslogFlushSettings();
      DynamicIndexSettingsFormatter.Set<string>(s, (IDictionary<string, object>) settings, "index.translog.flush_threshold_size", (Action<string>) (v => tf.ThresholdSize = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.translog.flush_threshold_period", (Action<Time>) (v => tf.ThresholdPeriod = v), formatterResolver);
      s.Merge = (IMergeSettings) new MergeSettings();
      IMergePolicySettings p = s.Merge.Policy = (IMergePolicySettings) new MergePolicySettings();
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.merge.policy.expunge_deletes_allowed", (Action<int?>) (v => p.ExpungeDeletesAllowed = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<string>(s, (IDictionary<string, object>) settings, "index.merge.policy.floor_segment", (Action<string>) (v => p.FloorSegment = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.merge.policy.max_merge_at_once", (Action<int?>) (v => p.MaxMergeAtOnce = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.merge.policy.max_merge_at_once_explicit", (Action<int?>) (v => p.MaxMergeAtOnceExplicit = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<string>(s, (IDictionary<string, object>) settings, "index.merge.policy.max_merged_segment", (Action<string>) (v => p.MaxMergedSegment = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.merge.policy.segments_per_tier", (Action<int?>) (v => p.SegmentsPerTier = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<double?>(s, (IDictionary<string, object>) settings, "index.merge.policy.reclaim_deletes_weight", (Action<double?>) (v => p.ReclaimDeletesWeight = v), formatterResolver);
      IMergeSchedulerSettings ms = s.Merge.Scheduler = (IMergeSchedulerSettings) new MergeSchedulerSettings();
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.merge.scheduler.max_thread_count", (Action<int?>) (v => ms.MaxThreadCount = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.merge.scheduler.auto_throttle", (Action<bool?>) (v => ms.AutoThrottle = v), formatterResolver);
      s.SlowLog = (ISlowLog) new SlowLog();
      ISlowLogSearch search = s.SlowLog.Search = (ISlowLogSearch) new SlowLogSearch();
      DynamicIndexSettingsFormatter.Set<LogLevel?>(s, (IDictionary<string, object>) settings, "index.search.slowlog.level", (Action<LogLevel?>) (v => search.LogLevel = v), formatterResolver);
      ISlowLogSearchQuery query = s.SlowLog.Search.Query = (ISlowLogSearchQuery) new SlowLogSearchQuery();
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.search.slowlog.threshold.query.warn", (Action<Time>) (v => query.ThresholdWarn = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.search.slowlog.threshold.query.info", (Action<Time>) (v => query.ThresholdInfo = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.search.slowlog.threshold.query.debug", (Action<Time>) (v => query.ThresholdDebug = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.search.slowlog.threshold.query.trace", (Action<Time>) (v => query.ThresholdTrace = v), formatterResolver);
      ISlowLogSearchFetch fetch = s.SlowLog.Search.Fetch = (ISlowLogSearchFetch) new SlowLogSearchFetch();
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.search.slowlog.threshold.fetch.warn", (Action<Time>) (v => fetch.ThresholdWarn = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.search.slowlog.threshold.fetch.info", (Action<Time>) (v => fetch.ThresholdInfo = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.search.slowlog.threshold.fetch.debug", (Action<Time>) (v => fetch.ThresholdDebug = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.search.slowlog.threshold.fetch.trace", (Action<Time>) (v => fetch.ThresholdTrace = v), formatterResolver);
      ISlowLogIndexing indexing = s.SlowLog.Indexing = (ISlowLogIndexing) new SlowLogIndexing();
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.indexing.slowlog.threshold.index.warn", (Action<Time>) (v => indexing.ThresholdWarn = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.indexing.slowlog.threshold.index.info", (Action<Time>) (v => indexing.ThresholdInfo = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.indexing.slowlog.threshold.index.debug", (Action<Time>) (v => indexing.ThresholdDebug = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<Time>(s, (IDictionary<string, object>) settings, "index.indexing.slowlog.threshold.index.trace", (Action<Time>) (v => indexing.ThresholdTrace = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<LogLevel?>(s, (IDictionary<string, object>) settings, "index.indexing.slowlog.level", (Action<LogLevel?>) (v => indexing.LogLevel = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.indexing.slowlog.source", (Action<int?>) (v => indexing.Source = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.number_of_shards", (Action<int?>) (v => s.NumberOfShards = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.number_of_routing_shards", (Action<int?>) (v => s.NumberOfRoutingShards = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<int?>(s, (IDictionary<string, object>) settings, "index.routing_partition_size", (Action<int?>) (v => s.RoutingPartitionSize = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.hidden", (Action<bool?>) (v => s.Hidden = v), formatterResolver);
      DynamicIndexSettingsFormatter.Set<FileSystemStorageImplementation?>(s, (IDictionary<string, object>) settings, "index.store.type", (Action<FileSystemStorageImplementation?>) (v => s.FileSystemStorageImplementation = v), formatterResolver);
      ISortingSettings sorting = s.Sorting = (ISortingSettings) new SortingSettings();
      DynamicIndexSettingsFormatter.SetArray<string[], string>(s, (IDictionary<string, object>) settings, "index.sort.field", (Action<string[]>) (v => sorting.Fields = (Fields) v), (Action<string>) (v => sorting.Fields = (Fields) new string[1]
      {
        v
      }), formatterResolver);
      DynamicIndexSettingsFormatter.SetArray<IndexSortOrder[], IndexSortOrder>(s, (IDictionary<string, object>) settings, "index.sort.order", (Action<IndexSortOrder[]>) (v => sorting.Order = v), (Action<IndexSortOrder>) (v => sorting.Order = new IndexSortOrder[1]
      {
        v
      }), formatterResolver);
      DynamicIndexSettingsFormatter.SetArray<IndexSortMode[], IndexSortMode>(s, (IDictionary<string, object>) settings, "index.sort.mode", (Action<IndexSortMode[]>) (v => sorting.Mode = v), (Action<IndexSortMode>) (v => sorting.Mode = new IndexSortMode[1]
      {
        v
      }), formatterResolver);
      DynamicIndexSettingsFormatter.SetArray<IndexSortMissing[], IndexSortMissing>(s, (IDictionary<string, object>) settings, "index.sort.missing", (Action<IndexSortMissing[]>) (v => sorting.Missing = v), (Action<IndexSortMissing>) (v => sorting.Missing = new IndexSortMissing[1]
      {
        v
      }), formatterResolver);
      s.Queries = (IQueriesSettings) new QueriesSettings();
      IQueriesCacheSettings queriesCache = s.Queries.Cache = (IQueriesCacheSettings) new QueriesCacheSettings();
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.queries.cache.enabled", (Action<bool?>) (v => queriesCache.Enabled = v), formatterResolver);
      ISoftDeleteSettings softDeletes = s.SoftDeletes = (ISoftDeleteSettings) new SoftDeleteSettings();
      DynamicIndexSettingsFormatter.Set<bool?>(s, (IDictionary<string, object>) settings, "index.soft_deletes.enabled", (Action<bool?>) (v => softDeletes.Enabled = v), formatterResolver);
      ISoftDeleteRetentionSettings softDeletesRetention = s.SoftDeletes.Retention = (ISoftDeleteRetentionSettings) new SoftDeleteRetentionSettings();
      DynamicIndexSettingsFormatter.Set<long?>(s, (IDictionary<string, object>) settings, "index.soft_deletes.enabled", (Action<long?>) (v => softDeletesRetention.Operations = v), formatterResolver);
      IDictionary<string, object> dictionary = (IDictionary<string, object>) s;
      foreach (KeyValuePair<string, object> keyValuePair in settings)
      {
        object setting = keyValuePair.Value;
        if (keyValuePair.Key == "analysis" || keyValuePair.Key == "index.analysis")
          s.Analysis = (IAnalysis) DynamicIndexSettingsFormatter.ReserializeAndDeserialize<Analysis>(setting, formatterResolver);
        if (keyValuePair.Key == "similarity" || keyValuePair.Key == "index.similarity")
          s.Similarity = (ISimilarities) DynamicIndexSettingsFormatter.ReserializeAndDeserialize<Similarities>(setting, formatterResolver);
        else
          dictionary.Add(keyValuePair.Key, setting);
      }
    }

    private static T ReserializeAndDeserialize<T>(
      object setting,
      IJsonFormatterResolver formatterResolver)
    {
      byte[] bytes = JsonSerializer.Serialize<object>(setting);
      IJsonFormatter<T> formatter = formatterResolver.GetFormatter<T>();
      JsonReader reader = new JsonReader(bytes);
      return formatter.Deserialize(ref reader, formatterResolver);
    }

    private static void Set<T>(
      IIndexSettings s,
      IDictionary<string, object> settings,
      string key,
      Action<T> assign,
      IJsonFormatterResolver formatterResolver)
    {
      object setting;
      if (!settings.TryGetValue(key, out setting))
        return;
      T obj = DynamicIndexSettingsFormatter.ConvertToValue<T>(setting, formatterResolver);
      assign(obj);
      s.Add(key, (object) obj);
      settings.Remove(key);
    }

    private static T ConvertToValue<T>(object setting, IJsonFormatterResolver formatterResolver)
    {
      switch (setting)
      {
        case T obj:
          return obj;
        case null:
          return default (T);
        case IConvertible _:
          Type conversionType = typeof (T).IsNullable() ? Nullable.GetUnderlyingType(typeof (T)) : typeof (T);
          try
          {
            return (T) Convert.ChangeType(setting, conversionType);
          }
          catch
          {
            break;
          }
      }
      JsonWriter writer = new JsonWriter();
      formatterResolver.GetFormatter<object>().Serialize(ref writer, setting, formatterResolver);
      JsonReader reader = new JsonReader(writer.GetBuffer().Array, 0);
      return formatterResolver.GetFormatter<T>().Deserialize(ref reader, formatterResolver);
    }

    private static void SetArray<TArray, TItem>(
      IIndexSettings s,
      IDictionary<string, object> settings,
      string key,
      Action<TArray> assign,
      Action<TItem> assign2,
      IJsonFormatterResolver formatterResolver)
      where TArray : IEnumerable<TItem>
    {
      object setting;
      if (!settings.TryGetValue(key, out setting))
        return;
      switch (setting)
      {
        case IEnumerable _:
          TArray array = DynamicIndexSettingsFormatter.ConvertToValue<TArray>(setting, formatterResolver);
          assign(array);
          s.Add(key, (object) array);
          break;
        default:
          TItem obj = DynamicIndexSettingsFormatter.ConvertToValue<TItem>(setting, formatterResolver);
          assign2(obj);
          s.Add(key, (object) obj);
          break;
      }
      settings.Remove(key);
    }

    private class IndexSettingsDictionaryFormatter : 
      VerbatimDictionaryInterfaceKeysFormatter<string, object>
    {
      protected override bool SkipValue(KeyValuePair<string, object> entry) => entry.Key != "index.refresh_interval" && base.SkipValue(entry);
    }
  }
}

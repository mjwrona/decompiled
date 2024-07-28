// Decompiled with JetBrains decompiler
// Type: Nest.NodeJvmStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NodeJvmStats
  {
    [DataMember(Name = "buffer_pools")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, NodeJvmStats.NodeBufferPool>))]
    public IReadOnlyDictionary<string, NodeJvmStats.NodeBufferPool> BufferPools { get; internal set; }

    [DataMember(Name = "classes")]
    public NodeJvmStats.JvmClassesStats Classes { get; internal set; }

    [DataMember(Name = "gc")]
    public NodeJvmStats.GarbageCollectionStats GarbageCollection { get; internal set; }

    [DataMember(Name = "mem")]
    public NodeJvmStats.MemoryStats Memory { get; internal set; }

    [DataMember(Name = "threads")]
    public NodeJvmStats.ThreadStats Threads { get; internal set; }

    [DataMember(Name = "timestamp")]
    public long Timestamp { get; internal set; }

    [DataMember(Name = "uptime")]
    public string Uptime { get; internal set; }

    [DataMember(Name = "uptime_in_millis")]
    public long UptimeInMilliseconds { get; internal set; }

    [DataContract]
    public class JvmClassesStats
    {
      [DataMember(Name = "current_loaded_count")]
      public long CurrentLoadedCount { get; internal set; }

      [DataMember(Name = "total_loaded_count")]
      public long TotalLoadedCount { get; internal set; }

      [DataMember(Name = "total_unloaded_count")]
      public long TotalUnloadedCount { get; internal set; }
    }

    [DataContract]
    public class MemoryStats
    {
      [DataMember(Name = "heap_committed")]
      public string HeapCommitted { get; internal set; }

      [DataMember(Name = "heap_committed_in_bytes")]
      public long HeapCommittedInBytes { get; internal set; }

      [DataMember(Name = "heap_max")]
      public string HeapMax { get; internal set; }

      [DataMember(Name = "heap_max_in_bytes")]
      public long HeapMaxInBytes { get; internal set; }

      [DataMember(Name = "heap_used")]
      public string HeapUsed { get; internal set; }

      [DataMember(Name = "heap_used_in_bytes")]
      public long HeapUsedInBytes { get; internal set; }

      [DataMember(Name = "heap_used_percent")]
      public long HeapUsedPercent { get; internal set; }

      [DataMember(Name = "non_heap_committed")]
      public string NonHeapCommitted { get; internal set; }

      [DataMember(Name = "non_heap_committed_in_bytes")]
      public long NonHeapCommittedInBytes { get; internal set; }

      [DataMember(Name = "non_heap_used")]
      public string NonHeapUsed { get; internal set; }

      [DataMember(Name = "non_heap_used_in_bytes")]
      public long NonHeapUsedInBytes { get; internal set; }

      [DataMember(Name = "pools")]
      [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, NodeJvmStats.MemoryStats.JvmPool>))]
      public IReadOnlyDictionary<string, NodeJvmStats.MemoryStats.JvmPool> Pools { get; internal set; }

      [DataContract]
      public class JvmPool
      {
        [DataMember(Name = "max")]
        public string Max { get; internal set; }

        [DataMember(Name = "max_in_bytes")]
        public long MaxInBytes { get; internal set; }

        [DataMember(Name = "peak_max")]
        public string PeakMax { get; internal set; }

        [DataMember(Name = "peak_max_in_bytes")]
        public long PeakMaxInBytes { get; internal set; }

        [DataMember(Name = "peak_used")]
        public string PeakUsed { get; internal set; }

        [DataMember(Name = "peak_used_in_bytes")]
        public long PeakUsedInBytes { get; internal set; }

        [DataMember(Name = "used")]
        public string Used { get; internal set; }

        [DataMember(Name = "used_in_bytes")]
        public long UsedInBytes { get; internal set; }
      }
    }

    [DataContract]
    public class ThreadStats
    {
      [DataMember(Name = "count")]
      public long Count { get; internal set; }

      [DataMember(Name = "peak_count")]
      public long PeakCount { get; internal set; }
    }

    [DataContract]
    public class GarbageCollectionStats
    {
      [DataMember(Name = "collectors")]
      [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, NodeJvmStats.GarbageCollectionGenerationStats>))]
      public IReadOnlyDictionary<string, NodeJvmStats.GarbageCollectionGenerationStats> Collectors { get; internal set; }
    }

    [DataContract]
    public class GarbageCollectionGenerationStats
    {
      [DataMember(Name = "collection_count")]
      public long CollectionCount { get; internal set; }

      [DataMember(Name = "collection_time")]
      public string CollectionTime { get; internal set; }

      [DataMember(Name = "collection_time_in_millis")]
      public long CollectionTimeInMilliseconds { get; internal set; }
    }

    [DataContract]
    public class NodeBufferPool
    {
      [DataMember(Name = "count")]
      public long Count { get; internal set; }

      [DataMember(Name = "total_capacity")]
      public string TotalCapacity { get; internal set; }

      [DataMember(Name = "total_capacity_in_bytes")]
      public long TotalCapacityInBytes { get; internal set; }

      [DataMember(Name = "used")]
      public string Used { get; internal set; }

      [DataMember(Name = "used_in_bytes")]
      public long UsedInBytes { get; internal set; }
    }
  }
}

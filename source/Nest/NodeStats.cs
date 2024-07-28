// Decompiled with JetBrains decompiler
// Type: Nest.NodeStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NodeStats
  {
    [DataMember(Name = "adaptive_selection")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, AdaptiveSelectionStats>))]
    public IReadOnlyDictionary<string, AdaptiveSelectionStats> AdaptiveSelection { get; internal set; } = EmptyReadOnly<string, AdaptiveSelectionStats>.Dictionary;

    [DataMember(Name = "breakers")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, BreakerStats>))]
    public IReadOnlyDictionary<string, BreakerStats> Breakers { get; internal set; }

    [DataMember(Name = "fs")]
    public FileSystemStats FileSystem { get; internal set; }

    [DataMember(Name = "host")]
    public string Host { get; internal set; }

    [DataMember(Name = "http")]
    public HttpStats Http { get; internal set; }

    [DataMember(Name = "indices")]
    public IndexStats Indices { get; internal set; }

    [DataMember(Name = "ingest")]
    public NodeIngestStats Ingest { get; internal set; }

    [DataMember(Name = "ip")]
    [JsonFormatter(typeof (SingleOrEnumerableFormatter<string>))]
    public IEnumerable<string> Ip { get; internal set; }

    [DataMember(Name = "jvm")]
    public NodeJvmStats Jvm { get; internal set; }

    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "os")]
    public OperatingSystemStats OperatingSystem { get; internal set; }

    [DataMember(Name = "process")]
    public ProcessStats Process { get; internal set; }

    [DataMember(Name = "roles")]
    public IEnumerable<NodeRole> Roles { get; internal set; }

    [DataMember(Name = "script")]
    public ScriptStats Script { get; internal set; }

    [DataMember(Name = "script_cache")]
    public ScriptCacheStats ScriptCache { get; internal set; }

    [DataMember(Name = "thread_pool")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, ThreadCountStats>))]
    public IReadOnlyDictionary<string, ThreadCountStats> ThreadPool { get; internal set; }

    [DataMember(Name = "timestamp")]
    public long Timestamp { get; internal set; }

    [DataMember(Name = "transport")]
    public TransportStats Transport { get; internal set; }

    [DataMember(Name = "transport_address")]
    public string TransportAddress { get; internal set; }

    [DataMember(Name = "indexing_pressure")]
    public IndexingPressureStats IndexingPressure { get; internal set; }
  }
}

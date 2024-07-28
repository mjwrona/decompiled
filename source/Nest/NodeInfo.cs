// Decompiled with JetBrains decompiler
// Type: Nest.NodeInfo
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
  public class NodeInfo
  {
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "transport_address")]
    public string TransportAddress { get; internal set; }

    [DataMember(Name = "host")]
    public string Host { get; internal set; }

    [DataMember(Name = "ip")]
    public string Ip { get; internal set; }

    [DataMember(Name = "version")]
    public string Version { get; internal set; }

    [DataMember(Name = "build_flavor")]
    public string BuildFlavor { get; internal set; }

    [DataMember(Name = "build_type")]
    public string BuildType { get; internal set; }

    [DataMember(Name = "build_hash")]
    public string BuildHash { get; internal set; }

    [DataMember(Name = "total_indexing_buffer")]
    public long? TotalIndexingBuffer { get; internal set; }

    [DataMember(Name = "roles")]
    public List<NodeRole> Roles { get; internal set; }

    [DataMember(Name = "attributes")]
    public IReadOnlyDictionary<string, string> Attributes { get; internal set; } = EmptyReadOnly<string, string>.Dictionary;

    [DataMember(Name = "settings")]
    public DynamicDictionary Settings { get; internal set; }

    [DataMember(Name = "os")]
    public NodeOperatingSystemInfo OperatingSystem { get; internal set; }

    [DataMember(Name = "process")]
    public NodeProcessInfo Process { get; internal set; }

    [DataMember(Name = "jvm")]
    public NodeJvmInfo Jvm { get; internal set; }

    [DataMember(Name = "http")]
    public NodeInfoHttp Http { get; internal set; }

    [DataMember(Name = "network")]
    public NodeInfoNetwork Network { get; internal set; }

    [DataMember(Name = "plugins")]
    public List<PluginStats> Plugins { get; internal set; }

    [DataMember(Name = "thread_pool")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, NodeThreadPoolInfo>))]
    public IReadOnlyDictionary<string, NodeThreadPoolInfo> ThreadPool { get; internal set; }

    [DataMember(Name = "transport")]
    public NodeInfoTransport Transport { get; internal set; }
  }
}

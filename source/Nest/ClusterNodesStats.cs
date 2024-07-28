// Decompiled with JetBrains decompiler
// Type: Nest.ClusterNodesStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterNodesStats
  {
    [DataMember(Name = "count")]
    public ClusterNodeCount Count { get; internal set; }

    [DataMember(Name = "discovery_types")]
    public IReadOnlyDictionary<string, int> DiscoveryTypes { get; internal set; }

    [DataMember(Name = "fs")]
    public ClusterFileSystem FileSystem { get; internal set; }

    [DataMember(Name = "jvm")]
    public ClusterJvm Jvm { get; internal set; }

    [DataMember(Name = "network_types")]
    public ClusterNetworkTypes NetworkTypes { get; internal set; }

    [DataMember(Name = "os")]
    public ClusterOperatingSystemStats OperatingSystem { get; internal set; }

    [DataMember(Name = "packaging_types")]
    public IReadOnlyCollection<NodePackagingType> PackagingTypes { get; internal set; }

    [DataMember(Name = "plugins")]
    public IReadOnlyCollection<PluginStats> Plugins { get; internal set; }

    [DataMember(Name = "process")]
    public ClusterProcess Process { get; internal set; }

    [DataMember(Name = "versions")]
    public IReadOnlyCollection<string> Versions { get; internal set; }

    [DataMember(Name = "ingest")]
    public ClusterIngestStats Ingest { get; internal set; }
  }
}

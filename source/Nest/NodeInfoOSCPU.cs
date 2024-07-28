// Decompiled with JetBrains decompiler
// Type: Nest.NodeInfoOSCPU
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NodeInfoOSCPU
  {
    [DataMember(Name = "cache_size")]
    public string CacheSize { get; internal set; }

    [DataMember(Name = "cache_size_in_bytes")]
    public int CacheSizeInBytes { get; internal set; }

    [DataMember(Name = "cores_per_socket")]
    public int CoresPerSocket { get; internal set; }

    [DataMember(Name = "mhz")]
    public int Mhz { get; internal set; }

    [DataMember(Name = "model")]
    public string Model { get; internal set; }

    [DataMember(Name = "total_cores")]
    public int TotalCores { get; internal set; }

    [DataMember(Name = "total_sockets")]
    public int TotalSockets { get; internal set; }

    [DataMember(Name = "vendor")]
    public string Vendor { get; internal set; }
  }
}

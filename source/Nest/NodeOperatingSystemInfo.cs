// Decompiled with JetBrains decompiler
// Type: Nest.NodeOperatingSystemInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NodeOperatingSystemInfo
  {
    [DataMember(Name = "arch")]
    public string Architecture { get; internal set; }

    [DataMember(Name = "available_processors")]
    public int AvailableProcessors { get; internal set; }

    [DataMember(Name = "cpu")]
    public NodeInfoOSCPU Cpu { get; internal set; }

    [DataMember(Name = "mem")]
    public NodeInfoMemory Mem { get; internal set; }

    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "pretty_name")]
    public string PrettyName { get; internal set; }

    [DataMember(Name = "refresh_interval_in_millis")]
    public int RefreshInterval { get; internal set; }

    [DataMember(Name = "swap")]
    public NodeInfoMemory Swap { get; internal set; }

    [DataMember(Name = "version")]
    public string Version { get; internal set; }
  }
}

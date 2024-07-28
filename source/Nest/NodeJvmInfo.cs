// Decompiled with JetBrains decompiler
// Type: Nest.NodeJvmInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NodeJvmInfo
  {
    [DataMember(Name = "gc_collectors")]
    public IEnumerable<string> GcCollectors { get; internal set; }

    [DataMember(Name = "mem")]
    public NodeInfoJvmMemory Memory { get; internal set; }

    [DataMember(Name = "memory_pools")]
    public IEnumerable<string> MemoryPools { get; internal set; }

    [DataMember(Name = "pid")]
    public int Pid { get; internal set; }

    [DataMember(Name = "start_time_in_millis")]
    public long StartTime { get; internal set; }

    [DataMember(Name = "version")]
    public string Version { get; internal set; }

    [DataMember(Name = "vm_name")]
    public string VMName { get; internal set; }

    [DataMember(Name = "vm_vendor")]
    public string VMVendor { get; internal set; }

    [DataMember(Name = "vm_version")]
    public string VMVersion { get; internal set; }
  }
}

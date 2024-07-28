// Decompiled with JetBrains decompiler
// Type: Nest.NodeInfoJvmMemory
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NodeInfoJvmMemory
  {
    [DataMember(Name = "direct_max")]
    public string DirectMax { get; internal set; }

    [DataMember(Name = "direct_max_in_bytes")]
    public long DirectMaxInBytes { get; internal set; }

    [DataMember(Name = "heap_init")]
    public string HeapInit { get; internal set; }

    [DataMember(Name = "heap_init_in_bytes")]
    public long HeapInitInBytes { get; internal set; }

    [DataMember(Name = "heap_max")]
    public string HeapMax { get; internal set; }

    [DataMember(Name = "heap_max_in_bytes")]
    public long HeapMaxInBytes { get; internal set; }

    [DataMember(Name = "non_heap_init")]
    public string NonHeapInit { get; internal set; }

    [DataMember(Name = "non_heap_init_in_bytes")]
    public long NonHeapInitInBytes { get; internal set; }

    [DataMember(Name = "non_heap_max")]
    public string NonHeapMax { get; internal set; }

    [DataMember(Name = "non_heap_max_in_bytes")]
    public long NonHeapMaxInBytes { get; internal set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.NodeThreadPoolInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NodeThreadPoolInfo
  {
    [DataMember(Name = "keep_alive")]
    public string KeepAlive { get; internal set; }

    [DataMember(Name = "max")]
    public int? Max { get; internal set; }

    [DataMember(Name = "core")]
    public int? Core { get; internal set; }

    [DataMember(Name = "size")]
    public int? Size { get; internal set; }

    [DataMember(Name = "queue_size")]
    public int? QueueSize { get; internal set; }

    [DataMember(Name = "type")]
    public string Type { get; internal set; }
  }
}

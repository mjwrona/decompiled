// Decompiled with JetBrains decompiler
// Type: Nest.IMoveClusterRerouteCommand
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public interface IMoveClusterRerouteCommand : IClusterRerouteCommand
  {
    [DataMember(Name = "from_node")]
    string FromNode { get; set; }

    [DataMember(Name = "index")]
    IndexName Index { get; set; }

    [DataMember(Name = "shard")]
    int? Shard { get; set; }

    [DataMember(Name = "to_node")]
    string ToNode { get; set; }
  }
}

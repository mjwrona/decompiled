// Decompiled with JetBrains decompiler
// Type: Nest.ClusterRerouteParameters
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterRerouteParameters
  {
    [DataMember(Name = "allow_primary")]
    public bool? AllowPrimary { get; set; }

    [DataMember(Name = "from_node")]
    public string FromNode { get; set; }

    [DataMember(Name = "index")]
    public string Index { get; set; }

    [DataMember(Name = "node")]
    public string Node { get; set; }

    [DataMember(Name = "shard")]
    public int Shard { get; set; }

    [DataMember(Name = "to_node")]
    public string ToNode { get; set; }
  }
}

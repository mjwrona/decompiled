// Decompiled with JetBrains decompiler
// Type: Nest.CatHealthRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatHealthRecord : ICatRecord
  {
    [DataMember(Name = "cluster")]
    public string Cluster { get; set; }

    [DataMember(Name = "epoch")]
    public string Epoch { get; set; }

    [DataMember(Name = "init")]
    public string Initializing { get; set; }

    [DataMember(Name = "node.data")]
    public string NodeData { get; set; }

    [DataMember(Name = "node.total")]
    public string NodeTotal { get; set; }

    [DataMember(Name = "pending_tasks")]
    public string PendingTasks { get; set; }

    [DataMember(Name = "pri")]
    public string Primary { get; set; }

    [DataMember(Name = "relo")]
    public string Relocating { get; set; }

    [DataMember(Name = "shards")]
    public string Shards { get; set; }

    [DataMember(Name = "status")]
    public string Status { get; set; }

    [DataMember(Name = "timestamp")]
    public string Timestamp { get; set; }

    [DataMember(Name = "unassign")]
    public string Unassigned { get; set; }
  }
}

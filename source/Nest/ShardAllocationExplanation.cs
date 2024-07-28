// Decompiled with JetBrains decompiler
// Type: Nest.ShardAllocationExplanation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class ShardAllocationExplanation
  {
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "index")]
    public IndexName Index { get; set; }

    [DataMember(Name = "index_uuid")]
    public string IndexUniqueId { get; set; }

    [DataMember(Name = "primary")]
    public bool Primary { get; set; }
  }
}

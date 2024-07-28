// Decompiled with JetBrains decompiler
// Type: Nest.AllocationStore
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class AllocationStore
  {
    [DataMember(Name = "allocation_id")]
    public string AllocationId { get; set; }

    [DataMember(Name = "found")]
    public bool? Found { get; set; }

    [DataMember(Name = "in_sync")]
    public bool? InSync { get; set; }

    [DataMember(Name = "matching_size_in_bytes")]
    public long? MatchingSizeInBytes { get; set; }

    [DataMember(Name = "matching_sync_id")]
    public bool? MatchingSyncId { get; set; }

    [DataMember(Name = "store_exception")]
    public string StoreException { get; set; }
  }
}

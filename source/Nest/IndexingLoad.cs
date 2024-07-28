// Decompiled with JetBrains decompiler
// Type: Nest.IndexingLoad
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IndexingLoad
  {
    [DataMember(Name = "combined_coordinating_and_primary_in_bytes")]
    public long CombinedCoordinatingAndPrimaryInBytes { get; internal set; }

    [DataMember(Name = "combined_coordinating_and_primary")]
    public string CombinedCoordinatingAndPrimary { get; internal set; }

    [DataMember(Name = "coordinating_in_bytes")]
    public long CoordinatingInBytes { get; internal set; }

    [DataMember(Name = "coordinating")]
    public string Coordinating { get; internal set; }

    [DataMember(Name = "primary_in_bytes")]
    public long PrimaryInBytes { get; internal set; }

    [DataMember(Name = "primary")]
    public string Primary { get; internal set; }

    [DataMember(Name = "replica_in_bytes")]
    public long ReplicaInBytes { get; internal set; }

    [DataMember(Name = "replica")]
    public string Replica { get; internal set; }

    [DataMember(Name = "all_in_bytes")]
    public long AllInBytes { get; internal set; }

    [DataMember(Name = "all")]
    public string All { get; internal set; }
  }
}

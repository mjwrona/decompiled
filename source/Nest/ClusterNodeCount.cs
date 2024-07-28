// Decompiled with JetBrains decompiler
// Type: Nest.ClusterNodeCount
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterNodeCount
  {
    [DataMember(Name = "coordinating_only")]
    public int CoordinatingOnly { get; internal set; }

    [DataMember(Name = "data")]
    public int Data { get; internal set; }

    [DataMember(Name = "ingest")]
    public int Ingest { get; internal set; }

    [DataMember(Name = "master")]
    public int Master { get; internal set; }

    [DataMember(Name = "total")]
    public int Total { get; internal set; }

    [DataMember(Name = "voting_only")]
    public int VotingOnly { get; internal set; }
  }
}

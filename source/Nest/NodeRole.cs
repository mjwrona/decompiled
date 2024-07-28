// Decompiled with JetBrains decompiler
// Type: Nest.NodeRole
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum NodeRole
  {
    [EnumMember(Value = "master")] Master,
    [EnumMember(Value = "data")] Data,
    [EnumMember(Value = "data_cold")] DataCold,
    [EnumMember(Value = "data_frozen")] DataFrozen,
    [EnumMember(Value = "data_content")] DataContent,
    [EnumMember(Value = "data_hot")] DataHot,
    [EnumMember(Value = "data_warm")] DataWarm,
    [EnumMember(Value = "client")] Client,
    [EnumMember(Value = "ingest")] Ingest,
    [EnumMember(Value = "ml")] MachineLearning,
    [EnumMember(Value = "voting_only")] VotingOnly,
    [EnumMember(Value = "transform")] Transform,
    [EnumMember(Value = "remote_cluster_client")] RemoteClusterClient,
    [EnumMember(Value = "coordinating_only")] CoordinatingOnly,
  }
}

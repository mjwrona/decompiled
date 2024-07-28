// Decompiled with JetBrains decompiler
// Type: Nest.FollowerInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class FollowerInfo
  {
    [DataMember(Name = "follower_index")]
    public string FollowerIndex { get; internal set; }

    [DataMember(Name = "remote_cluster")]
    public string RemoteCluster { get; internal set; }

    [DataMember(Name = "leader_index")]
    public string LeaderIndex { get; internal set; }

    [DataMember(Name = "status")]
    public FollowerIndexStatus Status { get; internal set; }

    [DataMember(Name = "parameters")]
    public FollowConfig Parameters { get; internal set; }
  }
}

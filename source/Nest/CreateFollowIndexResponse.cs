// Decompiled with JetBrains decompiler
// Type: Nest.CreateFollowIndexResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class CreateFollowIndexResponse : ResponseBase
  {
    [DataMember(Name = "follow_index_created")]
    public bool FollowIndexCreated { get; internal set; }

    [DataMember(Name = "follow_index_shards_acked")]
    public bool FollowIndexShardsAcked { get; internal set; }

    [DataMember(Name = "index_following_started")]
    public bool IndexFollowingStarted { get; internal set; }
  }
}

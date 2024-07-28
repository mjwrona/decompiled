// Decompiled with JetBrains decompiler
// Type: Nest.CcrStatsResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class CcrStatsResponse : ResponseBase
  {
    [DataMember(Name = "auto_follow_stats")]
    public CcrAutoFollowStats AutoFollowStats { get; internal set; }

    [DataMember(Name = "follow_stats")]
    public CcrFollowStats FollowStats { get; internal set; }
  }
}

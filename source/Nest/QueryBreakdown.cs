// Decompiled with JetBrains decompiler
// Type: Nest.QueryBreakdown
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class QueryBreakdown
  {
    [DataMember(Name = "advance")]
    public long Advance { get; internal set; }

    [DataMember(Name = "build_scorer")]
    public long BuildScorer { get; internal set; }

    [DataMember(Name = "create_weight")]
    public long CreateWeight { get; internal set; }

    [DataMember(Name = "match")]
    public long Match { get; internal set; }

    [DataMember(Name = "next_doc")]
    public long NextDoc { get; internal set; }

    [DataMember(Name = "score")]
    public long Score { get; internal set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.QueryProfile
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class QueryProfile
  {
    [DataMember(Name = "breakdown")]
    public QueryBreakdown Breakdown { get; internal set; }

    [DataMember(Name = "children")]
    public IEnumerable<QueryProfile> Children { get; internal set; }

    [DataMember(Name = "description")]
    public string Description { get; internal set; }

    [DataMember(Name = "time_in_nanos")]
    public long TimeInNanoseconds { get; internal set; }

    [DataMember(Name = "type")]
    public string Type { get; internal set; }
  }
}

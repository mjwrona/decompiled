// Decompiled with JetBrains decompiler
// Type: Nest.QueryWatchesResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class QueryWatchesResponse : ResponseBase
  {
    [DataMember(Name = "count")]
    public int Count { get; internal set; }

    [DataMember(Name = "watches")]
    public IReadOnlyCollection<WatchQueryResult> Watches { get; internal set; }
  }
}

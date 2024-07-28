// Decompiled with JetBrains decompiler
// Type: Nest.ShardStatistics
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class ShardStatistics
  {
    [DataMember(Name = "failed")]
    public int Failed { get; internal set; }

    [DataMember(Name = "failures")]
    public IReadOnlyCollection<ShardFailure> Failures { get; internal set; } = EmptyReadOnly<ShardFailure>.Collection;

    [DataMember(Name = "successful")]
    public int Successful { get; internal set; }

    [DataMember(Name = "total")]
    public int Total { get; internal set; }
  }
}

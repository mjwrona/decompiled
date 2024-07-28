// Decompiled with JetBrains decompiler
// Type: Nest.StringStatsAggregate
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;

namespace Nest
{
  public class StringStatsAggregate : MetricAggregateBase
  {
    public double AverageLength { get; set; }

    public long Count { get; set; }

    public int MaxLength { get; set; }

    public int MinLength { get; set; }

    public double Entropy { get; set; }

    public IReadOnlyDictionary<string, double> Distribution { get; set; } = EmptyReadOnly<string, double>.Dictionary;
  }
}

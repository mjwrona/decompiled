// Decompiled with JetBrains decompiler
// Type: Nest.BoxplotAggregate
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  public class BoxplotAggregate : MetricAggregateBase
  {
    [JsonFormatter(typeof (StringDoubleFormatter))]
    public double Min { get; set; }

    [JsonFormatter(typeof (StringDoubleFormatter))]
    public double Max { get; set; }

    [JsonFormatter(typeof (StringDoubleFormatter))]
    public double Q1 { get; set; }

    [JsonFormatter(typeof (StringDoubleFormatter))]
    public double Q2 { get; set; }

    [JsonFormatter(typeof (StringDoubleFormatter))]
    public double Q3 { get; set; }

    [JsonFormatter(typeof (StringDoubleFormatter))]
    public double Lower { get; set; }

    [JsonFormatter(typeof (StringDoubleFormatter))]
    public double Upper { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.IMovingAverageAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (MovingAverageAggregationFormatter))]
  public interface IMovingAverageAggregation : IPipelineAggregation, IAggregation
  {
    [DataMember(Name = "minimize")]
    bool? Minimize { get; set; }

    IMovingAverageModel Model { get; set; }

    [DataMember(Name = "predict")]
    int? Predict { get; set; }

    [DataMember(Name = "window")]
    int? Window { get; set; }
  }
}

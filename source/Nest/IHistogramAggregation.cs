// Decompiled with JetBrains decompiler
// Type: Nest.IHistogramAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (HistogramAggregation))]
  public interface IHistogramAggregation : IBucketAggregation, IAggregation
  {
    [DataMember(Name = "extended_bounds")]
    Nest.ExtendedBounds<double> ExtendedBounds { get; set; }

    [DataMember(Name = "hard_bounds")]
    Nest.HardBounds<double> HardBounds { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "interval")]
    double? Interval { get; set; }

    [DataMember(Name = "min_doc_count")]
    int? MinimumDocumentCount { get; set; }

    [DataMember(Name = "missing")]
    double? Missing { get; set; }

    [DataMember(Name = "offset")]
    double? Offset { get; set; }

    [DataMember(Name = "order")]
    HistogramOrder Order { get; set; }

    [DataMember(Name = "script")]
    IScript Script { get; set; }
  }
}

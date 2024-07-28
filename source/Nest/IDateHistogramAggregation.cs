// Decompiled with JetBrains decompiler
// Type: Nest.IDateHistogramAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (DateHistogramAggregation))]
  public interface IDateHistogramAggregation : IBucketAggregation, IAggregation
  {
    [DataMember(Name = "extended_bounds")]
    Nest.ExtendedBounds<DateMath> ExtendedBounds { get; set; }

    [DataMember(Name = "hard_bounds")]
    Nest.HardBounds<DateMath> HardBounds { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "format")]
    string Format { get; set; }

    [Obsolete("Deprecated in version 7.2.0, use CalendarInterval or FixedInterval instead")]
    [DataMember(Name = "interval")]
    Union<DateInterval, Time> Interval { get; set; }

    [DataMember(Name = "calendar_interval")]
    Union<DateInterval?, DateMathTime> CalendarInterval { get; set; }

    [DataMember(Name = "fixed_interval")]
    Time FixedInterval { get; set; }

    [DataMember(Name = "min_doc_count")]
    int? MinimumDocumentCount { get; set; }

    [DataMember(Name = "missing")]
    DateTime? Missing { get; set; }

    [DataMember(Name = "offset")]
    string Offset { get; set; }

    [DataMember(Name = "order")]
    HistogramOrder Order { get; set; }

    [DataMember(Name = "script")]
    IScript Script { get; set; }

    [DataMember(Name = "time_zone")]
    string TimeZone { get; set; }
  }
}

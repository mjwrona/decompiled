// Decompiled with JetBrains decompiler
// Type: Nest.INumericRangeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (FieldNameQueryFormatter<NumericRangeQuery, INumericRangeQuery>))]
  public interface INumericRangeQuery : IRangeQuery, IFieldNameQuery, IQuery
  {
    [DataMember(Name = "gt")]
    double? GreaterThan { get; set; }

    [DataMember(Name = "gte")]
    double? GreaterThanOrEqualTo { get; set; }

    [DataMember(Name = "lt")]
    double? LessThan { get; set; }

    [DataMember(Name = "lte")]
    double? LessThanOrEqualTo { get; set; }

    [DataMember(Name = "relation")]
    RangeRelation? Relation { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "from")]
    double? From { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "to")]
    double? To { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "include_lower")]
    bool? IncludeLower { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "include_upper")]
    bool? IncludeUpper { get; set; }
  }
}

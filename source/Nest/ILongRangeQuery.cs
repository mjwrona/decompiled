// Decompiled with JetBrains decompiler
// Type: Nest.ILongRangeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (FieldNameQueryFormatter<LongRangeQuery, ILongRangeQuery>))]
  public interface ILongRangeQuery : IRangeQuery, IFieldNameQuery, IQuery
  {
    [DataMember(Name = "gt")]
    long? GreaterThan { get; set; }

    [DataMember(Name = "gte")]
    long? GreaterThanOrEqualTo { get; set; }

    [DataMember(Name = "lt")]
    long? LessThan { get; set; }

    [DataMember(Name = "lte")]
    long? LessThanOrEqualTo { get; set; }

    [DataMember(Name = "relation")]
    RangeRelation? Relation { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "from")]
    long? From { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "to")]
    long? To { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "include_lower")]
    bool? IncludeLower { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "include_upper")]
    bool? IncludeUpper { get; set; }
  }
}

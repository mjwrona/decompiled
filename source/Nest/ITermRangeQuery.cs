// Decompiled with JetBrains decompiler
// Type: Nest.ITermRangeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (FieldNameQueryFormatter<TermRangeQuery, ITermRangeQuery>))]
  public interface ITermRangeQuery : IRangeQuery, IFieldNameQuery, IQuery
  {
    [DataMember(Name = "gt")]
    string GreaterThan { get; set; }

    [DataMember(Name = "gte")]
    string GreaterThanOrEqualTo { get; set; }

    [DataMember(Name = "lt")]
    string LessThan { get; set; }

    [DataMember(Name = "lte")]
    string LessThanOrEqualTo { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "from")]
    string From { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "to")]
    string To { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "include_lower")]
    bool? IncludeLower { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    [DataMember(Name = "include_upper")]
    bool? IncludeUpper { get; set; }
  }
}

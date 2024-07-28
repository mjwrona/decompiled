// Decompiled with JetBrains decompiler
// Type: Nest.FieldType
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum FieldType
  {
    [EnumMember(Value = "none")] None,
    [EnumMember(Value = "geo_point")] GeoPoint,
    [EnumMember(Value = "geo_shape")] GeoShape,
    [EnumMember(Value = "ip")] Ip,
    [EnumMember(Value = "binary")] Binary,
    [EnumMember(Value = "keyword")] Keyword,
    [EnumMember(Value = "text")] Text,
    [EnumMember(Value = "search_as_you_type")] SearchAsYouType,
    [EnumMember(Value = "date")] Date,
    [EnumMember(Value = "date_nanos")] DateNanos,
    [EnumMember(Value = "boolean")] Boolean,
    [EnumMember(Value = "completion")] Completion,
    [EnumMember(Value = "nested")] Nested,
    [EnumMember(Value = "object")] Object,
    [EnumMember(Value = "murmur3")] Murmur3Hash,
    [EnumMember(Value = "token_count")] TokenCount,
    [EnumMember(Value = "percolator")] Percolator,
    [EnumMember(Value = "integer")] Integer,
    [EnumMember(Value = "long")] Long,
    [EnumMember(Value = "unsigned_long")] UnsignedLong,
    [EnumMember(Value = "short")] Short,
    [EnumMember(Value = "byte")] Byte,
    [EnumMember(Value = "float")] Float,
    [EnumMember(Value = "half_float")] HalfFloat,
    [EnumMember(Value = "scaled_float")] ScaledFloat,
    [EnumMember(Value = "double")] Double,
    [EnumMember(Value = "integer_range")] IntegerRange,
    [EnumMember(Value = "float_range")] FloatRange,
    [EnumMember(Value = "long_range")] LongRange,
    [EnumMember(Value = "double_range")] DoubleRange,
    [EnumMember(Value = "date_range")] DateRange,
    [EnumMember(Value = "ip_range")] IpRange,
    [EnumMember(Value = "alias")] Alias,
    [EnumMember(Value = "join")] Join,
    [EnumMember(Value = "rank_feature")] RankFeature,
    [EnumMember(Value = "rank_features")] RankFeatures,
    [EnumMember(Value = "flattened")] Flattened,
    [EnumMember(Value = "shape")] Shape,
    [EnumMember(Value = "histogram")] Histogram,
    [EnumMember(Value = "constant_keyword")] ConstantKeyword,
    [EnumMember(Value = "wildcard")] Wildcard,
    [EnumMember(Value = "point")] Point,
    [EnumMember(Value = "version")] Version,
    [EnumMember(Value = "dense_vector")] DenseVector,
    [EnumMember(Value = "match_only_text")] MatchOnlyText,
  }
}

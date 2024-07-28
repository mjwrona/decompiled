// Decompiled with JetBrains decompiler
// Type: Nest.RangeType
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum RangeType
  {
    [EnumMember(Value = "integer_range")] IntegerRange,
    [EnumMember(Value = "float_range")] FloatRange,
    [EnumMember(Value = "long_range")] LongRange,
    [EnumMember(Value = "double_range")] DoubleRange,
    [EnumMember(Value = "date_range")] DateRange,
    [EnumMember(Value = "ip_range")] IpRange,
  }
}

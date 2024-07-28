// Decompiled with JetBrains decompiler
// Type: Nest.DistanceUnit
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum DistanceUnit
  {
    [EnumMember(Value = "in")] Inch,
    [EnumMember(Value = "ft")] Feet,
    [EnumMember(Value = "yd")] Yards,
    [EnumMember(Value = "mi")] Miles,
    [EnumMember(Value = "nmi"), AlternativeEnumMember("NM")] NauticalMiles,
    [EnumMember(Value = "km")] Kilometers,
    [EnumMember(Value = "m")] Meters,
    [EnumMember(Value = "cm")] Centimeters,
    [EnumMember(Value = "mm")] Millimeters,
  }
}

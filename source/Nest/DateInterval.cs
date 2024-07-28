// Decompiled with JetBrains decompiler
// Type: Nest.DateInterval
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum DateInterval
  {
    [EnumMember(Value = "second")] Second,
    [EnumMember(Value = "minute")] Minute,
    [EnumMember(Value = "hour")] Hour,
    [EnumMember(Value = "day")] Day,
    [EnumMember(Value = "week")] Week,
    [EnumMember(Value = "month")] Month,
    [EnumMember(Value = "quarter")] Quarter,
    [EnumMember(Value = "year")] Year,
  }
}

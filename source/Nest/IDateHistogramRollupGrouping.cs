// Decompiled with JetBrains decompiler
// Type: Nest.IDateHistogramRollupGrouping
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (DateHistogramRollupGrouping))]
  public interface IDateHistogramRollupGrouping
  {
    [DataMember(Name = "delay")]
    Time Delay { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "format")]
    string Format { get; set; }

    [DataMember(Name = "interval")]
    Time Interval { get; set; }

    [DataMember(Name = "time_zone")]
    string TimeZone { get; set; }
  }
}

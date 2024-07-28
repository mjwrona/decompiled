// Decompiled with JetBrains decompiler
// Type: Nest.IDateHistogramCompositeAggregationSource
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IDateHistogramCompositeAggregationSource : ICompositeAggregationSource
  {
    [DataMember(Name = "format")]
    string Format { get; set; }

    [DataMember(Name = "interval")]
    [Obsolete("Use FixedInterval or CalendarInterval")]
    Union<DateInterval?, Time> Interval { get; set; }

    [DataMember(Name = "calendar_interval")]
    Union<DateInterval?, DateMathTime> CalendarInterval { get; set; }

    [DataMember(Name = "fixed_interval")]
    Time FixedInterval { get; set; }

    [DataMember(Name = "time_zone")]
    string TimeZone { get; set; }
  }
}

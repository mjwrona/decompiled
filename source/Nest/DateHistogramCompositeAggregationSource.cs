// Decompiled with JetBrains decompiler
// Type: Nest.DateHistogramCompositeAggregationSource
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DateHistogramCompositeAggregationSource : 
    CompositeAggregationSourceBase,
    IDateHistogramCompositeAggregationSource,
    ICompositeAggregationSource
  {
    public DateHistogramCompositeAggregationSource(string name)
      : base(name)
    {
    }

    public string Format { get; set; }

    [Obsolete("Use FixedInterval or CalendarInterval")]
    public Union<DateInterval?, Time> Interval { get; set; }

    public Union<DateInterval?, DateMathTime> CalendarInterval { get; set; }

    public Time FixedInterval { get; set; }

    public string TimeZone { get; set; }

    protected override string SourceType => "date_histogram";
  }
}

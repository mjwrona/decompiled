// Decompiled with JetBrains decompiler
// Type: Nest.DateHistogramAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DateHistogramAggregation : 
    BucketAggregationBase,
    IDateHistogramAggregation,
    IBucketAggregation,
    IAggregation
  {
    private string _format;

    internal DateHistogramAggregation()
    {
    }

    public DateHistogramAggregation(string name)
      : base(name)
    {
    }

    public Nest.ExtendedBounds<DateMath> ExtendedBounds { get; set; }

    public Nest.HardBounds<DateMath> HardBounds { get; set; }

    public Field Field { get; set; }

    public string Format
    {
      get => string.IsNullOrEmpty(this._format) || this._format.Contains("date_optional_time") || this.ExtendedBounds == null && this.HardBounds == null && !this.Missing.HasValue ? this._format : this._format + "||date_optional_time";
      set => this._format = value;
    }

    [Obsolete("Deprecated in version 7.2.0, use CalendarInterval or FixedInterval instead")]
    public Union<DateInterval, Time> Interval { get; set; }

    public Union<DateInterval?, DateMathTime> CalendarInterval { get; set; }

    public Time FixedInterval { get; set; }

    public int? MinimumDocumentCount { get; set; }

    public DateTime? Missing { get; set; }

    public string Offset { get; set; }

    public HistogramOrder Order { get; set; }

    public IScript Script { get; set; }

    public string TimeZone { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.DateHistogram = (IDateHistogramAggregation) this;
  }
}

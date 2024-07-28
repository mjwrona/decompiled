// Decompiled with JetBrains decompiler
// Type: Nest.AutoDateHistogramAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class AutoDateHistogramAggregation : 
    BucketAggregationBase,
    IAutoDateHistogramAggregation,
    IBucketAggregation,
    IAggregation
  {
    private string _format;

    internal AutoDateHistogramAggregation()
    {
    }

    public AutoDateHistogramAggregation(string name)
      : base(name)
    {
    }

    public Field Field { get; set; }

    public int? Buckets { get; set; }

    public string Format
    {
      get => string.IsNullOrEmpty(this._format) || this._format.Contains("date_optional_time") || !this.Missing.HasValue ? this._format : this._format + "||date_optional_time";
      set => this._format = value;
    }

    public DateTime? Missing { get; set; }

    public string Offset { get; set; }

    public IDictionary<string, object> Params { get; set; }

    public IScript Script { get; set; }

    public string TimeZone { get; set; }

    public Nest.MinimumInterval? MinimumInterval { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.AutoDateHistogram = (IAutoDateHistogramAggregation) this;
  }
}

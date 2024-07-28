// Decompiled with JetBrains decompiler
// Type: Nest.DateRangeAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class DateRangeAggregation : 
    BucketAggregationBase,
    IDateRangeAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal DateRangeAggregation()
    {
    }

    public DateRangeAggregation(string name)
      : base(name)
    {
    }

    public Field Field { get; set; }

    public string Format { get; set; }

    public object Missing { get; set; }

    public IEnumerable<IDateRangeExpression> Ranges { get; set; }

    public string TimeZone { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.DateRange = (IDateRangeAggregation) this;
  }
}

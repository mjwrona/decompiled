// Decompiled with JetBrains decompiler
// Type: Nest.DateRangeExpressionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DateRangeExpressionDescriptor : 
    DescriptorBase<DateRangeExpressionDescriptor, IDateRangeExpression>,
    IDateRangeExpression
  {
    DateMath IDateRangeExpression.From { get; set; }

    string IDateRangeExpression.Key { get; set; }

    DateMath IDateRangeExpression.To { get; set; }

    public DateRangeExpressionDescriptor From(DateMath from) => this.Assign<DateMath>(from, (Action<IDateRangeExpression, DateMath>) ((a, v) => a.From = v));

    public DateRangeExpressionDescriptor To(DateMath to) => this.Assign<DateMath>(to, (Action<IDateRangeExpression, DateMath>) ((a, v) => a.To = v));

    public DateRangeExpressionDescriptor Key(string key) => this.Assign<string>(key, (Action<IDateRangeExpression, string>) ((a, v) => a.Key = v));
  }
}

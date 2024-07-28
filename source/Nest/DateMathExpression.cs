// Decompiled with JetBrains decompiler
// Type: Nest.DateMathExpression
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  [JsonFormatter(typeof (DateMathExpressionFormatter))]
  public class DateMathExpression : DateMath
  {
    public DateMathExpression(string anchor) => this.Anchor = (Union<DateTime, string>) anchor;

    public DateMathExpression(DateTime anchor) => this.Anchor = (Union<DateTime, string>) anchor;

    public DateMathExpression(
      Union<DateTime, string> anchor,
      DateMathTime range,
      DateMathOperation operation)
    {
      anchor.ThrowIfNull<Union<DateTime, string>>(nameof (anchor));
      range.ThrowIfNull<DateMathTime>(nameof (range));
      operation.ThrowIfNull<DateMathOperation>(nameof (operation));
      this.Anchor = anchor;
      this.Self.Ranges.Add(Tuple.Create<DateMathOperation, DateMathTime>(operation, range));
    }

    public DateMathExpression Add(DateMathTime expression)
    {
      this.Self.Ranges.Add(Tuple.Create<DateMathOperation, DateMathTime>(DateMathOperation.Add, expression));
      return this;
    }

    public DateMathExpression Subtract(DateMathTime expression)
    {
      this.Self.Ranges.Add(Tuple.Create<DateMathOperation, DateMathTime>(DateMathOperation.Subtract, expression));
      return this;
    }

    public DateMathExpression Operation(DateMathTime expression, DateMathOperation operation)
    {
      this.Self.Ranges.Add(Tuple.Create<DateMathOperation, DateMathTime>(operation, expression));
      return this;
    }

    public DateMath RoundTo(DateMathTimeUnit round)
    {
      this.Round = new DateMathTimeUnit?(round);
      return (DateMath) this;
    }
  }
}

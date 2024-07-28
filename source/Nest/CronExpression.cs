// Decompiled with JetBrains decompiler
// Type: Nest.CronExpression
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  [JsonFormatter(typeof (CronExpressionFormatter))]
  public class CronExpression : ScheduleBase, IEquatable<CronExpression>
  {
    private readonly string _expression;

    public CronExpression(string expression)
    {
      switch (expression)
      {
        case null:
          throw new ArgumentNullException(nameof (expression));
        case "":
          throw new ArgumentException("must have a length", nameof (expression));
        default:
          this._expression = expression;
          break;
      }
    }

    public bool Equals(CronExpression other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || string.Equals(this._expression, other._expression);
    }

    public static implicit operator CronExpression(string expression) => new CronExpression(expression);

    public override string ToString() => this._expression;

    internal override void WrapInContainer(IScheduleContainer container) => container.Cron = this;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((CronExpression) obj);
    }

    public override int GetHashCode()
    {
      string expression = this._expression;
      return expression == null ? 0 : expression.GetHashCode();
    }

    public static bool operator ==(CronExpression left, CronExpression right) => object.Equals((object) left, (object) right);

    public static bool operator !=(CronExpression left, CronExpression right) => !object.Equals((object) left, (object) right);
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.CronExpressions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class CronExpressions : 
    ScheduleBase,
    ICronExpressions,
    ISchedule,
    IEnumerable<CronExpression>,
    IEnumerable
  {
    private List<CronExpression> _expressions;

    public CronExpressions(IEnumerable<CronExpression> expressions) => this._expressions = expressions != null ? expressions.ToList<CronExpression>() : (List<CronExpression>) null;

    public CronExpressions(params CronExpression[] expressions) => this._expressions = expressions != null ? ((IEnumerable<CronExpression>) expressions).ToList<CronExpression>() : (List<CronExpression>) null;

    public CronExpressions()
    {
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<CronExpression> GetEnumerator() => (IEnumerator<CronExpression>) this._expressions?.GetEnumerator() ?? Enumerable.Empty<CronExpression>().GetEnumerator();

    public void Add(CronExpression expression)
    {
      if (this._expressions == null)
        this._expressions = new List<CronExpression>();
      this._expressions.Add(expression);
    }

    internal override void WrapInContainer(IScheduleContainer container) => container.CronExpressions = (ICronExpressions) this;

    public static implicit operator CronExpressions(CronExpression[] cronExpressions) => new CronExpressions(cronExpressions);
  }
}

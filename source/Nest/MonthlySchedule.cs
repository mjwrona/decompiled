// Decompiled with JetBrains decompiler
// Type: Nest.MonthlySchedule
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class MonthlySchedule : 
    ScheduleBase,
    IMonthlySchedule,
    ISchedule,
    IEnumerable<ITimeOfMonth>,
    IEnumerable
  {
    private List<ITimeOfMonth> _times;

    public MonthlySchedule(IEnumerable<ITimeOfMonth> times) => this._times = times != null ? times.ToList<ITimeOfMonth>() : (List<ITimeOfMonth>) null;

    public MonthlySchedule(params ITimeOfMonth[] times) => this._times = times != null ? ((IEnumerable<ITimeOfMonth>) times).ToList<ITimeOfMonth>() : (List<ITimeOfMonth>) null;

    public MonthlySchedule()
    {
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<ITimeOfMonth> GetEnumerator() => (IEnumerator<ITimeOfMonth>) this._times?.GetEnumerator() ?? Enumerable.Empty<ITimeOfMonth>().GetEnumerator();

    public void Add(ITimeOfMonth time)
    {
      if (this._times == null)
        this._times = new List<ITimeOfMonth>();
      this._times.Add(time);
    }

    internal override void WrapInContainer(IScheduleContainer container) => container.Monthly = (IMonthlySchedule) this;

    public static implicit operator MonthlySchedule(ITimeOfMonth[] timesOfMonth) => new MonthlySchedule(timesOfMonth);
  }
}

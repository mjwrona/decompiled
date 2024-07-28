// Decompiled with JetBrains decompiler
// Type: Nest.YearlySchedule
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class YearlySchedule : 
    ScheduleBase,
    IYearlySchedule,
    ISchedule,
    IEnumerable<ITimeOfYear>,
    IEnumerable
  {
    private List<ITimeOfYear> _times;

    public YearlySchedule(IEnumerable<ITimeOfYear> times) => this._times = times != null ? times.ToList<ITimeOfYear>() : (List<ITimeOfYear>) null;

    public YearlySchedule(params ITimeOfYear[] times) => this._times = times != null ? ((IEnumerable<ITimeOfYear>) times).ToList<ITimeOfYear>() : (List<ITimeOfYear>) null;

    public YearlySchedule()
    {
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<ITimeOfYear> GetEnumerator() => (IEnumerator<ITimeOfYear>) this._times?.GetEnumerator() ?? Enumerable.Empty<ITimeOfYear>().GetEnumerator();

    public void Add(ITimeOfYear time)
    {
      if (this._times == null)
        this._times = new List<ITimeOfYear>();
      this._times.Add(time);
    }

    internal override void WrapInContainer(IScheduleContainer container) => container.Yearly = (IYearlySchedule) this;

    public static implicit operator YearlySchedule(ITimeOfYear[] times) => new YearlySchedule(times);
  }
}

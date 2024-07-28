// Decompiled with JetBrains decompiler
// Type: Nest.WeeklySchedule
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class WeeklySchedule : 
    ScheduleBase,
    IWeeklySchedule,
    ISchedule,
    IEnumerable<ITimeOfWeek>,
    IEnumerable
  {
    private List<ITimeOfWeek> _times;

    public WeeklySchedule(IEnumerable<ITimeOfWeek> times) => this._times = times != null ? times.ToList<ITimeOfWeek>() : (List<ITimeOfWeek>) null;

    public WeeklySchedule(params ITimeOfWeek[] times) => this._times = times != null ? ((IEnumerable<ITimeOfWeek>) times).ToList<ITimeOfWeek>() : (List<ITimeOfWeek>) null;

    public WeeklySchedule()
    {
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<ITimeOfWeek> GetEnumerator() => (IEnumerator<ITimeOfWeek>) this._times?.GetEnumerator() ?? Enumerable.Empty<ITimeOfWeek>().GetEnumerator();

    public void Add(ITimeOfWeek time)
    {
      if (this._times == null)
        this._times = new List<ITimeOfWeek>();
      this._times.Add(time);
    }

    internal override void WrapInContainer(IScheduleContainer container) => container.Weekly = (IWeeklySchedule) this;

    public static implicit operator WeeklySchedule(ITimeOfWeek[] timesOfWeek) => new WeeklySchedule(timesOfWeek);
  }
}

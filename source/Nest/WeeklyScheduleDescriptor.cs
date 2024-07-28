// Decompiled with JetBrains decompiler
// Type: Nest.WeeklyScheduleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class WeeklyScheduleDescriptor : 
    DescriptorPromiseBase<WeeklyScheduleDescriptor, WeeklySchedule>
  {
    public WeeklyScheduleDescriptor()
      : base(new WeeklySchedule())
    {
    }

    public WeeklyScheduleDescriptor Add(Func<TimeOfWeekDescriptor, ITimeOfWeek> selector) => this.Assign<Func<TimeOfWeekDescriptor, ITimeOfWeek>>(selector, (Action<WeeklySchedule, Func<TimeOfWeekDescriptor, ITimeOfWeek>>) ((a, v) => a.Add(v.InvokeOrDefault<TimeOfWeekDescriptor, ITimeOfWeek>(new TimeOfWeekDescriptor()))));
  }
}

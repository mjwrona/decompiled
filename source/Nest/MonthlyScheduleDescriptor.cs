// Decompiled with JetBrains decompiler
// Type: Nest.MonthlyScheduleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MonthlyScheduleDescriptor : 
    DescriptorPromiseBase<MonthlyScheduleDescriptor, MonthlySchedule>
  {
    public MonthlyScheduleDescriptor()
      : base(new MonthlySchedule())
    {
    }

    public MonthlyScheduleDescriptor Add(Func<TimeOfMonthDescriptor, ITimeOfMonth> selector) => this.Assign<Func<TimeOfMonthDescriptor, ITimeOfMonth>>(selector, (Action<MonthlySchedule, Func<TimeOfMonthDescriptor, ITimeOfMonth>>) ((a, v) => a.Add(v.InvokeOrDefault<TimeOfMonthDescriptor, ITimeOfMonth>(new TimeOfMonthDescriptor()))));
  }
}

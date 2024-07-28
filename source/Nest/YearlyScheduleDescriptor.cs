// Decompiled with JetBrains decompiler
// Type: Nest.YearlyScheduleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class YearlyScheduleDescriptor : 
    DescriptorPromiseBase<YearlyScheduleDescriptor, YearlySchedule>
  {
    public YearlyScheduleDescriptor()
      : base(new YearlySchedule())
    {
    }

    public YearlyScheduleDescriptor Add(Func<TimeOfYearDescriptor, ITimeOfYear> selector) => this.Assign<Func<TimeOfYearDescriptor, ITimeOfYear>>(selector, (Action<YearlySchedule, Func<TimeOfYearDescriptor, ITimeOfYear>>) ((a, v) => a.Add(v.InvokeOrDefault<TimeOfYearDescriptor, ITimeOfYear>(new TimeOfYearDescriptor()))));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.DailyScheduleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class DailyScheduleDescriptor : 
    DescriptorBase<DailyScheduleDescriptor, IDailySchedule>,
    IDailySchedule,
    ISchedule
  {
    Union<IEnumerable<string>, IEnumerable<ITimeOfDay>> IDailySchedule.At { get; set; }

    public DailyScheduleDescriptor At(Func<TimeOfDayDescriptor, ITimeOfDay> selector) => this.Assign<Func<TimeOfDayDescriptor, ITimeOfDay>>(selector, (Action<IDailySchedule, Func<TimeOfDayDescriptor, ITimeOfDay>>) ((a, v) => a.At = new Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>((IEnumerable<ITimeOfDay>) new ITimeOfDay[1]
    {
      v != null ? v.InvokeOrDefault<TimeOfDayDescriptor, ITimeOfDay>(new TimeOfDayDescriptor()) : (ITimeOfDay) null
    })));

    public DailyScheduleDescriptor At(IEnumerable<ITimeOfDay> times) => this.Assign<Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>>(new Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>(times), (Action<IDailySchedule, Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>>) ((a, v) => a.At = v));

    public DailyScheduleDescriptor At(params ITimeOfDay[] times) => this.Assign<Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>>(new Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>((IEnumerable<ITimeOfDay>) times), (Action<IDailySchedule, Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>>) ((a, v) => a.At = v));

    public DailyScheduleDescriptor At(IEnumerable<string> times) => this.Assign<Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>>(new Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>(times), (Action<IDailySchedule, Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>>) ((a, v) => a.At = v));

    public DailyScheduleDescriptor At(params string[] times) => this.Assign<Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>>(new Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>((IEnumerable<string>) times), (Action<IDailySchedule, Union<IEnumerable<string>, IEnumerable<ITimeOfDay>>>) ((a, v) => a.At = v));
  }
}

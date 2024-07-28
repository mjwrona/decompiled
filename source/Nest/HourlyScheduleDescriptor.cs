// Decompiled with JetBrains decompiler
// Type: Nest.HourlyScheduleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class HourlyScheduleDescriptor : 
    DescriptorBase<HourlyScheduleDescriptor, IHourlySchedule>,
    IHourlySchedule,
    ISchedule
  {
    IEnumerable<int> IHourlySchedule.Minute { get; set; }

    public HourlyScheduleDescriptor Minute(params int[] minutes) => this.Assign<int[]>(minutes, (Action<IHourlySchedule, int[]>) ((a, v) => a.Minute = (IEnumerable<int>) v));

    public HourlyScheduleDescriptor Minute(IEnumerable<int> minutes) => this.Assign<IEnumerable<int>>(minutes, (Action<IHourlySchedule, IEnumerable<int>>) ((a, v) => a.Minute = v));
  }
}

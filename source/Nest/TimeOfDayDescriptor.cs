// Decompiled with JetBrains decompiler
// Type: Nest.TimeOfDayDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TimeOfDayDescriptor : DescriptorBase<TimeOfDayDescriptor, ITimeOfDay>, ITimeOfDay
  {
    IEnumerable<int> ITimeOfDay.Hour { get; set; }

    IEnumerable<int> ITimeOfDay.Minute { get; set; }

    public TimeOfDayDescriptor Hour(IEnumerable<int> hours) => this.Assign<IEnumerable<int>>(hours, (Action<ITimeOfDay, IEnumerable<int>>) ((a, v) => a.Hour = v));

    public TimeOfDayDescriptor Hour(params int[] hours) => this.Assign<int[]>(hours, (Action<ITimeOfDay, int[]>) ((a, v) => a.Hour = (IEnumerable<int>) v));

    public TimeOfDayDescriptor Minute(IEnumerable<int> minutes) => this.Assign<IEnumerable<int>>(minutes, (Action<ITimeOfDay, IEnumerable<int>>) ((a, v) => a.Minute = v));

    public TimeOfDayDescriptor Minute(params int[] minutes) => this.Assign<int[]>(minutes, (Action<ITimeOfDay, int[]>) ((a, v) => a.Minute = (IEnumerable<int>) v));
  }
}

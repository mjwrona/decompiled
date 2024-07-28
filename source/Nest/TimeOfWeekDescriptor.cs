// Decompiled with JetBrains decompiler
// Type: Nest.TimeOfWeekDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TimeOfWeekDescriptor : DescriptorBase<TimeOfWeekDescriptor, ITimeOfWeek>, ITimeOfWeek
  {
    IEnumerable<string> ITimeOfWeek.At { get; set; }

    IEnumerable<Day> ITimeOfWeek.On { get; set; }

    public TimeOfWeekDescriptor On(IEnumerable<Day> day) => this.Assign<IEnumerable<Day>>(day, (Action<ITimeOfWeek, IEnumerable<Day>>) ((a, v) => a.On = v));

    public TimeOfWeekDescriptor On(params Day[] day) => this.Assign<Day[]>(day, (Action<ITimeOfWeek, Day[]>) ((a, v) => a.On = (IEnumerable<Day>) v));

    public TimeOfWeekDescriptor At(IEnumerable<string> time) => this.Assign<IEnumerable<string>>(time, (Action<ITimeOfWeek, IEnumerable<string>>) ((a, v) => a.At = v));

    public TimeOfWeekDescriptor At(params string[] time) => this.Assign<string[]>(time, (Action<ITimeOfWeek, string[]>) ((a, v) => a.At = (IEnumerable<string>) v));
  }
}

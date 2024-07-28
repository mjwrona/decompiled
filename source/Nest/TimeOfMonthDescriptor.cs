// Decompiled with JetBrains decompiler
// Type: Nest.TimeOfMonthDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TimeOfMonthDescriptor : 
    DescriptorBase<TimeOfMonthDescriptor, ITimeOfMonth>,
    ITimeOfMonth
  {
    IEnumerable<string> ITimeOfMonth.At { get; set; }

    IEnumerable<int> ITimeOfMonth.On { get; set; }

    public TimeOfMonthDescriptor On(IEnumerable<int> dates) => this.Assign<IEnumerable<int>>(dates, (Action<ITimeOfMonth, IEnumerable<int>>) ((a, v) => a.On = v));

    public TimeOfMonthDescriptor On(params int[] dates) => this.Assign<int[]>(dates, (Action<ITimeOfMonth, int[]>) ((a, v) => a.On = (IEnumerable<int>) v));

    public TimeOfMonthDescriptor At(IEnumerable<string> time) => this.Assign<IEnumerable<string>>(time, (Action<ITimeOfMonth, IEnumerable<string>>) ((a, v) => a.At = v));

    public TimeOfMonthDescriptor At(params string[] time) => this.Assign<string[]>(time, (Action<ITimeOfMonth, string[]>) ((a, v) => a.At = (IEnumerable<string>) v));
  }
}

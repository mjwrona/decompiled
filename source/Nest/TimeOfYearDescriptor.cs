// Decompiled with JetBrains decompiler
// Type: Nest.TimeOfYearDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TimeOfYearDescriptor : DescriptorBase<TimeOfYearDescriptor, ITimeOfYear>, ITimeOfYear
  {
    IEnumerable<string> ITimeOfYear.At { get; set; }

    IEnumerable<Month> ITimeOfYear.In { get; set; }

    IEnumerable<int> ITimeOfYear.On { get; set; }

    public TimeOfYearDescriptor In(IEnumerable<Month> @in) => this.Assign<IEnumerable<Month>>(@in, (Action<ITimeOfYear, IEnumerable<Month>>) ((a, v) => a.In = v));

    public TimeOfYearDescriptor In(params Month[] @in) => this.Assign<Month[]>(@in, (Action<ITimeOfYear, Month[]>) ((a, v) => a.In = (IEnumerable<Month>) v));

    public TimeOfYearDescriptor On(IEnumerable<int> on) => this.Assign<IEnumerable<int>>(on, (Action<ITimeOfYear, IEnumerable<int>>) ((a, v) => a.On = v));

    public TimeOfYearDescriptor On(params int[] on) => this.Assign<int[]>(on, (Action<ITimeOfYear, int[]>) ((a, v) => a.On = (IEnumerable<int>) v));

    public TimeOfYearDescriptor At(IEnumerable<string> time) => this.Assign<IEnumerable<string>>(time, (Action<ITimeOfYear, IEnumerable<string>>) ((a, v) => a.At = v));

    public TimeOfYearDescriptor At(params string[] time) => this.Assign<string[]>(time, (Action<ITimeOfYear, string[]>) ((a, v) => a.At = (IEnumerable<string>) v));
  }
}

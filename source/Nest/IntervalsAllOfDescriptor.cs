// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsAllOfDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class IntervalsAllOfDescriptor : 
    IntervalsDescriptorBase<IntervalsAllOfDescriptor, IIntervalsAllOf>,
    IIntervalsAllOf,
    IIntervals
  {
    IEnumerable<IntervalsContainer> IIntervalsAllOf.Intervals { get; set; }

    int? IIntervalsAllOf.MaxGaps { get; set; }

    bool? IIntervalsAllOf.Ordered { get; set; }

    public IntervalsAllOfDescriptor MaxGaps(int? maxGaps) => this.Assign<int?>(maxGaps, (Action<IIntervalsAllOf, int?>) ((a, v) => a.MaxGaps = v));

    public IntervalsAllOfDescriptor Ordered(bool? ordered = true) => this.Assign<bool?>(ordered, (Action<IIntervalsAllOf, bool?>) ((a, v) => a.Ordered = v));

    public IntervalsAllOfDescriptor Intervals(
      Func<IntervalsListDescriptor, IPromise<List<IntervalsContainer>>> selector)
    {
      return this.Assign<Func<IntervalsListDescriptor, IPromise<List<IntervalsContainer>>>>(selector, (Action<IIntervalsAllOf, Func<IntervalsListDescriptor, IPromise<List<IntervalsContainer>>>>) ((a, v) => a.Intervals = (IEnumerable<IntervalsContainer>) v.InvokeOrDefault<IntervalsListDescriptor, IPromise<List<IntervalsContainer>>>(new IntervalsListDescriptor())?.Value));
    }
  }
}

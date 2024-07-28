// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsAnyOfDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class IntervalsAnyOfDescriptor : 
    IntervalsDescriptorBase<IntervalsAnyOfDescriptor, IIntervalsAnyOf>,
    IIntervalsAnyOf,
    IIntervals
  {
    IEnumerable<IntervalsContainer> IIntervalsAnyOf.Intervals { get; set; }

    public IntervalsAnyOfDescriptor Intervals(
      Func<IntervalsListDescriptor, IPromise<List<IntervalsContainer>>> selector)
    {
      return this.Assign<Func<IntervalsListDescriptor, IPromise<List<IntervalsContainer>>>>(selector, (Action<IIntervalsAnyOf, Func<IntervalsListDescriptor, IPromise<List<IntervalsContainer>>>>) ((a, v) => a.Intervals = (IEnumerable<IntervalsContainer>) v.InvokeOrDefault<IntervalsListDescriptor, IPromise<List<IntervalsContainer>>>(new IntervalsListDescriptor())?.Value));
    }
  }
}

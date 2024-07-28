// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsAllOf
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class IntervalsAllOf : IntervalsBase, IIntervalsAllOf, IIntervals
  {
    public IEnumerable<IntervalsContainer> Intervals { get; set; }

    public int? MaxGaps { get; set; }

    public bool? Ordered { get; set; }

    internal override void WrapInContainer(IIntervalsContainer container) => container.AllOf = (IIntervalsAllOf) this;
  }
}

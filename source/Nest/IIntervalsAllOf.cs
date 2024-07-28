// Decompiled with JetBrains decompiler
// Type: Nest.IIntervalsAllOf
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (IntervalsAllOf))]
  public interface IIntervalsAllOf : IIntervals
  {
    [DataMember(Name = "intervals")]
    IEnumerable<IntervalsContainer> Intervals { get; set; }

    [DataMember(Name = "max_gaps")]
    int? MaxGaps { get; set; }

    [DataMember(Name = "ordered")]
    bool? Ordered { get; set; }
  }
}

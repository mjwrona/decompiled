// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsMatch
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class IntervalsMatch : IntervalsBase, IIntervalsMatch, IIntervals
  {
    [DataMember(Name = "analyzer")]
    public string Analyzer { get; set; }

    [DataMember(Name = "max_gaps")]
    public int? MaxGaps { get; set; }

    [DataMember(Name = "ordered")]
    public bool? Ordered { get; set; }

    public string Query { get; set; }

    [DataMember(Name = "use_field")]
    public Field UseField { get; set; }

    internal override void WrapInContainer(IIntervalsContainer container) => container.Match = (IIntervalsMatch) this;
  }
}

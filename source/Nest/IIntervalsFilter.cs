// Decompiled with JetBrains decompiler
// Type: Nest.IIntervalsFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (IntervalsFilter))]
  public interface IIntervalsFilter
  {
    [DataMember(Name = "after")]
    IntervalsContainer After { get; set; }

    [DataMember(Name = "before")]
    IntervalsContainer Before { get; set; }

    [DataMember(Name = "contained_by")]
    IntervalsContainer ContainedBy { get; set; }

    [DataMember(Name = "containing")]
    IntervalsContainer Containing { get; set; }

    [DataMember(Name = "not_contained_by")]
    IntervalsContainer NotContainedBy { get; set; }

    [DataMember(Name = "not_containing")]
    IntervalsContainer NotContaining { get; set; }

    [DataMember(Name = "not_overlapping")]
    IntervalsContainer NotOverlapping { get; set; }

    [DataMember(Name = "overlapping")]
    IntervalsContainer Overlapping { get; set; }

    [DataMember(Name = "script")]
    IScript Script { get; set; }
  }
}

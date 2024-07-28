// Decompiled with JetBrains decompiler
// Type: Nest.IIntervalsFuzzy
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (IntervalsFuzzy))]
  public interface IIntervalsFuzzy : IIntervalsNoFilter
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "prefix_length")]
    int? PrefixLength { get; set; }

    [DataMember(Name = "transpositions")]
    bool? Transpositions { get; set; }

    [DataMember(Name = "fuzziness")]
    Fuzziness Fuzziness { get; set; }

    [DataMember(Name = "term")]
    string Term { get; set; }

    [DataMember(Name = "use_field")]
    Field UseField { get; set; }
  }
}

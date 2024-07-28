// Decompiled with JetBrains decompiler
// Type: Nest.IIntervalsContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public interface IIntervalsContainer
  {
    [DataMember(Name = "all_of")]
    IIntervalsAllOf AllOf { get; set; }

    [DataMember(Name = "any_of")]
    IIntervalsAnyOf AnyOf { get; set; }

    [DataMember(Name = "fuzzy")]
    IIntervalsFuzzy Fuzzy { get; set; }

    [DataMember(Name = "match")]
    IIntervalsMatch Match { get; set; }

    [DataMember(Name = "prefix")]
    IIntervalsPrefix Prefix { get; set; }

    [DataMember(Name = "wildcard")]
    IIntervalsWildcard Wildcard { get; set; }
  }
}

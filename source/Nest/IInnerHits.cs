// Decompiled with JetBrains decompiler
// Type: Nest.IInnerHits
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (InnerHits))]
  public interface IInnerHits
  {
    [DataMember(Name = "collapse")]
    IFieldCollapse Collapse { get; set; }

    [DataMember(Name = "docvalue_fields")]
    Fields DocValueFields { get; set; }

    [DataMember(Name = "explain")]
    bool? Explain { get; set; }

    [DataMember(Name = "from")]
    int? From { get; set; }

    [DataMember(Name = "highlight")]
    IHighlight Highlight { get; set; }

    [DataMember(Name = "ignore_unmapped")]
    bool? IgnoreUnmapped { get; set; }

    [DataMember(Name = "name")]
    string Name { get; set; }

    [DataMember(Name = "script_fields")]
    IScriptFields ScriptFields { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }

    [DataMember(Name = "sort")]
    IList<ISort> Sort { get; set; }

    [DataMember(Name = "_source")]
    Union<bool, ISourceFilter> Source { get; set; }

    [DataMember(Name = "version")]
    bool? Version { get; set; }
  }
}

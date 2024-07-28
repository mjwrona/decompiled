// Decompiled with JetBrains decompiler
// Type: Nest.InnerHits
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class InnerHits : IInnerHits
  {
    public IFieldCollapse Collapse { get; set; }

    public Fields DocValueFields { get; set; }

    public bool? Explain { get; set; }

    public int? From { get; set; }

    public IHighlight Highlight { get; set; }

    public bool? IgnoreUnmapped { get; set; }

    public string Name { get; set; }

    public IScriptFields ScriptFields { get; set; }

    public int? Size { get; set; }

    public IList<ISort> Sort { get; set; }

    public Union<bool, ISourceFilter> Source { get; set; }

    public bool? Version { get; set; }
  }
}

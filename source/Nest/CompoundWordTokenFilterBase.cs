// Decompiled with JetBrains decompiler
// Type: Nest.CompoundWordTokenFilterBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public abstract class CompoundWordTokenFilterBase : 
    TokenFilterBase,
    ICompoundWordTokenFilter,
    ITokenFilter
  {
    protected CompoundWordTokenFilterBase(string type)
      : base(type)
    {
    }

    public string HyphenationPatternsPath { get; set; }

    public int? MaxSubwordSize { get; set; }

    public int? MinSubwordSize { get; set; }

    public int? MinWordSize { get; set; }

    public bool? OnlyLongestMatch { get; set; }

    public IEnumerable<string> WordList { get; set; }

    public string WordListPath { get; set; }
  }
}

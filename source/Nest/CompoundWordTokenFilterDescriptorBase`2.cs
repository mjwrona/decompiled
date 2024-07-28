// Decompiled with JetBrains decompiler
// Type: Nest.CompoundWordTokenFilterDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class CompoundWordTokenFilterDescriptorBase<TCompound, TCompoundInterface> : 
    TokenFilterDescriptorBase<TCompound, TCompoundInterface>,
    ICompoundWordTokenFilter,
    ITokenFilter
    where TCompound : CompoundWordTokenFilterDescriptorBase<TCompound, TCompoundInterface>, TCompoundInterface
    where TCompoundInterface : class, ICompoundWordTokenFilter
  {
    string ICompoundWordTokenFilter.HyphenationPatternsPath { get; set; }

    int? ICompoundWordTokenFilter.MaxSubwordSize { get; set; }

    int? ICompoundWordTokenFilter.MinSubwordSize { get; set; }

    int? ICompoundWordTokenFilter.MinWordSize { get; set; }

    bool? ICompoundWordTokenFilter.OnlyLongestMatch { get; set; }

    IEnumerable<string> ICompoundWordTokenFilter.WordList { get; set; }

    string ICompoundWordTokenFilter.WordListPath { get; set; }

    public TCompound WordList(IEnumerable<string> wordList) => this.Assign<IEnumerable<string>>(wordList, (Action<TCompoundInterface, IEnumerable<string>>) ((a, v) => a.WordList = v));

    public TCompound WordList(params string[] wordList) => this.Assign<string[]>(wordList, (Action<TCompoundInterface, string[]>) ((a, v) => a.WordList = (IEnumerable<string>) v));

    public TCompound WordListPath(string path) => this.Assign<string>(path, (Action<TCompoundInterface, string>) ((a, v) => a.WordListPath = v));

    public TCompound HyphenationPatternsPath(string path) => this.Assign<string>(path, (Action<TCompoundInterface, string>) ((a, v) => a.HyphenationPatternsPath = v));

    public TCompound MinWordSize(int? minWordSize) => this.Assign<int?>(minWordSize, (Action<TCompoundInterface, int?>) ((a, v) => a.MinWordSize = v));

    public TCompound MinSubwordSize(int? minSubwordSize) => this.Assign<int?>(minSubwordSize, (Action<TCompoundInterface, int?>) ((a, v) => a.MinSubwordSize = v));

    public TCompound MaxSubwordSize(int? maxSubwordSize) => this.Assign<int?>(maxSubwordSize, (Action<TCompoundInterface, int?>) ((a, v) => a.MaxSubwordSize = v));

    public TCompound OnlyLongestMatch(bool? onlyLongest = true) => this.Assign<bool?>(onlyLongest, (Action<TCompoundInterface, bool?>) ((a, v) => a.OnlyLongestMatch = v));
  }
}

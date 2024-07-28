// Decompiled with JetBrains decompiler
// Type: Nest.StopTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class StopTokenFilterDescriptor : 
    TokenFilterDescriptorBase<StopTokenFilterDescriptor, IStopTokenFilter>,
    IStopTokenFilter,
    ITokenFilter
  {
    protected override string Type => "stop";

    bool? IStopTokenFilter.IgnoreCase { get; set; }

    bool? IStopTokenFilter.RemoveTrailing { get; set; }

    Nest.StopWords IStopTokenFilter.StopWords { get; set; }

    string IStopTokenFilter.StopWordsPath { get; set; }

    public StopTokenFilterDescriptor IgnoreCase(bool? ignoreCase = true) => this.Assign<bool?>(ignoreCase, (Action<IStopTokenFilter, bool?>) ((a, v) => a.IgnoreCase = v));

    public StopTokenFilterDescriptor RemoveTrailing(bool? removeTrailing = true) => this.Assign<bool?>(removeTrailing, (Action<IStopTokenFilter, bool?>) ((a, v) => a.RemoveTrailing = v));

    public StopTokenFilterDescriptor StopWords(Nest.StopWords stopWords) => this.Assign<Nest.StopWords>(stopWords, (Action<IStopTokenFilter, Nest.StopWords>) ((a, v) => a.StopWords = v));

    public StopTokenFilterDescriptor StopWords(IEnumerable<string> stopWords) => this.Assign<List<string>>(stopWords.ToListOrNullIfEmpty<string>(), (Action<IStopTokenFilter, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public StopTokenFilterDescriptor StopWords(params string[] stopWords) => this.Assign<string[]>(stopWords, (Action<IStopTokenFilter, string[]>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public StopTokenFilterDescriptor StopWordsPath(string path) => this.Assign<string>(path, (Action<IStopTokenFilter, string>) ((a, v) => a.StopWordsPath = v));
  }
}

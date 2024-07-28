// Decompiled with JetBrains decompiler
// Type: Nest.StandardAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class StandardAnalyzerDescriptor : 
    AnalyzerDescriptorBase<StandardAnalyzerDescriptor, IStandardAnalyzer>,
    IStandardAnalyzer,
    IAnalyzer
  {
    protected override string Type => "standard";

    int? IStandardAnalyzer.MaxTokenLength { get; set; }

    Nest.StopWords IStandardAnalyzer.StopWords { get; set; }

    public StandardAnalyzerDescriptor StopWords(params string[] stopWords) => this.Assign<string[]>(stopWords, (Action<IStandardAnalyzer, string[]>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public StandardAnalyzerDescriptor StopWords(IEnumerable<string> stopWords) => this.Assign<List<string>>(stopWords.ToListOrNullIfEmpty<string>(), (Action<IStandardAnalyzer, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public StandardAnalyzerDescriptor StopWords(Nest.StopWords stopWords) => this.Assign<Nest.StopWords>(stopWords, (Action<IStandardAnalyzer, Nest.StopWords>) ((a, v) => a.StopWords = v));

    public StandardAnalyzerDescriptor MaxTokenLength(int? maxTokenLength) => this.Assign<int?>(maxTokenLength, (Action<IStandardAnalyzer, int?>) ((a, v) => a.MaxTokenLength = v));
  }
}

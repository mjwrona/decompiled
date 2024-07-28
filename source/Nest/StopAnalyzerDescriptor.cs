// Decompiled with JetBrains decompiler
// Type: Nest.StopAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class StopAnalyzerDescriptor : 
    AnalyzerDescriptorBase<StopAnalyzerDescriptor, IStopAnalyzer>,
    IStopAnalyzer,
    IAnalyzer
  {
    protected override string Type => "stop";

    Nest.StopWords IStopAnalyzer.StopWords { get; set; }

    string IStopAnalyzer.StopwordsPath { get; set; }

    public StopAnalyzerDescriptor StopWords(params string[] stopWords) => this.Assign<string[]>(stopWords, (Action<IStopAnalyzer, string[]>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public StopAnalyzerDescriptor StopWords(IEnumerable<string> stopWords) => this.Assign<List<string>>(stopWords.ToListOrNullIfEmpty<string>(), (Action<IStopAnalyzer, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public StopAnalyzerDescriptor StopWords(Nest.StopWords stopWords) => this.Assign<Nest.StopWords>(stopWords, (Action<IStopAnalyzer, Nest.StopWords>) ((a, v) => a.StopWords = v));

    public StopAnalyzerDescriptor StopwordsPath(string path) => this.Assign<string>(path, (Action<IStopAnalyzer, string>) ((a, v) => a.StopwordsPath = v));
  }
}

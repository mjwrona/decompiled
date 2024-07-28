// Decompiled with JetBrains decompiler
// Type: Nest.PatternAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PatternAnalyzerDescriptor : 
    AnalyzerDescriptorBase<PatternAnalyzerDescriptor, IPatternAnalyzer>,
    IPatternAnalyzer,
    IAnalyzer
  {
    protected override string Type => "pattern";

    string IPatternAnalyzer.Flags { get; set; }

    bool? IPatternAnalyzer.Lowercase { get; set; }

    string IPatternAnalyzer.Pattern { get; set; }

    Nest.StopWords IPatternAnalyzer.StopWords { get; set; }

    public PatternAnalyzerDescriptor StopWords(params string[] stopWords) => this.Assign<string[]>(stopWords, (Action<IPatternAnalyzer, string[]>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public PatternAnalyzerDescriptor StopWords(IEnumerable<string> stopWords) => this.Assign<List<string>>(stopWords.ToListOrNullIfEmpty<string>(), (Action<IPatternAnalyzer, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public PatternAnalyzerDescriptor StopWords(Nest.StopWords stopWords) => this.Assign<Nest.StopWords>(stopWords, (Action<IPatternAnalyzer, Nest.StopWords>) ((a, v) => a.StopWords = v));

    public PatternAnalyzerDescriptor Pattern(string pattern) => this.Assign<string>(pattern, (Action<IPatternAnalyzer, string>) ((a, v) => a.Pattern = v));

    public PatternAnalyzerDescriptor Flags(string flags) => this.Assign<string>(flags, (Action<IPatternAnalyzer, string>) ((a, v) => a.Flags = v));

    public PatternAnalyzerDescriptor Lowercase(bool? lowercase = true) => this.Assign<bool?>(lowercase, (Action<IPatternAnalyzer, bool?>) ((a, v) => a.Lowercase = v));
  }
}

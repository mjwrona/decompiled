// Decompiled with JetBrains decompiler
// Type: Nest.SnowballAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SnowballAnalyzerDescriptor : 
    AnalyzerDescriptorBase<SnowballAnalyzerDescriptor, ISnowballAnalyzer>,
    ISnowballAnalyzer,
    IAnalyzer
  {
    protected override string Type => "snowball";

    SnowballLanguage? ISnowballAnalyzer.Language { get; set; }

    Nest.StopWords ISnowballAnalyzer.StopWords { get; set; }

    public SnowballAnalyzerDescriptor StopWords(Nest.StopWords stopWords) => this.Assign<Nest.StopWords>(stopWords, (Action<ISnowballAnalyzer, Nest.StopWords>) ((a, v) => a.StopWords = v));

    public SnowballAnalyzerDescriptor StopWords(IEnumerable<string> stopWords) => this.Assign<List<string>>(stopWords.ToListOrNullIfEmpty<string>(), (Action<ISnowballAnalyzer, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public SnowballAnalyzerDescriptor StopWords(params string[] stopWords) => this.Assign<List<string>>(((IEnumerable<string>) stopWords).ToListOrNullIfEmpty<string>(), (Action<ISnowballAnalyzer, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public SnowballAnalyzerDescriptor Language(SnowballLanguage? language) => this.Assign<SnowballLanguage?>(language, (Action<ISnowballAnalyzer, SnowballLanguage?>) ((a, v) => a.Language = v));
  }
}

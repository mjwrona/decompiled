// Decompiled with JetBrains decompiler
// Type: Nest.LanguageAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class LanguageAnalyzerDescriptor : 
    AnalyzerDescriptorBase<LanguageAnalyzerDescriptor, ILanguageAnalyzer>,
    ILanguageAnalyzer,
    IAnalyzer
  {
    private string _type = "language";

    protected override string Type => this._type;

    IEnumerable<string> ILanguageAnalyzer.StemExclusionList { get; set; }

    Nest.StopWords ILanguageAnalyzer.StopWords { get; set; }

    string ILanguageAnalyzer.StopwordsPath { get; set; }

    public LanguageAnalyzerDescriptor Language(Nest.Language? language)
    {
      this._type = language.HasValue ? language.GetValueOrDefault().GetStringValue().ToLowerInvariant() : (string) null;
      return this;
    }

    public LanguageAnalyzerDescriptor StopWords(Nest.StopWords stopWords) => this.Assign<Nest.StopWords>(stopWords, (Action<ILanguageAnalyzer, Nest.StopWords>) ((a, v) => a.StopWords = v));

    public LanguageAnalyzerDescriptor StopWords(params string[] stopWords) => this.Assign<string[]>(stopWords, (Action<ILanguageAnalyzer, string[]>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public LanguageAnalyzerDescriptor StopWords(IEnumerable<string> stopWords) => this.Assign<List<string>>(stopWords.ToListOrNullIfEmpty<string>(), (Action<ILanguageAnalyzer, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));
  }
}

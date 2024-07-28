// Decompiled with JetBrains decompiler
// Type: Nest.FingerprintAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class FingerprintAnalyzerDescriptor : 
    AnalyzerDescriptorBase<FingerprintAnalyzerDescriptor, IFingerprintAnalyzer>,
    IFingerprintAnalyzer,
    IAnalyzer
  {
    protected override string Type => "fingerprint";

    int? IFingerprintAnalyzer.MaxOutputSize { get; set; }

    bool? IFingerprintAnalyzer.PreserveOriginal { get; set; }

    string IFingerprintAnalyzer.Separator { get; set; }

    Nest.StopWords IFingerprintAnalyzer.StopWords { get; set; }

    string IFingerprintAnalyzer.StopWordsPath { get; set; }

    public FingerprintAnalyzerDescriptor Separator(string separator) => this.Assign<string>(separator, (Action<IFingerprintAnalyzer, string>) ((a, v) => a.Separator = v));

    public FingerprintAnalyzerDescriptor MaxOutputSize(int? maxOutputSize) => this.Assign<int?>(maxOutputSize, (Action<IFingerprintAnalyzer, int?>) ((a, v) => a.MaxOutputSize = v));

    public FingerprintAnalyzerDescriptor PreserveOriginal(bool? preserveOriginal = true) => this.Assign<bool?>(preserveOriginal, (Action<IFingerprintAnalyzer, bool?>) ((a, v) => a.PreserveOriginal = v));

    public FingerprintAnalyzerDescriptor StopWords(params string[] stopWords) => this.Assign<string[]>(stopWords, (Action<IFingerprintAnalyzer, string[]>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public FingerprintAnalyzerDescriptor StopWords(IEnumerable<string> stopWords) => this.Assign<List<string>>(stopWords.ToListOrNullIfEmpty<string>(), (Action<IFingerprintAnalyzer, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public FingerprintAnalyzerDescriptor StopWords(Nest.StopWords stopWords) => this.Assign<Nest.StopWords>(stopWords, (Action<IFingerprintAnalyzer, Nest.StopWords>) ((a, v) => a.StopWords = v));

    public FingerprintAnalyzerDescriptor StopWordsPath(string stopWordsPath) => this.Assign<string>(stopWordsPath, (Action<IFingerprintAnalyzer, string>) ((a, v) => a.StopWordsPath = v));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AnalyzersDescriptor : 
    IsADictionaryDescriptorBase<AnalyzersDescriptor, IAnalyzers, string, IAnalyzer>
  {
    public AnalyzersDescriptor()
      : base((IAnalyzers) new Analyzers())
    {
    }

    public AnalyzersDescriptor UserDefined(string name, IAnalyzer analyzer) => this.Assign(name, analyzer);

    public AnalyzersDescriptor Custom(
      string name,
      Func<CustomAnalyzerDescriptor, ICustomAnalyzer> selector)
    {
      return this.Assign(name, selector != null ? (IAnalyzer) selector(new CustomAnalyzerDescriptor()) : (IAnalyzer) null);
    }

    public AnalyzersDescriptor Keyword(
      string name,
      Func<KeywordAnalyzerDescriptor, IKeywordAnalyzer> selector = null)
    {
      return this.Assign(name, (IAnalyzer) selector.InvokeOrDefault<KeywordAnalyzerDescriptor, IKeywordAnalyzer>(new KeywordAnalyzerDescriptor()));
    }

    public AnalyzersDescriptor Language(
      string name,
      Func<LanguageAnalyzerDescriptor, ILanguageAnalyzer> selector)
    {
      return this.Assign(name, selector != null ? (IAnalyzer) selector(new LanguageAnalyzerDescriptor()) : (IAnalyzer) null);
    }

    public AnalyzersDescriptor Pattern(
      string name,
      Func<PatternAnalyzerDescriptor, IPatternAnalyzer> selector)
    {
      return this.Assign(name, selector != null ? (IAnalyzer) selector(new PatternAnalyzerDescriptor()) : (IAnalyzer) null);
    }

    public AnalyzersDescriptor Simple(
      string name,
      Func<SimpleAnalyzerDescriptor, ISimpleAnalyzer> selector = null)
    {
      return this.Assign(name, (IAnalyzer) selector.InvokeOrDefault<SimpleAnalyzerDescriptor, ISimpleAnalyzer>(new SimpleAnalyzerDescriptor()));
    }

    public AnalyzersDescriptor Snowball(
      string name,
      Func<SnowballAnalyzerDescriptor, ISnowballAnalyzer> selector)
    {
      return this.Assign(name, selector != null ? (IAnalyzer) selector(new SnowballAnalyzerDescriptor()) : (IAnalyzer) null);
    }

    public AnalyzersDescriptor Standard(
      string name,
      Func<StandardAnalyzerDescriptor, IStandardAnalyzer> selector)
    {
      return this.Assign(name, selector != null ? (IAnalyzer) selector(new StandardAnalyzerDescriptor()) : (IAnalyzer) null);
    }

    public AnalyzersDescriptor Stop(
      string name,
      Func<StopAnalyzerDescriptor, IStopAnalyzer> selector)
    {
      return this.Assign(name, selector != null ? (IAnalyzer) selector(new StopAnalyzerDescriptor()) : (IAnalyzer) null);
    }

    public AnalyzersDescriptor Whitespace(
      string name,
      Func<WhitespaceAnalyzerDescriptor, IWhitespaceAnalyzer> selector = null)
    {
      return this.Assign(name, (IAnalyzer) selector.InvokeOrDefault<WhitespaceAnalyzerDescriptor, IWhitespaceAnalyzer>(new WhitespaceAnalyzerDescriptor()));
    }

    public AnalyzersDescriptor Fingerprint(
      string name,
      Func<FingerprintAnalyzerDescriptor, IFingerprintAnalyzer> selector = null)
    {
      return this.Assign(name, (IAnalyzer) selector.InvokeOrDefault<FingerprintAnalyzerDescriptor, IFingerprintAnalyzer>(new FingerprintAnalyzerDescriptor()));
    }

    public AnalyzersDescriptor Kuromoji(
      string name,
      Func<KuromojiAnalyzerDescriptor, IKuromojiAnalyzer> selector = null)
    {
      return this.Assign(name, (IAnalyzer) selector.InvokeOrDefault<KuromojiAnalyzerDescriptor, IKuromojiAnalyzer>(new KuromojiAnalyzerDescriptor()));
    }

    public AnalyzersDescriptor Nori(
      string name,
      Func<NoriAnalyzerDescriptor, INoriAnalyzer> selector)
    {
      return this.Assign(name, selector != null ? (IAnalyzer) selector(new NoriAnalyzerDescriptor()) : (IAnalyzer) null);
    }

    public AnalyzersDescriptor Icu(string name, Func<IcuAnalyzerDescriptor, IIcuAnalyzer> selector) => this.Assign(name, selector != null ? (IAnalyzer) selector(new IcuAnalyzerDescriptor()) : (IAnalyzer) null);
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeTokenFiltersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AnalyzeTokenFiltersDescriptor : 
    DescriptorPromiseBase<AnalyzeTokenFiltersDescriptor, AnalyzeTokenFilters>
  {
    public AnalyzeTokenFiltersDescriptor()
      : base(new AnalyzeTokenFilters())
    {
    }

    public AnalyzeTokenFiltersDescriptor Name(string tokenFilter) => this.Assign<string>(tokenFilter, (Action<AnalyzeTokenFilters, string>) ((a, v) => a.AddIfNotNull<Union<string, ITokenFilter>>((Union<string, ITokenFilter>) v)));

    private AnalyzeTokenFiltersDescriptor AssignIfNotNull(ITokenFilter filter) => this.Assign<ITokenFilter>(filter, (Action<AnalyzeTokenFilters, ITokenFilter>) ((a, v) =>
    {
      if (v == null)
        return;
      a.Add(v);
    }));

    public AnalyzeTokenFiltersDescriptor DictionaryDecompounder(
      Func<DictionaryDecompounderTokenFilterDescriptor, IDictionaryDecompounderTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new DictionaryDecompounderTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor HyphenationDecompounder(
      Func<HyphenationDecompounderTokenFilterDescriptor, IHyphenationDecompounderTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new HyphenationDecompounderTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor EdgeNGram(
      Func<EdgeNGramTokenFilterDescriptor, IEdgeNGramTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new EdgeNGramTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Phonetic(
      Func<PhoneticTokenFilterDescriptor, IPhoneticTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new PhoneticTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Shingle(
      Func<ShingleTokenFilterDescriptor, IShingleTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new ShingleTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Stop(
      Func<StopTokenFilterDescriptor, IStopTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new StopTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Synonym(
      Func<SynonymTokenFilterDescriptor, ISynonymTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new SynonymTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor SynonymGraph(
      Func<SynonymGraphTokenFilterDescriptor, ISynonymGraphTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new SynonymGraphTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor WordDelimiter(
      Func<WordDelimiterTokenFilterDescriptor, IWordDelimiterTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new WordDelimiterTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor WordDelimiterGraph(
      Func<WordDelimiterGraphTokenFilterDescriptor, IWordDelimiterGraphTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new WordDelimiterGraphTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor AsciiFolding(
      Func<AsciiFoldingTokenFilterDescriptor, IAsciiFoldingTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new AsciiFoldingTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor CommonGrams(
      Func<CommonGramsTokenFilterDescriptor, ICommonGramsTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new CommonGramsTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor DelimitedPayload(
      Func<DelimitedPayloadTokenFilterDescriptor, IDelimitedPayloadTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new DelimitedPayloadTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Elision(
      Func<ElisionTokenFilterDescriptor, IElisionTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new ElisionTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Hunspell(
      Func<HunspellTokenFilterDescriptor, IHunspellTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new HunspellTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor KeepTypes(
      Func<KeepTypesTokenFilterDescriptor, IKeepTypesTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new KeepTypesTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor KeepWords(
      Func<KeepWordsTokenFilterDescriptor, IKeepWordsTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new KeepWordsTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor KeywordMarker(
      Func<KeywordMarkerTokenFilterDescriptor, IKeywordMarkerTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new KeywordMarkerTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor KStem(
      Func<KStemTokenFilterDescriptor, IKStemTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<KStemTokenFilterDescriptor, IKStemTokenFilter>(new KStemTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor Length(
      Func<LengthTokenFilterDescriptor, ILengthTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new LengthTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor LimitTokenCount(
      Func<LimitTokenCountTokenFilterDescriptor, ILimitTokenCountTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new LimitTokenCountTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Lowercase(
      Func<LowercaseTokenFilterDescriptor, ILowercaseTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<LowercaseTokenFilterDescriptor, ILowercaseTokenFilter>(new LowercaseTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor NGram(
      Func<NGramTokenFilterDescriptor, INGramTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new NGramTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor PatternCapture(
      Func<PatternCaptureTokenFilterDescriptor, IPatternCaptureTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new PatternCaptureTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor PatternReplace(
      Func<PatternReplaceTokenFilterDescriptor, IPatternReplaceTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new PatternReplaceTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor PorterStem(
      Func<PorterStemTokenFilterDescriptor, IPorterStemTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<PorterStemTokenFilterDescriptor, IPorterStemTokenFilter>(new PorterStemTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor Reverse(
      Func<ReverseTokenFilterDescriptor, IReverseTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<ReverseTokenFilterDescriptor, IReverseTokenFilter>(new ReverseTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor Snowball(
      Func<SnowballTokenFilterDescriptor, ISnowballTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new SnowballTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Condition(
      Func<ConditionTokenFilterDescriptor, IConditionTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new ConditionTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Stemmer(
      Func<StemmerTokenFilterDescriptor, IStemmerTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new StemmerTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Predicate(
      Func<PredicateTokenFilterDescriptor, IPredicateTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new PredicateTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor StemmerOverride(
      Func<StemmerOverrideTokenFilterDescriptor, IStemmerOverrideTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new StemmerOverrideTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Trim(
      Func<TrimTokenFilterDescriptor, ITrimTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<TrimTokenFilterDescriptor, ITrimTokenFilter>(new TrimTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor Truncate(
      Func<TruncateTokenFilterDescriptor, ITruncateTokenFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ITokenFilter) selector(new TruncateTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public AnalyzeTokenFiltersDescriptor Unique(
      Func<UniqueTokenFilterDescriptor, IUniqueTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<UniqueTokenFilterDescriptor, IUniqueTokenFilter>(new UniqueTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor Uppercase(
      Func<UppercaseTokenFilterDescriptor, IUppercaseTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<UppercaseTokenFilterDescriptor, IUppercaseTokenFilter>(new UppercaseTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor Fingerprint(
      Func<FingerprintTokenFilterDescriptor, IFingerprintTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<FingerprintTokenFilterDescriptor, IFingerprintTokenFilter>(new FingerprintTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor KuromojiStemmer(
      Func<KuromojiStemmerTokenFilterDescriptor, IKuromojiStemmerTokenFilter> selector = null)
    {
      return this.AssignIfNotNull((ITokenFilter) selector.InvokeOrDefault<KuromojiStemmerTokenFilterDescriptor, IKuromojiStemmerTokenFilter>(new KuromojiStemmerTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor KuromojiReadingForm(
      Func<KuromojiReadingFormTokenFilterDescriptor, IKuromojiReadingFormTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new KuromojiReadingFormTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor KuromojiPartOfSpeech(
      Func<KuromojiPartOfSpeechTokenFilterDescriptor, IKuromojiPartOfSpeechTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new KuromojiPartOfSpeechTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor IcuCollation(
      Func<IcuCollationTokenFilterDescriptor, IIcuCollationTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new IcuCollationTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor IcuFolding(
      Func<IcuFoldingTokenFilterDescriptor, IIcuFoldingTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new IcuFoldingTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor IcuNormalization(
      Func<IcuNormalizationTokenFilterDescriptor, IIcuNormalizationTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new IcuNormalizationTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor IcuTransform(
      Func<IcuTransformTokenFilterDescriptor, IIcuTransformTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new IcuTransformTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor NoriPartOfSpeech(
      Func<NoriPartOfSpeechTokenFilterDescriptor, INoriPartOfSpeechTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new NoriPartOfSpeechTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor Multiplexer(
      Func<MultiplexerTokenFilterDescriptor, IMultiplexerTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new MultiplexerTokenFilterDescriptor()));
    }

    public AnalyzeTokenFiltersDescriptor RemoveDuplicates(
      Func<RemoveDuplicatesTokenFilterDescriptor, IRemoveDuplicatesTokenFilter> selector)
    {
      return this.AssignIfNotNull((ITokenFilter) selector(new RemoveDuplicatesTokenFilterDescriptor()));
    }
  }
}
